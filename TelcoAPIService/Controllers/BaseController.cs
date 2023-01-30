using System.Web.Http;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Net;
using System.IO;
using Config = System.Configuration.ConfigurationManager;
using File = Microsoft.SharePoint.Client.File;
using TelcoAPIService.Models;
using System;
using System.Linq;

namespace TelcoAPIService.Controllers
{
    public class BaseController : ApiController
    {
        private readonly string WF_ARCHIVES = Config.AppSettings["WF_ARCHIVES"];
        private readonly string WF_INITIATORS = Config.AppSettings["WF_INITIATORS"];
        private readonly string WF_APPROVERS = Config.AppSettings["WF_APPROVERS"];
        private readonly string WF_VENDORS = Config.AppSettings["WF_VENDORS"];


        #region VIRTUAL_METHODS
        public virtual void GetNewClaimParams(BaseModel model, ListItem item)
        {
        }

        #endregion

        #region ACTIONS

        [Route("")]
        [Route("api/")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new { success = true, message = "SharePoint service is running..." }
            );
        }

        [Route("api/get-files/{ClaimID}/{ListName}")]
        [HttpGet]
        public HttpResponseMessage GetSharePointFiles(int ClaimID, string ListName)
        {
            ResponseModel response = new ResponseModel
            {
                Data = GetSharePointFilesInternal(ClaimID,ListName)
            };

            if (response.Data != null)
            {
                response.Message = response.Data.Count.ToString() + " Files retrieved successfully";
            }
            if (response.Data.Count >= 1)
            {
                response.Success = true;
                response.Files = response.Data.Count;
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            else if (response.Data.Count == 0)
            {
                response.Success = true;
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

            }
            else
            { 
                response.Success = false;
                response.Message = "Request failed, please try again.";
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }
        }

        [Route("api/get-initiators")]
        [HttpGet]
        public List<Dictionary<string, string>> GetSharePointInitiators()
            {
            return GetSharePointInitiatorsData();
            }
        [Route("api/get-approvers")]
        [HttpGet]
        public List<Dictionary<string, string>> GetSharePointApprovers()
            {
            return GetSharePointApproversData();
            }
        [Route("api/get-vendors")]
        [HttpGet]
        public List<Dictionary<string, string>> GetSharePointVendors()
            {
            return GetSharePointVendorsData();
            }



        [Route("api/get-damaged-files/{ClaimID}")]
        [HttpGet]
        public HttpResponseMessage GetSharePointDamagedFiles(int ClaimID)
            {
            ResponseModel response = new ResponseModel
                {
                Data = GetSharePointDamagedFilesInternal(ClaimID)
                };

            if (response.Data != null)
                {
                response.Message = response.Data.Count.ToString() + " Files retrieved successfully";
                }
            if (response.Data.Count >= 1)
                {
                response.Success = true;
                response.Files = response.Data.Count;
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
                }
            else if (response.Data.Count == 0)
                {
                response.Success = true;
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

                }
            else
                {
                response.Success = false;
                response.Message = "Request failed, please try again.";
                return Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }
            }

        [Route("api/delete-files/{ID}")]
        [HttpDelete]
        public HttpResponseMessage DeleteSharePointFiles(int Id)
        {
            var response = new {
                Success = true,
                Message = "Files Deleted Successfully"
            };
            return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
        }

        #endregion

        #region SHAREPOINT
        protected SecureString GetSecureString(string userPassword)
        {
            SecureString securePassword = new SecureString();
            foreach (char c in userPassword.ToCharArray())
            {
                securePassword.AppendChar(c);
            }

            return securePassword;
        }

        protected ClientContext GetSharePointAuth()
        {
            string siteUrl = Config.AppSettings["SP_WEBSITE_URL"];
            string username = Config.AppSettings["SP_USERNAME"];
            string password = Config.AppSettings["SP_PASSWORD"];

            return new ClientContext(siteUrl)
            {
                Credentials = new NetworkCredential(username, GetSecureString(password))
            };
        }

        protected ClientContext GetSharePointAuth2()
        {
            string siteUrl = Config.AppSettings["SP_WEBSITE_URL2"];
            string username = Config.AppSettings["SP_USERNAME"];
            string password = Config.AppSettings["SP_PASSWORD"];

            return new ClientContext(siteUrl)
            {
                Credentials = new NetworkCredential(username, GetSecureString(password))
            };
        }

        protected ListItemCollection GetSharePointList(string listName, int limit = 100)
        {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(listName));

            CamlQuery camlQuery = new CamlQuery
            {
                ViewXml = "<View><RowLimit>" + limit + "</RowLimit></View>"
            };

            ListItemCollection collListItem = oList.GetItems(camlQuery);

            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();

            return collListItem;
        }

        protected ListItem GetSharePointListItem(string listType, string wf_archives, string listName, int itemId)
        {
            ClientContext clientContext = GetSharePointAuth();


            List oList = clientContext.Web.Lists.GetById(new Guid(listName));
            ListItem collListItem = oList.GetItemById(itemId);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();

            List wfList = clientContext.Web.Lists.GetById(new Guid(wf_archives));
            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query> 
                        <Where>
                            <And>
                                <Eq><FieldRef Name='ClaimID'/><Value Type='Text'>" + itemId + @"</Value></Eq>
                                <Eq><FieldRef Name='Title'/><Value Type='Text'>" + listType + @"</Value></Eq>
                            </And>
                        </Where>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='ClaimState'/>
                        <FieldRef Name='Created'/>
                    </ViewFields>
                </View>"
                };
            ListItemCollection wflListItem = wfList.GetItems(camlQuery);
            clientContext.Load(wflListItem);
            clientContext.ExecuteQuery();

            if (wflListItem.Count > 0)
            {
                for (int i = 0; i < wflListItem.Count; i++)
                    {
                    if(wflListItem[i]["ClaimState"]!=null)
                        {
                        if (wflListItem[i]["ClaimState"].ToString() == "POC" && (!collListItem.FieldValues.ContainsKey("POCInitiatedTime")))
                            collListItem.FieldValues.Add("POCInitiatedTime", wflListItem[i]["Created"].ToString());
                        else if (wflListItem[i]["ClaimState"].ToString() == "Vendor" && (!collListItem.FieldValues.ContainsKey("VendorInitiatedTime")))
                            collListItem.FieldValues.Add("VendorInitiatedTime", wflListItem[i]["Created"].ToString());
                        else if (wflListItem[i]["ClaimState"].ToString() == "Finance" && (!collListItem.FieldValues.ContainsKey("FinanceInitiatedTime")))
                            collListItem.FieldValues.Add("FinanceInitiatedTime", wflListItem[i]["Created"].ToString());
                        else if (wflListItem[i]["ClaimState"].ToString() == "Finance Team" && (!collListItem.FieldValues.ContainsKey("FinanceTeamInitiatedTime")))
                            collListItem.FieldValues.Add("FinanceTeamInitiatedTime", wflListItem[i]["Created"].ToString());
                        else if (wflListItem[i]["ClaimState"].ToString() == "FixedAsset" && (!collListItem.FieldValues.ContainsKey("FixedAssetInitiatedTime")))
                            collListItem.FieldValues.Add("FixedAssetInitiatedTime", wflListItem[i]["Created"].ToString());
                        else
                            continue;
                        }
                    else
                        continue;
                    }
            }

            if (!collListItem.FieldValues.ContainsKey("POCInitiatedTime"))
                collListItem.FieldValues.Add("POCInitiatedTime", "");
            if (!collListItem.FieldValues.ContainsKey("VendorInitiatedTime"))
                collListItem.FieldValues.Add("VendorInitiatedTime", "");
            if (!collListItem.FieldValues.ContainsKey("FinanceInitiatedTime"))
                collListItem.FieldValues.Add("FinanceInitiatedTime", "");
            if (!collListItem.FieldValues.ContainsKey("FinanceTeamInitiatedTime"))
                collListItem.FieldValues.Add("FinanceTeamInitiatedTime", "");
            if (!collListItem.FieldValues.ContainsKey("FixedAssetInitiatedTime"))
                collListItem.FieldValues.Add("FixedAssetInitiatedTime", "");

            return collListItem;
        }
        public string GetDeductibleValue(ListItemCollection Items,string valueType="")
            {
            Int64 FinanceDeductibleValue = 0;
            Int64 InsuranceDeductibleValue = 0;
            Int64 DeductibleValue = 0;
            if(valueType == "InsuranceCompanyDeductible")
                {
                foreach (ListItem Item in Items)
                    {
                    if (Item.FieldValues["InsuranceCompanyDeductible"] != null)
                        {
                        if (Item.FieldValues["InsuranceCompanyDeductible"].ToString().All(char.IsDigit))
                            {
                            InsuranceDeductibleValue = Int64.Parse(Item.FieldValues["InsuranceCompanyDeductible"].ToString());
                            }
                        }
                    DeductibleValue += InsuranceDeductibleValue;
                    InsuranceDeductibleValue = 0;
                    }
                return DeductibleValue.ToString();
                }
            if (valueType == "FinanceDeductiblePolicy")
                {
                foreach (ListItem Item in Items)
                    {
                    if (Item.FieldValues["FinanceDeductiblePolicy"] != null)
                        {
                        if (Item.FieldValues["FinanceDeductiblePolicy"].ToString().All(char.IsDigit))
                            {
                            FinanceDeductibleValue = Int64.Parse(Item.FieldValues["FinanceDeductiblePolicy"].ToString());
                            }
                        }
                    DeductibleValue += FinanceDeductibleValue;
                    FinanceDeductibleValue = 0;
                    }
                return DeductibleValue.ToString();
                }
            else
                {
                foreach (ListItem Item in Items)
                    {
                    if (Item.FieldValues["InsuranceCompanyDeductible"] != null)
                        {
                        if (Item.FieldValues["InsuranceCompanyDeductible"].ToString().All(char.IsDigit))
                            {
                            InsuranceDeductibleValue = Int64.Parse(Item.FieldValues["InsuranceCompanyDeductible"].ToString());
                            }
                        }
                    if (Item.FieldValues["FinanceDeductiblePolicy"] != null)
                        {
                        if (Item.FieldValues["FinanceDeductiblePolicy"].ToString().All(char.IsDigit))
                            {
                            FinanceDeductibleValue = Int64.Parse(Item.FieldValues["FinanceDeductiblePolicy"].ToString());
                            }
                        }
                    DeductibleValue += InsuranceDeductibleValue + FinanceDeductibleValue;
                    FinanceDeductibleValue = 0;
                    InsuranceDeductibleValue = 0;
                    }
                return DeductibleValue.ToString();
                }

            }
        protected ListItemCollection FetchDataFromList(string listName, string DateFrom, string DateTo,string Region)
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(listName));
            CamlQuery camlQuery;
            string RegionAnd = "";
            if(Region != null && Region != "")
                {
                RegionAnd = @"   <And>
                                            <Eq><FieldRef Name='Region' /><Value Type='text'>" + Region + @"</Value></Eq>
                                    <And>";
                }

            if (DateTo == null && DateFrom == null)
                {

                camlQuery = new CamlQuery
                    {
                    ViewXml = @"<View Scope='RecursiveAll'></View>"
                    };
                }
            else
                {
                camlQuery = new CamlQuery
                    {
                    ViewXml = @"<View Scope='RecursiveAll'>
                        <Query>
                            <Where>
                                    " + RegionAnd + @"

                                            <Geq><FieldRef Name='Created' /><Value IncludeTimeValue='FALSE' Type='DateTime'>" + DateFrom + @"</Value></Geq>
                                            <Leq><FieldRef Name='Created' /><Value IncludeTimeValue='FALSE' Type='DateTime'>" + DateTo + @"</Value></Leq>
                                    </And>
                                    </And>

                            </Where>
                        </Query>
                    </View>"
                    };
                }
            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }
        protected ListItemCollection FetchDataFromList1(string listName, string DateFrom, string DateTo)
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(listName));
            CamlQuery camlQuery;

            if (DateTo == null && DateFrom == null)
                {

                camlQuery = new CamlQuery
                    {
                    ViewXml = @"<View Scope='RecursiveAll'></View>"
                    };
                }
            else
                {
                camlQuery = new CamlQuery
                    {
                    ViewXml = @"<View Scope='RecursiveAll'>
                        <Query>
                            <Where>
                                <And>
                                    <Geq>
                                        <FieldRef Name='Created' />
                                        <Value IncludeTimeValue='TRUE' Type='DateTime'>" + DateFrom + @"</Value>
                                    </Geq>
                                    <Leq>
                                        <FieldRef Name='Created' />
                                        <Value IncludeTimeValue='TRUE' Type='DateTime'>" + DateTo + @"</Value>
                                    </Leq>
                                </And>
                            </Where>
                        </Query>
                    </View>"
                    };
                }
            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }
        protected ListItemCollection FetchMyRequestData(string listName,string DateFrom, string DateTo)
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(listName));
            CamlQuery camlQuery;

            if (DateTo == null && DateFrom == null)
                {

                camlQuery = new CamlQuery
                    {
                    ViewXml = @"<View Scope='RecursiveAll'></View>"
                    };
                }
            else
                {
                camlQuery = new CamlQuery
                    {
                    ViewXml = @"<View Scope='RecursiveAll'>
                        <Query>
                            <Where>
                                <And>
                                    <Geq>
                                        <FieldRef Name='Created' />
                                        <Value IncludeTimeValue='TRUE' Type='DateTime'>" + DateFrom + @"</Value>
                                    </Geq>
                                    <Leq>
                                        <FieldRef Name='Created' />
                                        <Value IncludeTimeValue='TRUE' Type='DateTime'>" + DateTo + @"</Value>
                                    </Leq>
                                </And>
                            </Where>
                        </Query>
                    </View>"
                    };
                }
            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }
        protected ListItemCollection FetchRemindersData(string listName)
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(listName));
            CamlQuery camlQuery;


                camlQuery = new CamlQuery
                    {
                    ViewXml = @"<View Scope='RecursiveAll'></View>"
                    };
                
            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }
        protected ListItemCollection FetchInitiatorsList()
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(WF_INITIATORS));
            CamlQuery camlQuery;
                camlQuery = new CamlQuery
                    {
                    ViewXml = @"<View Scope='RecursiveAll'></View>"
                    };
                
            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }
        protected ListItemCollection FetchApproversList()
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(WF_APPROVERS));
            CamlQuery camlQuery;
            camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'></View>"
                };

            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }
        protected ListItemCollection FetchPOCList(string Priority, string Workflow, string Department)
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(WF_APPROVERS));
            //CamlQuery camlQuery;
            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query> 
                        <Where>
                            <And>
                             <And>
                              <Eq><FieldRef Name='Department'/>
                                <Value Type='Text'>" + Department + @"</Value>
                              </Eq>
                              <Eq><FieldRef Name='Workflow' />
                                <Value Type='Text'>" + Workflow + @"</Value>
                              </Eq>
                            </And>
                            <Eq><FieldRef Name='Priority' />
                                <Value Type='Number'>" + Priority + @"</Value>
                            </Eq>
                            </And>
                        </Where>
                    </Query>
                </View>"
                };

            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }
        protected ListItemCollection FetchVendorsUserList(string Workflow)
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(WF_VENDORS));
            //CamlQuery camlQuery;
            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query> 
                        <Where>
                              <Eq><FieldRef Name='Workflow' />
                                <Value Type='Text'>" + Workflow + @"</Value>
                              </Eq>
                        </Where>
                    </Query>
                </View>"
                };

            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }
        protected ListItemCollection FetchVendorsList()
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(WF_VENDORS));
            CamlQuery camlQuery;
            camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'></View>"
                };

            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }
        protected string FetchDaysTaken(string listName,string itemId,string ClaimType="")
        {
            ClientContext clientContext = GetSharePointAuth();
            List wfList = clientContext.Web.Lists.GetById(new Guid(listName));
            var camlQuery = new CamlQuery
            {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query> 
                        <Where>
                            <And>
                                <Eq><FieldRef Name='ClaimID'/><Value Type='Text'>" + itemId + @"</Value></Eq>
                                <Eq><FieldRef Name='Title'/><Value Type='Text'>" + ClaimType + @"</Value></Eq>
                            </And>
                        </Where>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='ClaimState'/>
                        <FieldRef Name='Created'/>
                        <FieldRef Name='Title'/>
                        <FieldRef Name='ClaimAction'/>
                    </ViewFields>
                </View>"
                };


            ListItemCollection wflListItem = wfList.GetItems(camlQuery);
            clientContext.Load(wflListItem);
            clientContext.ExecuteQuery();
            string date=null;
            foreach (var item in wflListItem)
                {
                    if (item.FieldValues["ClaimState"] != null && item.FieldValues["ClaimAction"] != null)
                    {
                    if (item.FieldValues["ClaimState"].ToString() == "Finance" && item.FieldValues["ClaimAction"].ToString() == "Approve")
                        {
                        date = (Convert.ToDateTime(item.FieldValues["Created"]).ToLocalTime()).ToString();
                        }
                    }
                    else if (item.FieldValues["ClaimAction"]!= null)
                    {
                    if (item.FieldValues["ClaimAction"].ToString() == "Reject")
                        {
                        date = (Convert.ToDateTime(item.FieldValues["Created"]).ToLocalTime()).ToString();
                        }
                    else if (item.FieldValues["ClaimAction"].ToString() == "Close")
                        {
                        date = (Convert.ToDateTime(item.FieldValues["Created"]).ToLocalTime()).ToString();
                        }
                    }
                }
                return date;
        }
        //Remnders Start
        protected ListItemCollection FetchInsuranceArchivedDate(string listName, string itemId, string ClaimType)
            {
            ClientContext clientContext = GetSharePointAuth();
            List wfList = clientContext.Web.Lists.GetById(new Guid(listName));
            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query>
                        <Where>
                             <And>
                             <And>
                             <And>
                                <Eq><FieldRef Name='ClaimID'/><Value Type='Text'>" + itemId + @"</Value></Eq>
                                <Eq><FieldRef Name='ClaimState'/><Value Type='Text'>" + "POC" + @"</Value></Eq>
                            </And>
                                <Eq><FieldRef Name='Title'/><Value Type='Text'>" + ClaimType + @"</Value></Eq>
                            </And>
                                <Eq><FieldRef Name='ClaimAction'/><Value Type='Text'>" + "Approve" + @"</Value></Eq>
                            </And>
                        </Where>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='ClaimState'/>
                        <FieldRef Name='Created'/>
                        <FieldRef Name='TestingCreatedDate'/>
                    </ViewFields>
                </View>"
                };
            ListItemCollection wflListItem = wfList.GetItems(camlQuery);
            clientContext.Load(wflListItem);
            clientContext.ExecuteQuery();
            return wflListItem;
            }
        protected ListItemCollection FetchFixedAssetArchivedDate(string listName, string itemId, string ClaimType)
            {
            ClientContext clientContext = GetSharePointAuth();
            List wfList = clientContext.Web.Lists.GetById(new Guid(listName));
            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query> 
                        <Where>
                             <And>
                             <And>
                             <And>
                                <Eq><FieldRef Name='ClaimID'/><Value Type='Text'>" + itemId + @"</Value></Eq>
                                <Eq><FieldRef Name='ClaimState'/><Value Type='Text'>" + "POC" + @"</Value></Eq>
                            </And>
                                <Eq><FieldRef Name='Title'/><Value Type='Text'>" + ClaimType + @"</Value></Eq>
                            </And>
                                <Eq><FieldRef Name='ClaimAction'/><Value Type='Text'>" + "Approve" + @"</Value></Eq>
                            </And>
                        </Where>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='ClaimState'/>
                        <FieldRef Name='Created'/>
                        <FieldRef Name='TestingCreatedDate'/>
                    </ViewFields>
                </View>"
                };
            ListItemCollection wflListItem = wfList.GetItems(camlQuery);
            clientContext.Load(wflListItem);
            clientContext.ExecuteQuery();
            return wflListItem;
            }
        protected ListItemCollection FetchVendorArchivedDate(string listName, string itemId, string ClaimType,string ClaimState="POC")
            {
            ClientContext clientContext = GetSharePointAuth();
            List wfList = clientContext.Web.Lists.GetById(new Guid(listName));
            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query>                     
                        <Where>
                             <And>
                             <And>
                             <And>
                                <Eq><FieldRef Name='ClaimID'/><Value Type='Text'>" + itemId + @"</Value></Eq>
                                <Eq><FieldRef Name='ClaimState'/><Value Type='Text'>" + ClaimState + @"</Value></Eq>
                            </And>
                                <Eq><FieldRef Name='Title'/><Value Type='Text'>" + ClaimType + @"</Value></Eq>
                            </And>
                                <Eq><FieldRef Name='ClaimAction'/><Value Type='Text'>" + "Approve" + @"</Value></Eq>
                            </And>
                        </Where>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='ClaimState'/>
                        <FieldRef Name='Created'/>
                        <FieldRef Name='TestingCreatedDate'/>
                    </ViewFields>
                </View>"
                };
            ListItemCollection wflListItem = wfList.GetItems(camlQuery);
            clientContext.Load(wflListItem);
            clientContext.ExecuteQuery();
            return wflListItem;
            }
        protected ListItemCollection FetchExceptionInfo(string listName, string itemId)
            {
            ClientContext clientContext = GetSharePointAuth();
            List wfList = clientContext.Web.Lists.GetById(new Guid(listName));
            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query>                     
                        <Where>
                             <And>
                                <Eq><FieldRef Name='ID'/><Value Type='Text'>" + itemId + @"</Value></Eq>
                                <Eq><FieldRef Name='Exceptional'/><Value Type='bool'>" + false + @"</Value></Eq>
                            </And>
                        </Where>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='Exceptional'/>
                    </ViewFields>
                </View>"
                };
            ListItemCollection wflListItem = wfList.GetItems(camlQuery);
            clientContext.Load(wflListItem);
            clientContext.ExecuteQuery();
            return wflListItem;
            }
        protected ListItemCollection FetchFinanceArchivedDate(string listName, string itemId, string ClaimType)
            {
            ClientContext clientContext = GetSharePointAuth();
            List wfList = clientContext.Web.Lists.GetById(new Guid(listName));
            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query> 
                        <Where>
                             <And>
                             <And>
                             <And>
                                <Eq><FieldRef Name='ClaimID'/><Value Type='Text'>" + itemId + @"</Value></Eq>
                                <Eq><FieldRef Name='ClaimState'/><Value Type='Text'>" + "Vendor" + @"</Value></Eq>
                            </And>
                                <Eq><FieldRef Name='Title'/><Value Type='Text'>" + ClaimType + @"</Value></Eq>
                            </And>
                                <Eq><FieldRef Name='ClaimAction'/><Value Type='Text'>" + "Approve" + @"</Value></Eq>
                            </And>
                        </Where>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='ClaimState'/>
                        <FieldRef Name='Created'/>
                        <FieldRef Name='TestingCreatedDate'/>
                    </ViewFields>
                    </Query>
                </View>"
                };
            ListItemCollection wflListItem = wfList.GetItems(camlQuery);
            clientContext.Load(wflListItem);
            clientContext.ExecuteQuery();
            return wflListItem;
            }
        //Reminders End
        protected ListItemCollection FetchInsuranceIntimation(string listName, string itemId, string ClaimType)
            {
            ClientContext clientContext = GetSharePointAuth();
            List wfList = clientContext.Web.Lists.GetById(new Guid(listName));
            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query> 
                        <Where>
                            <And>
                                <Eq><FieldRef Name='ClaimID'/><Value Type='Text'>" + itemId + @"</Value></Eq>
                                <Eq><FieldRef Name='ClaimState'/><Value Type='Text'>" + "Vendor" + @"</Value></Eq>
                            </And>
                            <And>
                                <Eq><FieldRef Name='Title'/><Value Type='Text'>" + ClaimType + @"</Value></Eq>
                            </And>
                        </Where>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='ClaimState'/>
                        <FieldRef Name='Created'/>
                    </ViewFields>
                </View>"
                };
            ListItemCollection wflListItem = wfList.GetItems(camlQuery);
            clientContext.Load(wflListItem);
            clientContext.ExecuteQuery();
            return wflListItem;
            }
        protected string GetClaimStatus(string StatusCode="")
            {
            if (StatusCode == "2")
                return "Pending";
            else if (StatusCode == "5")
                return "Completed";
            else
                return "Pending";
            }
        #region SP_FILES
        private List<Dictionary<string, string>> GetSharePointInitiatorsData()
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection collListItem = FetchInitiatorsList();
            foreach (ListItem items in collListItem)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ID", items.FieldValues["ID"].ToString());
                newItem.Add("Title", items.FieldValues["Title"].ToString());
                newItem.Add("Email", items.FieldValues["Email"].ToString());
                newItem.Add("WorkFlow", items.FieldValues["Workflow"].ToString());
                newItem.Add("Region", items.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
            return Response;
            }
        private List<Dictionary<string, string>> GetSharePointApproversData()
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection collListItem = FetchApproversList();
            foreach (ListItem items in collListItem)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ID", items.FieldValues["ID"].ToString());
                newItem.Add("Title", items.FieldValues["Title"].ToString());
                newItem.Add("Email", items.FieldValues["Email"].ToString());
                newItem.Add("WorkFlow", items.FieldValues["Workflow"].ToString());
                newItem.Add("Region", items.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
            return Response;
            }
        protected List<Dictionary<string, string>> GetSharePointApproversList(string Priority,string Workflow,string Department)
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection collListItem = FetchPOCList(Priority, Workflow, Department);
            foreach (ListItem items in collListItem)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ID", items.FieldValues["ID"].ToString());
                newItem.Add("Title", items.FieldValues["Title"].ToString());
                newItem.Add("Email", items.FieldValues["Email"].ToString());
                newItem.Add("WorkFlow", items.FieldValues["Workflow"].ToString());
                newItem.Add("Region", items.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
            return Response;
            }
        private List<Dictionary<string, string>> GetSharePointVendorsData()
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection collListItem = FetchVendorsList();
            foreach (ListItem items in collListItem)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ID", items.FieldValues["ID"].ToString());
                newItem.Add("Title", items.FieldValues["Title"].ToString());
                newItem.Add("Email", items.FieldValues["Email"].ToString());
                newItem.Add("WorkFlow", items.FieldValues["Workflow"].ToString());
                newItem.Add("Region", items.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
            return Response;
            }
        protected List<Dictionary<string, string>> GetSharePointVendorsList(string Workflow)
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection collListItem = FetchVendorsUserList(Workflow);
            foreach (ListItem items in collListItem)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("Title", items.FieldValues["Title"].ToString());
                newItem.Add("Email", items.FieldValues["Email"].ToString());
                newItem.Add("WorkFlow", items.FieldValues["Workflow"].ToString());
                newItem.Add("Region", items.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
            return Response;
            }
        public ArchivedModel getDaysTaken(string ID, DateTime createdDate, string StateType = "",string KPI="0", string ClaimType="")
            {
            if (StateType == "Vendor")
                {
                ArchivedModel Obj = new ArchivedModel();
                ListItemCollection ArchiveItems = FetchInsuranceIntimation(WF_ARCHIVES, ID,ClaimType);
                if (ArchiveItems.Count > 0)
                    {
                    DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["Created"]).ToLocalTime();
                    Obj.totalDays = "-";
                    Obj.ArchivedDate = ArchivedDate.ToLocalTime().ToString().Split('+')[0].ToString();
                    Obj.ClaimClosureStatus = "-";
                    return Obj;
                    }
                else
                    {
                    Obj.totalDays = "-";
                    Obj.ArchivedDate = "-";
                    Obj.ClaimClosureStatus = "-";
                    return Obj;
                    }
                }
            else
                {
                ArchivedModel Obj = new ArchivedModel();
                string ArchivedDate = FetchDaysTaken(WF_ARCHIVES, ID, ClaimType);
                DateTime CreatedDate = createdDate;
                decimal TotalDays = 0;
                if (ArchivedDate!=null)
                    {
                        
                            DateTime Date = Convert.ToDateTime(ArchivedDate).ToLocalTime();
                            decimal differvalue = (decimal)(Date - CreatedDate).TotalDays;
                            if (StateType == "Aging")
                                {
                                TotalDays = Math.Floor(differvalue);//int.Parse(KPI) - Math.Ceiling(differvalue);
                                }
                            else
                                {
                                TotalDays = Math.Floor(differvalue);
                                }
                            Obj.totalDays = TotalDays.ToString();
                            Obj.ArchivedDate = Date.ToLocalTime().ToString().Split('+')[0].ToString();
                            Obj.ClaimClosureStatus = "Completed";
                            return Obj; 
                }
                else
                {
                    if (StateType == "Aging")
                        {
                        decimal differvalue = (decimal)(DateTime.Now - CreatedDate).TotalDays;
                        TotalDays = Math.Floor(differvalue);
                        Obj.totalDays = TotalDays.ToString();
                        }
                    else
                        {
                        Obj.totalDays = "-";
                        }
                    
                Obj.ArchivedDate = "-";
                Obj.ClaimClosureStatus = "In Progress";
                return Obj;
                }

            }


        }
        private List<FilesModel> GetSharePointFilesInternal(int ClaimID = -1,string ListName="")
        {
            List<FilesModel> filesList = new List<FilesModel>();
            using (ClientContext clientContext = GetSharePointAuth())
            {
                string siteUrl = Config.AppSettings["SP_WEBSITE_URL"];
                string Folder = "/Shared Documents/"+ListName;
                List documentsList = clientContext.Web.Lists.GetByTitle("Documents"); // Shared Documents -> Documents

                var camlQuery = new CamlQuery
                {
                    ViewXml = @"<View Scope='RecursiveAll'>
                                <Query>
                                   <Where>
                                    <Eq>
                                      <FieldRef Name='FileDirRef'/>
                                      <Value Type='Text'>" + Folder + @" </Value>
                                    </Eq>
                                   </Where>
                                </Query></View>"
                };

                ListItemCollection listItems = documentsList.GetItems(camlQuery);
                clientContext.Load(listItems);
                clientContext.ExecuteQuery();

                foreach (ListItem item in listItems)
                {
                    if((string)item["ClaimID"]== ClaimID.ToString())
                        {
                            if ((bool)item["IsDamaged"] == false)
                            {
                            var id = item["ClaimID"];
                            var fileRef = (string)item.FieldValues["FileRef"];
                            var fileInfo = File.OpenBinaryDirect(clientContext, fileRef);
                            var DirectoryLocation = Config.AppSettings["FILE_UPLOAD_PATH"];
                            var fileName = Path.Combine(DirectoryLocation, (string)item.FieldValues["FileLeafRef"]);

                            using (var fileStream = System.IO.File.Create(fileName))
                                {
                                fileInfo.Stream.CopyTo(fileStream);
                                }

                            FilesModel Files = new FilesModel
                                {
                                // var protocol = HttpContext.Current.Request.IsSecureConnection == true ? "https" : "http";
                                // Files.FilePath = protocol + "://" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + ":" + HttpContext.Current.Request.ServerVariables["SERVER_PORT"] + "/Public/" + (string) item.FieldValues["FileLeafRef"];
                                FileName = (string)item.FieldValues["FileLeafRef"],
                                CreatedBy = (string)item.FieldValues["CreatedBy"]
                                };
                            // Files.FileContent = fileInfo;
                            filesList.Add(Files);
                            }
                        }


                }
            }
            return filesList;
        }
        private List<FilesModel> GetSharePointDamagedFilesInternal(int ClaimID = -1)
        {
            List<FilesModel> filesList = new List<FilesModel>();
            using (ClientContext clientContext = GetSharePointAuth())
                {
                string siteUrl = Config.AppSettings["SP_WEBSITE_URL"];
                string Folder = "/Shared Documents/CellSite";
                List documentsList = clientContext.Web.Lists.GetByTitle("Documents"); // Shared Documents -> Documents
                                                                          
                var camlQuery = new CamlQuery
                    {
                    ViewXml = @"<View Scope='RecursiveAll'>
                                <Query>
                                   <Where>
                                    <Eq>
                                      <FieldRef Name='FileDirRef'/>
                                      <Value Type='Text'>" + Folder + @" </Value>
                                    </Eq>
                                   </Where>
                                </Query></View>"
                    };

                ListItemCollection listItems = documentsList.GetItems(camlQuery);
                clientContext.Load(listItems);
                clientContext.ExecuteQuery();

                foreach (ListItem item in listItems)
                    {
                    if ((string)item["ClaimID"] == ClaimID.ToString())
                        {
                        if ((bool)item["IsDamaged"] == true)
                            {
                            var id = item["ClaimID"];
                            var fileRef = (string)item.FieldValues["FileRef"];
                            var fileInfo = File.OpenBinaryDirect(clientContext, fileRef);
                            var DirectoryLocation = Config.AppSettings["FILE_UPLOAD_PATH"];
                            var fileName = Path.Combine(DirectoryLocation, (string)item.FieldValues["FileLeafRef"]);

                            using (var fileStream = System.IO.File.Create(fileName))
                                {
                                fileInfo.Stream.CopyTo(fileStream);
                                }

                            FilesModel Files = new FilesModel
                                {
                                // var protocol = HttpContext.Current.Request.IsSecureConnection == true ? "https" : "http";
                                // Files.FilePath = protocol + "://" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + ":" + HttpContext.Current.Request.ServerVariables["SERVER_PORT"] + "/Public/" + (string) item.FieldValues["FileLeafRef"];
                                FileName = (string)item.FieldValues["FileLeafRef"],
                                CreatedBy = (string)item.FieldValues["CreatedBy"]
                                };
                            // Files.FileContent = fileInfo;
                            filesList.Add(Files);
                            }
                        }


                    }
                }
            return filesList;
            }
        protected int UploadFilesToSharePoint(List<string> FilesToUplaod,int ClaimID = -1,string CreatedBy = null,string ClaimName = null)
        {
            if (FilesToUplaod == null || ClaimID == -1 || ClaimName == null || CreatedBy == null)
            {
                return 0;
            }

            if (FilesToUplaod.Count > 0)
            {
                using (ClientContext clientContext = GetSharePointAuth())
                {
                    string siteUrl = Config.AppSettings["SP_WEBSITE_URL"];

                    //Shared Documents -> Documents
                    List documentsList = clientContext.Web.Lists.GetByTitle("Documents");

                    for (int i = 0; i < FilesToUplaod.Count; i++)
                    {
                        
                        var fileBytes = FilesToUplaod[i].Split(':');
                        var base64EncodedBytes = Convert.FromBase64String(fileBytes[0]);
                        var filename = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "- " + fileBytes[1];

                        if (filename.Contains(".jpg") || filename.Contains(".jpeg") || filename.Contains(".png") || filename.Contains(".doc")
                            || filename.Contains(".docx") || filename.Contains(".xls") || filename.Contains(".xlsx") || filename.Contains(".pdf")
                            || filename.Contains(".rar") || filename.Contains(".zip") || filename.Contains(".eml") 
                            )
                            {
                                                      
                                var fileCreationInformation = new FileCreationInformation
                                {
                                    Content = base64EncodedBytes,
                                    Overwrite = true,
                                    Url = siteUrl + "/" + Config.AppSettings["SP_SHARED_DOCUMENTS"] + "/" + ClaimName + "/" + filename
                                };

                                var uploadFile = documentsList.RootFolder.Files.Add(fileCreationInformation);
                                uploadFile.ListItemAllFields["CreatedBy"] = CreatedBy;
                                uploadFile.ListItemAllFields["ClaimID"] = ClaimID;
                                uploadFile.ListItemAllFields["IsDamaged"] = false;

                                uploadFile.ListItemAllFields.Update();
                                clientContext.Load(uploadFile);
                            }
                        }

                    clientContext.ExecuteQuery();
                }

                return FilesToUplaod.Count;
            }
            else
            {
                return 0;
            }
        }
        protected int UploadDamagedFilesToSharePoint(List<string> FilesToUplaod,int ClaimID = -1,string CreatedBy = null,string ClaimName = null)
            {
            if (FilesToUplaod == null || ClaimID == -1 || ClaimName == null || CreatedBy == null)
                {
                return 0;
                }

            if (FilesToUplaod.Count > 0)
                {
                using (ClientContext clientContext = GetSharePointAuth())
                    {
                    string siteUrl = Config.AppSettings["SP_WEBSITE_URL"];

                    //Shared Documents -> Documents
                    List documentsList = clientContext.Web.Lists.GetByTitle("Documents");

                    for (int i = 0; i < FilesToUplaod.Count; i++)
                        {
                        var fileBytes = FilesToUplaod[i].Split(':');
                        var base64EncodedBytes = Convert.FromBase64String(fileBytes[0]);
                        var filename = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "- " + fileBytes[1];

                        var fileCreationInformation = new FileCreationInformation
                            {
                            Content = base64EncodedBytes,
                            Overwrite = true,
                            Url = siteUrl + "/" + Config.AppSettings["SP_SHARED_DOCUMENTS"] + "/" + ClaimName + "/" + filename
                            };

                        var uploadFile = documentsList.RootFolder.Files.Add(fileCreationInformation);
                        uploadFile.ListItemAllFields["CreatedBy"] = CreatedBy;
                        uploadFile.ListItemAllFields["ClaimID"] = ClaimID;
                        uploadFile.ListItemAllFields["IsDamaged"] = true;

                        uploadFile.ListItemAllFields.Update();
                        clientContext.Load(uploadFile);
                        }

                    clientContext.ExecuteQuery();
                    }

                return FilesToUplaod.Count;
                }
            else
                {
                return 0;
                }
            }
        // FIXME: This will implemented after authorization policy implementation
        protected void DeleteSharePointFile(string listName, int itemId)
        {
            ClientContext clientContext = GetSharePointAuth();
            string siteUrl = Config.AppSettings["SP_WEBSITE_URL"];
            string Folder = "/Shared Documents/CellSite";
            List documentsList = clientContext.Web.Lists.GetByTitle("Documents"); // Shared Documents -> Documents

            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                                <Query>
                                   <Where>
                                    <Eq>
                                      <FieldRef Name='FileDirRef'/>
                                      <Value Type='Text'>" + Folder + @" </Value>
                                    </Eq>
                                   </Where>
                                </Query></View>"
                };

            ListItemCollection listItems = documentsList.GetItems(camlQuery);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();
            foreach (ListItem listitem in listItems)
                {
                if ((string)listitem["ClaimID"] == itemId.ToString())
                    {
                    if ((bool)listitem["IsDamaged"] == true)
                        {
                        listitem.DeleteObject();
                        clientContext.ExecuteQuery();
                        }
                    }

                }
            }
        #endregion

        #region CLAIM_PROCESSING
        protected HttpResponseMessage ProcessNewClaim(string ListID, BaseModel model)
        {
            try
                {
                ClientContext clientContext = GetSharePointAuth();
                var SPClaims = clientContext.Web.Lists.GetById(new Guid(ListID));
                ListItem SPClaim = null;

                if (model.Id == 0)
                    SPClaim = SPClaims.AddItem(new ListItemCreationInformation());
                else
                    {
                    var SPArchives = clientContext.Web.Lists.GetById(new Guid(WF_ARCHIVES));
                    // GetClaimType will translate into respective Workflow name which then builds a CamlQuery
                    ListItemCollection listItems = SPArchives.GetItems(BuildArchivesQuery(GetClaimType(model), model.Id));

                    clientContext.Load(listItems);
                    clientContext.ExecuteQuery();

                    if (listItems == null)
                        return Request.CreateResponse(
                            HttpStatusCode.Unauthorized,
                            new
                                {
                                Success = false,
                                Message = "You cannot have permission to update this record"
                                }, Configuration.Formatters.JsonFormatter
                            );
                    else
                        {
                        foreach (ListItem item in listItems)
                            {
                            if (item["ClaimState"].ToString() == "POC" && model.InitIsReject==false)
                                return Request.CreateResponse(
                                    HttpStatusCode.Unauthorized,
                                    new
                                        {
                                        Success = false,
                                        Message = "You cannot have permission to update this record"
                                        }, Configuration.Formatters.JsonFormatter
                                    );
                            }
                        }

                    SPClaim = SPClaims.GetItemById(model.Id);
                    clientContext.Load(SPClaim);
                    clientContext.ExecuteQuery();

                    if (SPClaim["InitIsReject"] is true)
                        SPClaim["InitIsReject"] = false;

                    if (SPClaim["IsDraft"] is true)
                        SPClaim["IsDraft"] = false;
                    }

                // overloaded function reterives data from respective workflow
                GetNewClaimParams(model, SPClaim);

                SPClaim.Update();
                clientContext.ExecuteQuery();

                if (GetClaimType(model)=="CellSite")
                    {
                    ResponseModel response = new ResponseModel
                        {
                        Files = UploadFilesToSharePoint(model.AttachmentBase64, SPClaim.Id, model.InitiatedBy, GetClaimType(model)),
                        DamageFiles = UploadDamagedFilesToSharePoint(model.DamagedFilesAttachmentBase64, SPClaim.Id, model.InitiatedBy, GetClaimType(model)),
                        Message = "Data accepted for further operations.",
                        Success = true
                        };
                    return Request.CreateResponse(
                    HttpStatusCode.OK,
                    response,
                    Configuration.Formatters.JsonFormatter
                    );
                    }
                else
                    {
                    ResponseModel response = new ResponseModel
                        {
                        Files = UploadFilesToSharePoint(model.AttachmentBase64, SPClaim.Id, model.InitiatedBy, GetClaimType(model)),
                        Message = "Data accepted for further operations.",
                        Success = true
                        };

                    return Request.CreateResponse(
                    HttpStatusCode.OK,
                    response,
                    Configuration.Formatters.JsonFormatter
                    );
                    }


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new
                    {
                        success = false,
                        message = "Request failed, please try again."
                    });
            }
        }

        protected HttpResponseMessage ProcessDraftClaim(string ListID, BaseModel model)
        {
            try
            {
                ClientContext clientContext = GetSharePointAuth();
                var SPClaims = clientContext.Web.Lists.GetById(new Guid(ListID));

                ListItem SPClaim = null;

                if (model.Id == 0)
                    SPClaim = SPClaims.AddItem(new ListItemCreationInformation());
                else
                {
                    var SPArchives = clientContext.Web.Lists.GetById(new Guid(WF_ARCHIVES));
                    ListItemCollection listItems = SPArchives.GetItems(BuildArchivesQuery(GetClaimType(model), model.Id));

                    clientContext.Load(listItems);
                    clientContext.ExecuteQuery();

                    if (listItems == null)
                        return Request.CreateResponse(
                            HttpStatusCode.Unauthorized,
                            new
                            {
                                Success = false,
                                Message = "You don't have permission to update this record"
                            }, Configuration.Formatters.JsonFormatter
                            );
                    else
                    {
                        foreach (ListItem item in listItems)
                        {
                            if (item["ClaimState"].ToString() == "POC")
                                return Request.CreateResponse(
                                    HttpStatusCode.Unauthorized,
                                    new
                                    {
                                        Success = false,
                                        Message = "You don't have permission to update this record"
                                    }, Configuration.Formatters.JsonFormatter
                                    );
                        }
                    }

                    SPClaim = SPClaims.GetItemById(model.Id);

                    clientContext.Load(SPClaim);
                    clientContext.ExecuteQuery();

                    if (SPClaim["InitIsReject"] is true)
                        SPClaim["InitIsReject"] = false;
                }

                SPClaim["IsDraft"] = true;
                SPClaim["InitIsReject"] = false;

                GetNewClaimParams(model, SPClaim);

                SPClaim.Update();
                clientContext.ExecuteQuery();

                if (GetClaimType(model) == "CellSite")
                    {
                    ResponseModel response = new ResponseModel
                        {
                        Files = UploadFilesToSharePoint(model.AttachmentBase64, SPClaim.Id, model.InitiatedBy, GetClaimType(model)),
                        DamageFiles = UploadDamagedFilesToSharePoint(model.DamagedFilesAttachmentBase64, SPClaim.Id, model.InitiatedBy, GetClaimType(model)),
                        Message = "Data accepted for further operations.",
                        Success = true
                        };
                    return Request.CreateResponse(
                    HttpStatusCode.OK,
                    response,
                    Configuration.Formatters.JsonFormatter
                    );
                    }
                else
                    {
                    ResponseModel response = new ResponseModel
                        {
                        Files = UploadFilesToSharePoint(model.AttachmentBase64, SPClaim.Id, model.InitiatedBy, GetClaimType(model)),
                        Message = "Data accepted for further operations.",
                        Success = true
                        };

                    return Request.CreateResponse(
                    HttpStatusCode.OK,
                    response,
                    Configuration.Formatters.JsonFormatter
                    );
                    }
                }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                     HttpStatusCode.BadRequest,
                     new
                     {
                         success = false,
                         message = "Request failed, please try again."
                     });
            }
        }

        public HttpResponseMessage ProcessPOCFeedback(string ListID, PocModel model)
        {
            try
            {
                ClientContext clientContext = GetSharePointAuth();

                var validate = ValidateWorkflowState(clientContext, model.ClaimFor, model.Id, "POC");
                // Allow processing for Null response
                if (validate != null)
                    return validate as HttpResponseMessage;

                List list = clientContext.Web.Lists.GetById(new Guid(ListID));
                ListItem SPClaim = list.GetItemById(model.Id);

                clientContext.Load(SPClaim);
                clientContext.ExecuteQuery();

                SPClaim["PocTaskOutcome"] = model.Action;
                SPClaim["IfPocTaskUpdate"] = true;
                SPClaim["PocDescription"] = model.Comments;
                SPClaim["Approver"] = model.InitiatedBy;
                SPClaim["IsDraft"] = false;

                if (model.ClaimFor == "CellSite")
                {
                    SPClaim["Manager"] = model.Manager;
                }

                SPClaim.Update();
                clientContext.ExecuteQuery();

                ResponseModel response = new ResponseModel
                {
                    Files = UploadFilesToSharePoint(model.AttachmentBase64, SPClaim.Id, model.InitiatedBy, model.ClaimFor),
                    Message = "Data accepted for further operations.",
                    Success = true
                };

                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                     HttpStatusCode.BadRequest,
                     new
                     {
                         success = false,
                         message = "Request failed, please try again."
                     });
            }
        }

        public HttpResponseMessage ProcessVendorFeedback(string ListID, VendorModel model)
        {
            try
            {
                ClientContext clientContext = GetSharePointAuth();
                var SPArchives = clientContext.Web.Lists.GetById(new Guid(WF_ARCHIVES));

                ListItemCollection listItems = SPArchives.GetItems(BuildArchivesQuery(model.ClaimFor, model.ID));

                clientContext.Load(listItems);
                clientContext.ExecuteQuery();

                foreach (ListItem item in listItems)
                {
                    var PreviousState = "";
                    if (model.ClaimFor == "MarineInland" || model.ClaimFor == "Cash")
                    {
                        PreviousState = "HOD-Finance Team"; //PreviousState = "Finance Team";
                        }
                    else
                    {
                        PreviousState = "HOD-POC"; //PreviousState = "POC";
                        }

                    if (!(PreviousState.Contains(item["ClaimState"].ToString())))
                        return Request.CreateResponse(HttpStatusCode.Unauthorized,
                            new
                            {
                                Success = false,
                                Message = "You don't have permission to update this record"
                            }, Configuration.Formatters.JsonFormatter
                            );
                }

                List list = clientContext.Web.Lists.GetById(new Guid(ListID));
                ListItem SPClaim = list.GetItemById(model.ID);

                clientContext.Load(SPClaim);
                clientContext.ExecuteQuery();

                SPClaim["vendorTaskOutcome"] = model.vendorTaskOutcome;
                SPClaim["ifVendorTaskUpdate"] = true;
                SPClaim["vendorComments"] = model.vendorComments;
                SPClaim["Approver"] = model.Approver;

                SPClaim.Update();
                clientContext.ExecuteQuery();

                ResponseModel response = new ResponseModel
                {
                    Files = UploadFilesToSharePoint(model.AttachmentBase64, SPClaim.Id, model.Approver, model.ClaimFor),
                    Message = "Data accepted for further operations.",
                    Success = true
                };

                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                     HttpStatusCode.BadRequest,
                     new
                     {
                         success = false,
                         message = "Request failed, please try again."
                     });
            }
        }

        public HttpResponseMessage ProcessFianceFeedback(string ListID, FinanceTeamModel model)
        {
            try
            {
                ClientContext clientContext = GetSharePointAuth();

                var validate = ValidateWorkflowState(clientContext, model.ClaimFor, model.ID, "Vendor");
                // Allow processing for Null response
                if (validate != null)
                    return validate as HttpResponseMessage;

                List list = clientContext.Web.Lists.GetById(new Guid(ListID));
                ListItem SPClaim = list.GetItemById(model.ID);

                clientContext.Load(SPClaim);
                clientContext.ExecuteQuery();

                SPClaim["financeTaskOutcome"] = model.financeTaskOutcome;
                SPClaim["FinanceDateOfLodgement"] = model.DateOfLodgement;
                SPClaim["FinanceChequeRecievedDate"] = model.ChequeRecievedDate;
                SPClaim["FinanceClaimAmount"] = model.ClaimAmount;
                SPClaim["FinanceRecovery"] = model.Recovery;
                SPClaim["FinanceChequeNumber"] = model.ChequeNumber;
                SPClaim["FinanceDeductiblePolicy"] = model.DeductiblePolicy;
                SPClaim["FinanceComments"] = model.FtComments;
                SPClaim["Approver"] = model.Approver;
                SPClaim["NBV"] = model.NBV;
                SPClaim["InsuranceCompanyDeductible"] = model.InsuranceCompanyDeductible;
                SPClaim["IsFinance"] = true;

                SPClaim.Update();
                clientContext.ExecuteQuery();

                return Request.CreateResponse(
                     HttpStatusCode.OK,
                     new
                     {
                         success = true,
                         message = "Data accepted for further operations."
                     });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                     HttpStatusCode.BadRequest,
                     new
                     {
                         success = false,
                         message = "Request failed, please try again."
                     });
            }
        }
        public HttpResponseMessage ProcessHODFeedback(string ListID, HODModel model)
            {
            try
                {
                ClientContext clientContext = GetSharePointAuth();
                //var SPArchives = clientContext.Web.Lists.GetById(new Guid(WF_ARCHIVES));

                //ListItemCollection listItems = SPArchives.GetItems(BuildArchivesQuery(model.ClaimFor, model.ID));

                //clientContext.Load(listItems);
                //clientContext.ExecuteQuery();

                //foreach (ListItem item in listItems)
                //    {
                //    var PreviousState = "";
                //    if (model.ClaimFor == "MarineInland" || model.ClaimFor == "Cash")
                //        {
                //        PreviousState = "Finance Team";
                //        }
                //    else
                //        {
                //        PreviousState = "POC";
                //        }

                //    if (item["ClaimState"].ToString() != PreviousState)
                //        return Request.CreateResponse(HttpStatusCode.Unauthorized,
                //            new
                //                {
                //                Success = false,
                //                Message = "You don't have permission to update this record"
                //                }, Configuration.Formatters.JsonFormatter
                //            );
                //    }
                   if (model.Exceptional==false)
                       return Request.CreateResponse(HttpStatusCode.Unauthorized,
                           new
                               {
                               Success = false,
                               Message = "You don't have permission to update this record"
                               }, Configuration.Formatters.JsonFormatter
                           );
                   
                List list = clientContext.Web.Lists.GetById(new Guid(ListID));
                ListItem SPClaim = list.GetItemById(model.ID);

                clientContext.Load(SPClaim);
                clientContext.ExecuteQuery();

                SPClaim["hodComments"] = model.hodcomments;
                SPClaim["ifHodTaskUpdate"] = true;
                SPClaim["hodTaskOutcome"] = model.hodTaskOutcome;

                SPClaim.Update();
                clientContext.ExecuteQuery();

                return Request.CreateResponse(
                     HttpStatusCode.OK,
                     new
                         {
                         success = true,
                         message = "Data accepted for further operations."
                         });
                }
            catch (Exception ex)
                {
                return Request.CreateResponse(
                     HttpStatusCode.BadRequest,
                     new
                         {
                         success = false,
                         message = "Request failed, please try again."
                         });
                }
            }
        public HttpResponseMessage ProcessFianceInsuranceTeamReviewForm(string ListID, FinInsuranceReviewModel model)
        {
            try
            {
                ClientContext clientContext = GetSharePointAuth();

                var validate = ValidateWorkflowState(clientContext, model.ClaimFor, model.ID, "POC");
                // Allow processing for Null response
                if (validate != null)
                    return validate as HttpResponseMessage;

                List list = clientContext.Web.Lists.GetById(new Guid(ListID));
                ListItem SPClaim = list.GetItemById(model.ID);

                clientContext.Load(SPClaim);
                clientContext.ExecuteQuery();

                SPClaim["POCName"] = model.PocName;
                SPClaim["POCNumber"] = model.PocContactNo;
                SPClaim["EFICS"] = model.PettyAmountStolen;
                SPClaim["financeTeamTaskOutcome"] = model.Action;
                SPClaim["financeTeamComments"] = model.Comments;        
                SPClaim["ifFinanceTeamTaskUpdate"] = true;

                SPClaim.Update();
                clientContext.ExecuteQuery();

                ResponseModel response = new ResponseModel
                {
                    Files = UploadFilesToSharePoint(model.AttachmentBase64, SPClaim.Id, model.Approver, model.ClaimFor),
                    Message = "Data accepted for further operations.",
                    Success = true
                };

                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                      HttpStatusCode.BadRequest,
                      new
                      {
                          success = false,
                          message = "Request failed, please try again."
                      });
            }
        }

        public HttpResponseMessage ProcessFixedAssetTeam(string ListID, FixedAssetTeamModel model)
            {
            try
                {
                ClientContext clientContext = GetSharePointAuth();

                var validate = ValidateWorkflowState(clientContext, model.ClaimFor, model.Id, "POC");
                // Allow processing for Null response
                if (validate != null)
                    return validate as HttpResponseMessage;

                List list = clientContext.Web.Lists.GetById(new Guid(ListID));
                ListItem SPClaim = list.GetItemById(model.Id);

                clientContext.Load(SPClaim);
                clientContext.ExecuteQuery();

                
                SPClaim["ifFixedAssetTaskUpdate"] = true;

                SPClaim.Update();
                clientContext.ExecuteQuery();
                //DeleteSharePointFile(ListID, model.Id);
                ResponseModel response = new ResponseModel
                    {
                    //Files = UploadFilesToSharePoint(model.Da, SPClaim.Id, model.InitiatedBy, model.ClaimFor),
                    
                    Files = UploadDamagedFilesToSharePoint(model.DamagedFilesAttachmentBase64, SPClaim.Id, model.InitiatedBy, model.ClaimFor),
                    Message = "Data accepted for further operations.",
                    Success = true
                    };

                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
                }
            catch (Exception ex)
                {
                return Request.CreateResponse(
                      HttpStatusCode.BadRequest,
                      new
                          {
                          success = false,
                          message = "Request failed, please try again."
                          });
                }
            }

        public HttpResponseMessage ProcessTeamManagerAttachmentDoc(string ListID, ManagerAttachmentModel model)
        {
            try
            {
                ClientContext clientContext = GetSharePointAuth();

                var validate = ValidateWorkflowState(clientContext, model.ClaimFor, model.ID, "Vendor");
                // Allow processing for Null response
                if (validate != null)
                    return validate as HttpResponseMessage;

                List list = clientContext.Web.Lists.GetById(new Guid(ListID));
                ListItem SPClaim = list.GetItemById(model.ID);

                clientContext.Load(SPClaim);
                clientContext.ExecuteQuery();

                SPClaim["POCName"] = model.PocName;
                SPClaim["POCNumber"] = model.PocContactNo;
                SPClaim["EFICS"] = model.StolenAmount;
                SPClaim["financeTeamTaskOutcome"] = model.Action;
                SPClaim["financeTeamComments"] = model.Comments;
                SPClaim["IsFinance"] = true;

                SPClaim.Update();
                clientContext.ExecuteQuery();

                ResponseModel response = new ResponseModel
                {
                    Files = UploadFilesToSharePoint(model.AttachmentBase64, SPClaim.Id, model.Approver, model.ClaimFor),
                    Message = "Data accepted for further operations.",
                    Success = true
                };

                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(
                     HttpStatusCode.BadRequest,
                     new
                     {
                         success = false,
                         message = "Request failed, please try again."
                     });
            }
        }

        private object ValidateWorkflowState(ClientContext clientContext, string claimFor, int id, string previousState)
        {
            var SPArchives = clientContext.Web.Lists.GetById(new Guid(WF_ARCHIVES));
            ListItemCollection listItems = SPArchives.GetItems(BuildArchivesQuery(claimFor, id));

            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            foreach (ListItem item in listItems)
            {
                if(item["ClaimState"]!=null)
                    {
                    if (item["ClaimState"].ToString() != previousState && item["ClaimState"].ToString() != "HOD")
                        return Request.CreateResponse(
                            HttpStatusCode.Unauthorized,
                            new
                                {
                                Success = false,
                                Message = "You don't have permission to update this record"
                                }, Configuration.Formatters.JsonFormatter
                            );
                    }
            }

            return null;
        }

        private CamlQuery BuildArchivesQuery(string claimFor, int id)
        {
            var CamlQuery = new CamlQuery
            {
                ViewXml = @"
                <View Scope='RecursiveAll'>
                    <Query>
                        <Where>
                            <And>
                                <Eq><FieldRef Name='ClaimID'/><Value Type='Text'>" + id + @"</Value></Eq>                                  
                                <Eq><FieldRef Name='Title'/><Value Type='Text'>" + GetWorkflowName(claimFor) + @"</Value></Eq>
                            </And>
                        </Where>
                        <OrderBy>
                            <FieldRef Name='Created' Ascending='FALSE' />
                        </OrderBy>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='ClaimState'/>
                        <FieldRef Name='ClaimID'/>
                    </ViewFields>
                    <RowLimit Paged='TRUE'>1</RowLimit>
                </View>"
            };

            return CamlQuery;
        }

        private string GetWorkflowName(string claimFor)
        {
            switch (claimFor)
            {
                case "MarineInland":
                    return "Marine Inland";
                case "Laptop":
                    return "Laptop";
                case "Biometric":
                    return "Biometric Devices";
                case "Handset":
                    return "Handsets";
                case "Marine":
                case "MarineImport":
                    return "Marine Imports";
                case "Cash":
                    return "Cash in Safe Claims";
                case "Bsd":
                    return "Highvalue Tools";
                case "Vehicle":
                    return "Motor Vehicles";
                case "CellSite":
                    return " Cell Site Claims";
                default:
                    return "";
            }
        }

        private string GetClaimType(BaseModel model)
        {
            if (model.GetType() == typeof(BiometricModel))
                return "Biometric";
            else if (model.GetType() == typeof(BsdModel))
                return "Bsd";
            else if (model.GetType() == typeof(CashModel))
                return "Cash";
            else if (model.GetType() == typeof(CellSiteModel))
                return "CellSite";
            else if (model.GetType() == typeof(HandsetModel))
                return "Handset";
            else if (model.GetType() == typeof(LaptopModel))
                return "Laptop";
            else if (model.GetType() == typeof(MarineInlandModel))
                return "MarineInland";
            else if (model.GetType() == typeof(MarineImportModel))
                return "Marine";
            else if (model.GetType() == typeof(VehicleModel))
                return "Vehicle";
            else
                return "Default";
        }

        #endregion

        #endregion
    }
}