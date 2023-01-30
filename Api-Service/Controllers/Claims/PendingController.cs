using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApiService.Helpers;
using ApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ApiService.Controllers
{
    [ApiController]
    [Route("api/claim/[controller]")]
    public class PendingController : ControllerBase
    {
        private readonly ILogger<CellSitesController> _logger;
        private readonly string SP_HOSTNAME;
        private readonly string GET_ALL_PENDING_CLAIMS;

        public PendingController(ILogger<CellSitesController> logger , IConfiguration configuration)
        {
            _logger = logger;
            this.SP_HOSTNAME = configuration["SharepointHostname"];
            this.GET_ALL_PENDING_CLAIMS = configuration["pendingClaims"];
        }

        [HttpGet("list")]
        public string GetAllPendingClaim()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(GET_ALL_PENDING_CLAIMS);
                    request.Method = Method.GET;
                    request.AddQueryParameter("email", user_email);
                    IRestResponse response = client.Execute(request);
                    return response.Content;
                }
                return null;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }
    }
}

