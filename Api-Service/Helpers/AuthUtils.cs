/*using ApiService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
*/
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiService.Models;
using RestSharp;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApiService.Helpers
{
    public class AuthUtils
    {
        public static Dictionary<string, object> GenerateVendorToken(
            VendorAuthModel model,
            string secretKey = "dF0@d3s[z%R%Z-&Ww-7O",
            string hostname = null,
            string OTP=null
            )
        {
            if (string.IsNullOrEmpty(hostname)) {
                throw new ArgumentNullException("Hostname cannot be Null or Empty");
            }

            var client = new RestClient(hostname);
            var request = new RestRequest("/api/wfauth/get-roles");
            request.Method = Method.GET;
            request.AddQueryParameter("email", model.Email);
            var response = client.Execute(request);
            var content = response.Content;
            Regex reg = new Regex("\"");
            string scopeString = reg.Replace(content, string.Empty).TrimEnd(',');

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("email", model.Email.ToString()),
                    new Claim("Secret2FA", OTP),
                    new Claim("scopes", scopeString)
                }),
                Issuer = "mercurialminds.com",
                Audience = "telco-jicp.com",
                Expires = DateTime.UtcNow.AddDays(30),
                
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
           // token.
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("token", tokenHandler.WriteToken(token));
            result.Add("Expiry", token.ValidTo);
            result.Add("scopes", scopeString);

            return result;
        }
        public static Dictionary<string, object> GenerateToken(
            LoginModel model,
            string secretKey = "dF0@d3s[z%R%Z-&Ww-7O",
            string hostname = null
            )
        {
            if (string.IsNullOrEmpty(hostname)) {
                throw new ArgumentNullException("Hostname cannot be Null or Empty");
            }

            var client = new RestClient(hostname);

            var request = new RestRequest("/api/wfauth/get-roles");
            request.Method = Method.GET;
            request.AddQueryParameter("email", model.Email);
            var response = client.Execute(request);
            var content = response.Content;
            Regex reg = new Regex("\"");

            string scopeString = reg.Replace(content, string.Empty).TrimEnd(',');

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("email", model.Email.ToString()),
                    new Claim("scopes", scopeString)
                }),
                Issuer = "mercurialminds.com",
                Audience = "telco-jicp.com",
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("token", tokenHandler.WriteToken(token));
            result.Add("scopes", scopeString);
            result.Add("Expiry", token.ValidTo);

            return result;
        }

        public static string? GetUserEmail(string token)
        {
            if (token == null)
                return null;
            try
            {
                var jwt = new JwtSecurityToken(token);
                return jwt.Claims.First(x => x.Type == "email").Value.ToString();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
