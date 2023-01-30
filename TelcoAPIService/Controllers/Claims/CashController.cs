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
    [System.Web.Http.RoutePrefix("api/cash")]
    public class CashController : BaseController
    {
        private readonly string SP_CASH = Config.AppSettings["SP_CASH"];
        private readonly string WF_ARCHIVES = Config.AppSettings["WF_ARCHIVES"];
        private readonly string CASH_TYPE = Config.AppSettings["CASH"];
        private readonly string CASH_TYPE_KEY = Config.AppSettings["CASH_KEY"];
        private readonly string WF_TYPE = "Cash in Safe Claims";
        private readonly string KPI_CASH = Config.AppSettings["KPI_CASH"];

        #region ACTIONS

        [HttpGet]
        [Route("all")]
        public List<Dictionary<string, string>> GetAll(string DateFrom = null, string DateTo = null, string ApiType = null, string KPI = "0", string GainStartValue = "0", string GainEndValue = "0", string Region = "", string Status = "")
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> GainLossFilteredResponse = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> StatusFilteredResponse = new List<Dictionary<string, string>>();
            ListItemCollection listItems = FetchDataFromList(SP_CASH, DateFrom, DateTo, Region);
            foreach (ListItem listItem in listItems)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                Int64 FinanceDeductibleValue = 0;
                Int64 InsuranceDeductibleValue = 0;
                //DateTime claimEndDate, claimInitiatedDate;

                //ListItem claimDate = FetchDateFromArchive("Biometric", biometricItem["ID"].ToString())[0];

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", CASH_TYPE);
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
                    newItem.Add("DaysTaken", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "", "", "Cash in Safe Claims").totalDays);
                    newItem.Add("KPI", KPI_CASH);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("Region", listItem.FieldValues["Region"].ToString());
                    newItem.Add("POCName", listItem.FieldValues["POCName"].ToString());
                    newItem.Add("POCNumber", listItem.FieldValues["POCNumber"].ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "", "", "Cash in Safe Claims").ArchivedDate);
                    newItem.Add("DOI", listItem.FieldValues["IncidentDate"].ToString());
                    newItem.Add("FranchiseID", listItem.FieldValues["FranchiseID"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    if (listItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", listItem.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "Vendor", "", "Cash in Safe Claims").ArchivedDate);
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
                    newItem.Add("KpiDays", KPI_CASH);
                    newItem.Add("Aging", getDaysTaken(listItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(listItem.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_CASH, "Cash in Safe Claims").totalDays);

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

                if (!(Int32.Parse(KPI_CASH) >= Int32.Parse(KPI)))
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
            return GetSharePointListItem(WF_TYPE, WF_ARCHIVES, SP_CASH, itemId).FieldValues;
        }

        [HttpPost]
        [Route("new")]
        public HttpResponseMessage NewClaim(CashModel model)
        {
            return ProcessNewClaim(SP_CASH, model);
        }

        [HttpPost]
        [Route("draft")]
        public HttpResponseMessage SaveDraft(CashModel model)
        {
            return ProcessDraftClaim(SP_CASH, model);
        }

        [HttpPost]
        [Route("poc-feedback")]
        public HttpResponseMessage SavePOCFeedback(PocModel model)
        {
            model.ClaimFor = "Cash";
            return ProcessPOCFeedback(SP_CASH, model);
        }

        [HttpPost]
        [Route("vendor-feedback")]
        public HttpResponseMessage SaveVendorFeedback(VendorModel model)
        {
            model.ClaimFor = "Cash";
            return ProcessVendorFeedback(SP_CASH, model);
        }

        [HttpPost]
        [Route("finance-feedback")]
        public HttpResponseMessage SaveFianceFeedback(FinanceTeamModel ftModel)
        {
            ftModel.ClaimFor = "Cash";
            return ProcessFianceFeedback(SP_CASH, ftModel);
        }

        [HttpPost]
        [Route("finance-review")]
        public HttpResponseMessage SaveFianceInsuranceTeamReview(FinInsuranceReviewModel ftModel)
        {
            ftModel.ClaimFor = "Cash";
            return ProcessFianceInsuranceTeamReviewForm(SP_CASH, ftModel);
        }

        /*[HttpPost]
        [Route("manager-attachment")]
        public HttpResponseMessage SaveTeamManagerAttachmentDoc(TeamManagerAttachmentDocModel ftModel)
        {
            ftModel.ClaimFor = "Cash";
            return ProcessTeamManagerAttachmentDoc(SP_CASH, ftModel);
        }*/
        [HttpPost]
        [Route("hod-feedback")]
        public HttpResponseMessage SaveHODFeedback(HODModel ftModel)
            {
            ftModel.ClaimFor = "Cash";
            return ProcessHODFeedback(SP_CASH, ftModel);
            }
        #endregion

        #region CUSTOM_METHODS
        public override void GetNewClaimParams(BaseModel model, ListItem SPClaim)
        {
            CashModel cash = model as CashModel;
            SPClaim["InitiatedBy"] = cash.InitiatedBy;
            SPClaim["Title"] = cash.ReferenceNumber;
            SPClaim["IncidentDate"] = cash.Date;
            SPClaim["ClaimType"] = cash.ClaimType;
            SPClaim["Region"] = cash.Region;
            SPClaim["City"] = cash.City;
            SPClaim["Exceptional"] = cash.Exceptional;

            //  SPClaim["PocDescription"] = marine.PettyId;
            SPClaim["BriefDescription"] = cash.IncidentReason;
            SPClaim["POCName"] = cash.PocName;
            SPClaim["POCNumber"] = cash.POCNumber;
            SPClaim["EFICS"] = cash.PettyAmountStolen;
            SPClaim["FranchiseID"] = cash.FranchiseID;

        }

        #endregion
        }
    }
