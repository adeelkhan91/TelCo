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
    public class BiometricController : ControllerBase
    {

        // string apiResponse;
        private readonly ILogger<BiometricController> _logger;
        private readonly string SP_HOSTNAME;
        private readonly string GET_ALL;
        private readonly string GET_BIOMETRIC_BY_ID;
        private readonly string ADD_INITIATOR_BIOMETRIC;
        private readonly string SAVE_INITIATOR_AS_DRAFT;
        private readonly string UPDATE_POC_BIOMETRIC;
        private readonly string UPDATE_VENDOR_BIOMETRIC;
        private readonly string UPDATE_HOD_BIOMETRIC;
        private readonly string UPDATE_FINANCE_BIOMETRIC;
        private readonly string DELETE_FILES_BY_ID;
        //private readonly string GET_WF_INITIATORS;
        //private readonly string GET_WF_APPROVERS;

        public BiometricController(ILogger<BiometricController> logger, /*IClaimRepository claimRepository,*/ IConfiguration configuration)
        {
            _logger = logger;
            // this.claimRepository = claimRepository;
            this.SP_HOSTNAME = configuration["SharepointHostname"];
            this.GET_ALL = configuration.GetValue<string>("Biometric:getAll");
            this.GET_BIOMETRIC_BY_ID = configuration.GetValue<string>("Biometric:getById");
            this.ADD_INITIATOR_BIOMETRIC = configuration.GetValue<string>("Biometric:addNewInitiator");
            this.SAVE_INITIATOR_AS_DRAFT = configuration.GetValue<string>("Biometric:saveInDraft");
            this.UPDATE_POC_BIOMETRIC = configuration.GetValue<string>("Biometric:updatePoc");
            this.UPDATE_VENDOR_BIOMETRIC = configuration.GetValue<string>("Biometric:updateVendor");
            this.UPDATE_FINANCE_BIOMETRIC = configuration.GetValue<string>("Biometric:updateFinance");
            this.UPDATE_HOD_BIOMETRIC = configuration.GetValue<string>("Biometric:updateHOD");
            this.DELETE_FILES_BY_ID = configuration["deleteFiles"];
            //this.GET_WF_INITIATORS = configuration.GetValue<string>("AuthorizationList:getAllInitiator");
            //this.GET_WF_APPROVERS = configuration.GetValue<string>("AuthorizationList:getAllApprover");
        }

        [HttpGet("all")]
        public string GetBiometrics()
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(GET_ALL);
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
        public string GetInitiatorBiometric(string id)
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(GET_BIOMETRIC_BY_ID + "/" + id);
                request.Method = Method.GET;
                IRestResponse response = client.Execute(request);

                JObject jsonResponse = JObject.Parse(response.Content);
                jsonResponse["IncidentDate"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["IncidentDate"]).ToLocalTime().ToString().Split("+")[0]);
                if(jsonResponse["POCInitiatedTime"] != null)
                {
                    if (jsonResponse["POCInitiatedTime"].ToString() != "")
                        jsonResponse["POCInitiatedTime"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["POCInitiatedTime"]).ToLocalTime().ToString().Split("+")[0]);
                }
                if (jsonResponse["VendorInitiatedTime"] != null)
                {
                    if (jsonResponse["VendorInitiatedTime"].ToString() != "")
                        jsonResponse["VendorInitiatedTime"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["VendorInitiatedTime"]).ToLocalTime().ToString().Split("+")[0]);
                }
                if (jsonResponse["FinanceInitiatedTime"] != null)
                {
                    if (jsonResponse["FinanceInitiatedTime"].ToString() != "")
                        jsonResponse["FinanceInitiatedTime"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["FinanceInitiatedTime"]).ToLocalTime().ToString().Split("+")[0]);
                }
                if (jsonResponse["FinanceTeamInitiatedTime"] != null)
                {
                    if (jsonResponse["FinanceTeamInitiatedTime"].ToString() != "")
                        jsonResponse["FinanceTeamInitiatedTime"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["FinanceTeamInitiatedTime"]).ToLocalTime().ToString().Split("+")[0]);
                }
                if (jsonResponse["FixedAssetInitiatedTime"] != null)
                {
                    if (jsonResponse["FixedAssetInitiatedTime"].ToString() != "")
                        jsonResponse["FixedAssetInitiatedTime"] = Convert.ToDateTime(Convert.ToDateTime(jsonResponse["FixedAssetInitiatedTime"]).ToLocalTime().ToString().Split("+")[0]);
                }
                    return jsonResponse.ToString();
            }
            catch (Exception)
            {
                return "Error retrieving data from the database";
            }
        }

        [HttpPost("initiator")]
        public string AddBiometricInitiator([FromForm] BiometricModel biometric)
        { 
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);   
                if (user_email != null)
                {
                    //RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(ADD_INITIATOR_BIOMETRIC);
                    biometric.Date = Convert.ToDateTime(biometric.Date.ToShortDateString() + " " + biometric.Time);
                    ProcessFiles(request, biometric);
                    biometric.InitiatedBy = user_email;
                    request.AddJsonBody(biometric);
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

        private void ProcessFiles(RestRequest req, FilesModel biometric)
        {
            if (biometric.Attachment == null)
            {
                return;
            }

            if (biometric.Attachment.Count <= 0)
            {
                return;
            }
            if (biometric.AttachmentBase64 == null)
                biometric.AttachmentBase64 = new List<string>();
            foreach (var file in biometric.Attachment)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        s = s + ':' + file.FileName;
                        biometric.AttachmentBase64.Add(s);
                        ///////////req.AddFile("Attachment", fileBytes, file.FileName, file.ContentType);
                    }
                }
            }

            biometric.Attachment.Clear();
            biometric.Attachment = null;

        }

        [HttpPost("saveAsDraft")]
        public string SaveBiometricInitiatorAsDraft([FromForm] BiometricModel biometric)
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
                    biometric.InitiatedBy = user_email;
                    biometric.Date = Convert.ToDateTime(biometric.Date.ToShortDateString() + " " + biometric.Time);
                    ProcessFiles(request,  biometric);
                    request.AddJsonBody(biometric);
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
        public string AddBiometricPoc([FromForm] PocModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_POC_BIOMETRIC);
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
        public string AddBiometricVendor([FromForm] VendorModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_VENDOR_BIOMETRIC);
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
        public string AddBiometricFinance([FromForm] FinanceTeamModel ftModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_FINANCE_BIOMETRIC);
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
        public string AddBiometricHod([FromForm] HODModel hodModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_HOD_BIOMETRIC);
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
