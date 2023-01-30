using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using Config = System.Configuration.ConfigurationManager;
using System.Web;
using System;
using System.Linq;
using TelcoAPIService.Models;
using System.Net.Http;
using System.Net;
using System.Web.Http;

namespace TelcoAPIService.Controllers
    {
    [System.Web.Http.RoutePrefix("api/wfauth")]
    public class WFAuthController : BaseController
        {
        private readonly string WF_INITIATORS = Config.AppSettings["WF_INITIATORS"];
        private readonly string WF_APPROVERS = Config.AppSettings["WF_APPROVERS"];
        private readonly string WF_VENDORS = Config.AppSettings["WF_VENDORS"];
        // WORKFLOW_KEYS
        private readonly string SP_REGION = Config.AppSettings["SP_REGION"];
        private readonly string SP_TITLE = Config.AppSettings["SP_TITLE"];
        private readonly string SP_EMAIL = Config.AppSettings["SP_EMAIL"];
        private readonly string SP_WORKFLOW = Config.AppSettings["SP_WORKFLOW"];
        private readonly string SP_DEPT = Config.AppSettings["SP_DEPT"];

        #region ACTIONS

        [HttpGet]
        [Route("get-initiators")]
        public string[] GetAllInitiators()
            {
            ListItemCollection items = GetSharePointList(WF_INITIATORS);
            List<string> response = new List<string>();
            foreach (ListItem item in items)
                {
                response.Add(item["Initiator_Email"].ToString());
                }

            return response.ToArray();
            }

        [HttpGet]
        [Route("get-roles")]
        public string GetAllRoles()
            {
            var user_email = HttpContext.Current.Request.QueryString.Get(0).ToString();
            ClientContext clientContext = GetSharePointAuth();
            string InitiatorList = GetInitiatorRoles(clientContext, user_email);
            string ApproverList = GetApproverRoles(clientContext, user_email);
            string VendorList = GetVendorRoles(clientContext, user_email);
            return InitiatorList + ApproverList + VendorList;
            }
        

        [HttpPost]
        [Route("add-initiator")]
        public HttpResponseMessage AddNewInitiator(NewInitiatorModel initiatorModel)
            {
            try
                {
                ClientContext clientContext = GetSharePointAuth();
                var SPClaims = clientContext.Web.Lists.GetById(new Guid(WF_INITIATORS));
                ListItem SPClaim = SPClaims.AddItem(new ListItemCreationInformation());
                SPClaim[SP_EMAIL] = initiatorModel.Email;
                SPClaim[SP_REGION] = initiatorModel.Region;
                SPClaim[SP_TITLE] = initiatorModel.Title;
                SPClaim[SP_WORKFLOW] = initiatorModel.Workflow;
                SPClaim.Update();
                clientContext.ExecuteQuery();
                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new
                        {
                        Success = true,
                        Message = "Record inserted"
                        }, Configuration.Formatters.JsonFormatter
                    );
                }
            catch(Exception ex)
                {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new
                        {
                        success = false,
                        message = "Request failed, please try again."
                        }
                    );
                }
            }
        

        [HttpPost]
        [Route("add-vendor")]
        public HttpResponseMessage AddNewVendor(NewVendorModel vendorModel)
            {
            try
                {
                ClientContext clientContext = GetSharePointAuth();
                var SPClaims = clientContext.Web.Lists.GetById(new Guid(WF_VENDORS));
                ListItem SPClaim = SPClaims.AddItem(new ListItemCreationInformation());
                SPClaim[SP_EMAIL] = vendorModel.Email;
                SPClaim[SP_REGION] = vendorModel.Region;
                SPClaim[SP_TITLE] = vendorModel.Title;
                SPClaim[SP_WORKFLOW] = vendorModel.Workflow;
                SPClaim.Update();
                clientContext.ExecuteQuery();
                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new
                        {
                        Success = true,
                        Message = "Record inserted"
                        }, Configuration.Formatters.JsonFormatter
                    );
                }
            catch(Exception ex)
                {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new
                        {
                        success = false,
                        message = "Request failed, please try again."
                        }
                    );
                }
            }
        

        [HttpPost]
        [Route("add-approval")]
        public HttpResponseMessage AddNewApproval(NewApprovalModel approvalModel)
            {
            try
                {
                ClientContext clientContext = GetSharePointAuth();
                var SPClaims = clientContext.Web.Lists.GetById(new Guid(WF_APPROVERS));
                ListItem SPClaim = SPClaims.AddItem(new ListItemCreationInformation());
                SPClaim[SP_EMAIL] = approvalModel.Email;
                SPClaim[SP_REGION] = approvalModel.Region;
                SPClaim[SP_TITLE] = approvalModel.Title;
                SPClaim[SP_WORKFLOW] = approvalModel.Workflow;
                SPClaim[SP_DEPT] = approvalModel.Department;
                SPClaim.Update();
                clientContext.ExecuteQuery();
                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new
                        {
                        Success = true,
                        Message = "Record inserted"
                        }, Configuration.Formatters.JsonFormatter
                    );
                }
            catch(Exception ex)
                {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new
                        {
                        success = false,
                        message = "Request failed, please try again."
                        }
                    );
                }
            }
        

        [HttpDelete]
        [Route("delete-initiator/{itemId}")]
        public HttpResponseMessage DeleteInitiator(string itemId)
            {
            try
                {
                ClientContext clientContext = GetSharePointAuth();
                List SPClaims = clientContext.Web.Lists.GetById(new Guid(WF_INITIATORS));
                ListItem SPClaim = SPClaims.GetItemById(itemId);
                SPClaim.DeleteObject();
                clientContext.ExecuteQuery();

                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new
                        {
                        Success = true,
                        Message = "Record Deleted"
                        }, Configuration.Formatters.JsonFormatter
                    );
                }
            catch(Exception ex)
                {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new
                        {
                        success = false,
                        message = "Request failed, please try again."
                        }
                    );
                }
            }
        

        [HttpDelete]
        [Route("delete-vendor/{itemId}")]
        public HttpResponseMessage DeleteVendor(string itemId)
            {
            try
                {
                ClientContext clientContext = GetSharePointAuth();
                List SPClaims = clientContext.Web.Lists.GetById(new Guid(WF_VENDORS));
                ListItem SPClaim = SPClaims.GetItemById(itemId);
                SPClaim.DeleteObject();
                clientContext.ExecuteQuery();
                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new
                        {
                        Success = true,
                        Message = "Record Deleted"
                        }, Configuration.Formatters.JsonFormatter
                    );
                }
            catch(Exception ex)
                {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new
                        {
                        success = false,
                        message = "Request failed, please try again."
                        }
                    );
                }
            }
        

        [HttpDelete]
        [Route("delete-approval/{itemId}")]
        public HttpResponseMessage DeleteApproval(string itemId)
            {
            try
                {
                ClientContext clientContext = GetSharePointAuth();
                List SPClaims = clientContext.Web.Lists.GetById(new Guid(WF_APPROVERS));
                ListItem SPClaim = SPClaims.GetItemById(itemId);
                SPClaim.DeleteObject();
                clientContext.ExecuteQuery();

                return Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new
                        {
                        Success = true,
                        Message = "Record Deleted"
                        }, Configuration.Formatters.JsonFormatter
                    );
                }
            catch(Exception ex)
                {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new
                        {
                        success = false,
                        message = "Request failed, please try again."
                        }
                    );
                }
            }

        #endregion

        #region CUSTOM_METHODS

        private string GetInitiatorRoles(ClientContext clientContext, string user_email)
            {
            List initiatorList = clientContext.Web.Lists.GetById(new Guid(WF_INITIATORS));
            var camlQuery = new CamlQuery
                {
                //camlQuery.ViewXml = @"<View Scope='RecursiveAll'>
                //<Query>
                //    <Where>
                //        <Or>
                //            <Eq><FieldRef Name='Initiator_Email'/><Value Type='Text'>" + user_email + @"</Value></Eq>
                //            <Eq><FieldRef Name='Initiator_Email'/><Value Type='Text'>*</Value></Eq>
                //        </Or>
                //    </Where>
                //</Query>
                //<ViewFields>
                //    <FieldRef Name='Initiator_Region'/>
                //    <FieldRef Name='Workflow' />
                //</ViewFields>
                //</View>";
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
                string RequiredScope = item["Workflow"].ToString().Replace(" ", "_") + "-Initiator";
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

        private string GetApproverRoles(ClientContext clientContext, string user_email)
            {
            List initiatorList = clientContext.Web.Lists.GetById(new Guid(WF_APPROVERS));
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
                    <FieldRef Name='Department' />
                    <FieldRef Name='Priority' />
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
                string RequiredScope = item["Workflow"].ToString().Replace(" ", "_") + "-" + item["Department"].ToString().Replace(" ", "_") + "-Priority:::" + item["Priority"].ToString();
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
        #endregion
        }
    }
