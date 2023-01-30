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
    [System.Web.Http.RoutePrefix("api/biometric")]
    public class BiometricController : BaseController
    {
        private readonly string SP_BIOMETRIC = Config.AppSettings["SP_BIOMETRIC"];
        private readonly string WF_ARCHIVES = Config.AppSettings["WF_ARCHIVES"];
        private readonly string BIOMETRIC_TYPE = Config.AppSettings["BIOMETRIC"];
        private readonly string BIOMETRIC_TYPE_KEY = Config.AppSettings["BIOMETRIC_KEY"];
        private readonly string KPI_BIOMETRIC = Config.AppSettings["KPI_BIOMETRIC"];
        private readonly string WF_TYPE = "Biometric Devices";

        #region ACTIONS

        [HttpGet]
        [Route("all")]
        public List<Dictionary<string, string>> GetAll(string DateFrom = null, string DateTo = null, string ApiType = null, string KPI = "0", string GainStartValue = "0", string GainEndValue = "0", string Region = "", string Status = "")
        {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> GainLossFilteredResponse = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> StatusFilteredResponse = new List<Dictionary<string, string>>();
            ListItemCollection listItems = FetchDataFromList(SP_BIOMETRIC, DateFrom, DateTo,Region);
            //if (ApiType == "deductible")
            //    {
            //    Dictionary<string, string> newItem = new Dictionary<string, string>();
            //    newItem.Add("ClaimCategory", BIOMETRIC_TYPE);
            //    newItem.Add("ClaimCount", listItems.Count.ToString());
            //    newItem.Add("DeductibleValue", "34");
            //    Response.Add(newItem);
            //    }
            //else
                //{
                foreach (ListItem listItem in listItems)
                    {
                    double Recovery = 0, ClaimAmount = 0;
                    string GainLoss = "";
                    Int64 FinanceDeductibleValue = 0;
                    Int64 InsuranceDeductibleValue = 0;
                    //DateTime claimEndDate, claimInitiatedDate;

                    //ListItem claimDate = FetchDateFromArchive("Biometric", listItem["ID"].ToString())[0];

                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("ClaimCategory", BIOMETRIC_TYPE);
                    newItem.Add("ClaimId", listItem.FieldValues["ItemID"].ToString());
                    newItem.Add("ClaimType", listItem.FieldValues["ClaimType"].ToString());
                    if (ApiType == "all")
                        {

                        if (listItem.FieldValues["FinanceRecovery"] != null)
                            {
                            Recovery = Int32.Parse(listItem.FieldValues["FinanceRecovery"].ToString());
                            }

                        if (listItem.FieldValues["FinanceClaimAmount"] != null)
                            {
                            ClaimAmount = Int32.Parse(listItem.FieldValues["FinanceClaimAmount"].ToString());
                            }

                        newItem.Add("ReferenceNo", listItem.FieldValues["Title"].ToString());
                        if (listItem.FieldValues["financeTaskOutcome"] != null)
                            {
                            if (listItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                                {
                                newItem.Add("ClaimStatus", "Completed");
                                }
                            else if (listItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                                {
                                newItem.Add("ClaimStatus", "Closed");
                                }
                            else
                                {
                                newItem.Add("ClaimStatus", "Rejected");
                                }
                            }
                        else if (listItem.FieldValues["financeTaskOutcome"] != null)
                            {
                            if (listItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                                {
                                newItem.Add("ClaimStatus", "Rejected");
                                }
                            }
                        else if (listItem.FieldValues["PocTaskOutcome"] != null)
                            {
                            if (listItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                                {
                                newItem.Add("ClaimStatus", "Rejected");
                                }
                            else
                                {
                                newItem.Add("ClaimStatus", "Pending");
                                }
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Pending");
                            }
                        newItem.Add("Created", Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                        newItem.Add("DaysTaken", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "", "", "Biometric Devices").totalDays);
                        newItem.Add("AmountRecevied", ClaimAmount.ToString());
                        newItem.Add("NBV", Recovery.ToString());
                        newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                        newItem.Add("KPI", KPI_BIOMETRIC);
                        newItem.Add("Region", listItem.FieldValues["Region"].ToString());
                        newItem.Add("POCName", listItem.FieldValues["POCName"].ToString());
                        newItem.Add("Make", listItem.FieldValues["Make"].ToString());
                        newItem.Add("POCNumber", listItem.FieldValues["POCNumber"].ToString());
                        newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"]== "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                        newItem.Add("ClosureDate", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "", "", "Biometric Devices").ArchivedDate);
                        newItem.Add("DOI", listItem.FieldValues["IncidentDate"].ToString());
                        newItem.Add("InsuranceCompany", "-");
                        newItem.Add("IntimationInsuranceCompany", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "Vendor","", "Biometric Devices").ArchivedDate);
                        if (listItem.FieldValues["PocTaskOutcome"] != null)
                            {
                            newItem.Add("InternalDepartment", listItem.FieldValues["PocTaskOutcome"].ToString());
                            }
                        else
                            {
                            newItem.Add("InternalDepartment", "-");
                            }
                        if (listItem.FieldValues["FinanceComments"] != null)
                            {
                            newItem.Add("FinanceComments", listItem.FieldValues["FinanceComments"].ToString());
                            }
                        else
                            {
                            newItem.Add("FinanceComments", "-");
                            }
                        }
                    else if (ApiType == "aging")
                        {
                        newItem.Add("ReferenceNo", listItem.FieldValues["Title"].ToString());
                        if (listItem.FieldValues["financeTaskOutcome"] != null)
                            {
                            if (listItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                                {
                                newItem.Add("ClaimStatus", "Completed");
                                }
                            else if (listItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                                {
                                newItem.Add("ClaimStatus", "Closed");
                                }
                            else
                                {
                                newItem.Add("ClaimStatus", "Rejected");
                                }
                            }
                        else if (listItem.FieldValues["financeTaskOutcome"] != null)
                            {
                            if (listItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                                {
                                newItem.Add("ClaimStatus", "Rejected");
                                }
                            }
                        else if (listItem.FieldValues["PocTaskOutcome"] != null)
                            {
                            if (listItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                                {
                                newItem.Add("ClaimStatus", "Rejected");
                                }
                            else
                                {
                                newItem.Add("ClaimStatus", "Pending");
                                }
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Pending");
                            }
                        newItem.Add("KpiDays", KPI_BIOMETRIC);
                        newItem.Add("Aging", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_BIOMETRIC,"Biometric Devices").totalDays);

                        }
                    else if (ApiType == "deductible")
                        {
                    newItem.Add("ReferenceNo", listItem.FieldValues["Title"].ToString());
                    if (listItem.FieldValues["InsuranceCompanyDeductible"] != null)
                            {
                            if (listItem.FieldValues["InsuranceCompanyDeductible"].ToString().All(char.IsDigit))
                                {
                                InsuranceDeductibleValue = Int64.Parse(listItem.FieldValues["InsuranceCompanyDeductible"].ToString());
                                }
                            }
                        if (listItem.FieldValues["FinanceDeductiblePolicy"] != null)
                            {
                            if (listItem.FieldValues["FinanceDeductiblePolicy"].ToString().All(char.IsDigit))
                                {
                                FinanceDeductibleValue = Int64.Parse(listItem.FieldValues["FinanceDeductiblePolicy"].ToString());
                                }
                            }
                        newItem.Add("DeductibleValue", (InsuranceDeductibleValue + FinanceDeductibleValue).ToString());
                        InsuranceDeductibleValue = 0;
                        FinanceDeductibleValue = 0;
                        }
                    else
                        {
                        if (listItem.FieldValues["InsuranceCompanyDeductible"] != null)
                            {
                            if (listItem.FieldValues["InsuranceCompanyDeductible"].ToString().All(char.IsDigit))
                                {
                                InsuranceDeductibleValue = Int64.Parse(listItem.FieldValues["InsuranceCompanyDeductible"].ToString());
                                }
                            }
                        if (listItem.FieldValues["FinanceDeductiblePolicy"] != null)
                            {
                            if (listItem.FieldValues["FinanceDeductiblePolicy"].ToString().All(char.IsDigit))
                                {
                                FinanceDeductibleValue = Int64.Parse(listItem.FieldValues["FinanceDeductiblePolicy"].ToString());
                                }
                            }
                        if (listItem.FieldValues["FinanceRecovery"] != null)
                            {
                            Recovery += Convert.ToDouble(listItem.FieldValues["FinanceRecovery"].ToString());
                            }

                        if (listItem.FieldValues["FinanceClaimAmount"] != null)
                            {
                            ClaimAmount += Convert.ToDouble(listItem.FieldValues["FinanceClaimAmount"].ToString());
                            }
                        newItem.Add("ClaimAmount", ClaimAmount.ToString());
                        newItem.Add("PolicyDeductible", FinanceDeductibleValue.ToString());
                        newItem.Add("InsuranceCoDeductible", InsuranceDeductibleValue.ToString());
                        newItem.Add("AmountReceive", Recovery.ToString());

                        }
                    GainLoss = (Recovery - ClaimAmount).ToString();
                    newItem.Add("LossGain", GainLoss);
                    Response.Add(newItem);
                    }
            ////}
            //Gain Loss Filter
            if ((GainStartValue == "0" && GainEndValue == "0") || (GainStartValue == null && GainEndValue == null))
                {
                GainLossFilteredResponse = Response;
                }
            else
                {
                foreach (var item in Response)
                    {
                    if ((GainStartValue != "0" && GainEndValue != "0"))
                        {
                        if (Convert.ToDouble(item["Gain"]) >= Convert.ToDouble(GainStartValue) && Convert.ToDouble(item["Gain"]) <= Convert.ToDouble(GainEndValue))
                            {
                            GainLossFilteredResponse.Add(item);
                            }
                        }
                    else if(GainStartValue == "0")
                        {
                        if (Convert.ToDouble(item["Gain"]) <= Convert.ToDouble(GainEndValue))
                            {
                            GainLossFilteredResponse.Add(item);
                            }
                        }
                    else if (GainEndValue == "0")
                        {
                        if (Convert.ToDouble(item["Gain"]) >= Convert.ToDouble(GainStartValue))
                            {
                            GainLossFilteredResponse.Add(item);
                            }
                        }
                    }
                }

            if (Status == "" || Status == null)
                {
                StatusFilteredResponse = GainLossFilteredResponse;
                }
            else
                {
                foreach (var item in GainLossFilteredResponse)
                    {
                    if (item["ClaimStatus"] == Status)
                        {
                        StatusFilteredResponse.Add(item);
                        }
                    }
                }

            //return StatusFilteredResponse;
            if (ApiType == "all")
                {

                if (!(Int32.Parse(KPI_BIOMETRIC) >= Int32.Parse(KPI)))
                    {
                    StatusFilteredResponse = new List<Dictionary<string, string>>();
                    }
                }
            return StatusFilteredResponse;
        }

        [HttpGet]
        [Route("getById/{itemId:int}")]
        public Dictionary<string, object> GetByID(int itemId)
        {
            return GetSharePointListItem(WF_TYPE, WF_ARCHIVES, SP_BIOMETRIC, itemId).FieldValues;
        }

        [HttpPost]
        [Route("new")]
        public HttpResponseMessage NewClaim(BiometricModel biometric)
        {
            return ProcessNewClaim(SP_BIOMETRIC, biometric);
        }

        [HttpPost]
        [Route("draft")]
        public HttpResponseMessage SaveDraft(BiometricModel biometric)
        {
            return ProcessDraftClaim(SP_BIOMETRIC, biometric);
        }

        [HttpPost]
        [Route("poc-feedback")]
        public HttpResponseMessage SavePOCFeedback(PocModel model)
        {
            model.ClaimFor = "Biometric";
            return ProcessPOCFeedback(SP_BIOMETRIC, model);
        }

        [HttpPost]
        [Route("vendor-feedback")]
        public HttpResponseMessage SaveVendorFeedback(VendorModel model)
        {
            model.ClaimFor = "Biometric";
            return ProcessVendorFeedback(SP_BIOMETRIC, model);
        }

        [HttpPost]
        [Route("finance-feedback")]
        public HttpResponseMessage SaveFianceFeedback(FinanceTeamModel ftModel)
        {
            ftModel.ClaimFor = "Biometric";
            return ProcessFianceFeedback(SP_BIOMETRIC, ftModel);
        }
        [HttpPost]
        [Route("hod-feedback")]
        public HttpResponseMessage SaveHODFeedback(HODModel ftModel)
            {
            ftModel.ClaimFor = "Biometric";
            return ProcessHODFeedback(SP_BIOMETRIC, ftModel);
            }
        #endregion

        #region CUSTOM_METHODS
        public override void GetNewClaimParams(BaseModel model, ListItem SPClaim)
        {
            BiometricModel biometric = model as BiometricModel;
            SPClaim["InitiatedBy"] = biometric.InitiatedBy;
            SPClaim["Title"] = biometric.ReferenceNumber;
            SPClaim["IncidentDate"] = biometric.Date;
            SPClaim["ClaimType"] = biometric.ClaimType;
            SPClaim["Region"] = biometric.Region;
            SPClaim["City"] = biometric.City;
            SPClaim["EmployeeName"] = biometric.EmployeeName;
            SPClaim["Exceptional"] = biometric.Exceptional;
            SPClaim["Serial_x0023_"] = biometric.SerialNo;
            SPClaim["Make"] = biometric.Make;
            SPClaim["Manager"] = biometric.AssignToManager;
            SPClaim["POCName"] = biometric.PocName;
            SPClaim["POCNumber"] = biometric.PocContactNo;
            SPClaim["BriefDescription"] = biometric.Description;
         

        }
        #endregion
    }
}
