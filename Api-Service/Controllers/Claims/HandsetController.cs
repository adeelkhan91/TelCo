using System;
using System.Collections.Generic;
using System.IO;
using ApiService.Helpers;
using ApiService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ApiService.Controllers
{
    [ApiController]
    [Route("api/claim/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HandsetController : ControllerBase
    {
        private readonly ILogger<HandsetController> _logger;
        private readonly string SP_HOSTNAME;
        private readonly string GET_ALL_HANDSETS;
        private readonly string GET_HANDSET_BY_ID;
        private readonly string ADD_INITIATOR_HANDSET;
        private readonly string SAVE_INITIATOR_AS_DRAFT;
        private readonly string UPDATE_POC_HANDSET;
        private readonly string UPDATE_VENDOR_HANDSET;
        private readonly string UPDATE_FINANCE_HANDSET;
        private readonly string UPDATE_HOD_HANDSET;

        public HandsetController(ILogger<HandsetController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.SP_HOSTNAME = configuration["SharepointHostname"];
            this.GET_ALL_HANDSETS = configuration.GetValue<string>("Handset:getAll");
            this.GET_HANDSET_BY_ID = configuration.GetValue<string>("Handset:getById");
            this.ADD_INITIATOR_HANDSET = configuration.GetValue<string>("Handset:addNewInitiator");
            this.SAVE_INITIATOR_AS_DRAFT = configuration.GetValue<string>("Handset:saveInDraft");
            this.UPDATE_POC_HANDSET = configuration.GetValue<string>("Handset:updatePoc");
            this.UPDATE_VENDOR_HANDSET = configuration.GetValue<string>("Handset:updateVendor");
            this.UPDATE_FINANCE_HANDSET = configuration.GetValue<string>("Handset:updateFinance");
            this.UPDATE_HOD_HANDSET = configuration.GetValue<string>("Handset:updateHOD");
        }

        [HttpGet("all")]
        public string GetHandsets()
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(GET_ALL_HANDSETS);
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
        public string GetInitiatorHandset(string id)
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(GET_HANDSET_BY_ID + "/" + id);
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
        public string AddHandsetInitiator([FromForm] HandsetModel handset)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(ADD_INITIATOR_HANDSET);
                    handset.Date = Convert.ToDateTime(handset.Date.ToShortDateString() + " " + handset.Time);
                    handset.InitiatedBy = user_email;
                    ProcessFiles(request, handset);
                    request.AddJsonBody(handset);
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
        public string SaveHandsetInitiatorAsDraft([FromForm] HandsetModel handset)
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
                    handset.Date = Convert.ToDateTime(handset.Date.ToShortDateString() + " " + handset.Time);
                    ProcessFiles(request, handset);
                    handset.InitiatedBy = user_email;
                    request.AddJsonBody(handset);
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
        public string AddHandsetPoc([FromForm] PocModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_POC_HANDSET);
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
        public string AddHandsetVendor([FromForm] VendorModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_VENDOR_HANDSET);
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
        public string AddHandsetFinance([FromForm] FinanceTeamModel ftModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);

                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_FINANCE_HANDSET);
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
        public string AddHandsetHod([FromForm] HODModel hodModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_HOD_HANDSET);
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
