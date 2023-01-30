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
    public class CellSitesController : ControllerBase
    {
        private readonly ILogger<CellSitesController> _logger;
        private readonly string SP_HOSTNAME;
        private readonly string GET_ALL_CELL_SITE;
        private readonly string GET_CELL_SITE_BY_ID;
        private readonly string ADD_INITIATOR_CELL_SITE;
        private readonly string SAVE_INITIATOR_AS_DRAFT;
        private readonly string UPDATE_POC_CELL_SITE;
        private readonly string UPDATE_VENDOR_CELL_SITE;
        private readonly string UPDATE_FINANCE_CELL_SITE;

        private readonly string UPDATE_HOD_CELL_STIE;
        private readonly string DELETE_FILES_BY_ID;
        private readonly string FIXED_ASSET_TEAM;

        private readonly string FINANCE_INSURANCE_TEAM_REVIEW;
        private readonly string MANAGER_ATTACHMENT_DOC;

        public CellSitesController(ILogger<CellSitesController> logger , IConfiguration configuration)
        {
            _logger = logger;
            this.SP_HOSTNAME = configuration["SharepointHostname"];
            this.GET_ALL_CELL_SITE = configuration.GetValue<string>("CELL_SITE:getAll");
            this.GET_CELL_SITE_BY_ID = configuration.GetValue<string>("CELL_SITE:getById");
            this.ADD_INITIATOR_CELL_SITE = configuration.GetValue<string>("CELL_SITE:addNewInitiator");
            this.SAVE_INITIATOR_AS_DRAFT = configuration.GetValue<string>("CELL_SITE:saveInDraft");
            this.UPDATE_POC_CELL_SITE = configuration.GetValue<string>("CELL_SITE:updatePoc");
            this.UPDATE_VENDOR_CELL_SITE = configuration.GetValue<string>("CELL_SITE:updateVendor");
            this.UPDATE_FINANCE_CELL_SITE = configuration.GetValue<string>("CELL_SITE:updateFinance");
            this.UPDATE_HOD_CELL_STIE = configuration.GetValue<string>("CELL_SITE:updateHOD");
            this.FINANCE_INSURANCE_TEAM_REVIEW = configuration.GetValue<string>("CELL_SITE:financeInsuranceReview");
            this.FIXED_ASSET_TEAM = configuration.GetValue<string>("CELL_SITE:fixedAssetTeam");

            this.MANAGER_ATTACHMENT_DOC = configuration.GetValue<string>("CELL_SITE:managerAttachmentDoc");

            this.DELETE_FILES_BY_ID = configuration["deleteFiles"];
        }

        [HttpGet("all")]
        public string GetCellSites()
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(GET_ALL_CELL_SITE);
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
        public string GetInitiatorCellSite(string id)
        {
            try
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SP_HOSTNAME);
                var request = new RestRequest(GET_CELL_SITE_BY_ID + "/" + id);
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
        public string AddCellSiteInitiator([FromForm] CellSiteModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(ADD_INITIATOR_CELL_SITE);
                    model.Date = Convert.ToDateTime(model.Date.ToShortDateString() + " " + model.Time);
                    ProcessFiles(request, model);
                    ProcessDamageFiles(request, model);

                    model.InitiatedBy = user_email;
                    request.AddJsonBody(model);
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

        private void ProcessFiles(RestRequest req, FilesModel model)
        {
            if (model.Attachment == null)
            {
                return;
            }

            if (model.Attachment.Count <= 0)
            {
                return;
            }

            if (model.AttachmentBase64 == null)
                model.AttachmentBase64 = new List<string>();

            foreach (var file in model.Attachment)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        s = s + ':' + file.FileName;
                        model.AttachmentBase64.Add(s);
                        ///////////req.AddFile("Attachment", fileBytes, file.FileName, file.ContentType);
                    }
                }
            }

            model.Attachment.Clear();
            model.Attachment = null;

        }

        private void ProcessDamageFiles(RestRequest req, FilesModel model)
        {
            if (model.DamagedFilesAttachment == null)
            {
                return;
            }

            if (model.DamagedFilesAttachment.Count <= 0)
            {
                return;
            }

            if (model.DamagedFilesAttachmentBase64 == null)
                model.DamagedFilesAttachmentBase64 = new List<string>();

           
            foreach (var file in model.DamagedFilesAttachment)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        s = s + ':' + file.FileName;
                        model.DamagedFilesAttachmentBase64.Add(s);
                        ///////////req.AddFile("Attachment", fileBytes, file.FileName, file.ContentType);
                    }
                }
            }

            model.DamagedFilesAttachment.Clear();
            model.DamagedFilesAttachment = null;

        }

        [HttpPost("saveAsDraft")]
        public string SaveCellSiteInitiatorAsDraft([FromForm] CellSiteModel model)
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
                    model.InitiatedBy = user_email;
                    model.Date = Convert.ToDateTime(model.Date.ToShortDateString() + " " + model.Time);
                    ProcessFiles(request, model);
                    ProcessDamageFiles(request, model);

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

        [HttpPut("poc")]
        public string AddCellSitePoc([FromForm] PocModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_POC_CELL_SITE);
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
        public string AddCellSiteVendor([FromForm] VendorModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_VENDOR_CELL_SITE);
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

       
        [HttpPut("fixed-asset-team")]
        public string FixedAssetTeam([FromForm] CellSiteModel model)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(FIXED_ASSET_TEAM);
                    model.InitiatedBy = user_email;
                    ProcessDamageFiles(request, model);
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

        /*[HttpPut("team-manager-attach-doc")]
        public string TeamManagerAttachDoc([FromForm] TeamManagerAttachmentDocModel ftModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(MANAGER_ATTACHMENT_DOC);
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
        }*/

        [HttpPut("finance")]
        public string AddCellSiteFinance([FromForm] FinanceTeamModel ftModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_FINANCE_CELL_SITE);
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
        public string AddCellSiteHod([FromForm] HODModel hodModel)
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SP_HOSTNAME);
                    var request = new RestRequest(UPDATE_HOD_CELL_STIE);
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

