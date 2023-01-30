using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using ApiService.Controllers.Claims;
using ApiService.Helpers;
using ApiService.Models;
using Newtonsoft.Json.Linq;

namespace ApiService.Controllers
{
    [ApiController]
    [Route("api/claim/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LaptopController : ControllerBase
    {
        // string apiResponse;
        private readonly ILogger<LaptopController> _logger;
        private readonly string SP_HOSTNAME;
        private readonly string GET_ALL_LAPTOPS;
        private readonly string GET_LAPTOP_BY_ID;
        private readonly string ADD_INITIATOR_LAPTOP;
        private readonly string SAVE_INITIATOR_AS_DRAFT;
        private readonly string UPDATE_POC_LAPTOP;
        private readonly string UPDATE_VENDOR_LAPTOP;
        private readonly string UPDATE_FINANCE_LAPTOP;

        private readonly string UPDATE_HOD_LAPTOP;
        private readonly string DELETE_FILES_BY_ID;
        //private readonly string GET_WF_INITIATORS;
        //private readonly string GET_WF_APPROVERS;

        public LaptopController(ILogger<LaptopController> logger, /*IClaimRepository claimRepository,*/ IConfiguration configuration)
        {
            _logger = logger;
            // this.claimRepository = claimRepository;
            this.SP_HOSTNAME = configuration["SharepointHostname"];
            this.GET_ALL_LAPTOPS = configuration.GetValue<string>("Laptop:getAll");
            this.GET_LAPTOP_BY_ID = configuration.GetValue<string>("Laptop:getById");
            this.ADD_INITIATOR_LAPTOP = configuration.GetValue<string>("Laptop:addNewInitiator");
            this.SAVE_INITIATOR_AS_DRAFT = configuration.GetValue<string>("Laptop:saveInDraft");
            this.UPDATE_POC_LAPTOP = configuration.GetValue<string>("Laptop:updatePoc");
            this.UPDATE_VENDOR_LAPTOP = configuration.GetValue<string>("Laptop:updateVendor");
            this.UPDATE_FINANCE_LAPTOP = configuration.GetValue<string>("Laptop:updateFinance");
            this.UPDATE_HOD_LAPTOP = configuration.GetValue<string>("Laptop:updateHOD");
            this.DELETE_FILES_BY_ID = configuration["deleteFiles"];
            //this.GET_WF_INITIATORS = configuration.GetValue<string>("AuthorizationList:getAllInitiator");
            //this.GET_WF_APPROVERS = configuration.GetValue<string>("AuthorizationList:getAllApprover");
        }

        [HttpGet("all")]
        public string GetLaptops()
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(GET_ALL_LAPTOPS);
                request.Method = Method.GET;
                IRestResponse response = client.Execute(request);
                return response.Content;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }

        [HttpGet("get-by/{id}")]
        public string GetInitiatorLaptop(string id)
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(GET_LAPTOP_BY_ID + "/" + id);
                request.Method = Method.GET;
                IRestResponse response = client.Execute(request);

                JObject jsonResponse = JObject.Parse(response.Content);
                jsonResponse["IncidentDate"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["IncidentDate"]).ToLocalTime().ToString().Split("+")[0]);
                if (jsonResponse["POCInitiatedTime"].ToString() != "")
                    jsonResponse["POCInitiatedTime"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["POCInitiatedTime"]).ToLocalTime().ToString().Split("+")[0]);
                if (jsonResponse["VendorInitiatedTime"].ToString() != "")
                    jsonResponse["VendorInitiatedTime"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["VendorInitiatedTime"]).ToLocalTime().ToString().Split("+")[0]);
                if (jsonResponse["FinanceInitiatedTime"].ToString() != "")
                    jsonResponse["FinanceInitiatedTime"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["FinanceInitiatedTime"]).ToLocalTime().ToString().Split("+")[0]);
                if (jsonResponse["FinanceTeamInitiatedTime"].ToString() != "")
                    jsonResponse["FinanceTeamInitiatedTime"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["FinanceTeamInitiatedTime"]).ToLocalTime().ToString().Split("+")[0]);
                if (jsonResponse["FixedAssetInitiatedTime"].ToString() != "")
                    jsonResponse["FixedAssetInitiatedTime"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["FixedAssetInitiatedTime"]).ToLocalTime().ToString().Split("+")[0]);
                return jsonResponse.ToString();
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }

        [HttpPost("initiator")]
        public string AddLaptopInitiator([FromForm] LaptopModel laptop)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);   
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(ADD_INITIATOR_LAPTOP);
                    laptop.Date = Convert.ToDateTime(laptop.Date.ToShortDateString() + " " + laptop.Time);
                    ProcessFiles(request, laptop);
                    laptop.InitiatedBy = user_email;
                    request.AddJsonBody(laptop);
                    request.Method = Method.POST;
                    var response = client.Execute(request);
                    return response.Content;

                    /*var auth_user = Request.Headers["User-Email"].ToString().ToLower();
                    for (int i = 0; i < response.Content.Length; i++)
                    {
                        if (auth_user == response.Content[i].ToString().ToLower())
                        {
                            var request1 = new RestRequest(ADD_INITIATOR_LAPTOP);
                            ProcessFiles(request1, laptop);
                            request1.AddJsonBody(laptop);
                            request1.Method = Method.POST;
                            var response1 = client.Execute(request);
                            return response1.Content;
                        }
                    }*/
                }
                return null;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }

        [HttpGet("files/{id}")]
        public string DeleteFiles(string id)
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(DELETE_FILES_BY_ID + "/" + id);
                request.Method = Method.GET;
                IRestResponse response = client.Execute(request);
                return response.Content;
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }

        private void ProcessFiles(RestRequest req, FilesModel laptop)
        {
            if (laptop.Attachment == null)
            {
                return;
            }

            if (laptop.Attachment.Count <= 0)
            {
                return;
            }

            if (laptop.AttachmentBase64 == null)
                laptop.AttachmentBase64 = new List<string>();

            foreach (var file in laptop.Attachment)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        s = s + ':' + file.FileName;
                        laptop.AttachmentBase64.Add(s);
                        ///////////req.AddFile("Attachment", fileBytes, file.FileName, file.ContentType);
                    }
                }
            }

            laptop.Attachment.Clear();
            laptop.Attachment = null;

        }

        [HttpPost("saveAsDraft")]
        public string SaveLaptopInitiatorAsDraft([FromForm] LaptopModel laptop)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(SAVE_INITIATOR_AS_DRAFT);
                    laptop.InitiatedBy = user_email;
                    laptop.Date = Convert.ToDateTime(laptop.Date.ToShortDateString() + " " + laptop.Time);
                    ProcessFiles(request, laptop);
                    request.AddJsonBody(laptop);
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

        [HttpPut("poc")]
        public string AddLaptopPoc([FromForm] PocModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_POC_LAPTOP);
                    // model.Date = Convert.ToDateTime(model.Date.ToShortDateString() + " " + model.Time);
                    model.InitiatedBy = user_email;
                    ProcessFiles(request, model);
                    request.AddJsonBody(model);
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

        [HttpPut("vendor")]
        public string AddLaptopVendor([FromForm] VendorModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_VENDOR_LAPTOP);
                    model.Approver = user_email;
                    ProcessFiles(request, model);
                    request.AddJsonBody(model);
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

        [HttpPut("finance")]
        public string AddLaptopFinance([FromForm] FinanceTeamModel ftModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_FINANCE_LAPTOP);
                    ftModel.Approver = user_email;
                    request.AddJsonBody(ftModel);
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

        [HttpPut("hod")]
        public string AddLaptopHod([FromForm] HODModel hodModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_HOD_LAPTOP);
                    hodModel.Approver = user_email;
                    request.AddJsonBody(hodModel);
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
    }
}
