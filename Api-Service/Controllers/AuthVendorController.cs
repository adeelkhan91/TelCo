using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiService.Models;
using RestSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Principal;
using System.Linq;
using ApiService.Helpers;
using MimeKit;
using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ApiService.Controllers
{
    [ApiController]
    [Route("api/claim/[controller]")]
    public class AuthVendorController : ControllerBase
    {
        private readonly ILogger<AuthVendorController> _logger;
        private readonly string SP_HOSTNAME;
        private readonly string VENDOR_LOGIN;
        private readonly string VENDOR_SIGNUP;
        private readonly string SECRET_KEY;
        private readonly string Mail;
        private readonly string DisplayName;
        private readonly string Password;
        private readonly string Host;
        private readonly int Port;

        public AuthVendorController(ILogger<AuthVendorController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.SP_HOSTNAME = configuration["SharepointHostname"];
            this.VENDOR_LOGIN = configuration.GetValue<string>("Vendor_Auth:login");
            this.VENDOR_SIGNUP = configuration.GetValue<string>("Vendor_Auth:signup");
            this.SECRET_KEY = configuration.GetValue<string>("SECURE_KEY");
            //this.MAIL_SETTINGS = configuration.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            this.Mail=configuration.GetValue<string>("Mail");
            this.DisplayName=configuration.GetValue<string>("DisplayName");
            this.Password=configuration.GetValue<string>("Password");
            this.Host=configuration.GetValue<string>("Host");
            this.Port=configuration.GetValue<int>("Port");
        }

        [HttpPost]
        [Route("signup")]
        public string Signup([FromForm] VendorAuthModel signup)
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(VENDOR_SIGNUP);
                request.AddJsonBody(signup);
                request.Method = Method.POST;
                var response = client.Execute(request);
                return response.Content;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }
        public async Task SendEmailAsync(string OTP,string Email)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(Mail);
            email.To.Add(MailboxAddress.Parse(Email));
            email.Subject = "telco | One Time Password (OTP) For Vendor Portal";
            var builder = new BodyBuilder();
            builder.HtmlBody = "Dear User, <br> <br> Please enter the OTP below to sign in to the Vendor Portal: <br> <br> <strong>" + OTP + "</strong> <br> <br> telco Team";
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();


            //Old Version
            //smtp.Connect(Host, Port, SecureSocketOptions.StartTls);
            ////smtp.Authenticate(Mail, Password);
            //await smtp.SendAsync(email);
            //smtp.Disconnect(true);


            //Updated Version
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            smtp.Connect(Host, Port, SecureSocketOptions.StartTls);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
        public async Task<IActionResult> SendMail(string OTP, string Email)
        {
            try
            {
                await SendEmailAsync(OTP,Email);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static string EncryptString(string value)
        {
            value = value.Replace('1', 'a');
            return value;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] VendorAuthModel login)
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(VENDOR_LOGIN);
                request.AddJsonBody(login);
                request.Method = Method.POST;
                var response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Random r = new Random();
                    var x = r.Next(0, 1000000);
                    string OTP = x.ToString("000000");
                    var token = AuthUtils.GenerateVendorToken(login, SECRET_KEY, SP_HOSTNAME, OTP);
                    await SendMail(OTP,login.Email);
                    return Ok(token);
                }
                else
                {
                    return BadRequest("Request failed, Please try again.");
                }

            }

            catch (Exception)
            {
                return BadRequest("Request failed, Please try again.");
            }
        }

    }     
}
