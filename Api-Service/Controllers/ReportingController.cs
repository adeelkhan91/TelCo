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
    [Route("api/[controller]")]
    public class ReportingController : ControllerBase
    {
        private readonly ILogger<CellSitesController> _logger;
        private readonly string SP_HOSTNAME;
        private readonly string CLAIMS_REPORT;
        private readonly string AGING_REPORT;
        private readonly string DEDUCTIBLE_REPORT;
        private readonly string GAINLOSS_REPORT;
        private readonly string SUMMARY_REPORT;
        // Claim Name
        private readonly string BIOMETRIC_CLAIM;
        private readonly string BSD_CLAIM;
        private readonly string CASH_CLAIM;
        private readonly string CELLSITE_CLAIM;
        private readonly string HANDSET_CLAIM;
        private readonly string LAPTOP_CLAIM;
        private readonly string MARINEINLINE_CLAIM;
        private readonly string MARINEIMPORT_CLAIM;
        private readonly string VEHICLE_CLAIM;
        // Get All By Category
        private readonly string BIOMETRIC_ALL;
        private readonly string BSD_ALL;
        private readonly string CASH_ALL;
        private readonly string CELLSITE_ALL;
        private readonly string HANDSET_ALL;
        private readonly string LAPTOP_ALL;
        private readonly string MARINEINLINE_ALL;
        private readonly string MARINEIMPORT_ALL;
        private readonly string VEHICLE_ALL;

        // Get All By Category
        private readonly string MY_REQUEST_REPORT;
        private readonly string MY_PENDING_REQUEST_REPORT;

        public ReportingController(ILogger<CellSitesController> logger , IConfiguration configuration)
        {
            _logger = logger;
            this.SP_HOSTNAME = configuration["SharepointHostname"];
            this.CLAIMS_REPORT = configuration["claimsReport"];
            this.MY_REQUEST_REPORT = configuration["myRequestReport"];
            this.MY_PENDING_REQUEST_REPORT = configuration["myPendingRequestReport"];
            this.AGING_REPORT = configuration["agingReport"];
            this.DEDUCTIBLE_REPORT = configuration["deductibleReport"];
            this.GAINLOSS_REPORT = configuration["gainLossReport"];
            this.SUMMARY_REPORT = configuration["summaryReport"];
            // Initialize Claim Name
            this.BIOMETRIC_CLAIM = configuration.GetValue<string>("Biometric:claimName");
            this.BSD_CLAIM = configuration.GetValue<string>("Bsd:claimName");
            this.CASH_CLAIM = configuration.GetValue<string>("CASH:claimName");
            this.CELLSITE_CLAIM = configuration.GetValue<string>("CELL_SITE:claimName");
            this.HANDSET_CLAIM = configuration.GetValue<string>("Handset:claimName");
            this.LAPTOP_CLAIM = configuration.GetValue<string>("Laptop:claimName");
            this.MARINEIMPORT_CLAIM = configuration.GetValue<string>("MARINE_IMPORT:claimName");
            this.MARINEINLINE_CLAIM = configuration.GetValue<string>("MARINE_INLAND:claimName");
            this.VEHICLE_CLAIM = configuration.GetValue<string>("Vehicle:claimName");
            // Initialize Claim All Endpoint
            this.BIOMETRIC_ALL = configuration.GetValue<string>("Biometric:getAll");
            this.BSD_ALL = configuration.GetValue<string>("Bsd:getAll");
            this.CASH_ALL = configuration.GetValue<string>("CASH:getAll");
            this.CELLSITE_ALL = configuration.GetValue<string>("CELL_SITE:getAll");
            this.HANDSET_ALL = configuration.GetValue<string>("Handset:getAll");
            this.LAPTOP_ALL = configuration.GetValue<string>("Laptop:getAll");
            this.MARINEIMPORT_ALL = configuration.GetValue<string>("MARINE_IMPORT:getAll");
            this.MARINEINLINE_ALL = configuration.GetValue<string>("MARINE_INLAND:getAll");
            this.VEHICLE_ALL = configuration.GetValue<string>("Vehicle:getAll");
        }

        [HttpGet("all")]
        public string GetAllPendingClaim()
        {
            string ClaimType = "", DateTo = "", DateFrom = ""; string KPI="0";
            string GainStartValue = "0", GainEndValue = "0";
            string Region = "", Status = "";
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    RestClient client = new RestClient(SP_HOSTNAME);
                    RestRequest request = new RestRequest();
                    if (Request.QueryString.HasValue)
                    {
                        ClaimType = Request.Query["ClaimType"];
                        DateTo = Request.Query["ClaimTo"];
                        DateFrom = Request.Query["ClaimFrom"];
                        KPI = Request.Query["KPI"];
                        GainStartValue = Request.Query["GainStartValue"];
                        GainEndValue = Request.Query["GainEndValue"];
                        Region = Request.Query["Region"];
                        Status = Request.Query["Status"];

                        if (ClaimType == BIOMETRIC_CLAIM)
                        {
                            request = new RestRequest(BIOMETRIC_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "all");
                            request.AddQueryParameter("KPI", KPI);
                            request.AddQueryParameter("GainStartValue", GainStartValue);
                            request.AddQueryParameter("GainEndValue", GainEndValue);
                            request.AddQueryParameter("Region", Region);
                            request.AddQueryParameter("Status", Status);
                        }
                        else if (ClaimType == BSD_CLAIM)
                        {
                            request = new RestRequest(BSD_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "all");
                            request.AddQueryParameter("KPI", KPI);
                            request.AddQueryParameter("GainStartValue", GainStartValue);
                            request.AddQueryParameter("GainEndValue", GainEndValue);
                            request.AddQueryParameter("Region", Region);
                            request.AddQueryParameter("Status", Status);
                        }
                        else if (ClaimType == CASH_CLAIM)
                        {
                            request = new RestRequest(CASH_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "all");
                            request.AddQueryParameter("KPI", KPI);
                            request.AddQueryParameter("GainStartValue", GainStartValue);
                            request.AddQueryParameter("GainEndValue", GainEndValue);
                            request.AddQueryParameter("Region", Region);
                            request.AddQueryParameter("Status", Status);
                        }
                        else if (ClaimType == CELLSITE_CLAIM)
                        {
                            request = new RestRequest(CELLSITE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "all");
                            request.AddQueryParameter("KPI", KPI);
                            request.AddQueryParameter("GainStartValue", GainStartValue);
                            request.AddQueryParameter("GainEndValue", GainEndValue);
                            request.AddQueryParameter("Region", Region);
                            request.AddQueryParameter("Status", Status);
                        }
                        else if (ClaimType == HANDSET_CLAIM)
                        {
                            request = new RestRequest(HANDSET_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "all");
                            request.AddQueryParameter("KPI", KPI);
                            request.AddQueryParameter("GainStartValue", GainStartValue);
                            request.AddQueryParameter("GainEndValue", GainEndValue);
                            request.AddQueryParameter("Region", Region);
                            request.AddQueryParameter("Status", Status);
                        }
                        else if (ClaimType == LAPTOP_CLAIM)
                        {
                            request = new RestRequest(LAPTOP_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "all"); request.AddQueryParameter("KPI", KPI);
                            request.AddQueryParameter("GainStartValue", GainStartValue);
                            request.AddQueryParameter("GainEndValue", GainEndValue);
                            request.AddQueryParameter("Region", Region);
                            request.AddQueryParameter("Status", Status);
                        }
                        else if (ClaimType == MARINEIMPORT_CLAIM)
                        {
                            request = new RestRequest(MARINEIMPORT_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "all");
                            request.AddQueryParameter("KPI", KPI);
                            request.AddQueryParameter("GainStartValue", GainStartValue);
                            request.AddQueryParameter("GainEndValue", GainEndValue);
                            request.AddQueryParameter("Region", Region);
                            request.AddQueryParameter("Status", Status);
                        }
                        else if (ClaimType == MARINEINLINE_CLAIM)
                        {
                            request = new RestRequest(MARINEINLINE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "all");
                            request.AddQueryParameter("KPI", KPI);
                            request.AddQueryParameter("GainStartValue", GainStartValue);
                            request.AddQueryParameter("GainEndValue", GainEndValue);
                            request.AddQueryParameter("Region", Region);
                            request.AddQueryParameter("Status", Status);
                        }
                        else if (ClaimType == VEHICLE_CLAIM)
                        {
                            request = new RestRequest(VEHICLE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "all");
                            request.AddQueryParameter("KPI", KPI);
                            request.AddQueryParameter("GainStartValue", GainStartValue);
                            request.AddQueryParameter("GainEndValue", GainEndValue);
                            request.AddQueryParameter("Region", Region);
                            request.AddQueryParameter("Status", Status);
                        }
                        else
                        {
                            request = new RestRequest(CLAIMS_REPORT);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("KPI", KPI);
                            request.AddQueryParameter("GainStartValue", GainStartValue);
                            request.AddQueryParameter("GainEndValue", GainEndValue);
                            request.AddQueryParameter("Region", Region);
                            request.AddQueryParameter("Status", Status);
                        }
                    }
                    else
                    {
                        request = new RestRequest(CLAIMS_REPORT);
                    }
                    request.Method = Method.GET;
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
        [HttpGet("myRequests")]
        public string GetAllRequestedClaim()
        {
            string ClaimType = "", DateTo = "", DateFrom = "", Email = "";
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    RestClient client = new RestClient(SP_HOSTNAME);
                    RestRequest request = new RestRequest();
                    if (Request.QueryString.HasValue)
                    {
                        DateTo = Request.Query["ClaimTo"];
                        DateFrom = Request.Query["ClaimFrom"];
                        Email = user_email;//Request.Query["Email"];


                        request = new RestRequest(MY_REQUEST_REPORT);
                        request.AddQueryParameter("DateFrom", DateFrom);
                        request.AddQueryParameter("DateTo", DateTo);
                        request.AddQueryParameter("Email", Email);

                    }
                    else
                    {
                        request = new RestRequest(MY_REQUEST_REPORT);
                    }
                    request.Method = Method.GET;
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

        [HttpGet("getTotalRequestsCount")]
        public string GetTotalRequestsCount()
        {
            string ClaimType = "", DateTo = "", DateFrom = "", Email = "";
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    RestClient client = new RestClient(SP_HOSTNAME);
                    RestRequest request = new RestRequest();
                    if (Request.QueryString.HasValue)
                    {
                        DateTo = Request.Query["ClaimTo"];
                        DateFrom = Request.Query["ClaimFrom"];
                        Email = user_email;// Request.Query["Email"];


                        request = new RestRequest("/api/reporting/getTotalRequestsCount");
                        request.AddQueryParameter("DateFrom", DateFrom);
                        request.AddQueryParameter("DateTo", DateTo);
                        request.AddQueryParameter("Email", Email);

                    }
                    else
                    {
                        request = new RestRequest("/api/reporting/getTotalRequestsCount");
                    }
                    request.Method = Method.GET;
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

        [HttpGet("myPendingRequests")]
        public string GetAllPendingRequestedClaim()
        {
            string ClaimType = "", DateTo = "", DateFrom = "", Scopes = "", ClaimCategory = "";
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    RestClient client = new RestClient(SP_HOSTNAME);
                    RestRequest request = new RestRequest();
                    if (Request.QueryString.HasValue)
                    {
                        DateTo = Request.Query["ClaimTo"];
                        DateFrom = Request.Query["ClaimFrom"];
                        Scopes = Request.Query["Scopes"];
                        ClaimCategory = Request.Query["ClaimCategory"];

                        request = new RestRequest(MY_PENDING_REQUEST_REPORT);
                        request.AddQueryParameter("DateFrom", DateFrom);
                        request.AddQueryParameter("DateTo", DateTo);
                        request.AddQueryParameter("Scopes", Scopes);
                        request.AddQueryParameter("ClaimCategory", ClaimCategory);

                    }
                    else
                    {
                        request = new RestRequest(MY_PENDING_REQUEST_REPORT);
                    }
                    request.Method = Method.GET;
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

        [HttpGet("aging")]
        public string GetAgingClaim()
        {
            string ClaimType = "", DateTo = "", DateFrom = "";
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    RestClient client = new RestClient(SP_HOSTNAME);
                    RestRequest request = new RestRequest();
                    if (Request.QueryString.HasValue)
                    {
                        ClaimType = Request.Query["ClaimType"];
                        DateTo = Request.Query["ClaimTo"];
                        DateFrom = Request.Query["ClaimFrom"];

                        if (ClaimType == BIOMETRIC_CLAIM)
                        {
                            request = new RestRequest(BIOMETRIC_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "aging");
                            request.AddQueryParameter("ClaimType", ClaimType);
                        }
                        else if (ClaimType == BSD_CLAIM)
                        {
                            request = new RestRequest(BSD_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "aging");
                            request.AddQueryParameter("ClaimType", ClaimType);
                        }
                        else if (ClaimType == CASH_CLAIM)
                        {
                            request = new RestRequest(CASH_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "aging");
                            request.AddQueryParameter("ClaimType", ClaimType);
                        }
                        else if (ClaimType == CELLSITE_CLAIM)
                        {
                            request = new RestRequest(CELLSITE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "aging");
                            request.AddQueryParameter("ClaimType", ClaimType);
                        }
                        else if (ClaimType == HANDSET_CLAIM)
                        {
                            request = new RestRequest(HANDSET_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "aging");
                            request.AddQueryParameter("ClaimType", ClaimType);
                        }
                        else if (ClaimType == LAPTOP_CLAIM)
                        {
                            request = new RestRequest(LAPTOP_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "aging");
                            request.AddQueryParameter("ClaimType", ClaimType);
                        }
                        else if (ClaimType == MARINEIMPORT_CLAIM)
                        {
                            request = new RestRequest(MARINEIMPORT_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "aging");
                            request.AddQueryParameter("ClaimType", ClaimType);
                        }
                        else if (ClaimType == MARINEINLINE_CLAIM)
                        {
                            request = new RestRequest(MARINEINLINE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "aging");
                            request.AddQueryParameter("ClaimType", ClaimType);
                        }
                        else if (ClaimType == VEHICLE_CLAIM)
                        {
                            request = new RestRequest(VEHICLE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "aging");
                            request.AddQueryParameter("ClaimType", ClaimType);
                        }
                        else
                        {
                            request = new RestRequest(AGING_REPORT);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ClaimType", ClaimType);
                        }
                    }
                    else
                    {
                        request = new RestRequest(AGING_REPORT);
                    }
                    request.Method = Method.GET;
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

        [HttpGet("deductible-report")]
        public string GetDeductibleReport()
        {
            string ClaimType = "", DateTo = "", DateFrom = "";
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    RestClient client = new RestClient(SP_HOSTNAME);
                    RestRequest request = new RestRequest();
                    if (Request.QueryString.HasValue)
                    {
                        ClaimType = Request.Query["ClaimType"];
                        DateTo = Request.Query["ClaimTo"];
                        DateFrom = Request.Query["ClaimFrom"];

                        if (ClaimType == BIOMETRIC_CLAIM)
                        {
                            request = new RestRequest(BIOMETRIC_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "deductible");
                        }
                        else if (ClaimType == BSD_CLAIM)
                        {
                            request = new RestRequest(BSD_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "deductible");
                        }
                        else if (ClaimType == CASH_CLAIM)
                        {
                            request = new RestRequest(CASH_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "deductible");
                        }
                        else if (ClaimType == CELLSITE_CLAIM)
                        {
                            request = new RestRequest(CELLSITE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "deductible");
                        }
                        else if (ClaimType == HANDSET_CLAIM)
                        {
                            request = new RestRequest(HANDSET_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "deductible");
                        }
                        else if (ClaimType == LAPTOP_CLAIM)
                        {
                            request = new RestRequest(LAPTOP_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "deductible");
                        }
                        else if (ClaimType == MARINEIMPORT_CLAIM)
                        {
                            request = new RestRequest(MARINEIMPORT_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "deductible");
                        }
                        else if (ClaimType == MARINEINLINE_CLAIM)
                        {
                            request = new RestRequest(MARINEINLINE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "deductible");
                        }
                        else if (ClaimType == VEHICLE_CLAIM)
                        {
                            request = new RestRequest(VEHICLE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "deductible");
                        }
                        else
                        {
                            request = new RestRequest(DEDUCTIBLE_REPORT);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                        }
                    }
                    else
                    {
                        request = new RestRequest(DEDUCTIBLE_REPORT);
                    }
                    request.Method = Method.GET;
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

        [HttpGet("lossgain-report")]
        public string GetLossGainReport()
        {
            string ClaimType = "", DateTo = "", DateFrom = "";
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    RestClient client = new RestClient(SP_HOSTNAME);
                    RestRequest request = new RestRequest();
                    if (Request.QueryString.HasValue)
                    {
                        ClaimType = Request.Query["ClaimType"];
                        DateTo = Request.Query["ClaimTo"];
                        DateFrom = Request.Query["ClaimFrom"];

                        if (ClaimType == BIOMETRIC_CLAIM)
                        {
                            request = new RestRequest(BIOMETRIC_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "lossgain");
                        }
                        else if (ClaimType == BSD_CLAIM)
                        {
                            request = new RestRequest(BSD_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "lossgain");
                        }
                        else if (ClaimType == CASH_CLAIM)
                        {
                            request = new RestRequest(CASH_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "lossgain");
                        }
                        else if (ClaimType == CELLSITE_CLAIM)
                        {
                            request = new RestRequest(CELLSITE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "lossgain");
                        }
                        else if (ClaimType == HANDSET_CLAIM)
                        {
                            request = new RestRequest(HANDSET_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "lossgain");
                        }
                        else if (ClaimType == LAPTOP_CLAIM)
                        {
                            request = new RestRequest(LAPTOP_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "lossgain");
                        }
                        else if (ClaimType == MARINEIMPORT_CLAIM)
                        {
                            request = new RestRequest(MARINEIMPORT_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "lossgain");
                        }
                        else if (ClaimType == MARINEINLINE_CLAIM)
                        {
                            request = new RestRequest(MARINEINLINE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "lossgain");
                        }
                        else if (ClaimType == VEHICLE_CLAIM)
                        {
                            request = new RestRequest(VEHICLE_ALL);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                            request.AddQueryParameter("ApiType", "lossgain");
                        }
                        else
                        {
                            request = new RestRequest(GAINLOSS_REPORT);
                            request.AddQueryParameter("DateFrom", DateFrom);
                            request.AddQueryParameter("DateTo", DateTo);
                        }
                    }
                    else
                    {
                        request = new RestRequest(GAINLOSS_REPORT);
                    }
                    request.Method = Method.GET;
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

        [HttpGet("summary-report")]
        public string SummaryGainReport()
        {
            string ClaimTypes = "";
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    RestClient client = new RestClient(SP_HOSTNAME);
                    RestRequest request = new RestRequest();
                    request = new RestRequest(SUMMARY_REPORT);
                    if (Request.QueryString.HasValue)
                    {
                        ClaimTypes = Request.Query["ClaimTypes"];
                        //ClaimStatus = Request.Query["ClaimStatus"];
                        request.AddQueryParameter("ClaimTypes", ClaimTypes);
                        //request.AddQueryParameter("ClaimStatus", ClaimStatus);
                    }
                    request.Method = Method.GET;
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

