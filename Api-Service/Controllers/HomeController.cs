
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Newtonsoft.Json.Linq;
using ApiService.Helpers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly string GetSharePointfiles;
    private readonly string GetSharePointDamagedfiles;
    private readonly string SPHostname;
    private readonly string GetSharePointInitiatorsEP;
    private readonly string GetSharePointApproversEP;
    private readonly string GetSharePointVendorsEP;

    public HomeController(IConfiguration configuration)
    {

        this.SPHostname = configuration["SharepointHostname"];
        this.GetSharePointfiles = configuration.GetValue<string>("getSharePointfiles");
        this.GetSharePointDamagedfiles = configuration.GetValue<string>("getSharePointDamagedfiles");
        this.GetSharePointInitiatorsEP = configuration.GetValue<string>("getSharePointInitiators");
        this.GetSharePointApproversEP = configuration.GetValue<string>("getSharePointApprovers");
        this.GetSharePointVendorsEP = configuration.GetValue<string>("getSharePointVendors");

    }

    public IActionResult Error()
        {
            // Get the details of the exception that occurred
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                // Get which route the exception occurred at
                string routeWhereExceptionOccurred = exceptionFeature.Path;

                // Get the exception that occurred
                Exception exceptionThatOccurred = exceptionFeature.Error;
            }

            return NotFound(new {code = 404, message = "Not Found" });
        }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { code = 200, message = "API Service is running..." });
    }

    [HttpGet("get-files/{ClaimID}/{ListName}")]
    public string GetSharePointFiles(int ClaimID,string ListName)
    {
        try
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            string user_email = AuthUtils.GetUserEmail(token);
            if (user_email != null)
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SPHostname);
                var request = new RestRequest(GetSharePointfiles + "/" + ClaimID + "/" + ListName);
                request.Method = Method.GET;
                var Content = client.Execute(request).Content;
                JObject ResponseObject = JObject.Parse(Content);
                ResponseObject["BasePath"] = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/content/";
                return ResponseObject.ToString();
            }
            return null;
        }
        catch (Exception)
        {
            return "Error retrieving data from the database";
        }
    }
    [HttpGet("get-damaged-files/{ClaimID}")]
    public string GetSharePointDamagedFiles(int ClaimID)
    {
        try
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            string user_email = AuthUtils.GetUserEmail(token);
            if (user_email != null)
            {
            RestClient obj = new RestClient();
            var client = new RestClient(SPHostname);
            var request = new RestRequest(GetSharePointDamagedfiles + "/" + ClaimID);
            request.Method = Method.GET;
            var Content = client.Execute(request).Content;
            JObject ResponseObject = JObject.Parse(Content);
            ResponseObject["BasePath"] = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/content/";
            return ResponseObject.ToString();
            }
            return null;
        }
        catch (Exception)
        {
            return "Error retrieving data from the database";
        }
    }
    [HttpGet("get-initiators")]
    public string GetSharePointInitiators()
    {
        try
        {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
                string user_email = AuthUtils.GetUserEmail(token);
                if (user_email != null)
                {
                    RestClient obj = new RestClient();
                    var client = new RestClient(SPHostname);
                    var request = new RestRequest(GetSharePointInitiatorsEP);
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
    [HttpGet("get-approvers")]
    public string GetSharePointApprovers(int ClaimID)
    {
        try
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            string user_email = AuthUtils.GetUserEmail(token);
            if (user_email != null)
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SPHostname);
                var request = new RestRequest(GetSharePointApproversEP);
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
    [HttpGet("get-vendors")]
    public string GetSharePointVendors(int ClaimID)
    {
        try
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            string user_email = AuthUtils.GetUserEmail(token);
            if (user_email != null)
            {
                RestClient obj = new RestClient();
                var client = new RestClient(SPHostname);
                var request = new RestRequest(GetSharePointVendorsEP);
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