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

namespace ApiService.Controllers.Claims
{
    [ApiController]
    [Route("api/claim/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly string SP_HOSTNAME;
        private readonly string GET_INITIATOR_ROLES;
        private readonly string GET_APPROVER_ROLES;
        private readonly string SECRET_KEY;
        private readonly string ADD_INITIATOR;
        private readonly string DELETE_INITIATOR;
        private readonly string ADD_VENDOR;
        private readonly string DELETE_VENDOR;
        private readonly string ADD_APPROVAL;
        private readonly string DELETE_APPROVAL;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.SP_HOSTNAME = configuration["SharepointHostname"];
            this.GET_INITIATOR_ROLES = configuration.GetValue<string>("AuthorizationList:getInitiatorRoles");
            this.GET_APPROVER_ROLES = configuration.GetValue<string>("AuthorizationList:getApproverRoles");
            this.ADD_INITIATOR = configuration.GetValue<string>("AdminUrl:addInitiator");
            this.DELETE_INITIATOR = configuration.GetValue<string>("AdminUrl:deleteInitiator");
            this.ADD_VENDOR = configuration.GetValue<string>("AdminUrl:addVendor");
            this.DELETE_VENDOR = configuration.GetValue<string>("AdminUrl:deleteVendor");
            this.ADD_APPROVAL = configuration.GetValue<string>("AdminUrl:addApproval");
            this.DELETE_APPROVAL = configuration.GetValue<string>("AdminUrl:deleteApproval");
            this.SECRET_KEY = configuration.GetValue<string>("SECURE_KEY");
        }

        [HttpPost]
        [Route("add-initiator")]
        public string AddNewInitiator([FromForm] NewInitiatorModel initiatorModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(ADD_INITIATOR);
                    request.AddJsonBody(initiatorModel);
                    request.Method = Method.POST;
                    var response = client.Execute(request);
                    return response.Content;
                }
                return null;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }

        [HttpDelete]
        [Route("delete-initiator/{itemId}")]
        public string DeleteInitiator(string itemId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(DELETE_INITIATOR + "/" + itemId);
                    request.Method = Method.DELETE;
                    var response = client.Execute(request);
                    return response.Content;
                }
                return null;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }

        [HttpPost]
        [Route("add-vendor")]
        public string AddNewVendor([FromForm] NewVendorModel vendorModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(ADD_VENDOR);
                    request.AddJsonBody(vendorModel);
                    request.Method = Method.POST;
                    var response = client.Execute(request);
                    return response.Content;
                }
                return null;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }
        [HttpDelete]
        [Route("delete-vendor/{itemId}")]
        public string DeleteVendor(string itemId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(DELETE_VENDOR + "/" + itemId);
                    request.Method = Method.DELETE;
                    var response = client.Execute(request);
                    return response.Content;
                }
                return null;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }


        [HttpPost]
        [Route("add-approval")]
        public string AddNewApproval([FromForm] NewApprovalModel approvalModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(ADD_APPROVAL);
                    request.AddJsonBody(approvalModel);
                    request.Method = Method.POST;
                    var response = client.Execute(request);
                    return response.Content;
                }
                return null;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }
        [HttpDelete]
        [Route("delete-approval/{itemId}")]
        public string DeleteApproval(string itemId)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(DELETE_APPROVAL + "/" + itemId);
                    request.Method = Method.DELETE;
                    var response = client.Execute(request);
                    return response.Content;
                }
                return null;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }


        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromForm] LoginModel model)
        {
            try
            {

                var token = AuthUtils.GenerateToken(model, SECRET_KEY, SP_HOSTNAME);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest("Request failed, Please try again.");
            }
        }
    }
        
}
