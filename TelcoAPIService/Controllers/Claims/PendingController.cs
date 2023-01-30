using Microsoft.SharePoint.Client;
using TelcoAPIService.Models;
using System.Collections.Generic;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using Config = System.Configuration.ConfigurationManager;
using System.Net.Http;
using System;
using System.Linq;

namespace TelcoAPIService.Controllers
    {
    [System.Web.Http.RoutePrefix("api/pending")]
    public class PendingController : BaseController
        {
        private readonly string WORKFLOW_ARCHIVES = Config.AppSettings["WF_ARCHIVES"];

        // CLAIMS TITLE
        private readonly string CELLSITE_TITLE = Config.AppSettings["CELLSITE_TITLE"];
        private readonly string MARINEINLAND_TITLE = Config.AppSettings["MARINEINLAND_TITLE"];
        private readonly string CASH_TITLE = Config.AppSettings["CASH_TITLE"];

        // CLAIMS ACTION
        private readonly string APPROVE_ACTION = Config.AppSettings["APPROVE_ACTION"];
        private readonly string SUBMIT_ACTION = Config.AppSettings["SUBMIT_ACTION"];

        // CLAIMS 
        private readonly string POC_STATE = Config.AppSettings["POC_STATE"];
        private readonly string FIXEDASSET_STATE = Config.AppSettings["FIXEDASSET_STATE"];
        private readonly string FINANCE_TEAM_STATE = Config.AppSettings["FINANCE_TEAM_STATE"];

        private readonly string WF_VENDORS = Config.AppSettings["WF_VENDORS"];

        [HttpGet]
        [Route("list")]
        public List<Dictionary<string, string>> GetAll()
            {

            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> ScopeBasedFilteredResponse = new List<Dictionary<string, string>>();
            ListItemCollection items = GetSharePointPendingWorkflowList(WORKFLOW_ARCHIVES);
            //var rest = items.Select(i => i["ClaimID"].ToString()).Distinct();
            var UniqueClaim = items.GroupBy(x => x["ClaimID"]).Select(y => y.First()).Distinct();
            var user_email = System.Web.HttpContext.Current.Request.QueryString.Get(0).ToString();
            ClientContext clientContext = GetSharePointAuth();
            string VendorList = GetVendorRoles(clientContext, user_email);
            //List<string> response = new List<string>();
            foreach (ListItem claim in UniqueClaim)
                {
                if (claim["ClaimAction"] != null && claim["ClaimState"] != null)
                    {
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    if (claim["Title"].ToString() == CELLSITE_TITLE && claim["ClaimAction"].ToString() == SUBMIT_ACTION && (claim["ClaimState"].ToString() == FIXEDASSET_STATE))
                        {
                        newItem.Add("Title", claim["Title"].ToString());
                        newItem.Add("ClaimID", claim["ClaimID"].ToString());
                        newItem.Add("ClaimStatus", "Pending");
                        if (claim["UserID"] != null)
                            newItem.Add("UserID", claim["UserID"].ToString());
                        else
                            newItem.Add("UserID", "");
                        newItem.Add("Created", claim["Created"].ToString());
                        }
                    else if ((claim["Title"].ToString() == MARINEINLAND_TITLE || claim["Title"].ToString() == CASH_TITLE) && claim["ClaimAction"].ToString() == APPROVE_ACTION && (claim["ClaimState"].ToString() == FINANCE_TEAM_STATE))
                        {
                        newItem.Add("Title", claim["Title"].ToString());
                        newItem.Add("ClaimStatus", "Pending");
                        newItem.Add("ClaimID", claim["ClaimID"].ToString());
                        if (claim["UserID"] != null)
                            newItem.Add("UserID", claim["UserID"].ToString());
                        else
                            newItem.Add("UserID", "");
                        newItem.Add("Created", claim["Created"].ToString());
                        }
                    else if ((claim["Title"].ToString() != MARINEINLAND_TITLE && claim["Title"].ToString() != CASH_TITLE && claim["Title"].ToString() != CELLSITE_TITLE) && claim["ClaimAction"].ToString() == APPROVE_ACTION)
                        {
                        if (claim["ClaimState"].ToString() == "HOD")
                            {
                            newItem.Add("Title", claim["Title"].ToString());
                            newItem.Add("ClaimStatus", "Pending");
                            newItem.Add("ClaimID", claim["ClaimID"].ToString());
                            if (claim["UserID"] != null)
                                newItem.Add("UserID", claim["UserID"].ToString());
                            else
                                newItem.Add("UserID", "");
                            newItem.Add("Created", claim["Created"].ToString());
                            }

                        else if (claim["ClaimState"].ToString() == POC_STATE)
                            {
                            var id = "";
                            if (claim["Title"].ToString() == "Laptop")
                                {
                                id = Config.AppSettings["SP_LAPTOP"];
                                }
                            if (claim["Title"].ToString() == "Highvalue Tools")
                                {
                                id = Config.AppSettings["SP_BSD"];
                                }
                            if (claim["Title"].ToString() == "Handsets")
                                {
                                id = Config.AppSettings["SP_HANDSET"];
                                }
                            if (claim["Title"].ToString() == "Motor Vehicles")
                                {
                                id = Config.AppSettings["SP_VEHICLE"];
                                }
                            if (claim["Title"].ToString() == "Marine Imports")
                                {
                                id = Config.AppSettings["SP_MARINE_IMPORT"];
                                }
                            if (claim["Title"].ToString() == "Biometric Devices")
                                {
                                id = Config.AppSettings["SP_BIOMETRIC"];
                                }
                            ListItemCollection ExceptionalItems = FetchExceptionInfo(id, claim["ClaimID"].ToString());
                            if (ExceptionalItems != null)
                                {
                                if (ExceptionalItems.Count == 1)
                                    {
                                    newItem.Add("Title", claim["Title"].ToString());
                                    newItem.Add("ClaimStatus", "Pending");
                                    newItem.Add("ClaimID", claim["ClaimID"].ToString());
                                    if (claim["UserID"] != null)
                                        newItem.Add("UserID", claim["UserID"].ToString());
                                    else
                                        newItem.Add("UserID", "");
                                    newItem.Add("Created", claim["Created"].ToString());
                                    }
                                }
                            }

                        else if ((claim["ClaimAction"].ToString() == APPROVE_ACTION && claim["ClaimState"].ToString() == "Vendor"))
                            {
                            newItem.Add("Title", claim["Title"].ToString());
                            newItem.Add("ClaimStatus", "Approve");
                            newItem.Add("ClaimID", claim["ClaimID"].ToString());
                            if (claim["UserID"] != null)
                                newItem.Add("UserID", claim["UserID"].ToString());
                            else
                                newItem.Add("UserID", "");
                            newItem.Add("Created", claim["Created"].ToString());
                            }
                        }

                    else if ((claim["ClaimAction"].ToString() == APPROVE_ACTION && claim["ClaimState"].ToString() == "Vendor"))
                        {
                        newItem.Add("Title", claim["Title"].ToString());
                        newItem.Add("ClaimStatus", "Approve");
                        newItem.Add("ClaimID", claim["ClaimID"].ToString());
                        if (claim["UserID"] != null)
                            newItem.Add("UserID", claim["UserID"].ToString());
                        else
                            newItem.Add("UserID", "");
                        newItem.Add("Created", claim["Created"].ToString());
                        }

                    if (newItem.Count > 0)
                        {
                        Response.Add(newItem);
                        }
                    }
                }
            foreach (var item in Response)
                {
                if (VendorList.Contains("Biometric_Devices-Vendor"))
                    {
                    if (item["Title"].ToString() == "Biometric Devices")
                        {
                        ScopeBasedFilteredResponse.Add(item);
                        }
                    }
                if (VendorList.Contains("Cash_in_Safe_Claims-Vendor"))
                    {
                    if (item["Title"].ToString() == "Cash in Safe Claims")
                        {
                        ScopeBasedFilteredResponse.Add(item);
                        }
                    }
                if (VendorList.Contains("Laptop-Vendor"))
                    {
                    if (item["Title"].ToString() == "Laptop")
                        {
                        ScopeBasedFilteredResponse.Add(item);
                        }
                    }
                if (VendorList.Contains("Cell_Site_Claims-Vendor"))
                    {
                    if (item["Title"].ToString() == "Cell Site Claims")
                        {
                        ScopeBasedFilteredResponse.Add(item);
                        }
                    }
                if (VendorList.Contains("Motor_Vehicles-Vendor"))
                    {
                    if (item["Title"].ToString() == "Motor Vehicles")
                        {
                        ScopeBasedFilteredResponse.Add(item);
                        }
                    }
                if (VendorList.Contains("Handsets-Vendor"))
                    {
                    if (item["Title"].ToString() == "Handsets")
                        {
                        ScopeBasedFilteredResponse.Add(item);
                        }
                    }
                if (VendorList.Contains("Marine_Inland-Vendor"))
                    {
                    if (item["Title"].ToString() == "Marine Inland")
                        {
                        ScopeBasedFilteredResponse.Add(item);
                        }
                    }
                if (VendorList.Contains("Highvalue_Tools-Vendor"))
                    {
                    if (item["Title"].ToString() == "Highvalue Tools")
                        {
                        ScopeBasedFilteredResponse.Add(item);
                        }
                    }
                if (VendorList.Contains("Marine_Imports-Vendor"))
                    {
                    if (item["Title"].ToString() == "Marine Imports")
                        {
                        ScopeBasedFilteredResponse.Add(item);
                        }
                    }
                }


            return ScopeBasedFilteredResponse;
            }
        protected ListItemCollection GetSharePointPendingWorkflowList(string listName)
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(listName));

            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query> 
                        <OrderBy>
                            <FieldRef Name='Created' Ascending='FALSE' />
                        </OrderBy>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='ClaimID'/>
                        <FieldRef Name='Title'/>
                        <FieldRef Name='ClaimState'/>
                        <FieldRef Name='ClaimAction'/>
                        <FieldRef Name='UserID'/>
                        <FieldRef Name='Created'/>
                    </ViewFields>
                </View>"
                };

            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }

        public string GetVendorRoles(ClientContext clientContext, string user_email)
            {
            List initiatorList = clientContext.Web.Lists.GetById(new Guid(WF_VENDORS));
            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                <Query>
                    <Where>
                        <Or>
                            <Eq><FieldRef Name='Email'/><Value Type='Text'>" + user_email + @"</Value></Eq>
                            <Eq><FieldRef Name='Email'/><Value Type='Text'>*</Value></Eq>
                        </Or>
                    </Where>
                </Query>
                <ViewFields>
                    <FieldRef Name='Region'/>
                    <FieldRef Name='Workflow' />
                </ViewFields>
            </View>"
                };

            ListItemCollection listItems = initiatorList.GetItems(camlQuery);

            /*if (listItems == null)
            {
                return "";
            }*/

            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            if (!listItems.AreItemsAvailable)
                {
                return "";
                }


            string response = null;
            foreach (ListItem item in listItems)
                {
                string RequiredScope = item["Workflow"].ToString().Replace(" ", "_") + "-Vendor";
                if (response == null)
                    {
                    response += RequiredScope + ",";
                    }
                else
                    {
                    if (response.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(RequiredScope) is false)
                        {
                        response += RequiredScope + ",";
                        }
                    }
                }
            return response;
            }
        }
    }
