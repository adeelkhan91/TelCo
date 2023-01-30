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
    [System.Web.Http.RoutePrefix("api/marine-import")]
    public class MarineImportController : BaseController
    {
        private readonly string SP_MARINE_IMPORT = Config.AppSettings["SP_MARINE_IMPORT"];
        private readonly string WF_ARCHIVES = Config.AppSettings["WF_ARCHIVES"];
        private readonly string MARINEIMPORT_TYPE = Config.AppSettings["MARINEIMPORT"];
        private readonly string MARINEIMPORT_TYPE_KEY = Config.AppSettings["MARINEIMPORT_KEY"];
        private readonly string KPI_MARINE_IMPORT = Config.AppSettings["KPI_MARINE_IMPORT"];
        private readonly string WF_TYPE = "Marine Imports";

        #region ACTIONS

        [HttpGet]
        [Route("all")]
        public List<Dictionary<string, string>> GetAll(string DateFrom = null, string DateTo = null, string ApiType = null, string KPI = "0", string GainStartValue = "0", string GainEndValue = "0", string Region = "", string Status = "")
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> GainLossFilteredResponse = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> StatusFilteredResponse = new List<Dictionary<string, string>>();
            ListItemCollection listItems = FetchDataFromList(SP_MARINE_IMPORT, DateFrom, DateTo, Region);
            foreach (ListItem listItem in listItems)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                Int64 FinanceDeductibleValue = 0;
                Int64 InsuranceDeductibleValue = 0;
                //DateTime claimEndDate, claimInitiatedDate;

                //ListItem claimDate = FetchDateFromArchive("Biometric", biometricItem["ID"].ToString())[0];

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", MARINEIMPORT_TYPE);
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
                    newItem.Add("DaysTaken", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "", "", "Marine Imports").totalDays);


                    newItem.Add("KPI", KPI_MARINE_IMPORT);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("Region", listItem.FieldValues["Region"].ToString());
                    newItem.Add("POCName", listItem.FieldValues["POCName"].ToString());
                    newItem.Add("POCNumber", listItem.FieldValues["POCNumber"].ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "", "", "Marine Imports").ArchivedDate);
                    newItem.Add("DOI", listItem.FieldValues["IncidentDate"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    if (listItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", listItem.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "Vendor", "", "Marine Imports").ArchivedDate);
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
                    newItem.Add("KpiDays", KPI_MARINE_IMPORT);
                    newItem.Add("Aging", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_MARINE_IMPORT,"Marine Imports").totalDays);

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
                    else if (GainStartValue == "0")
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

                if (!(Int32.Parse(KPI_MARINE_IMPORT) >= Int32.Parse(KPI)))
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
            return GetSharePointListItem(WF_TYPE, WF_ARCHIVES, SP_MARINE_IMPORT, itemId).FieldValues;
        }

        [HttpPost]
        [Route("new")]
        public HttpResponseMessage NewClaim(MarineImportModel model)
        {
            return ProcessNewClaim(SP_MARINE_IMPORT, model);
        }

        [HttpPost]
        [Route("draft")]
        public HttpResponseMessage SaveDraft(MarineImportModel model)
        {
            return ProcessDraftClaim(SP_MARINE_IMPORT, model);
        }

        [HttpPost]
        [Route("poc-feedback")]
        public HttpResponseMessage SavePOCFeedback(PocModel model)
        {
            model.ClaimFor = "MarineImport";
            return ProcessPOCFeedback(SP_MARINE_IMPORT, model);
        }

        [HttpPost]
        [Route("vendor-feedback")]
        public HttpResponseMessage SaveVendorFeedback(VendorModel model)
        {
            model.ClaimFor = "MarineImport";
            return ProcessVendorFeedback(SP_MARINE_IMPORT, model);
        }

        [HttpPost]
        [Route("finance-feedback")]
        public HttpResponseMessage SaveFianceFeedback(FinanceTeamModel ftModel)
        {
            ftModel.ClaimFor = "MarineImport";
            return ProcessFianceFeedback(SP_MARINE_IMPORT, ftModel);
        }
        [HttpPost]
        [Route("hod-feedback")]
        public HttpResponseMessage SaveHODFeedback(HODModel ftModel)
            {
            ftModel.ClaimFor = "MarineImport";
            return ProcessHODFeedback(SP_MARINE_IMPORT, ftModel);
            }
        #endregion

        #region CUSTOM_METHODS
        public override void GetNewClaimParams(BaseModel model, ListItem SPClaim)
        {
            MarineImportModel marine = model as MarineImportModel;
            SPClaim["InitiatedBy"] = marine.InitiatedBy;
            SPClaim["Title"] = marine.ReferenceNumber;
            SPClaim["IncidentDate"] = marine.Date;
            SPClaim["ClaimType"] = marine.ClaimType;
            SPClaim["Region"] = marine.Region;
            SPClaim["City"] = marine.City;

            SPClaim["Exceptional"] = marine.Exceptional;
            SPClaim["BriefDescription"] = marine.IncidentReason;
            SPClaim["Manager"] = marine.AssignToManager;
            SPClaim["POCName"] = marine.PocName;
            SPClaim["POCNumber"] = marine.PocContactNo;


        }
        #endregion
    }
}
