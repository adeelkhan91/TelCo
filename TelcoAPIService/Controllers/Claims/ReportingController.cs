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
using System.Threading.Tasks;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace TelcoAPIService.Controllers
    {
    [System.Web.Http.RoutePrefix("api/reporting")]
    public class ReportingController : BaseController
        {
        // Sharepoint API
        private readonly string WF_ARCHIVE = Config.AppSettings["WF_ARCHIVES"];
        private readonly string BIOMETRIC = Config.AppSettings["SP_BIOMETRIC"];
        private readonly string BSD = Config.AppSettings["SP_BSD"];
        private readonly string CASH = Config.AppSettings["SP_CASH"];
        private readonly string CELL_SITE = Config.AppSettings["SP_CELL_SITE"];
        private readonly string HANDSET = Config.AppSettings["SP_HANDSET"];
        private readonly string LAPTOP = Config.AppSettings["SP_LAPTOP"];
        private readonly string MARINE_IMPORT = Config.AppSettings["SP_MARINE_IMPORT"];
        private readonly string MARINE_INLAND = Config.AppSettings["SP_MARINE_INLAND"];
        private readonly string VEHICLE = Config.AppSettings["SP_VEHICLE"];
        private readonly string FinHubURL = Config.AppSettings["FinHubURL"];
        private readonly string VendorURL = Config.AppSettings["SP_VendorURL"];

        // Sharepoint ClaimType
        private readonly string BIOMETRIC_TYPE = Config.AppSettings["BIOMETRIC"];
        private readonly string BSD_TYPE = Config.AppSettings["BSD"];
        private readonly string CASH_TYPE = Config.AppSettings["CASH"];
        private readonly string CELLSITE_TYPE = Config.AppSettings["CELLSITE"];
        private readonly string HANDSET_TYPE = Config.AppSettings["HANDSET"];
        private readonly string LAPTOP_TYPE = Config.AppSettings["LAPTOP"];
        private readonly string MARINEINLAND_TYPE = Config.AppSettings["MARINEINLAND"];
        private readonly string MARINEIMPORT_TYPE = Config.AppSettings["MARINEIMPORT"];
        private readonly string VEHICLE_TYPE = Config.AppSettings["VEHICLE"];

        // Sharepoint ClaimTypeKey
        private readonly string BIOMETRIC_TYPE_KEY = Config.AppSettings["BIOMETRIC_KEY"];
        private readonly string BSD_TYPE_KEY = Config.AppSettings["BSD_KEY"];
        private readonly string CASH_TYPE_KEY = Config.AppSettings["CASH_KEY"];
        private readonly string CELLSITE_TYPE_KEY = Config.AppSettings["CELLSITE_KEY"];
        private readonly string HANDSET_TYPE_KEY = Config.AppSettings["HANDSET_KEY"];
        private readonly string LAPTOP_TYPE_KEY = Config.AppSettings["LAPTOP_KEY"];
        private readonly string MARINEINLAND_TYPE_KEY = Config.AppSettings["MARINEINLAND_KEY"];
        private readonly string MARINEIMPORT_TYPE_KEY = Config.AppSettings["MARINEIMPORT_KEY"];
        private readonly string VEHICLE_TYPE_KEY = Config.AppSettings["VEHICLE_KEY"];

        // Sharepoint ClaimKpiKey
        private readonly string KPI_BIOMETRIC = Config.AppSettings["KPI_BIOMETRIC"];
        private readonly string KPI_BSD = Config.AppSettings["KPI_BSD"];
        private readonly string KPI_CASH = Config.AppSettings["KPI_CASH"];
        private readonly string KPI_CELL_SITE = Config.AppSettings["KPI_CELL_SITE"];
        private readonly string KPI_HANDSET = Config.AppSettings["KPI_HANDSET"];
        private readonly string KPI_LAPTOP = Config.AppSettings["KPI_LAPTOP"];
        private readonly string KPI_MARINE_IMPORT = Config.AppSettings["KPI_MARINE_IMPORT"];
        private readonly string KPI_MARINE_INLAND = Config.AppSettings["KPI_MARINE_INLAND"];
        private readonly string KPI_VEHICLE = Config.AppSettings["KPI_VEHICLE"];
        private readonly string Mail= Config.AppSettings["Mail"];
        private readonly string DisplayName= Config.AppSettings["DisplayName"];
        private readonly string Password= Config.AppSettings["Password"];
        private readonly string Host= Config.AppSettings["Host"];
        private readonly string Port = Config.AppSettings["Port"];



        [HttpGet]
        [Route("list")]
        public List<Dictionary<string, string>> GetAll(string DateFrom=null, String DateTo=null,string KPI="0", string GainStartValue="0", string GainEndValue="0",string Region="",string Status="")
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> GainLossFilteredResponse = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> StatusFilteredResponse = new List<Dictionary<string, string>>();
            ListItemCollection biometricItems = FetchDataFromList(BIOMETRIC, DateFrom, DateTo, Region);
            ListItemCollection bsdItems = FetchDataFromList(BSD, DateFrom, DateTo,Region);
            ListItemCollection cashItems = FetchDataFromList(CASH, DateFrom, DateTo, Region);
            ListItemCollection cellsites = FetchDataFromList(CELL_SITE, DateFrom, DateTo, Region);
            ListItemCollection handsetItems = FetchDataFromList(HANDSET, DateFrom, DateTo, Region);
            ListItemCollection laptopItems = FetchDataFromList(LAPTOP, DateFrom, DateTo, Region);
            ListItemCollection marineImportItems = FetchDataFromList(MARINE_IMPORT, DateFrom, DateTo, Region);
            ListItemCollection marineInlandItems = FetchDataFromList(MARINE_INLAND, DateFrom, DateTo, Region);
            ListItemCollection vehicleItems = FetchDataFromList(VEHICLE, DateFrom, DateTo, Region);

            if(Int32.Parse(KPI_BIOMETRIC) >= Int32.Parse(KPI))
                {
                foreach (ListItem biometricItem in biometricItems)
                    {
                    double Recovery = 0, ClaimAmount = 0;
                    //DateTime claimEndDate, claimInitiatedDate;

                    //ListItem claimDate = FetchDateFromArchive("Biometric", biometricItem["ID"].ToString())[0];

                    Dictionary<string, string> newItem = new Dictionary<string, string>();

                    newItem.Add("ClaimId", biometricItem.FieldValues["ItemID"].ToString());
                    newItem.Add("Exceptional", biometricItem.FieldValues["Exceptional"].ToString());
                    if (biometricItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery = Convert.ToDouble(biometricItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (biometricItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount = Convert.ToDouble(biometricItem.FieldValues["FinanceClaimAmount"].ToString());
                        }

                    newItem.Add("ReferenceNo", biometricItem.FieldValues["Title"].ToString());
                    if (biometricItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", biometricItem.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    newItem.Add("Created", Convert.ToDateTime(biometricItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                    newItem.Add("ClaimCategory", BIOMETRIC_TYPE);
                    newItem.Add("ClaimType", biometricItem.FieldValues["ClaimType"].ToString());
                    if (biometricItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (biometricItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (biometricItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (biometricItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                    if (biometricItem.FieldValues["FinanceComments"] != null)
                        {
                        newItem.Add("FinanceComments", biometricItem.FieldValues["FinanceComments"].ToString());
                        }
                    else
                        {
                        newItem.Add("FinanceComments", "-");
                        }
                    newItem.Add("DaysTaken", getDaysTaken(biometricItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(biometricItem.FieldValues["Created"]).ToLocalTime(),"", "", "Biometric Devices").totalDays);
                    newItem.Add("KPI", KPI_BIOMETRIC);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Region", biometricItem.FieldValues["Region"].ToString());
                    newItem.Add("POCName", biometricItem.FieldValues["POCName"].ToString());
                    newItem.Add("Make", biometricItem.FieldValues["Make"].ToString());
                    newItem.Add("POCNumber", biometricItem.FieldValues["POCNumber"].ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(biometricItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(biometricItem.FieldValues["Created"]).ToLocalTime(),"", "", "Biometric Devices").ArchivedDate);
                    newItem.Add("DOI", biometricItem.FieldValues["IncidentDate"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(biometricItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(biometricItem.FieldValues["Created"]).ToLocalTime(), "Vendor","","Biometric Devices").ArchivedDate);
                    Response.Add(newItem);
                    }
                }
            if (Int32.Parse(KPI_BSD) >= Int32.Parse(KPI))
                {
                foreach (ListItem bsdItem in bsdItems)
                    {
                    double Recovery = 0, ClaimAmount = 0;
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("ClaimId", bsdItem.FieldValues["ItemID"].ToString());
                    newItem.Add("Exceptional", bsdItem.FieldValues["Exceptional"].ToString());
                    //ListItemCollection claimDate = FetchDateFromArchive("Highvalue Tools", bsdItem["ID"].ToString());
                    if (bsdItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery = Convert.ToDouble(bsdItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (bsdItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount = Convert.ToDouble(bsdItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    if (bsdItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", bsdItem.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    newItem.Add("ReferenceNo", bsdItem.FieldValues["Title"].ToString());
                    newItem.Add("Created", Convert.ToDateTime(bsdItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                    newItem.Add("ClaimCategory", BSD_TYPE);
                    newItem.Add("ClaimType", bsdItem.FieldValues["ClaimType"].ToString());

                    if (bsdItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (bsdItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (bsdItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (bsdItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                    if (bsdItem.FieldValues["FinanceComments"] != null)
                        {
                        newItem.Add("FinanceComments", bsdItem.FieldValues["FinanceComments"].ToString());
                        }
                    else
                        {
                        newItem.Add("FinanceComments", "-");
                        }
                    newItem.Add("DaysTaken", getDaysTaken(bsdItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(bsdItem.FieldValues["Created"]).ToLocalTime(), "", "", "Highvalue Tools").totalDays);

                    newItem.Add("KPI", KPI_BSD);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("Region", bsdItem.FieldValues["Region"].ToString());
                    newItem.Add("POCName", bsdItem.FieldValues["POCName"].ToString());
                    newItem.Add("Make", bsdItem.FieldValues["Make"].ToString());
                    newItem.Add("POCNumber", bsdItem.FieldValues["POCNumber"].ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(bsdItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(bsdItem.FieldValues["Created"]).ToLocalTime(),"", "", "Highvalue Tools").ArchivedDate);
                    newItem.Add("DOI", bsdItem.FieldValues["IncidentDate"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(bsdItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(bsdItem.FieldValues["Created"]).ToLocalTime(), "Vendor", "", "Highvalue Tools").ArchivedDate);
                    Response.Add(newItem);
                    }
                }
            if (Int32.Parse(KPI_CASH) >= Int32.Parse(KPI))
                {
                foreach (ListItem cashItem in cashItems)
                    {
                    double Recovery = 0, ClaimAmount = 0;
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("ClaimId", cashItem.FieldValues["ItemID"].ToString());
                    newItem.Add("Exceptional", cashItem.FieldValues["Exceptional"].ToString());
                    //ListItemCollection claimDate = FetchDateFromArchive("Biometric", cashItem["ID"].ToString());
                    if (cashItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery = Convert.ToDouble(cashItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (cashItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount = Convert.ToDouble(cashItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    if (cashItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", cashItem.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    newItem.Add("ReferenceNo", cashItem.FieldValues["Title"].ToString());
                    newItem.Add("Created", Convert.ToDateTime(cashItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                    newItem.Add("ClaimCategory", CASH_TYPE);
                    newItem.Add("ClaimType", cashItem.FieldValues["ClaimType"].ToString());
                    if (cashItem.FieldValues["FinanceComments"] != null)
                        {
                        newItem.Add("FinanceComments", cashItem.FieldValues["FinanceComments"].ToString());
                        }
                    else
                        {
                        newItem.Add("FinanceComments", "-");
                        }
                    if (cashItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (cashItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (cashItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (cashItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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

                    newItem.Add("DaysTaken", getDaysTaken(cashItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(cashItem.FieldValues["Created"]).ToLocalTime(),"", "", "Cash in Safe Claims").totalDays);
                    newItem.Add("KPI", KPI_CASH);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("Region", cashItem.FieldValues["Region"].ToString());
                    newItem.Add("POCName", cashItem.FieldValues["POCName"].ToString());
                    newItem.Add("POCNumber", cashItem.FieldValues["POCNumber"].ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(cashItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(cashItem.FieldValues["Created"]).ToLocalTime(),"", "", "Cash in Safe Claims").ArchivedDate);
                    newItem.Add("DOI", cashItem.FieldValues["IncidentDate"].ToString());
                    newItem.Add("FranchiseID", cashItem.FieldValues["FranchiseID"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(cashItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(cashItem.FieldValues["Created"]).ToLocalTime(), "Vendor", "", "Cash in Safe Claims").ArchivedDate);
                    Response.Add(newItem);
                    }
                }
            if (Int32.Parse(KPI_CELL_SITE) >= Int32.Parse(KPI))
                {
                foreach (ListItem cellsite in cellsites)
                    {
                    double Recovery = 0, ClaimAmount = 0;
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("ClaimId", cellsite.FieldValues["ItemID"].ToString());
                    newItem.Add("Exceptional", cellsite.FieldValues["Exceptional"].ToString());
                    //ListItemCollection claimDate = FetchDateFromArchive("Cell Site Claims", cellsite["ID"].ToString());
                    if (cellsite.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery = Convert.ToDouble(cellsite.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (cellsite.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount = Convert.ToDouble(cellsite.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    if (cellsite.FieldValues["FinanceComments"] != null)
                        {
                        newItem.Add("FinanceComments", cellsite.FieldValues["FinanceComments"].ToString());
                        }
                    else
                        {
                        newItem.Add("FinanceComments", "-");
                        }
                    if (cellsite.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", cellsite.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    newItem.Add("ReferenceNo", cellsite.FieldValues["Title"].ToString());
                    newItem.Add("Created", Convert.ToDateTime(cellsite.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                    newItem.Add("ClaimCategory", CELLSITE_TYPE);
                    newItem.Add("ClaimType", cellsite.FieldValues["ClaimType"].ToString());
                    if (cellsite.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (cellsite.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (cellsite.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (cellsite.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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

                    newItem.Add("DaysTaken", getDaysTaken(cellsite.FieldValues["ItemID"].ToString(), Convert.ToDateTime(cellsite.FieldValues["Created"]).ToLocalTime(),"", "", "Cell Site Claims").totalDays);

                    newItem.Add("KPI", KPI_CELL_SITE);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("Region", cellsite.FieldValues["Region"].ToString());
                    newItem.Add("POCName", cellsite.FieldValues["POCName"].ToString());
                    newItem.Add("SiteCode", cellsite.FieldValues["SiteCode"].ToString());
                    newItem.Add("POCNumber", cellsite.FieldValues["POCNumber"].ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(cellsite.FieldValues["ItemID"].ToString(), Convert.ToDateTime(cellsite.FieldValues["Created"]).ToLocalTime(),"", "", "Cell Site Claims").ArchivedDate);
                    newItem.Add("DOI", cellsite.FieldValues["IncidentDate"].ToString());
                    newItem.Add("Entity", cellsite.FieldValues["Entity"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(cellsite.FieldValues["ItemID"].ToString(), Convert.ToDateTime(cellsite.FieldValues["Created"]).ToLocalTime(), "Vendor", "", "Cell Site Claims").ArchivedDate);
                    Response.Add(newItem);
                    }
                }
            if (Int32.Parse(KPI_HANDSET) >= Int32.Parse(KPI))
                {
                foreach (ListItem handsetItem in handsetItems)
                    {
                    double Recovery = 0, ClaimAmount = 0;
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("ClaimId", handsetItem.FieldValues["ItemID"].ToString());
                    newItem.Add("Exceptional", handsetItem.FieldValues["Exceptional"].ToString());
                    //ListItemCollection claimDate = FetchDateFromArchive("Handsets", handsetItem["ID"].ToString());
                    if (handsetItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery = Convert.ToDouble(handsetItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (handsetItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount = Convert.ToDouble(handsetItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    newItem.Add("ReferenceNo", handsetItem.FieldValues["Title"].ToString());
                    newItem.Add("Created", Convert.ToDateTime(handsetItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                    newItem.Add("ClaimCategory", HANDSET_TYPE);
                    newItem.Add("ClaimType", handsetItem.FieldValues["ClaimType"].ToString());

                    if (handsetItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (handsetItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (handsetItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (handsetItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                    if (handsetItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", handsetItem.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    if (handsetItem.FieldValues["FinanceComments"] != null)
                        {
                        newItem.Add("FinanceComments", handsetItem.FieldValues["FinanceComments"].ToString());
                        }
                    else
                        {
                        newItem.Add("FinanceComments", "-");
                        }
                    newItem.Add("DaysTaken", getDaysTaken(handsetItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(handsetItem.FieldValues["Created"]).ToLocalTime(),"", "", "Handsets").totalDays);


                    newItem.Add("KPI", KPI_HANDSET);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("Region", handsetItem.FieldValues["Region"].ToString());
                    newItem.Add("Make", handsetItem.FieldValues["Make"].ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(handsetItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(handsetItem.FieldValues["Created"]).ToLocalTime(),"", "", "Handsets").ArchivedDate);
                    newItem.Add("DOI", handsetItem.FieldValues["IncidentDate"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(handsetItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(handsetItem.FieldValues["Created"]).ToLocalTime(), "Vendor", "", "Handsets").ArchivedDate);
                    Response.Add(newItem);
                    }
                }
            if (Int32.Parse(KPI_LAPTOP) >= Int32.Parse(KPI))
                {
                foreach (ListItem laptopItem in laptopItems)
                    {
                    double Recovery = 0, ClaimAmount = 0;
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("ClaimId", laptopItem.FieldValues["ItemID"].ToString());
                    newItem.Add("Exceptional", laptopItem.FieldValues["Exceptional"].ToString());
                    //ListItemCollection claimDate = FetchDateFromArchive("Laptop", laptopItem["ID"].ToString());
                    if (laptopItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery = Convert.ToDouble(laptopItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (laptopItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount = Convert.ToDouble(laptopItem.FieldValues["FinanceClaimAmount"].ToString());
                        }

                    newItem.Add("ReferenceNo", laptopItem.FieldValues["Title"].ToString());
                    newItem.Add("Created", Convert.ToDateTime(laptopItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                    newItem.Add("ClaimCategory", LAPTOP_TYPE);
                    newItem.Add("ClaimType", laptopItem.FieldValues["ClaimType"].ToString());
                    if (laptopItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", laptopItem.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    if (laptopItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (laptopItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (laptopItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (laptopItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                    if (laptopItem.FieldValues["FinanceComments"] != null)
                        {
                        newItem.Add("FinanceComments", laptopItem.FieldValues["FinanceComments"].ToString());
                        }
                    else
                        {
                        newItem.Add("FinanceComments", "-");
                        }
                    newItem.Add("DaysTaken", getDaysTaken(laptopItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(laptopItem.FieldValues["Created"]).ToLocalTime(),"", "", "Laptop").totalDays);


                    newItem.Add("KPI", KPI_LAPTOP);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("Region", laptopItem.FieldValues["Region"].ToString());
                    newItem.Add("Make", laptopItem.FieldValues["Make"].ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(laptopItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(laptopItem.FieldValues["Created"]).ToLocalTime(),"", "", "Laptop").ArchivedDate);
                    newItem.Add("DOI", laptopItem.FieldValues["IncidentDate"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(laptopItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(laptopItem.FieldValues["Created"]).ToLocalTime(), "Vendor", "", "Laptop").ArchivedDate);
                    Response.Add(newItem);
                    }
                }
            if (Int32.Parse(KPI_MARINE_IMPORT) >= Int32.Parse(KPI))
                {
                foreach (ListItem marineImportItem in marineImportItems)
                    {
                    double Recovery = 0, ClaimAmount = 0;
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("ClaimId", marineImportItem.FieldValues["ItemID"].ToString());
                    newItem.Add("Exceptional", marineImportItem.FieldValues["Exceptional"].ToString());
                    //ListItemCollection claimDate = FetchDateFromArchive("Marine Imports", marineImportItem["ID"].ToString());
                    if (marineImportItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery = Convert.ToDouble(marineImportItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (marineImportItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount = Convert.ToDouble(marineImportItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    newItem.Add("ReferenceNo", marineImportItem.FieldValues["Title"].ToString());
                    newItem.Add("Created", Convert.ToDateTime(marineImportItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                    newItem.Add("ClaimCategory", MARINEIMPORT_TYPE);
                    newItem.Add("ClaimType", marineImportItem.FieldValues["ClaimType"].ToString());
                    if (marineImportItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", marineImportItem.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    if (marineImportItem.FieldValues["FinanceComments"] != null)
                        {
                        newItem.Add("FinanceComments", marineImportItem.FieldValues["FinanceComments"].ToString());
                        }
                    else
                        {
                        newItem.Add("FinanceComments", "-");
                        }
                    if (marineImportItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (marineImportItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (marineImportItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (marineImportItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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

                    newItem.Add("DaysTaken", getDaysTaken(marineImportItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(marineImportItem.FieldValues["Created"]).ToLocalTime(),"", "", "Marine Imports").totalDays);


                    newItem.Add("KPI", KPI_MARINE_IMPORT);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("Region", marineImportItem.FieldValues["Region"].ToString());
                    newItem.Add("POCName", marineImportItem.FieldValues["POCName"].ToString());
                    newItem.Add("POCNumber", marineImportItem.FieldValues["POCNumber"].ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(marineImportItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(marineImportItem.FieldValues["Created"]).ToLocalTime(),"", "", "Marine Imports").ArchivedDate);
                    newItem.Add("DOI", marineImportItem.FieldValues["IncidentDate"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(marineImportItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(marineImportItem.FieldValues["Created"]).ToLocalTime(), "Vendor", "", "Marine Imports").ArchivedDate);
                    Response.Add(newItem);
                    }
                }
            if (Int32.Parse(KPI_MARINE_INLAND) >= Int32.Parse(KPI))
                {
                foreach (ListItem marineInlandItem in marineInlandItems)
                    {
                    double Recovery = 0, ClaimAmount = 0;
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("ClaimId", marineInlandItem.FieldValues["ItemID"].ToString());
                    newItem.Add("Exceptional", marineInlandItem.FieldValues["Exceptional"].ToString());
                    //ListItemCollection claimDate = FetchDateFromArchive("Marine Inland", marineInlandItem["ID"].ToString());
                    if (marineInlandItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery = Convert.ToDouble(marineInlandItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (marineInlandItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount = Convert.ToDouble(marineInlandItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    if (marineInlandItem.FieldValues["FinanceComments"] != null)
                        {
                        newItem.Add("FinanceComments", marineInlandItem.FieldValues["FinanceComments"].ToString());
                        }
                    else
                        {
                        newItem.Add("FinanceComments", "-");
                        }
                    if (marineInlandItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", marineInlandItem.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    newItem.Add("ReferenceNo", marineInlandItem.FieldValues["Title"].ToString());
                    newItem.Add("Created", Convert.ToDateTime(marineInlandItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                    newItem.Add("ClaimCategory", MARINEINLAND_TYPE);
                    newItem.Add("ClaimType", marineInlandItem.FieldValues["ClaimType"].ToString());

                    if (marineInlandItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (marineInlandItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (marineInlandItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (marineInlandItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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

                    newItem.Add("DaysTaken", getDaysTaken(marineInlandItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(marineInlandItem.FieldValues["Created"]).ToLocalTime(),"", "", "Marine Inland").totalDays);


                    newItem.Add("KPI", KPI_MARINE_INLAND);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("Region", marineInlandItem.FieldValues["Region"].ToString());
                    newItem.Add("POCName", marineInlandItem.FieldValues["POCName"].ToString());
                    newItem.Add("POCNumber", marineInlandItem.FieldValues["POCNumber"].ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(marineInlandItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(marineInlandItem.FieldValues["Created"]).ToLocalTime(),"", "", "Marine Inland").ArchivedDate);
                    newItem.Add("DOI", marineInlandItem.FieldValues["IncidentDate"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(marineInlandItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(marineInlandItem.FieldValues["Created"]).ToLocalTime(), "Vendor", "", "Marine Inland").ArchivedDate);
                    Response.Add(newItem);
                    }
                }
            if (Int32.Parse(KPI_VEHICLE) >= Int32.Parse(KPI))
                {
                foreach (ListItem vehicleItem in vehicleItems)
                    {
                    double Recovery = 0, ClaimAmount = 0;
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("ClaimId", vehicleItem.FieldValues["ItemID"].ToString());
                    newItem.Add("Exceptional", vehicleItem.FieldValues["Exceptional"].ToString());
                    //ListItemCollection claimDate = FetchDateFromArchive("Motor Vehicles", vehicleItem["ID"].ToString());
                    if (vehicleItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery = Convert.ToDouble(vehicleItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (vehicleItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount = Convert.ToDouble(vehicleItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    if (vehicleItem.FieldValues["FinanceComments"] != null)
                        {
                        newItem.Add("FinanceComments", vehicleItem.FieldValues["FinanceComments"].ToString());
                        }
                    else
                        {
                        newItem.Add("FinanceComments", "-");
                        }
                    if (vehicleItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        newItem.Add("InternalDepartment", vehicleItem.FieldValues["PocTaskOutcome"].ToString());
                        }
                    else
                        {
                        newItem.Add("InternalDepartment", "-");
                        }
                    newItem.Add("ReferenceNo", vehicleItem.FieldValues["Title"].ToString());
                    newItem.Add("Created", Convert.ToDateTime(vehicleItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                    newItem.Add("ClaimCategory", VEHICLE_TYPE);
                    newItem.Add("ClaimType", vehicleItem.FieldValues["ClaimType"].ToString());

                    if (vehicleItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (vehicleItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (vehicleItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (vehicleItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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

                    newItem.Add("DaysTaken", getDaysTaken(vehicleItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(vehicleItem.FieldValues["Created"]).ToLocalTime(),"", "", "Motor Vehicles").totalDays);


                    newItem.Add("KPI", KPI_VEHICLE);
                    newItem.Add("AmountRecevied", ClaimAmount.ToString());
                    newItem.Add("NBV", Recovery.ToString());
                    newItem.Add("Gain", (Recovery - ClaimAmount).ToString());
                    newItem.Add("Region", vehicleItem.FieldValues["Region"].ToString());
                    newItem.Add("POCName", vehicleItem.FieldValues["POCName"].ToString());
                    newItem.Add("Make", vehicleItem.FieldValues["Make"].ToString());
                    newItem.Add("EngineNumber", vehicleItem.FieldValues["EngineNumber"].ToString());
                    newItem.Add("RegistrationNumber", vehicleItem.FieldValues["RegistrationNumber"].ToString());
                    newItem.Add("ChassisNumber", vehicleItem.FieldValues["ChassisNumber"].ToString());
                    newItem.Add("ModelNumber", vehicleItem.FieldValues["ModelNumber"].ToString());
                    newItem.Add("POCNumber", vehicleItem.FieldValues["POCNumber"].ToString());
                    newItem.Add("ClaimClosureStatus", newItem["ClaimStatus"] == "Rejected" ? "Completed" : newItem["ClaimStatus"]);
                    newItem.Add("ClosureDate", getDaysTaken(vehicleItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(vehicleItem.FieldValues["Created"]).ToLocalTime(),"", "", "Motor Vehicles").ArchivedDate);
                    newItem.Add("DOI", vehicleItem.FieldValues["IncidentDate"].ToString());
                    newItem.Add("InsuranceCompany", "-");
                    newItem.Add("IntimationInsuranceCompany", getDaysTaken(vehicleItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(vehicleItem.FieldValues["Created"]).ToLocalTime(),"Vendor", "", "Motor Vehicles").ArchivedDate);
                    Response.Add(newItem);
                    }
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

            if (Status == ""  || Status == null)
                {
                StatusFilteredResponse = GainLossFilteredResponse;
                }
            else
                {
                foreach (var item in GainLossFilteredResponse)
                    {
                    if (item["ClaimStatus"] ==Status)
                        {
                        StatusFilteredResponse.Add(item);
                        }
                    }
                }

            return StatusFilteredResponse;
            }
        [HttpGet]
        [Route("myRequest")]
        public List<Dictionary<string, string>> GetMyRequests(string DateFrom = null, String DateTo = null,string Email=null)
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection biometricItems = FetchMyRequestData(BIOMETRIC, DateFrom, DateTo);
            ListItemCollection bsdItems = FetchMyRequestData(BSD, DateFrom, DateTo);
            ListItemCollection cashItems = FetchMyRequestData(CASH, DateFrom, DateTo);
            ListItemCollection cellsites = FetchMyRequestData(CELL_SITE, DateFrom, DateTo);
            ListItemCollection handsetItems = FetchMyRequestData(HANDSET, DateFrom, DateTo);
            ListItemCollection laptopItems = FetchMyRequestData(LAPTOP, DateFrom, DateTo);
            ListItemCollection marineImportItems = FetchMyRequestData(MARINE_IMPORT, DateFrom, DateTo);
            ListItemCollection marineInlandItems = FetchMyRequestData(MARINE_INLAND, DateFrom, DateTo);
            ListItemCollection vehicleItems = FetchMyRequestData(VEHICLE, DateFrom, DateTo);

            foreach (ListItem biometricItem in biometricItems)
                {
                if(biometricItem.FieldValues["InitiatedBy"].ToString()==Email)
                    {
                     

                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("BriefDescription", biometricItem.FieldValues["BriefDescription"].ToString());
                    newItem.Add("Reference#", biometricItem.FieldValues["Title"].ToString());
                    newItem.Add("ClaimCategory", BIOMETRIC_TYPE);
                    newItem.Add("ClaimType", biometricItem.FieldValues["ClaimType"].ToString());
                    if (biometricItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (biometricItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (biometricItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (biometricItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                    newItem.Add("IsDraft", biometricItem.FieldValues["IsDraft"].ToString());
                    newItem.Add("InitIsReject", biometricItem.FieldValues["InitIsReject"].ToString());                    
                    newItem.Add("ClaimId", biometricItem.FieldValues["ItemID"].ToString());
                    newItem.Add("DOI", biometricItem.FieldValues["IncidentDate"].ToString());
                    Response.Add(newItem);
                    }
                }
            foreach (ListItem bsdItem in bsdItems)
                {
                if (bsdItem.FieldValues["InitiatedBy"].ToString() == Email)
                    {
                     
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    
                    newItem.Add("IsDraft", bsdItem.FieldValues["IsDraft"].ToString());
                    newItem.Add("InitIsReject", bsdItem.FieldValues["InitIsReject"].ToString());
                    newItem.Add("ClaimId", bsdItem.FieldValues["ItemID"].ToString());
                    newItem.Add("BriefDescription", bsdItem.FieldValues["BriefDescription"].ToString());
                    newItem.Add("Reference#", bsdItem.FieldValues["Title"].ToString());
                    newItem.Add("ClaimCategory", BSD_TYPE);
                    newItem.Add("ClaimType", bsdItem.FieldValues["ClaimType"].ToString());

                    if (bsdItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (bsdItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (bsdItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (bsdItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                    newItem.Add("DOI", bsdItem.FieldValues["IncidentDate"].ToString());
                    Response.Add(newItem);
                    }
                }
            foreach (ListItem cashItem in cashItems)
                {
                if (cashItem.FieldValues["InitiatedBy"].ToString() == Email)
                    {
                     
                    Dictionary<string, string> newItem = new Dictionary<string, string>();

                    newItem.Add("IsDraft", cashItem.FieldValues["IsDraft"].ToString());
                    newItem.Add("InitIsReject", cashItem.FieldValues["InitIsReject"].ToString());
                    newItem.Add("ClaimId",cashItem.FieldValues["ItemID"].ToString());
                    newItem.Add("BriefDescription", cashItem.FieldValues["BriefDescription"].ToString());
                    newItem.Add("Reference#", cashItem.FieldValues["Title"].ToString()); 
                    newItem.Add("ClaimCategory", CASH_TYPE);
                    newItem.Add("ClaimType", cashItem.FieldValues["ClaimType"].ToString());

                    if (cashItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (cashItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (cashItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (cashItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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

                    newItem.Add("DOI", cashItem.FieldValues["IncidentDate"].ToString());
                    Response.Add(newItem);
                    }
                }
            foreach (ListItem cellsite in cellsites)
                {
                if (cellsite.FieldValues["InitiatedBy"].ToString() == Email)
                    {
                     
                    Dictionary<string, string> newItem = new Dictionary<string, string>();

                    newItem.Add("IsDraft", cellsite.FieldValues["IsDraft"].ToString());
                    newItem.Add("InitIsReject", cellsite.FieldValues["InitIsReject"].ToString());
                    newItem.Add("ClaimId", cellsite.FieldValues["ItemID"].ToString());
                    newItem.Add("BriefDescription", cellsite.FieldValues["BriefDescription"].ToString());
                    newItem.Add("Reference#", cellsite.FieldValues["Title"].ToString());
                    newItem.Add("ClaimCategory", CELLSITE_TYPE);
                    newItem.Add("ClaimType", cellsite.FieldValues["ClaimType"].ToString());
                    if (cellsite.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (cellsite.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (cellsite.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (cellsite.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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

                    newItem.Add("DOI", cellsite.FieldValues["IncidentDate"].ToString());
                    Response.Add(newItem);
                    }
                }
            foreach (ListItem handsetItem in handsetItems)
                {
                if (handsetItem.FieldValues["InitiatedBy"].ToString() == Email)
                    {
                     
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("IsDraft", handsetItem.FieldValues["IsDraft"].ToString());
                    newItem.Add("InitIsReject", handsetItem.FieldValues["InitIsReject"].ToString());
                    newItem.Add("ClaimId", handsetItem.FieldValues["ItemID"].ToString());
                    newItem.Add("BriefDescription", handsetItem.FieldValues["BriefDescription"].ToString());
                    newItem.Add("Reference#", handsetItem.FieldValues["Title"].ToString()); 
                    newItem.Add("ClaimCategory", HANDSET_TYPE);
                    newItem.Add("ClaimType", handsetItem.FieldValues["ClaimType"].ToString());

                    if (handsetItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (handsetItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (handsetItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (handsetItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                    newItem.Add("DOI", handsetItem.FieldValues["IncidentDate"].ToString());
                    Response.Add(newItem);
                    }
                }
            foreach (ListItem laptopItem in laptopItems)
                {
                if (laptopItem.FieldValues["InitiatedBy"].ToString() == Email)
                    {
                     
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    newItem.Add("IsDraft", laptopItem.FieldValues["IsDraft"].ToString());
                    newItem.Add("InitIsReject", laptopItem.FieldValues["InitIsReject"].ToString());
                    newItem.Add("ClaimId", laptopItem.FieldValues["ItemID"].ToString());
                    newItem.Add("BriefDescription", laptopItem.FieldValues["BriefDescription"].ToString());
                    newItem.Add("Reference#", laptopItem.FieldValues["Title"].ToString()); 
                    newItem.Add("ClaimCategory", LAPTOP_TYPE);
                    newItem.Add("ClaimType", laptopItem.FieldValues["ClaimType"].ToString());
                    if (laptopItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (laptopItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (laptopItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (laptopItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                    newItem.Add("DOI", laptopItem.FieldValues["IncidentDate"].ToString());
                     Response.Add(newItem);
                    }
                }
            foreach (ListItem marineImportItem in marineImportItems)
                {
                if (marineImportItem.FieldValues["InitiatedBy"].ToString() == Email)
                    {
                     
                    Dictionary<string, string> newItem = new Dictionary<string, string>();

                    newItem.Add("IsDraft", marineImportItem.FieldValues["IsDraft"].ToString());
                    newItem.Add("InitIsReject", marineImportItem.FieldValues["InitIsReject"].ToString());
                    newItem.Add("ClaimId", marineImportItem.FieldValues["ItemID"].ToString());
                    newItem.Add("BriefDescription", marineImportItem.FieldValues["BriefDescription"].ToString());
                    newItem.Add("Reference#", marineImportItem.FieldValues["Title"].ToString()); 
                    newItem.Add("ClaimCategory", MARINEIMPORT_TYPE);
                    newItem.Add("ClaimType", marineImportItem.FieldValues["ClaimType"].ToString());

                    if (marineImportItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (marineImportItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (marineImportItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (marineImportItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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

                    newItem.Add("DOI", marineImportItem.FieldValues["IncidentDate"].ToString());
                    Response.Add(newItem);
                    }
                }
            foreach (ListItem marineInlandItem in marineInlandItems)
                {
                if (marineInlandItem.FieldValues["InitiatedBy"].ToString() == Email)
                    {
                     
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    
                    newItem.Add("IsDraft", marineInlandItem.FieldValues["IsDraft"].ToString());
                    newItem.Add("InitIsReject", marineInlandItem.FieldValues["InitIsReject"].ToString());
                    newItem.Add("ClaimId", marineInlandItem.FieldValues["ItemID"].ToString());
                    newItem.Add("BriefDescription", marineInlandItem.FieldValues["BriefDescription"].ToString());
                    newItem.Add("Reference#", marineInlandItem.FieldValues["Title"].ToString()); 
                    newItem.Add("ClaimCategory", MARINEINLAND_TYPE);
                    newItem.Add("ClaimType", marineInlandItem.FieldValues["ClaimType"].ToString());

                    if (marineInlandItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (marineInlandItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (marineInlandItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (marineInlandItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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

                    newItem.Add("DOI", marineInlandItem.FieldValues["IncidentDate"].ToString());
                    Response.Add(newItem);
                    }
                }
            foreach (ListItem vehicleItem in vehicleItems)
                {
                if (vehicleItem.FieldValues["InitiatedBy"].ToString() == Email)
                    {
                     
                    Dictionary<string, string> newItem = new Dictionary<string, string>();
                    
                    newItem.Add("IsDraft", vehicleItem.FieldValues["IsDraft"].ToString());
                    newItem.Add("InitIsReject", vehicleItem.FieldValues["InitIsReject"].ToString());
                    newItem.Add("ClaimId", vehicleItem.FieldValues["ItemID"].ToString());
                    newItem.Add("BriefDescription", vehicleItem.FieldValues["BriefDescription"].ToString());
                    newItem.Add("Reference#", vehicleItem.FieldValues["Title"].ToString()); 
                    newItem.Add("ClaimCategory", VEHICLE_TYPE);
                    newItem.Add("ClaimType", vehicleItem.FieldValues["ClaimType"].ToString());

                    if (vehicleItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                            {
                            newItem.Add("ClaimStatus", "Completed");
                            }
                        else if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                            {
                            newItem.Add("ClaimStatus", "Closed");
                            }
                        else
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (vehicleItem.FieldValues["financeTaskOutcome"] != null)
                        {
                        if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                            {
                            newItem.Add("ClaimStatus", "Rejected");
                            }
                        }
                    else if (vehicleItem.FieldValues["PocTaskOutcome"] != null)
                        {
                        if (vehicleItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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

                    newItem.Add("DOI", vehicleItem.FieldValues["IncidentDate"].ToString());
                    Response.Add(newItem);
                    }
                }
            return Response;
            }
        [HttpGet]
        [Route("getTotalRequestsCount")]
        public List<Dictionary<string, string>> GetTotalRequestsCOunt(string DateFrom = null, String DateTo = null, string Email = null)
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection biometricItems = FetchMyRequestData(BIOMETRIC, DateFrom, DateTo);
            ListItemCollection bsdItems = FetchMyRequestData(BSD, DateFrom, DateTo);
            ListItemCollection cashItems = FetchMyRequestData(CASH, DateFrom, DateTo);
            ListItemCollection cellsites = FetchMyRequestData(CELL_SITE, DateFrom, DateTo);
            ListItemCollection handsetItems = FetchMyRequestData(HANDSET, DateFrom, DateTo);
            ListItemCollection laptopItems = FetchMyRequestData(LAPTOP, DateFrom, DateTo);
            ListItemCollection marineImportItems = FetchMyRequestData(MARINE_IMPORT, DateFrom, DateTo);
            ListItemCollection marineInlandItems = FetchMyRequestData(MARINE_INLAND, DateFrom, DateTo);
            ListItemCollection vehicleItems = FetchMyRequestData(VEHICLE, DateFrom, DateTo);

            int TotalBiometricCount=0;
            int BioMetricRejectCount = 0;
            int BioMetricApproveCount = 0;

            int TotalBSDCount = 0;
            int BSDRejectCount = 0;
            int BSDApproveCount = 0;

            int TotalCashCount = 0;
            int CashRejectCount = 0;
            int CashApproveCount = 0;

            int TotalCellSiteCount = 0;
            int CellSiteRejectCount = 0;
            int CellSiteApproveCount = 0;

            int TotalHandsetCount = 0;
            int HandsetRejectCount = 0;
            int HandsetApproveCount = 0;

            int TotalLaptopCount = 0;
            int LaptopRejectCount = 0;
            int LaptopApproveCount = 0;

            int TotalMImportCount = 0;
            int MImportRejectCount = 0;
            int MImportApproveCount = 0;

            int TotalMInlandCount = 0;
            int MInlandRejectCount = 0;
            int MInlandApproveCount = 0;

            int TotalVehicleCount = 0;
            int VehicleRejectCount = 0;
            int VehicleApproveCount = 0;
            Dictionary<string, string> newItem = new Dictionary<string, string>();


            foreach (ListItem biometricItem in biometricItems)
                {
                TotalBiometricCount++;
                    if(biometricItem.FieldValues["PocTaskOutcome"]!=null)
                    {
                    if (biometricItem.FieldValues["PocTaskOutcome"].ToString()=="Reject")
                        {
                            BioMetricRejectCount++;
                        }
                    }
                if (biometricItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                            BioMetricRejectCount++;
                        }
                    }
                if (biometricItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                            BioMetricApproveCount++;
                        }
                    }
                }
            foreach (ListItem bsdItem in bsdItems)
                {
                TotalBSDCount++;
                if (bsdItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (bsdItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        BSDRejectCount++;
                        }
                    }
                if (bsdItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        BSDRejectCount++;
                        }
                    }
                if (bsdItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        BSDApproveCount++;
                        }
                    }
                }
            foreach (ListItem cashItem in cashItems)
                {
                TotalCashCount++;
                if (cashItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (cashItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        CashRejectCount++;
                        }
                    }
                if (cashItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        CashRejectCount++;
                        }
                    }
                if (cashItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        CashApproveCount++;
                        }
                    }
                }
            foreach (ListItem cellsite in cellsites)
                {
                TotalCellSiteCount++;
                if (cellsite.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (cellsite.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        CellSiteRejectCount++;
                        }
                    }
                if (cellsite.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        CellSiteRejectCount++;
                        }
                    }
                if (cellsite.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        CellSiteApproveCount++;
                        }
                    }
                }
            foreach (ListItem handsetItem in handsetItems)
                {
                TotalHandsetCount++;
                if (handsetItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (handsetItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        HandsetRejectCount++;
                        }
                    }
                if (handsetItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        HandsetRejectCount++;
                        }
                    }
                if (handsetItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        HandsetApproveCount++;
                        }
                    }
                }
            foreach (ListItem laptopItem in laptopItems)
                {
                TotalLaptopCount++;
                if (laptopItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (laptopItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        LaptopRejectCount++;
                        }
                    }
                if (laptopItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        LaptopRejectCount++;
                        }
                    }
                if (laptopItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        LaptopApproveCount++;
                        }
                    }
                }
            foreach (ListItem marineImportItem in marineImportItems)
                {
                TotalMImportCount++;
                if (marineImportItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (marineImportItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        MImportRejectCount++;
                        }
                    }
                if (marineImportItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        MImportRejectCount++;
                        }
                    }
                if (marineImportItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        MImportApproveCount++;
                        }
                    }
                }
            foreach (ListItem marineInlandItem in marineInlandItems)
                {
                TotalMInlandCount++;
                if (marineInlandItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (marineInlandItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        MInlandRejectCount++;
                        }
                    }
                if (marineInlandItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        MInlandRejectCount++;
                        }
                    }
                if (marineInlandItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        MInlandApproveCount++;
                        }
                    }
                }
            foreach (ListItem vehicleItem in vehicleItems)
                {
                TotalVehicleCount++;
                if (vehicleItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (vehicleItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        VehicleRejectCount++;
                        }
                    }
                if (vehicleItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        VehicleRejectCount++;
                        }
                    }
                if (vehicleItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        VehicleApproveCount++;
                        }
                    }
                }
            newItem.Add("TotalBiometricCount", TotalBiometricCount.ToString());
            newItem.Add("BiometricInitiatedCount", (TotalBiometricCount - BioMetricRejectCount - BioMetricApproveCount).ToString());
            newItem.Add("BioMetricRejectCount", BioMetricRejectCount.ToString());
            newItem.Add("BioMetricApproveCount", BioMetricApproveCount.ToString());

            newItem.Add("TotalBSDCount", TotalBSDCount.ToString());
            newItem.Add("BSDInitiatedCount", (TotalBSDCount - BSDRejectCount - BSDApproveCount).ToString());
            newItem.Add("BSDRejectCount", BSDRejectCount.ToString());
            newItem.Add("BSDApproveCount", BSDApproveCount.ToString());

            newItem.Add("TotalCashCount", TotalCashCount.ToString());
            newItem.Add("CashInitiatedCount", (TotalCashCount - CashRejectCount - CashApproveCount).ToString());
            newItem.Add("CashRejectCount", CashRejectCount.ToString());
            newItem.Add("CashApproveCount", CashApproveCount.ToString());

            newItem.Add("TotalCellSiteCount", TotalCellSiteCount.ToString());
            newItem.Add("CellSiteInitiatedCount", (TotalCellSiteCount - CellSiteRejectCount - CellSiteApproveCount).ToString());
            newItem.Add("CellSiteRejectCount", CellSiteRejectCount.ToString());
            newItem.Add("CellSiteApproveCount", CellSiteApproveCount.ToString());

            newItem.Add("TotalHandsetCount", TotalHandsetCount.ToString());
            newItem.Add("HandsetInitiatedCount", (TotalHandsetCount - HandsetRejectCount - HandsetApproveCount).ToString());
            newItem.Add("HandsetRejectCount", HandsetRejectCount.ToString());
            newItem.Add("HandsetApproveCount", HandsetApproveCount.ToString());

            newItem.Add("TotalLaptopCount", TotalLaptopCount.ToString());
            newItem.Add("LaptopInitiatedCount", (TotalLaptopCount - LaptopRejectCount - LaptopApproveCount).ToString());
            newItem.Add("LaptopRejectCount", LaptopRejectCount.ToString());
            newItem.Add("LaptopApproveCount", LaptopApproveCount.ToString());

            newItem.Add("TotalMImportCount", TotalMImportCount.ToString());
            newItem.Add("MImportInitiatedCount", (TotalMImportCount - MImportRejectCount - MImportApproveCount).ToString());
            newItem.Add("MImportRejectCount", MImportRejectCount.ToString());
            newItem.Add("MImportApproveCount", MImportApproveCount.ToString());

            newItem.Add("TotalMInlandCount", TotalMInlandCount.ToString());
            newItem.Add("MInlandInitiatedCount", (TotalMInlandCount - MInlandRejectCount - MInlandApproveCount).ToString());
            newItem.Add("MInlandRejectCount", MInlandRejectCount.ToString());
            newItem.Add("MInlandApproveCount", MInlandApproveCount.ToString());

            newItem.Add("TotalVehicleCount", TotalVehicleCount.ToString());
            newItem.Add("VehicleInitiatedCount", (TotalVehicleCount - VehicleRejectCount - VehicleApproveCount).ToString());
            newItem.Add("VehicleRejectCount", VehicleRejectCount.ToString());
            newItem.Add("VehicleApproveCount", VehicleApproveCount.ToString());
            Response.Add(newItem);
            return Response;
            }

        [HttpGet]
        [Route("myPendingRequest")]
        public List<Dictionary<string, string>> GetMyPendingRequests(string DateFrom = null, String DateTo = null, string Scopes = null, string ClaimCategory = "")
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();

            ListItemCollection biometricItems=null;
            ListItemCollection bsdItems= null;
            ListItemCollection cashItems = null;
            ListItemCollection cellsites = null;
            ListItemCollection handsetItems = null;
            ListItemCollection laptopItems = null;
            ListItemCollection marineImportItems = null;
            ListItemCollection marineInlandItems = null;
            ListItemCollection vehicleItems = null;

            if (ClaimCategory != null)
                {
                if (ClaimCategory == "Biometric")
                    {
                    biometricItems = FetchMyRequestData(BIOMETRIC, DateFrom, DateTo);
                    }
                if (ClaimCategory == "Bsd")
                    {
                    bsdItems = FetchMyRequestData(BSD, DateFrom, DateTo);
                    }
                if (ClaimCategory == "Cash")
                    {
                    cashItems = FetchMyRequestData(CASH, DateFrom, DateTo);
                    }
                if (ClaimCategory == "Cellsite")
                    {
                    cellsites = FetchMyRequestData(CELL_SITE, DateFrom, DateTo);
                    }
                if (ClaimCategory == "Handset")
                    {
                    handsetItems = FetchMyRequestData(HANDSET, DateFrom, DateTo);
                    }
                if (ClaimCategory == "Laptop")
                    {
                    laptopItems = FetchMyRequestData(LAPTOP, DateFrom, DateTo);
                    }
                if (ClaimCategory == "MarineImport")
                    {
                    marineImportItems = FetchMyRequestData(MARINE_IMPORT, DateFrom, DateTo);
                    }
                if (ClaimCategory == "MarineInland")
                    {
                    marineInlandItems = FetchMyRequestData(MARINE_INLAND, DateFrom, DateTo);
                    }
                if (ClaimCategory == "Vehicle")
                    {
                    vehicleItems = FetchMyRequestData(VEHICLE, DateFrom, DateTo);
                    }
                }
            else
                {
                    biometricItems = FetchMyRequestData(BIOMETRIC, DateFrom, DateTo);
                    bsdItems = FetchMyRequestData(BSD, DateFrom, DateTo);
                    cashItems = FetchMyRequestData(CASH, DateFrom, DateTo);
                    cellsites = FetchMyRequestData(CELL_SITE, DateFrom, DateTo);
                    handsetItems = FetchMyRequestData(HANDSET, DateFrom, DateTo);
                    laptopItems = FetchMyRequestData(LAPTOP, DateFrom, DateTo);
                    marineImportItems = FetchMyRequestData(MARINE_IMPORT, DateFrom, DateTo);
                    marineInlandItems = FetchMyRequestData(MARINE_INLAND, DateFrom, DateTo);
                    vehicleItems = FetchMyRequestData(VEHICLE, DateFrom, DateTo);                    
                }
                
                if (biometricItems !=null)
                {
                foreach (ListItem biometricItem in biometricItems)
                {
                if (Scopes.Contains("Biometric_Devices-POC") && biometricItem.FieldValues["PocDescription"] == null && biometricItem.FieldValues["IsDraft"].ToString() == "False")
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", biometricItem.FieldValues["BriefDescription"].ToString());
                newItem.Add("Reference#", biometricItem.FieldValues["Title"].ToString());
                newItem.Add("PocTask", "Pending");
                newItem.Add("ClaimId", biometricItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", biometricItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", biometricItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(biometricItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", BIOMETRIC_TYPE);
                newItem.Add("ClaimType", biometricItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", biometricItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", biometricItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Biometric_Devices-Finance") && biometricItem.FieldValues["IsFinance"].ToString() == "False" && biometricItem.FieldValues["PocDescription"] != null && biometricItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", biometricItem.FieldValues["BriefDescription"].ToString());
                newItem.Add("Reference#", biometricItem.FieldValues["Title"].ToString());
                newItem.Add("FinanceTask", "Pending");
                newItem.Add("ClaimId", biometricItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", biometricItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", biometricItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(biometricItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", BIOMETRIC_TYPE);
                newItem.Add("ClaimType", biometricItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", biometricItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", biometricItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Biometric_Devices-Finance") && biometricItem.FieldValues["IsFinance"].ToString() == "True" && biometricItem.FieldValues["PocDescription"] != null && biometricItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", biometricItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", biometricItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Approve");
                newItem.Add("ClaimId", biometricItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", biometricItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", biometricItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(biometricItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", BIOMETRIC_TYPE);
                newItem.Add("ClaimType", biometricItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", biometricItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", biometricItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                }
                }
                if (bsdItems != null)
                {
                foreach (ListItem bsdItem in bsdItems)
                {
                if (Scopes.Contains("Highvalue_Tools-POC") && bsdItem.FieldValues["PocDescription"] == null && bsdItem.FieldValues["IsDraft"].ToString() == "False")
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", bsdItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", bsdItem.FieldValues["Title"].ToString());
                        newItem.Add("PocTask", "Pending");
                newItem.Add("ClaimId", bsdItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", bsdItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", bsdItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(bsdItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", BSD_TYPE);
                newItem.Add("ClaimType", bsdItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", bsdItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", bsdItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (bsdItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True" && Scopes.Contains("Highvalue_Tools-Finance") && bsdItem.FieldValues["IsFinance"].ToString() == "False" && bsdItem.FieldValues["PocDescription"] != null)
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", bsdItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", bsdItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Pending");
                newItem.Add("ClaimId", bsdItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", bsdItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", bsdItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(bsdItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", BSD_TYPE);
                newItem.Add("ClaimType", bsdItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", bsdItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", bsdItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Highvalue_Tools-Finance") && bsdItem.FieldValues["IsFinance"].ToString() == "True" && bsdItem.FieldValues["PocDescription"] != null && bsdItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", bsdItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", bsdItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Approve");
                newItem.Add("ClaimId", bsdItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", bsdItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", bsdItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(bsdItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", BSD_TYPE);
                newItem.Add("ClaimType", bsdItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", bsdItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", bsdItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                }
                }
                if (cashItems != null)
                {
                foreach (ListItem cashItem in cashItems)
                {
                if (Scopes.Contains("Cash_in_Safe_Claims-POC") && cashItem.FieldValues["PocDescription"] == null && cashItem.FieldValues["IsDraft"].ToString() == "False")
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", cashItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", cashItem.FieldValues["Title"].ToString());
                        newItem.Add("PocTask", "Pending");
                newItem.Add("ClaimId", cashItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", cashItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", cashItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(cashItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", CASH_TYPE);
                newItem.Add("ClaimType", cashItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", cashItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", cashItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (cashItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True" && Scopes.Contains("Cash_in_Safe_Claims-Finance") && cashItem.FieldValues["IsFinance"].ToString() == "False" && cashItem.FieldValues["PocDescription"] != null)
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", cashItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", cashItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Pending");
                newItem.Add("ClaimId", cashItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", cashItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", cashItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(cashItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", CASH_TYPE);
                newItem.Add("ClaimType", cashItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", cashItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", cashItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Cash_in_Safe_Claims-Finance") && cashItem.FieldValues["IsFinance"].ToString() == "True" && cashItem.FieldValues["PocDescription"] != null && cashItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", cashItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", cashItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Approve");
                newItem.Add("ClaimId", cashItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", cashItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", cashItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(cashItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", CASH_TYPE);
                newItem.Add("ClaimType", cashItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", cashItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", cashItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Cash_in_Safe_Claims-Finance") && cashItem.FieldValues["financeTeamTaskOutcome"] == null && cashItem.FieldValues["IfPocTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", cashItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", cashItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTeamTaskOutcome", "Pending");
                newItem.Add("ClaimId", cashItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", cashItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", cashItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(cashItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", CASH_TYPE);
                newItem.Add("ClaimType", cashItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", cashItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", cashItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }

                }
                }
                if (cellsites != null)
                {
                foreach (ListItem cellsite in cellsites)
                {
                if (Scopes.Contains("Cell_Site_Claims-POC") && cellsite.FieldValues["PocDescription"] == null && cellsite.FieldValues["IsDraft"].ToString() == "False")
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", cellsite.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", cellsite.FieldValues["Title"].ToString());
                        newItem.Add("PocTask", "Pending");
                newItem.Add("ClaimId", cellsite.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", cellsite.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", cellsite.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(cellsite.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", CELLSITE_TYPE);
                newItem.Add("ClaimType", cellsite.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", cellsite.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", cellsite.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (cellsite.FieldValues["ifVendorTaskUpdate"].ToString() == "True" && Scopes.Contains("Cell_Site_Claims-Finance") && cellsite.FieldValues["IsFinance"].ToString() == "False" && cellsite.FieldValues["PocDescription"] != null)
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", cellsite.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", cellsite.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Pending");
                newItem.Add("ClaimId", cellsite.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", cellsite.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", cellsite.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(cellsite.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", CELLSITE_TYPE);
                newItem.Add("ClaimType", cellsite.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", cellsite.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", cellsite.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Cell_Site_Claims-Finance") && cellsite.FieldValues["IsFinance"].ToString() == "True" && cellsite.FieldValues["PocDescription"] != null && cellsite.FieldValues["ifVendorTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", cellsite.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", cellsite.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Approve");
                newItem.Add("ClaimId", cellsite.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", cellsite.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", cellsite.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(cellsite.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", CELLSITE_TYPE);
                newItem.Add("ClaimType", cellsite.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", cellsite.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", cellsite.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Cell_Site_Claims-Finance") && cellsite.FieldValues["ifFixedAssetTaskUpdate"].ToString() == "False" && cellsite.FieldValues["IfPocTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", cellsite.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", cellsite.FieldValues["Title"].ToString());
                        newItem.Add("FixedAssetTaskUpdate", "Pending");
                newItem.Add("ClaimId", cellsite.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", cellsite.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", cellsite.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(cellsite.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", CELLSITE_TYPE);
                newItem.Add("ClaimType", cellsite.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", cellsite.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", cellsite.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }

                }
                }
                if (handsetItems != null)
                {
                foreach (ListItem handsetItem in handsetItems)
                {
                if (Scopes.Contains("Handsets-POC") && handsetItem.FieldValues["PocDescription"] == null && handsetItem.FieldValues["IsDraft"].ToString() == "False")
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", handsetItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", handsetItem.FieldValues["Title"].ToString());
                        newItem.Add("PocTask", "Pending");
                newItem.Add("ClaimId", handsetItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", handsetItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", handsetItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(handsetItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", HANDSET_TYPE);
                newItem.Add("ClaimType", handsetItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", handsetItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", handsetItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (handsetItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True" && Scopes.Contains("Handsets-Finance") && handsetItem.FieldValues["IsFinance"].ToString() == "False" && handsetItem.FieldValues["PocDescription"] != null)
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", handsetItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", handsetItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Pending");
                newItem.Add("ClaimId", handsetItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", handsetItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", handsetItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(handsetItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", HANDSET_TYPE);
                newItem.Add("ClaimType", handsetItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", handsetItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", handsetItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Handsets-Finance") && handsetItem.FieldValues["IsFinance"].ToString() == "True" && handsetItem.FieldValues["PocDescription"] != null && handsetItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True")
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", handsetItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", handsetItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Approve");
                newItem.Add("ClaimId", handsetItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", handsetItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", handsetItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(handsetItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", HANDSET_TYPE);
                newItem.Add("ClaimType", handsetItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", handsetItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", handsetItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                }
                }
                if (laptopItems != null)
                {
                foreach (ListItem laptopItem in laptopItems)
                {
                if (Scopes.Contains("Laptop-POC") && laptopItem.FieldValues["PocDescription"] == null && laptopItem.FieldValues["IsDraft"].ToString() == "False")
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", laptopItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", laptopItem.FieldValues["Title"].ToString());
                        newItem.Add("PocTask", "Pending");
                newItem.Add("ClaimId", laptopItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", laptopItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", laptopItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(laptopItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", LAPTOP_TYPE);
                newItem.Add("ClaimType", laptopItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", laptopItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", laptopItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (laptopItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True" && Scopes.Contains("Laptop-Finance") && laptopItem.FieldValues["IsFinance"].ToString() == "False" && laptopItem.FieldValues["PocDescription"] != null)
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", laptopItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", laptopItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Pending");
                newItem.Add("ClaimId", laptopItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", laptopItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", laptopItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(laptopItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", LAPTOP_TYPE);
                newItem.Add("ClaimType", laptopItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", laptopItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", laptopItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Laptop-Finance") && laptopItem.FieldValues["IsFinance"].ToString() == "True" && laptopItem.FieldValues["PocDescription"] != null && laptopItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", laptopItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", laptopItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Approve");
                newItem.Add("ClaimId", laptopItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", laptopItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", laptopItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(laptopItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", LAPTOP_TYPE);
                newItem.Add("ClaimType", laptopItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", laptopItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", laptopItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                }
                }
                if (marineImportItems != null)
                {
                foreach (ListItem marineImportItem in marineImportItems)
                {
                if (Scopes.Contains("Marine_Imports-POC") && marineImportItem.FieldValues["PocDescription"] == null && marineImportItem.FieldValues["IsDraft"].ToString() == "False")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", marineImportItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", marineImportItem.FieldValues["Title"].ToString());
                        newItem.Add("PocTask", "Pending");
                newItem.Add("ClaimId", marineImportItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", marineImportItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", marineImportItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(marineImportItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", MARINEIMPORT_TYPE);
                newItem.Add("ClaimType", marineImportItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", marineImportItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", marineImportItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (marineImportItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True" && Scopes.Contains("Marine_Imports-Finance") && marineImportItem.FieldValues["IsFinance"].ToString() == "False" && marineImportItem.FieldValues["PocDescription"] != null)
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", marineImportItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", marineImportItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Pending");
                newItem.Add("ClaimId", marineImportItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", marineImportItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", marineImportItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(marineImportItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", MARINEIMPORT_TYPE);
                newItem.Add("ClaimType", marineImportItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", marineImportItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", marineImportItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Marine_Imports-Finance") && marineImportItem.FieldValues["IsFinance"].ToString() == "True" && marineImportItem.FieldValues["PocDescription"] != null && marineImportItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", marineImportItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", marineImportItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Approve");
                newItem.Add("ClaimId", marineImportItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", marineImportItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", marineImportItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(marineImportItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", MARINEIMPORT_TYPE);
                newItem.Add("ClaimType", marineImportItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", marineImportItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", marineImportItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                }
                }
                if (marineInlandItems != null)
                {
                foreach (ListItem marineInlandItem in marineInlandItems)
                {
                if (Scopes.Contains("Marine_Inland-POC") && marineInlandItem.FieldValues["PocDescription"] == null && marineInlandItem.FieldValues["IsDraft"].ToString() == "False")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", marineInlandItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", marineInlandItem.FieldValues["Title"].ToString());
                        newItem.Add("PocTask", "Pending");
                newItem.Add("ClaimId", marineInlandItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", marineInlandItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", marineInlandItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(marineInlandItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", MARINEINLAND_TYPE);
                newItem.Add("ClaimType", marineInlandItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", marineInlandItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", marineInlandItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (marineInlandItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True" && Scopes.Contains("Marine_Inland-Finance") && marineInlandItem.FieldValues["IsFinance"].ToString() == "False" && marineInlandItem.FieldValues["PocDescription"] != null)
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", marineInlandItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", marineInlandItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Pending");
                newItem.Add("ClaimId", marineInlandItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", marineInlandItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", marineInlandItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(marineInlandItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", MARINEINLAND_TYPE);
                newItem.Add("ClaimType", marineInlandItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", marineInlandItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", marineInlandItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Marine_Inland-Finance") && marineInlandItem.FieldValues["IsFinance"].ToString() == "True" && marineInlandItem.FieldValues["PocDescription"] != null && marineInlandItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", marineInlandItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", marineInlandItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Approve");
                newItem.Add("ClaimId", marineInlandItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", marineInlandItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", marineInlandItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(marineInlandItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", MARINEINLAND_TYPE);
                newItem.Add("ClaimType", marineInlandItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", marineInlandItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", marineInlandItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Marine_Inland-Finance") && marineInlandItem.FieldValues["financeTeamTaskOutcome"] == null && marineInlandItem.FieldValues["IfPocTaskUpdate"].ToString() == "True")
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", marineInlandItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", marineInlandItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTeamTaskOutcome", "Pending");
                newItem.Add("ClaimId", marineInlandItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", marineInlandItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", marineInlandItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(marineInlandItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", MARINEINLAND_TYPE);
                newItem.Add("ClaimType", marineInlandItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", marineInlandItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", marineInlandItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                }
                }
                if (vehicleItems != null)
                {
                foreach (ListItem vehicleItem in vehicleItems)
                {
                if (Scopes.Contains("Motor_Vehicles-POC") && vehicleItem.FieldValues["PocDescription"] == null && vehicleItem.FieldValues["IsDraft"].ToString() == "False")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", vehicleItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", vehicleItem.FieldValues["Title"].ToString());
                        newItem.Add("PocTask", "Pending");
                newItem.Add("ClaimId", vehicleItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", vehicleItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", vehicleItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(vehicleItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", VEHICLE_TYPE);
                newItem.Add("ClaimType", vehicleItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", vehicleItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", vehicleItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (vehicleItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True" && Scopes.Contains("Motor_Vehicles-Finance") && vehicleItem.FieldValues["IsFinance"].ToString() == "False" && vehicleItem.FieldValues["PocDescription"] != null)
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", vehicleItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", vehicleItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Pending");
                newItem.Add("ClaimId", vehicleItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", vehicleItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", vehicleItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(vehicleItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", VEHICLE_TYPE);
                newItem.Add("ClaimType", vehicleItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", vehicleItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", vehicleItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                if (Scopes.Contains("Motor_Vehicles-Finance") && vehicleItem.FieldValues["IsFinance"].ToString() == "True" && vehicleItem.FieldValues["PocDescription"] != null && vehicleItem.FieldValues["ifVendorTaskUpdate"].ToString() == "True")
                {

                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("BriefDescription", vehicleItem.FieldValues["BriefDescription"].ToString());
                        newItem.Add("Reference#", vehicleItem.FieldValues["Title"].ToString());
                        newItem.Add("FinanceTask", "Approve");
                newItem.Add("ClaimId", vehicleItem.FieldValues["ItemID"].ToString());
                newItem.Add("InitiatedBy", vehicleItem.FieldValues["InitiatedBy"].ToString());
                newItem.Add("ReferenceNo", vehicleItem.FieldValues["Title"].ToString());
                newItem.Add("Created", Convert.ToDateTime(vehicleItem.FieldValues["Created"]).ToLocalTime().ToString().Split('+')[0].ToString());
                newItem.Add("ClaimCategory", VEHICLE_TYPE);
                newItem.Add("ClaimType", vehicleItem.FieldValues["ClaimType"].ToString());
                newItem.Add("DOI", vehicleItem.FieldValues["IncidentDate"].ToString());
                newItem.Add("Region", vehicleItem.FieldValues["Region"].ToString());
                Response.Add(newItem);
                }
                }
                }
                return Response;
    }

        [HttpGet]
        [Route("aging")]
        public List<Dictionary<string, string>> GetAging(string DateFrom = null, String DateTo = null)
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection biometricItems = FetchDataFromList(BIOMETRIC, DateFrom, DateTo, "");
            ListItemCollection bsdItems = FetchDataFromList(BSD, DateFrom, DateTo, "");
            ListItemCollection cashItems = FetchDataFromList(CASH, DateFrom, DateTo, "");
            ListItemCollection cellsites = FetchDataFromList(CELL_SITE, DateFrom, DateTo, "");
            ListItemCollection handsetItems = FetchDataFromList(HANDSET, DateFrom, DateTo, "");
            ListItemCollection laptopItems = FetchDataFromList(LAPTOP, DateFrom, DateTo, "");
            ListItemCollection marineImportItems = FetchDataFromList(MARINE_IMPORT, DateFrom, DateTo, "");
            ListItemCollection marineInlandItems = FetchDataFromList(MARINE_INLAND, DateFrom, DateTo, "");
            ListItemCollection vehicleItems = FetchDataFromList(VEHICLE, DateFrom, DateTo, "");

            foreach (ListItem biometricItem in biometricItems)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ReferenceNo", biometricItem.FieldValues["Title"].ToString());
                newItem.Add("ClaimCategory", BIOMETRIC_TYPE);
                newItem.Add("ClaimType", biometricItem.FieldValues["ClaimType"].ToString());
                //if (biometricItem.FieldValues[BIOMETRIC_TYPE_KEY] != null)
                //    {
                //    newItem.Add("ClaimStatus", GetClaimStatus(biometricItem.FieldValues[BIOMETRIC_TYPE_KEY].ToString()));
                //    }
                //else
                //    {
                //    newItem.Add("ClaimStatus", "Pending");
                //    }
                if (biometricItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        newItem.Add("ClaimStatus", "Completed");
                        }
                    else if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                        {
                        newItem.Add("ClaimStatus", "Closed");
                        }
                    else
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (biometricItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (biometricItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (biometricItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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



                //Math.Ceiling(((DateTime.Now.ToLocalTime())-(Convert.ToDateTime(biometricItem.FieldValues["Created"]).ToLocalTime())).TotalDays).ToString()
                newItem.Add("KpiDays", KPI_BIOMETRIC);
                newItem.Add("Aging", getDaysTaken(biometricItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(biometricItem.FieldValues["Created"]).ToLocalTime(),"Aging", KPI_BIOMETRIC,"Biometric Devices").totalDays);
                Response.Add(newItem);
                }
            foreach (ListItem bsdItem in bsdItems)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ReferenceNo", bsdItem.FieldValues["Title"].ToString());
                newItem.Add("ClaimCategory", BSD_TYPE);
                newItem.Add("ClaimType", bsdItem.FieldValues["ClaimType"].ToString());
                if (bsdItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        newItem.Add("ClaimStatus", "Completed");
                        }
                    else if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                        {
                        newItem.Add("ClaimStatus", "Closed");
                        }
                    else
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (bsdItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (bsdItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (bsdItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                newItem.Add("KpiDays", KPI_BSD);
                newItem.Add("Aging", getDaysTaken(bsdItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(bsdItem.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_BSD, "Highvalue Tools").totalDays);
                Response.Add(newItem);
                }
            foreach (ListItem cashItem in cashItems)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ReferenceNo", cashItem.FieldValues["Title"].ToString());
                newItem.Add("ClaimCategory", CASH_TYPE);
                newItem.Add("ClaimType", cashItem.FieldValues["ClaimType"].ToString());
                if (cashItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        newItem.Add("ClaimStatus", "Completed");
                        }
                    else if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                        {
                        newItem.Add("ClaimStatus", "Closed");
                        }
                    else
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (cashItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (cashItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (cashItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                newItem.Add("Aging", getDaysTaken(cashItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(cashItem.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_CASH, "Cash in Safe Claims").totalDays);
                Response.Add(newItem);
                }
            foreach (ListItem cellsite in cellsites)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ReferenceNo", cellsite.FieldValues["Title"].ToString());
                newItem.Add("ClaimCategory", CELLSITE_TYPE);
                newItem.Add("ClaimType", cellsite.FieldValues["ClaimType"].ToString());
                if (cellsite.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        newItem.Add("ClaimStatus", "Completed");
                        }
                    else if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Close")
                        {
                        newItem.Add("ClaimStatus", "Closed");
                        }
                    else
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (cellsite.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (cellsite.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (cellsite.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                newItem.Add("KpiDays", KPI_CELL_SITE);
                newItem.Add("Aging", getDaysTaken(cellsite.FieldValues["ItemID"].ToString(), Convert.ToDateTime(cellsite.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_CELL_SITE, "Cell Site Claims").totalDays);
                Response.Add(newItem);
                }
            foreach (ListItem handsetItem in handsetItems)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ReferenceNo", handsetItem.FieldValues["Title"].ToString());
                newItem.Add("ClaimCategory", HANDSET_TYPE);
                newItem.Add("ClaimType", handsetItem.FieldValues["ClaimType"].ToString());
                if (handsetItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        newItem.Add("ClaimStatus", "Completed");
                        }
                    else if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                        {
                        newItem.Add("ClaimStatus", "Closed");
                        }
                    else
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (handsetItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (handsetItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (handsetItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                newItem.Add("KpiDays", KPI_HANDSET);
                newItem.Add("Aging", getDaysTaken(handsetItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(handsetItem.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_HANDSET,"Handsets").totalDays);
                Response.Add(newItem);
                }
            foreach (ListItem laptopItem in laptopItems)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ReferenceNo", laptopItem.FieldValues["Title"].ToString());
                newItem.Add("ClaimCategory", LAPTOP_TYPE);
                newItem.Add("ClaimType", laptopItem.FieldValues["ClaimType"].ToString());
                if (laptopItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        newItem.Add("ClaimStatus", "Completed");
                        }
                    else if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                        {
                        newItem.Add("ClaimStatus", "Closed");
                        }
                    else
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (laptopItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (laptopItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (laptopItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                newItem.Add("KpiDays", KPI_LAPTOP);
                newItem.Add("Aging", getDaysTaken(laptopItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(laptopItem.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_LAPTOP,"Laptop").totalDays);
                Response.Add(newItem);
                }
            foreach (ListItem marineImportItem in marineImportItems)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ReferenceNo", marineImportItem.FieldValues["Title"].ToString());
                newItem.Add("ClaimCategory", MARINEIMPORT_TYPE);
                newItem.Add("ClaimType", marineImportItem.FieldValues["ClaimType"].ToString());
                if (marineImportItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        newItem.Add("ClaimStatus", "Completed");
                        }
                    else if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                        {
                        newItem.Add("ClaimStatus", "Closed");
                        }
                    else
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (marineImportItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (marineImportItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (marineImportItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                newItem.Add("Aging", getDaysTaken(marineImportItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(marineImportItem.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_MARINE_IMPORT, "Marine Imports").totalDays);
                Response.Add(newItem);
                }
            foreach (ListItem marineInlandItem in marineInlandItems)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ReferenceNo", marineInlandItem.FieldValues["Title"].ToString());
                newItem.Add("ClaimCategory", MARINEINLAND_TYPE);
                newItem.Add("ClaimType", marineInlandItem.FieldValues["ClaimType"].ToString());
                if (marineInlandItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        newItem.Add("ClaimStatus", "Completed");
                        }
                    else if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                        {
                        newItem.Add("ClaimStatus", "Closed");
                        }
                    else
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (marineInlandItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (marineInlandItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (marineInlandItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                newItem.Add("KpiDays", KPI_MARINE_INLAND);
                newItem.Add("Aging", getDaysTaken(marineInlandItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(marineInlandItem.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_MARINE_INLAND, "Marine Inland").totalDays);
                Response.Add(newItem);
                }
            foreach (ListItem vehicleItem in vehicleItems)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ReferenceNo", vehicleItem.FieldValues["Title"].ToString());
                newItem.Add("ClaimCategory", VEHICLE_TYPE);
                newItem.Add("ClaimType", vehicleItem.FieldValues["ClaimType"].ToString());
                if (vehicleItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        newItem.Add("ClaimStatus", "Completed");
                        }
                    else if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Close")
                        {
                        newItem.Add("ClaimStatus", "Closed");
                        }
                    else
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (vehicleItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        newItem.Add("ClaimStatus", "Rejected");
                        }
                    }
                else if (vehicleItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (vehicleItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
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
                newItem.Add("KpiDays", KPI_VEHICLE);
                newItem.Add("Aging", getDaysTaken(vehicleItem.FieldValues["ItemID"].ToString(), Convert.ToDateTime(vehicleItem.FieldValues["Created"]).ToLocalTime(), "Aging", KPI_VEHICLE, "Motor Vehicles").totalDays);
                Response.Add(newItem);
                }
            return Response;
            }

        [HttpGet]
        [Route("deductiable")]
        public List<Dictionary<string, string>> GetDeductible(string DateFrom = null, String DateTo = null)
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection biometricItems = FetchDataFromList(BIOMETRIC, DateFrom, DateTo,"");
            ListItemCollection bsdItems = FetchDataFromList(BSD, DateFrom, DateTo, "");
            ListItemCollection cashItems = FetchDataFromList(CASH, DateFrom, DateTo, "");
            ListItemCollection cellsites = FetchDataFromList(CELL_SITE, DateFrom, DateTo, "");
            ListItemCollection handsetItems = FetchDataFromList(HANDSET, DateFrom, DateTo, "");
            ListItemCollection laptopItems = FetchDataFromList(LAPTOP, DateFrom, DateTo, "");
            ListItemCollection marineImportItems = FetchDataFromList(MARINE_IMPORT, DateFrom, DateTo, "");
            ListItemCollection marineInlandItems = FetchDataFromList(MARINE_INLAND, DateFrom, DateTo, "");
            ListItemCollection vehicleItems = FetchDataFromList(VEHICLE, DateFrom, DateTo, "");

            if (biometricItems != null)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", BIOMETRIC_TYPE);
                newItem.Add("ClaimCount", biometricItems.Count().ToString());
                newItem.Add("DeductibleValue", GetDeductibleValue(biometricItems));
                Response.Add(newItem);
                }
            if (bsdItems != null)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", BSD_TYPE);
                newItem.Add("ClaimCount", bsdItems.Count().ToString());
                newItem.Add("DeductibleValue", GetDeductibleValue(bsdItems));
                Response.Add(newItem);
                }
            if (cashItems != null)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", CASH_TYPE);
                newItem.Add("ClaimCount", cashItems.Count().ToString());
                newItem.Add("DeductibleValue", GetDeductibleValue(cashItems));
                Response.Add(newItem);
                }
            if (cellsites != null)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", CELLSITE_TYPE);
                newItem.Add("ClaimCount", cellsites.Count().ToString());
                newItem.Add("DeductibleValue", GetDeductibleValue(cellsites));
                Response.Add(newItem);
                }
            if (handsetItems != null)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", HANDSET_TYPE);
                newItem.Add("ClaimCount", handsetItems.Count().ToString());
                newItem.Add("DeductibleValue", GetDeductibleValue(handsetItems));
                Response.Add(newItem);
                }
            if (laptopItems != null)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", LAPTOP_TYPE);
                newItem.Add("ClaimCount", laptopItems.Count().ToString());
                newItem.Add("DeductibleValue", GetDeductibleValue(laptopItems));
                Response.Add(newItem);
                }
            if (marineImportItems != null)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", MARINEIMPORT_TYPE);
                newItem.Add("ClaimCount", marineImportItems.Count().ToString());
                newItem.Add("DeductibleValue", GetDeductibleValue(marineImportItems));
                Response.Add(newItem);
                }
            if (marineInlandItems != null)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", MARINEINLAND_TYPE);
                newItem.Add("ClaimCount", marineInlandItems.Count().ToString());
                newItem.Add("DeductibleValue", GetDeductibleValue(marineInlandItems));
                Response.Add(newItem);
                }
            if (vehicleItems != null)
                {
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                newItem.Add("ClaimCategory", VEHICLE_TYPE);
                newItem.Add("ClaimCount", vehicleItems.Count().ToString());
                newItem.Add("DeductibleValue", GetDeductibleValue(vehicleItems));
                Response.Add(newItem);
                }
            return Response;
            }

        [HttpGet]
        [Route("gain-loss")]
        public List<Dictionary<string, string>> GetLossGain(string DateFrom = null, String DateTo = null)
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection biometricItems = FetchDataFromList(BIOMETRIC, DateFrom, DateTo,"");
            ListItemCollection bsdItems = FetchDataFromList(BSD, DateFrom, DateTo, "");
            ListItemCollection cashItems = FetchDataFromList(CASH, DateFrom, DateTo, "");
            ListItemCollection cellsites = FetchDataFromList(CELL_SITE, DateFrom, DateTo, "");
            ListItemCollection handsetItems = FetchDataFromList(HANDSET, DateFrom, DateTo, "");
            ListItemCollection laptopItems = FetchDataFromList(LAPTOP, DateFrom, DateTo, "");
            ListItemCollection marineImportItems = FetchDataFromList(MARINE_IMPORT, DateFrom, DateTo, "");
            ListItemCollection marineInlandItems = FetchDataFromList(MARINE_INLAND, DateFrom, DateTo, "");
            ListItemCollection vehicleItems = FetchDataFromList(VEHICLE, DateFrom, DateTo, "");

            if (biometricItems != null)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                foreach (ListItem biometricItem in biometricItems)
                    {
                    if (biometricItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery += Convert.ToDouble(biometricItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (biometricItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount += Convert.ToDouble(biometricItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    }
                GainLoss = (Recovery - ClaimAmount).ToString();

                newItem.Add("ClaimCategory", BIOMETRIC_TYPE);
                 
                newItem.Add("ClaimCount", biometricItems.Count().ToString());
                newItem.Add("ClaimAmount", ClaimAmount.ToString());
                newItem.Add("PolicyDeductible", GetDeductibleValue(biometricItems,"FinanceDeductiblePolicy"));
                newItem.Add("InsuranceCoDeductible", GetDeductibleValue(biometricItems, "InsuranceCompanyDeductible"));
                newItem.Add("AmountReceive", Recovery.ToString());
                newItem.Add("LossGain", GainLoss);
                Response.Add(newItem);
                }
            if (bsdItems != null)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                foreach (ListItem bsdItem in bsdItems)
                    {
                    if (bsdItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery += Convert.ToDouble(bsdItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (bsdItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount += Convert.ToDouble(bsdItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    }

                GainLoss = (Recovery - ClaimAmount).ToString();
                //if (Recovery >= ClaimAmount)
                //    {
                //    GainLoss = "Gain";
                //    }
                //else
                //    {
                //    GainLoss = "Loss";
                //    }
                newItem.Add("ClaimCategory", BSD_TYPE);
                 
                newItem.Add("ClaimCount", bsdItems.Count().ToString());
                newItem.Add("ClaimAmount", ClaimAmount.ToString());
                newItem.Add("PolicyDeductible", GetDeductibleValue(bsdItems, "FinanceDeductiblePolicy"));
                newItem.Add("InsuranceCoDeductible", GetDeductibleValue(bsdItems, "InsuranceCompanyDeductible"));
                newItem.Add("AmountReceive", Recovery.ToString());
                newItem.Add("LossGain", GainLoss);
                Response.Add(newItem);
                }
            if (cashItems != null)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                foreach (ListItem cashItem in cashItems)
                    {
                    if (cashItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery += Convert.ToDouble(cashItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (cashItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount += Convert.ToDouble(cashItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    }
                GainLoss = (Recovery - ClaimAmount).ToString();
                newItem.Add("ClaimCategory", CASH_TYPE);
                 
                newItem.Add("ClaimCount", cashItems.Count().ToString());
                newItem.Add("ClaimAmount", ClaimAmount.ToString());
                newItem.Add("PolicyDeductible", GetDeductibleValue(cashItems, "FinanceDeductiblePolicy"));
                newItem.Add("InsuranceCoDeductible", GetDeductibleValue(cashItems, "InsuranceCompanyDeductible"));
                newItem.Add("AmountReceive", Recovery.ToString());
                newItem.Add("LossGain", GainLoss);
                Response.Add(newItem);
                }
            if (cellsites != null)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                foreach (ListItem cellsite in cellsites)
                    {
                    if (cellsite.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery += Convert.ToDouble(cellsite.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (cellsite.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount += Convert.ToDouble(cellsite.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    }
                GainLoss = (Recovery - ClaimAmount).ToString();
                newItem.Add("ClaimCategory", CELLSITE_TYPE);
                 
                newItem.Add("ClaimCount", cellsites.Count().ToString());
                newItem.Add("ClaimAmount", ClaimAmount.ToString());
                newItem.Add("PolicyDeductible", GetDeductibleValue(cellsites, "FinanceDeductiblePolicy"));
                newItem.Add("InsuranceCoDeductible", GetDeductibleValue(cellsites, "InsuranceCompanyDeductible"));
                newItem.Add("AmountReceive", Recovery.ToString());
                newItem.Add("LossGain", GainLoss);
                Response.Add(newItem);
                }
            if (handsetItems != null)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                foreach (ListItem handsetItem in handsetItems)
                    {
                    if (handsetItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery += Convert.ToDouble(handsetItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (handsetItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount += Convert.ToDouble(handsetItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    }
                GainLoss = (Recovery - ClaimAmount).ToString();
                newItem.Add("ClaimCategory", HANDSET_TYPE);
                 
                newItem.Add("ClaimCount", handsetItems.Count().ToString());
                newItem.Add("ClaimAmount", ClaimAmount.ToString());
                newItem.Add("PolicyDeductible", GetDeductibleValue(handsetItems, "FinanceDeductiblePolicy"));
                newItem.Add("InsuranceCoDeductible", GetDeductibleValue(handsetItems, "InsuranceCompanyDeductible"));
                newItem.Add("AmountReceive", Recovery.ToString());
                newItem.Add("LossGain", GainLoss);
                Response.Add(newItem);
                }
            if (laptopItems != null)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                foreach (ListItem laptopItem in laptopItems)
                    {
                    if (laptopItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery += Convert.ToDouble(laptopItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (laptopItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount += Convert.ToDouble(laptopItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    }
                GainLoss = (Recovery - ClaimAmount).ToString();
                newItem.Add("ClaimCategory", LAPTOP_TYPE);
                 
                newItem.Add("ClaimCount", laptopItems.Count().ToString());
                newItem.Add("ClaimAmount", ClaimAmount.ToString());
                newItem.Add("PolicyDeductible", GetDeductibleValue(laptopItems, "FinanceDeductiblePolicy"));
                newItem.Add("InsuranceCoDeductible", GetDeductibleValue(laptopItems, "InsuranceCompanyDeductible"));
                newItem.Add("AmountReceive", Recovery.ToString());
                newItem.Add("LossGain", GainLoss);
                Response.Add(newItem);
                }
            if (marineImportItems != null)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                foreach (ListItem marineImportItem in marineImportItems)
                    {
                    if (marineImportItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery += Convert.ToDouble(marineImportItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (marineImportItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount += Convert.ToDouble(marineImportItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    }
                GainLoss = (Recovery - ClaimAmount).ToString();
                newItem.Add("ClaimCategory", MARINEIMPORT_TYPE);
                 
                newItem.Add("ClaimCount", marineImportItems.Count().ToString());
                newItem.Add("ClaimAmount", ClaimAmount.ToString());
                newItem.Add("PolicyDeductible", GetDeductibleValue(marineImportItems, "FinanceDeductiblePolicy"));
                newItem.Add("InsuranceCoDeductible", GetDeductibleValue(marineImportItems, "InsuranceCompanyDeductible"));
                newItem.Add("AmountReceive", Recovery.ToString());
                newItem.Add("LossGain", GainLoss);
                Response.Add(newItem);
                }
            if (marineInlandItems != null)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                foreach (ListItem marineInlandItem in marineInlandItems)
                    {
                    if (marineInlandItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery += Convert.ToDouble(marineInlandItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (marineInlandItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount += Convert.ToDouble(marineInlandItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    }
                GainLoss = (Recovery - ClaimAmount).ToString();
                newItem.Add("ClaimCategory", MARINEINLAND_TYPE);
                 
                newItem.Add("ClaimCount", marineInlandItems.Count().ToString());
                newItem.Add("ClaimAmount", ClaimAmount.ToString());
                newItem.Add("PolicyDeductible", GetDeductibleValue(marineInlandItems, "FinanceDeductiblePolicy"));
                newItem.Add("InsuranceCoDeductible", GetDeductibleValue(marineInlandItems, "InsuranceCompanyDeductible"));
                newItem.Add("AmountReceive", Recovery.ToString());
                newItem.Add("LossGain", GainLoss);
                Response.Add(newItem);
                }
            if (vehicleItems != null)
                {
                double Recovery = 0, ClaimAmount = 0;
                string GainLoss = "";
                Dictionary<string, string> newItem = new Dictionary<string, string>();
                foreach (ListItem vehicleItem in vehicleItems)
                    {
                    if (vehicleItem.FieldValues["FinanceRecovery"] != null)
                        {
                        Recovery += Convert.ToDouble(vehicleItem.FieldValues["FinanceRecovery"].ToString());
                        }

                    if (vehicleItem.FieldValues["FinanceClaimAmount"] != null)
                        {
                        ClaimAmount += Convert.ToDouble(vehicleItem.FieldValues["FinanceClaimAmount"].ToString());
                        }
                    }
                GainLoss = (Recovery - ClaimAmount).ToString();
                newItem.Add("ClaimCategory", VEHICLE_TYPE);
                 
                newItem.Add("ClaimCount", vehicleItems.Count().ToString());
                newItem.Add("ClaimAmount", ClaimAmount.ToString());
                newItem.Add("PolicyDeductible", GetDeductibleValue(vehicleItems, "FinanceDeductiblePolicy"));
                newItem.Add("InsuranceCoDeductible", GetDeductibleValue(vehicleItems, "InsuranceCompanyDeductible"));
                newItem.Add("AmountReceive", Recovery.ToString());
                newItem.Add("LossGain", GainLoss);
                Response.Add(newItem);
                }
            return Response;
            }

        [HttpGet]
        [Route("summary")]
        public List<Dictionary<string, string>> GetSummary(string DateFrom = null, String DateTo = null, string Email = null)
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();
            ListItemCollection biometricItems = FetchMyRequestData(BIOMETRIC, DateFrom, DateTo);
            ListItemCollection bsdItems = FetchMyRequestData(BSD, DateFrom, DateTo);
            ListItemCollection cashItems = FetchMyRequestData(CASH, DateFrom, DateTo);
            ListItemCollection cellsites = FetchMyRequestData(CELL_SITE, DateFrom, DateTo);
            ListItemCollection handsetItems = FetchMyRequestData(HANDSET, DateFrom, DateTo);
            ListItemCollection laptopItems = FetchMyRequestData(LAPTOP, DateFrom, DateTo);
            ListItemCollection marineImportItems = FetchMyRequestData(MARINE_IMPORT, DateFrom, DateTo);
            ListItemCollection marineInlandItems = FetchMyRequestData(MARINE_INLAND, DateFrom, DateTo);
            ListItemCollection vehicleItems = FetchMyRequestData(VEHICLE, DateFrom, DateTo);

            int TotalBiometricCount = 0;
            int BioMetricRejectCount = 0;
            int BioMetricApproveCount = 0;

            int TotalBSDCount = 0;
            int BSDRejectCount = 0;
            int BSDApproveCount = 0;

            int TotalCashCount = 0;
            int CashRejectCount = 0;
            int CashApproveCount = 0;

            int TotalCellSiteCount = 0;
            int CellSiteRejectCount = 0;
            int CellSiteApproveCount = 0;

            int TotalHandsetCount = 0;
            int HandsetRejectCount = 0;
            int HandsetApproveCount = 0;

            int TotalLaptopCount = 0;
            int LaptopRejectCount = 0;
            int LaptopApproveCount = 0;

            int TotalMImportCount = 0;
            int MImportRejectCount = 0;
            int MImportApproveCount = 0;

            int TotalMInlandCount = 0;
            int MInlandRejectCount = 0;
            int MInlandApproveCount = 0;

            int TotalVehicleCount = 0;
            int VehicleRejectCount = 0;
            int VehicleApproveCount = 0;
            Dictionary<string, string> newItemBiometric = new Dictionary<string, string>();
            Dictionary<string, string> newItemBSD = new Dictionary<string, string>();
            Dictionary<string, string> newItemCash = new Dictionary<string, string>();
            Dictionary<string, string> newItemCellSite = new Dictionary<string, string>();
            Dictionary<string, string> newItemHandset = new Dictionary<string, string>();
            Dictionary<string, string> newItemLaptop = new Dictionary<string, string>();
            Dictionary<string, string> newItemMarineImport = new Dictionary<string, string>();
            Dictionary<string, string> newItemMarineInland = new Dictionary<string, string>();
            Dictionary<string, string> newItemVehicle = new Dictionary<string, string>();


            foreach (ListItem biometricItem in biometricItems)
                {
                TotalBiometricCount++;
                if (biometricItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (biometricItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        BioMetricRejectCount++;
                        }
                    }
                if (biometricItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        BioMetricRejectCount++;
                        }
                    }
                if (biometricItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (biometricItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        BioMetricApproveCount++;
                        }
                    }
                }
            foreach (ListItem bsdItem in bsdItems)
                {
                TotalBSDCount++;
                if (bsdItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (bsdItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        BSDRejectCount++;
                        }
                    }
                if (bsdItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        BSDRejectCount++;
                        }
                    }
                if (bsdItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (bsdItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        BSDApproveCount++;
                        }
                    }
                }
            foreach (ListItem cashItem in cashItems)
                {
                TotalCashCount++;
                if (cashItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (cashItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        CashRejectCount++;
                        }
                    }
                if (cashItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        CashRejectCount++;
                        }
                    }
                if (cashItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cashItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        CashApproveCount++;
                        }
                    }
                }
            foreach (ListItem cellsite in cellsites)
                {
                TotalCellSiteCount++;
                if (cellsite.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (cellsite.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        CellSiteRejectCount++;
                        }
                    }
                if (cellsite.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        CellSiteRejectCount++;
                        }
                    }
                if (cellsite.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (cellsite.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        CellSiteApproveCount++;
                        }
                    }
                }
            foreach (ListItem handsetItem in handsetItems)
                {
                TotalHandsetCount++;
                if (handsetItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (handsetItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        HandsetRejectCount++;
                        }
                    }
                if (handsetItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        HandsetRejectCount++;
                        }
                    }
                if (handsetItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (handsetItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        HandsetApproveCount++;
                        }
                    }
                }
            foreach (ListItem laptopItem in laptopItems)
                {
                TotalLaptopCount++;
                if (laptopItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (laptopItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        LaptopRejectCount++;
                        }
                    }
                if (laptopItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        LaptopRejectCount++;
                        }
                    }
                if (laptopItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (laptopItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        LaptopApproveCount++;
                        }
                    }
                }
            foreach (ListItem marineImportItem in marineImportItems)
                {
                TotalMImportCount++;
                if (marineImportItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (marineImportItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        MImportRejectCount++;
                        }
                    }
                if (marineImportItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        MImportRejectCount++;
                        }
                    }
                if (marineImportItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineImportItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        MImportApproveCount++;
                        }
                    }
                }
            foreach (ListItem marineInlandItem in marineInlandItems)
                {
                TotalMInlandCount++;
                if (marineInlandItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (marineInlandItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        MInlandRejectCount++;
                        }
                    }
                if (marineInlandItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        MInlandRejectCount++;
                        }
                    }
                if (marineInlandItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (marineInlandItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        MInlandApproveCount++;
                        }
                    }
                }
            foreach (ListItem vehicleItem in vehicleItems)
                {
                TotalVehicleCount++;
                if (vehicleItem.FieldValues["PocTaskOutcome"] != null)
                    {
                    if (vehicleItem.FieldValues["PocTaskOutcome"].ToString() == "Reject")
                        {
                        VehicleRejectCount++;
                        }
                    }
                if (vehicleItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Reject")
                        {
                        VehicleRejectCount++;
                        }
                    }
                if (vehicleItem.FieldValues["financeTaskOutcome"] != null)
                    {
                    if (vehicleItem.FieldValues["financeTaskOutcome"].ToString() == "Approve")
                        {
                        VehicleApproveCount++;
                        }
                    }
                }




            newItemBiometric.Add("Total", TotalBiometricCount.ToString());
            newItemBiometric.Add("InProcess", (TotalBiometricCount - BioMetricRejectCount - BioMetricApproveCount).ToString());
            newItemBiometric.Add("Rejected", BioMetricRejectCount.ToString());
            newItemBiometric.Add("Complete", BioMetricApproveCount.ToString());
            newItemBiometric.Add("ClaimType", "Biometric");
            Response.Add(newItemBiometric);

            newItemBSD.Add("Total", TotalBSDCount.ToString());
            newItemBSD.Add("InProcess", (TotalBSDCount - BSDRejectCount - BSDApproveCount).ToString());
            newItemBSD.Add("Rejected", BSDRejectCount.ToString());
            newItemBSD.Add("Complete", BSDApproveCount.ToString());
            newItemBSD.Add("ClaimType", "BSD");
            Response.Add(newItemBSD);

            newItemCash.Add("Total", TotalCashCount.ToString());
            newItemCash.Add("InProcess", (TotalCashCount - CashRejectCount - CashApproveCount).ToString());
            newItemCash.Add("Rejected", CashRejectCount.ToString());
            newItemCash.Add("Complete", CashApproveCount.ToString());
            newItemCash.Add("ClaimType", "Cash");
            Response.Add(newItemCash);

            newItemCellSite.Add("Total", TotalCellSiteCount.ToString());
            newItemCellSite.Add("InProcess", (TotalCellSiteCount - CellSiteRejectCount - CellSiteApproveCount).ToString());
            newItemCellSite.Add("Rejected", CellSiteRejectCount.ToString());
            newItemCellSite.Add("Complete", CellSiteApproveCount.ToString());
            newItemCellSite.Add("ClaimType", "CellSite");
            Response.Add(newItemCellSite);

            newItemHandset.Add("Total", TotalHandsetCount.ToString());
            newItemHandset.Add("InProcess", (TotalHandsetCount - HandsetRejectCount - HandsetApproveCount).ToString());
            newItemHandset.Add("Rejected", HandsetRejectCount.ToString());
            newItemHandset.Add("Complete", HandsetApproveCount.ToString());
            newItemHandset.Add("ClaimType", "Handset");
            Response.Add(newItemHandset);

            newItemLaptop.Add("Total", TotalLaptopCount.ToString());
            newItemLaptop.Add("InProcess", (TotalLaptopCount - LaptopRejectCount - LaptopApproveCount).ToString());
            newItemLaptop.Add("Rejected", LaptopRejectCount.ToString());
            newItemLaptop.Add("Complete", LaptopApproveCount.ToString());
            newItemLaptop.Add("ClaimType", "Laptop");
            Response.Add(newItemLaptop);

            newItemMarineImport.Add("Total", TotalMImportCount.ToString());
            newItemMarineImport.Add("InProcess", (TotalMImportCount - MImportRejectCount - MImportApproveCount).ToString());
            newItemMarineImport.Add("Rejected", MImportRejectCount.ToString());
            newItemMarineImport.Add("Complete", MImportApproveCount.ToString());
            newItemMarineImport.Add("ClaimType", "Marine Import");
            Response.Add(newItemMarineImport);

            newItemMarineInland.Add("Total", TotalMInlandCount.ToString());
            newItemMarineInland.Add("InProcess", (TotalMInlandCount - MInlandRejectCount - MInlandApproveCount).ToString());
            newItemMarineInland.Add("Rejected", MInlandRejectCount.ToString());
            newItemMarineInland.Add("Complete", MInlandApproveCount.ToString());
            newItemMarineInland.Add("ClaimType", "Marine Inland");
            Response.Add(newItemMarineInland);

            newItemVehicle.Add("Total", TotalVehicleCount.ToString());
            newItemVehicle.Add("InProcess", (TotalVehicleCount - VehicleRejectCount - VehicleApproveCount).ToString());
            newItemVehicle.Add("Rejected", VehicleRejectCount.ToString());
            newItemVehicle.Add("Complete", VehicleApproveCount.ToString());
            newItemVehicle.Add("ClaimType", "Vehicle");
            Response.Add(newItemVehicle);
            return Response;
            }

        protected ListItemCollection FetchDateFromArchive(string listTitle, string claimID)
            {
            ClientContext clientContext = GetSharePointAuth();
            List oList = clientContext.Web.Lists.GetById(new Guid(WF_ARCHIVE));

            var camlQuery = new CamlQuery
                {
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query>
                        <Where>
                            <And>
                                <Eq><FieldRef Name='Title'/><Value Type='Text'>" + listTitle + @"</Value></Eq>
                                <Eq><FieldRef Name='ClaimID'/><Value Type='Text'>" + claimID + @"</Value></Eq>
                                <Eq><FieldRef Name='ClaimState'/><Value Type='Text'>Finance</Value></Eq>
                            </And>
                        </Where>
                        <OrderBy>
                            <FieldRef Name='Created' Ascending='FALSE' />
                        </OrderBy>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='Created'/>
                    </ViewFields>
                </View>"
                };

            ListItemCollection collListItem = oList.GetItems(camlQuery);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();
            return collListItem;
            }

        public async Task SendEmailAsync(string [] EmailArray,string Level,string ClaimTitle, string ReferenceNumber, string Link,string HODConcent="")
            {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(Mail);
            var HODBody = HODConcent;
            foreach (string ToEMailId in EmailArray)
                {
                email.To.Add(MailboxAddress.Parse(ToEMailId)); //adding multiple TO Email Id  
                }
            if (Level.Contains("Pending Approval"))
                {
                email.Subject = "Telco | Reminder | " + Level;
                //HODBody = "Your HOD has been informed as well.";
                }
            else
                {
                email.Subject = "Telco | Reminder | Response Required for " + Level;
                }
            //email.Subject = "Telco | Reminder | Response Required for " + Level;
            var builder = new BodyBuilder();
            builder.HtmlBody = "Dear Concerned, <br> <br> Your response is required for the " + ClaimTitle + " Initiated. " + HODBody + "<br> <br> <strong> Reference #: </strong>" + ReferenceNumber + " <br> <br> <strong> Please click the link: </strong>" + Link + " <br> <br> Thankyou.<br> <br> Regards,<br> Telco Team";
            email.Body = builder.ToMessageBody();
            var smtp = new SmtpClient();
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
            smtp.Connect(Host, Int16.Parse(Port), SecureSocketOptions.StartTls);
            //smtp.Authenticate(Mail, Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            }
        public async Task SendMail(string[] EmailArray, string Level, string ClaimTitle, string ReferenceNumber, string Link, string HODConcent = "")
            {
            try
                {
                await SendEmailAsync(EmailArray,Level,ClaimTitle,ReferenceNumber,Link,HODConcent);
                //return Ok();
                }
            catch (Exception ex)
                {
                throw;
                }
            }

        [HttpGet]
        [Route("sendReminders")]
        public List<Dictionary<string, string>> SendReminders()
            {
            List<Dictionary<string, string>> Response = new List<Dictionary<string, string>>();

            ListItemCollection biometricItems = null;
            ListItemCollection bsdItems = null;
            ListItemCollection cashItems = null;
            ListItemCollection cellsites = null;
            ListItemCollection handsetItems = null;
            ListItemCollection laptopItems = null;
            ListItemCollection marineImportItems = null;
            ListItemCollection marineInlandItems = null;
            ListItemCollection vehicleItems = null;
            string[] POCUserArray = new string [] {};
            string[] POCHODArray = new string[] {};
            string[] InsUserArray = new string[] { };
            string[] InsHODArray = new string[] { };
            string[] FixedAssetUserArray = new string[] { };
            string[] FixedAssetHODArray = new string[] { };
            string[] VendorUserArray = new string[] {};
            string[] VendorHODArray = new string[] {};
            string[] FinanceUserArray = new string[] {};
            string[] FinanceHODArray = new string[] {};

            biometricItems = FetchRemindersData(BIOMETRIC);
            bsdItems = FetchRemindersData(BSD);            
            cashItems = FetchRemindersData(CASH);            
            cellsites = FetchRemindersData(CELL_SITE);               
            handsetItems = FetchRemindersData(HANDSET);
            laptopItems = FetchRemindersData(LAPTOP);            
            marineImportItems = FetchRemindersData(MARINE_IMPORT);            
            marineInlandItems = FetchRemindersData(MARINE_INLAND);            
            vehicleItems = FetchRemindersData(VEHICLE);

            //biometricItems = null;
            //bsdItems = null;
            //cashItems = null;
            //cellsites = null;
            //handsetItems = null;
            //laptopItems = null;
            //marineImportItems = null;
            //marineInlandItems = null;
            //vehicleItems = null;

            if (biometricItems != null)
                {
                POCUserArray = new string[] { };
                POCHODArray = new string[] { };
                VendorUserArray = new string[] { };
                FinanceUserArray = new string[] { };
                FinanceHODArray = new string[] { };

                List<Dictionary<string, string>> POCUserList = GetSharePointApproversList("0", "Biometric Devices", "POC");
                foreach (var item in POCUserList)
                    {
                    var tempList = POCUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> POCHODList = GetSharePointApproversList("1", "Biometric Devices", "POC");
                foreach (var item in POCHODList)
                    {
                    var tempList = POCHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCHODArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> VendorUserList = GetSharePointVendorsList("Biometric Devices");
                foreach (var item in VendorUserList)
                    {
                    var tempList = VendorUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    VendorUserArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> FinanceUserList = GetSharePointApproversList("0", "Biometric Devices", "Finance");
                foreach (var item in FinanceUserList)
                    {
                    var tempList = FinanceUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> FinanceHODList = GetSharePointApproversList("1", "Biometric Devices", "Finance");
                foreach (var item in FinanceHODList)
                    {
                    var tempList = FinanceHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceHODArray = tempList.ToArray();
                    }
                ///
                ///
                foreach (ListItem biometricItem in biometricItems)
                    {
                    if (biometricItem.FieldValues["PocDescription"] == null && biometricItem.FieldValues["IsDraft"]?.ToString() == "False")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        DateTime CreatedDate = Convert.ToDateTime(biometricItem.FieldValues["TestingCreatedDate"]).ToLocalTime();
                        decimal differvalue = (decimal)(CurrentDate - CreatedDate).TotalDays;
                        decimal TotalDays = Math.Floor(differvalue);
                        if (TotalDays == 15)
                            {
                            //var tempList = POCHODArray.ToList();
                            //foreach (var item in POCUserArray)
                            //    {
                            //    tempList.Add(item);
                            //    }
                            //POCHODArray = tempList.ToArray();
                            var EmailsSetHod = new HashSet<string>(POCHODArray);
                            var EmailsSetUser = new HashSet<string>(POCUserArray);
                            POCHODArray = EmailsSetHod.ToArray();
                            POCUserArray = EmailsSetUser.ToArray();
                            SendMail(POCUserArray, "Biometric Devices Claim on POC", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/pocfeedback/" + biometricItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                            SendMail(POCHODArray, "Pending Approval for Biometric Devices Claim on POC", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/financefeedback/" + biometricItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                            }
                        else if (TotalDays == 10)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Biometric Devices Claim on POC", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/pocfeedback/" + biometricItem.FieldValues["ItemID"].ToString());
                            }
                        else if (TotalDays == 5)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Biometric Devices Claim on POC", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/pocfeedback/" + biometricItem.FieldValues["ItemID"].ToString());
                            }
                        }
                    else if (biometricItem.FieldValues["ifHodTaskUpdate"]?.ToString() == "false" && (biometricItem.FieldValues["Exceptional"])?.ToString() == "true"
                        && (biometricItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, biometricItem.FieldValues["ItemID"].ToString(), "Biometric Devices");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }

                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Biometric Devices Claim on HOD Approval", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/hodfeedback/" + biometricItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Biometric Devices Claim on HOD Approval", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/hodfeedback/" + biometricItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Biometric Devices Claim on HOD Approval", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/hodfeedback/" + biometricItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }

                    else if (biometricItem.FieldValues["ifVendorTaskUpdate"]?.ToString() == "False" && (biometricItem.FieldValues["PocTaskOutcome"]).ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, biometricItem.FieldValues["ItemID"].ToString(), "Biometric Devices");
                        if (ArchiveItems.Count >= 1)
                        { 
                        DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                        decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                        decimal TotalDays = Math.Floor(differvalue);
                        if (TotalDays == 15)
                            {
                                
                                //tempList.Add("telcoRep.j@telco.com");
                                //VendorUserArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(VendorUserArray);
                                //VendorUserArray = EmailsSet.ToArray();


                                var EmailsSetUser = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSetUser.ToArray();
                                string[] VendorHOD = new string[] { "telcoRep.j@telco.com" };
                                SendMail(VendorHOD, "Pending Approval for Biometric Devices Claim on Vendor", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/financefeedback/" + biometricItem.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                                SendMail(VendorUserArray, "Biometric Devices Claim on Vendor", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), VendorURL + "vendor/biometric/" + biometricItem.FieldValues["ItemID"].ToString());
                            }
                        else if (TotalDays == 10)
                            {

                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Biometric Devices Claim on Vendor", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), VendorURL + "vendor/biometric/" + biometricItem.FieldValues["ItemID"].ToString());
                            }
                        else if (TotalDays == 5)
                            {

                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Biometric Devices Claim on Vendor", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), VendorURL + "vendor/biometric/" + biometricItem.FieldValues["ItemID"].ToString());
                            }
                        }
                        }
                    else if (biometricItem.FieldValues["financeTaskOutcome"] == null && biometricItem.FieldValues["vendorTaskOutcome"]?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchFinanceArchivedDate(WF_ARCHIVE, biometricItem.FieldValues["ItemID"].ToString(), "Biometric Devices");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = FinanceHODArray.ToList();
                                //foreach (var item in FinanceUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //FinanceHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(FinanceHODArray);
                                //FinanceHODArray = EmailsSet.ToArray();
                                var EmailsSetHod = new HashSet<string>(FinanceHODArray);
                                var EmailsSetUser = new HashSet<string>(FinanceUserArray);
                                FinanceHODArray = EmailsSetHod.ToArray();
                                FinanceUserArray = EmailsSetUser.ToArray();
                                SendMail(FinanceUserArray, "Biometric Devices Claim on Finance", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/financefeedback/" + biometricItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                SendMail(FinanceHODArray, "Pending Approval for Biometric Devices Claim on Finance", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/financefeedback/" + biometricItem.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                                }
                            else if (TotalDays == 10)
                                {

                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Biometric Devices Claim on Finance", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/financefeedback/" + biometricItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {

                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Biometric Devices Claim on Finance", "Biometric Devices Claim", biometricItem.FieldValues["Title"].ToString(), FinHubURL + "biometric/financefeedback/" + biometricItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    
                    }
                }
            if (bsdItems != null)
                {
                POCUserArray = new string[] { };
                POCHODArray = new string[] { };
                VendorUserArray = new string[] { };
                FinanceUserArray = new string[] { };
                FinanceHODArray = new string[] { };

                List<Dictionary<string, string>> POCUserList = GetSharePointApproversList("0", "Highvalue Tools", "POC");
                foreach (var item in POCUserList)
                    {
                    var tempList = POCUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> POCHODList = GetSharePointApproversList("1", "Highvalue Tools", "POC");
                foreach (var item in POCHODList)
                    {
                    var tempList = POCHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCHODArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> VendorUserList = GetSharePointVendorsList("Highvalue Tools");
                foreach (var item in VendorUserList)
                    {
                    var tempList = VendorUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    VendorUserArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> FinanceUserList = GetSharePointApproversList("0", "Highvalue Tools", "Finance");
                foreach (var item in FinanceUserList)
                    {
                    var tempList = FinanceUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> FinanceHODList = GetSharePointApproversList("1", "Highvalue Tools", "Finance");
                foreach (var item in FinanceHODList)
                    {
                    var tempList = FinanceHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceHODArray = tempList.ToArray();
                    }
                ///
                ///
                foreach (ListItem bsdItem in bsdItems)
                    {
                     if (bsdItem.FieldValues["PocDescription"] == null && bsdItem.FieldValues["IsDraft"]?.ToString() == "False")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        DateTime CreatedDate = Convert.ToDateTime(bsdItem.FieldValues["TestingCreatedDate"]).ToLocalTime();
                        decimal differvalue = (decimal)(CurrentDate - CreatedDate).TotalDays;
                        decimal TotalDays = Math.Floor(differvalue);
                        if (TotalDays == 15)
                            {
                            //var tempList = POCHODArray.ToList();
                            //foreach (var item in POCUserArray)
                            //    {
                            //    tempList.Add(item);
                            //    }
                            //POCHODArray = tempList.ToArray();
                            //var EmailsSet = new HashSet<string>(POCHODArray);
                            //POCHODArray = EmailsSet.ToArray();
                            var EmailsSetHod = new HashSet<string>(POCHODArray);
                            var EmailsSetUser = new HashSet<string>(POCUserArray);
                            POCHODArray = EmailsSetHod.ToArray();
                            POCUserArray = EmailsSetUser.ToArray();
                            SendMail(POCUserArray, "High Value Tools Claim on POC", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/pocfeedback/" + bsdItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                            SendMail(POCHODArray, "Pending Approval for High Value Tools Claim on POC", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/financefeedback/" + bsdItem.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                            }
                        else if (TotalDays == 10)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "High Value Tools Claim on POC", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/pocfeedback/" + bsdItem.FieldValues["ItemID"].ToString());
                            }
                        else if (TotalDays == 5)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "High Value Tools Claim on POC", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/pocfeedback/" + bsdItem.FieldValues["ItemID"].ToString());
                            }
                        }
                    else if (bsdItem.FieldValues["ifHodTaskUpdate"]?.ToString() == "false" && (bsdItem.FieldValues["Exceptional"])?.ToString() == "true"
                  && (bsdItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, bsdItem.FieldValues["ItemID"].ToString(), "Highvalue Tools");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }

                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Highvalue Tools Claim on HOD Approval", "Highvalue Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/hodfeedback/" + bsdItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Highvalue Tools Claim on HOD Approval", "Highvalue Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/hodfeedback/" + bsdItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Highvalue Tools Claim on HOD Approval", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/hodfeedback/" + bsdItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    else if (bsdItem.FieldValues["ifVendorTaskUpdate"]?.ToString() == "False" && (bsdItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, bsdItem.FieldValues["ItemID"].ToString(), "Highvalue Tools");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                
                                //tempList.Add("telcoRep.j@telco.com");
                                //VendorUserArray = tempList.ToArray();

                                //var EmailsSet = new HashSet<string>(VendorUserArray);
                                //VendorUserArray = EmailsSet.ToArray();
                                var EmailsSetUser = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSetUser.ToArray();
                                string[] VendorHOD = new string[] { "telcoRep.j@telco.com" };
                                SendMail(VendorHOD, "Pending Approval for High Value Tools Claim on Vendor", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/financefeedback/" + bsdItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(VendorUserArray, "High Value Tools Claim on Vendor", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), VendorURL + "vendor/bsd/" + bsdItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "High Value Tools Claim on Vendor", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), VendorURL + "vendor/bsd/" + bsdItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "High Value Tools Claim on Vendor", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), VendorURL + "vendor/bsd/" + bsdItem.FieldValues["ItemID"].ToString());
                                }
                            }


                        }
                    else if (bsdItem.FieldValues["financeTaskOutcome"] == null && bsdItem.FieldValues["vendorTaskOutcome"]?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchFinanceArchivedDate(WF_ARCHIVE, bsdItem.FieldValues["ItemID"].ToString(), "Highvalue Tools");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = FinanceHODArray.ToList();
                                //foreach (var item in FinanceUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //FinanceHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(FinanceHODArray);
                                //FinanceHODArray = EmailsSet.ToArray();
                                var EmailsSetHod = new HashSet<string>(FinanceHODArray);
                                var EmailsSetUser = new HashSet<string>(FinanceUserArray);
                                FinanceHODArray = EmailsSetHod.ToArray();
                                FinanceUserArray = EmailsSetUser.ToArray();
                                SendMail(FinanceHODArray, "Pending Approval for High Value Tools Claim on Finance", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/financefeedback/" + bsdItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(FinanceUserArray, "High Value Tools Claim on Finance", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/financefeedback/" + bsdItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "High Value Tools Claim on Finance", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/financefeedback/" + bsdItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "High Value Tools Claim on Finance", "High Value Tools Claim", bsdItem.FieldValues["Title"].ToString(), FinHubURL + "bsd/financefeedback/" + bsdItem.FieldValues["ItemID"].ToString());
                                }
                            }

                        }
                    }
                }
            if (handsetItems != null)
                {
                POCUserArray = new string[] { };
                POCHODArray = new string[] { };
                VendorUserArray = new string[] { };
                FinanceUserArray = new string[] { };
                FinanceHODArray = new string[] { };

                List<Dictionary<string, string>> POCUserList = GetSharePointApproversList("0", "Handsets", "POC");
                foreach (var item in POCUserList)
                    {
                    var tempList = POCUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> POCHODList = GetSharePointApproversList("1", "Handsets", "POC");
                foreach (var item in POCHODList)
                    {
                    var tempList = POCHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCHODArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> VendorUserList = GetSharePointVendorsList("Handsets");
                foreach (var item in VendorUserList)
                    {
                    var tempList = VendorUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    VendorUserArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> FinanceUserList = GetSharePointApproversList("0", "Handsets", "Finance");
                foreach (var item in FinanceUserList)
                    {
                    var tempList = FinanceUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> FinanceHODList = GetSharePointApproversList("1", "Handsets", "Finance");
                foreach (var item in FinanceHODList)
                    {
                    var tempList = FinanceHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceHODArray = tempList.ToArray();
                    }
                ///
                ///
                foreach (ListItem handsetItem in handsetItems)
                    {
                    if (handsetItem.FieldValues["PocDescription"] == null && handsetItem.FieldValues["IsDraft"]?.ToString() == "False")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        DateTime CreatedDate = Convert.ToDateTime(handsetItem.FieldValues["TestingCreatedDate"]).ToLocalTime();
                        decimal differvalue = (decimal)(CurrentDate - CreatedDate).TotalDays;
                        decimal TotalDays = Math.Floor(differvalue);
                        if (TotalDays == 15)
                            {
                            //var tempList = POCHODArray.ToList();
                            //foreach (var item in POCUserArray)
                            //    {
                            //    tempList.Add(item);
                            //    }
                            //POCHODArray = tempList.ToArray();
                            //var EmailsSet = new HashSet<string>(POCHODArray);
                            //POCHODArray = EmailsSet.ToArray();
                            var EmailsSetHod = new HashSet<string>(POCHODArray);
                            var EmailsSetUser = new HashSet<string>(POCUserArray);
                            POCHODArray = EmailsSetHod.ToArray();
                            POCUserArray = EmailsSetUser.ToArray();
                            SendMail(POCUserArray, "Handset Claim on POC", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/pocfeedback/" + handsetItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                            SendMail(POCHODArray, "Pending Approval for Handset Claim on POC", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/financefeedback/" + handsetItem.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                            }
                        else if (TotalDays == 10)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Handset Claim on POC", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/pocfeedback/" + handsetItem.FieldValues["ItemID"].ToString());
                            }
                        else if (TotalDays == 5)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Handset Claim on POC", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/pocfeedback/" + handsetItem.FieldValues["ItemID"].ToString());
                            }
                        }
                    else if (handsetItem.FieldValues["ifHodTaskUpdate"]?.ToString() == "false" && (handsetItem.FieldValues["Exceptional"])?.ToString() == "true"
              && (handsetItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, handsetItem.FieldValues["ItemID"].ToString(), "Handsets");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }

                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Handset Claim on HOD Approval", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/hodfeedback/" + handsetItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Handset Claim on HOD Approval", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/hodfeedback/" + handsetItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Handset Claim on HOD Approval", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/hodfeedback/" + handsetItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    else if (handsetItem.FieldValues["ifVendorTaskUpdate"]?.ToString() == "False" && (handsetItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, handsetItem.FieldValues["ItemID"].ToString(), "Handsets");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                
                                //tempList.Add("telcoRep.j@telco.com");
                                //VendorUserArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(VendorUserArray);
                                //VendorUserArray = EmailsSet.ToArray();
                                var EmailsSetUser = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSetUser.ToArray();
                                string[] VendorHOD = new string[] { "telcoRep.j@telco.com" };
                                SendMail(VendorHOD, "Pending Approval for Handset Claim on Vendor", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/financefeedback/" + handsetItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(VendorUserArray, "Handset Claim on Vendor", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), VendorURL + "vendor/handset" + handsetItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Handset Claim on Vendor", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), VendorURL + "vendor/handset" + handsetItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Handset Claim on Vendor", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), VendorURL + "vendor/handset" + handsetItem.FieldValues["ItemID"].ToString());
                                }
                            }

                        }
                    else if (handsetItem.FieldValues["financeTaskOutcome"] == null && handsetItem.FieldValues["vendorTaskOutcome"]?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchFinanceArchivedDate(WF_ARCHIVE, handsetItem.FieldValues["ItemID"].ToString(), "Handsets");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = FinanceHODArray.ToList();
                                //foreach (var item in FinanceUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //FinanceHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(FinanceHODArray);
                                //FinanceHODArray = EmailsSet.ToArray();
                                var EmailsSetHod = new HashSet<string>(FinanceHODArray);
                                var EmailsSetUser = new HashSet<string>(FinanceUserArray);
                                FinanceHODArray = EmailsSetHod.ToArray();
                                FinanceUserArray = EmailsSetUser.ToArray();
                                SendMail(FinanceHODArray, "Pending Approval for Handset Claim on Finance", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/financefeedback/" + handsetItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(FinanceUserArray, "Handset Claim on Finance", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/financefeedback/" + handsetItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Handset Claim on Finance", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/financefeedback/" + handsetItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Handset Claim on Finance", "Handset Claim", handsetItem.FieldValues["Title"].ToString(), FinHubURL + "handset/financefeedback/" + handsetItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    }
                }
            if (laptopItems != null)
                {
                POCUserArray = new string[] { };
                POCHODArray = new string[] { };
                VendorUserArray = new string[] { };
                FinanceUserArray = new string[] { };
                FinanceHODArray = new string[] { };
                List<Dictionary<string, string>> POCUserList = GetSharePointApproversList("0", "Laptop", "POC");
                foreach (var item in POCUserList)
                    {
                    var tempList = POCUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> POCHODList = GetSharePointApproversList("1", "Laptop", "POC");
                foreach (var item in POCHODList)
                    {
                    var tempList = POCHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCHODArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> VendorUserList = GetSharePointVendorsList("Laptop");
                foreach (var item in VendorUserList)
                    {
                    var tempList = VendorUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    VendorUserArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> FinanceUserList = GetSharePointApproversList("0", "Laptop", "Finance");
                foreach (var item in FinanceUserList)
                    {
                    var tempList = FinanceUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> FinanceHODList = GetSharePointApproversList("1", "Laptop", "Finance");
                foreach (var item in FinanceHODList)
                    {
                    var tempList = FinanceHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceHODArray = tempList.ToArray();
                    }
                ///
                ///
                foreach (ListItem laptopItem in laptopItems)
                    {
                    if (laptopItem.FieldValues["PocDescription"] == null && laptopItem.FieldValues["IsDraft"]?.ToString() == "False")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        DateTime CreatedDate = Convert.ToDateTime(laptopItem.FieldValues["TestingCreatedDate"]).ToLocalTime();
                        decimal differvalue = (decimal)(CurrentDate - CreatedDate).TotalDays;
                        decimal TotalDays = Math.Floor(differvalue);
                        if (TotalDays == 15)
                            {
                            //var tempList = POCHODArray.ToList();
                            //foreach (var item in POCUserArray)
                            //    {
                            //    tempList.Add(item);
                            //    }
                            //POCHODArray = tempList.ToArray();
                            //var EmailsSet = new HashSet<string>(POCHODArray);
                            //POCHODArray = EmailsSet.ToArray();
                            var EmailsSetHod = new HashSet<string>(POCHODArray);
                            var EmailsSetUser = new HashSet<string>(POCUserArray);
                            POCHODArray = EmailsSetHod.ToArray();
                            POCUserArray = EmailsSetUser.ToArray();
                            SendMail(POCUserArray, "Laptop Claim on POC", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/pocfeedback/" + laptopItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                            SendMail(POCHODArray, "Pending Approval for Laptop Claim on POC", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/financefeedback/" + laptopItem.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                            }
                        else if (TotalDays == 10)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Laptop Claim on POC", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/pocfeedback/" + laptopItem.FieldValues["ItemID"].ToString());
                            }
                        else if (TotalDays == 5)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Laptop Claim on POC", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/pocfeedback/" + laptopItem.FieldValues["ItemID"].ToString());
                            }
                        }
                    else if (laptopItem.FieldValues["ifHodTaskUpdate"]?.ToString() == "False" && (laptopItem.FieldValues["Exceptional"])?.ToString() == "True"
                        && (laptopItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, laptopItem.FieldValues["ItemID"].ToString(), "Laptop");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }

                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Laptop Claim on HOD Approval", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/hodfeedback/" + laptopItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Laptop Claim on HOD Approval", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/hodfeedback/" + laptopItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Laptop Claim on HOD Approval", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/hodfeedback/" + laptopItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }

                    else if (laptopItem.FieldValues["ifVendorTaskUpdate"]?.ToString() == "False" && (laptopItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, laptopItem.FieldValues["ItemID"].ToString(), "Laptop");
                        if(ArchiveItems.Count>=1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                
                                //tempList.Add("telcoRep.j@telco.com");
                                //VendorUserArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(VendorUserArray);
                                //VendorUserArray = EmailsSet.ToArray();
                                var EmailsSetUser = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSetUser.ToArray();
                                string[] VendorHOD = new string[] { "telcoRep.j@telco.com" };
                                SendMail(VendorHOD, "Pending Approval for Laptop Claim on Vendor", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/financefeedback/" + laptopItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(VendorUserArray, "Laptop Claim on Vendor", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), VendorURL + "vendor/laptop/" + laptopItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Laptop Claim on Vendor", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), VendorURL + "vendor/laptop/" + laptopItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Laptop Claim on Vendor", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), VendorURL + "vendor/laptop/" + laptopItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        


                        }
                    else if (laptopItem.FieldValues["financeTaskOutcome"] == null && laptopItem.FieldValues["vendorTaskOutcome"]?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchFinanceArchivedDate(WF_ARCHIVE, laptopItem.FieldValues["ItemID"].ToString(), "Laptop");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);

                            if (TotalDays == 15)
                                {
                                //var tempList = FinanceHODArray.ToList();
                                //foreach (var item in FinanceUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //FinanceHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(FinanceHODArray);
                                //FinanceHODArray = EmailsSet.ToArray();
                                var EmailsSetHod = new HashSet<string>(FinanceHODArray);
                                var EmailsSetUser = new HashSet<string>(FinanceUserArray);
                                FinanceHODArray = EmailsSetHod.ToArray();
                                FinanceUserArray = EmailsSetUser.ToArray();
                                SendMail(FinanceHODArray, "Pending Approval for Laptop Claim on Finance", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/financefeedback/" + laptopItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(FinanceUserArray, "Laptop Claim on Finance", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/financefeedback/" + laptopItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Laptop Claim on Finance", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/financefeedback/" + laptopItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Laptop Claim on Finance", "Laptop Claim", laptopItem.FieldValues["Title"].ToString(), FinHubURL + "laptop/financefeedback/" + laptopItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    }
                }
            if (marineImportItems != null)
                {
                POCUserArray = new string[] { };
                POCHODArray = new string[] { };
                VendorUserArray = new string[] { };
                FinanceUserArray = new string[] { };
                FinanceHODArray = new string[] { };
                List<Dictionary<string, string>> POCUserList = GetSharePointApproversList("0", "Marine Imports", "POC");
                foreach (var item in POCUserList)
                    {
                    var tempList = POCUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> POCHODList = GetSharePointApproversList("1", "Marine Imports", "POC");
                foreach (var item in POCHODList)
                    {
                    var tempList = POCHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCHODArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> VendorUserList = GetSharePointVendorsList("Marine Imports");
                foreach (var item in VendorUserList)
                    {
                    var tempList = VendorUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    VendorUserArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> FinanceUserList = GetSharePointApproversList("0", "Marine Imports", "Finance");
                foreach (var item in FinanceUserList)
                    {
                    var tempList = FinanceUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> FinanceHODList = GetSharePointApproversList("1", "Marine Imports", "Finance");
                foreach (var item in FinanceHODList)
                    {
                    var tempList = FinanceHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceHODArray = tempList.ToArray();
                    }
                foreach (ListItem marineImportItem in marineImportItems)
                    {
                    if (marineImportItem.FieldValues["PocDescription"] == null && marineImportItem.FieldValues["IsDraft"]?.ToString() == "False")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        DateTime CreatedDate = Convert.ToDateTime(marineImportItem.FieldValues["TestingCreatedDate"]).ToLocalTime();
                        decimal differvalue = (decimal)(CurrentDate - CreatedDate).TotalDays;
                        decimal TotalDays = Math.Floor(differvalue);
                        if (TotalDays == 15)
                            {
                            //var tempList = POCHODArray.ToList();
                            //foreach (var item in POCUserArray)
                            //    {
                            //    tempList.Add(item);
                            //    }
                            //POCHODArray = tempList.ToArray();
                            //var EmailsSet = new HashSet<string>(POCHODArray);
                            //POCHODArray = EmailsSet.ToArray();
                            var EmailsSetHod = new HashSet<string>(POCHODArray);
                            var EmailsSetUser = new HashSet<string>(POCUserArray);
                            POCHODArray = EmailsSetHod.ToArray();
                            POCUserArray = EmailsSetUser.ToArray();
                            SendMail(POCUserArray, "Marine Import Claim on POC", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/pocfeedback/" + marineImportItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                            SendMail(POCHODArray, "Pending Approval for Marine Import Claim on POC", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/financefeedback/" + marineImportItem.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                            }
                        else if (TotalDays == 10)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Marine Import Claim on POC", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/pocfeedback/" + marineImportItem.FieldValues["ItemID"].ToString());
                            }
                        else if (TotalDays == 5)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Marine Import Claim on POC", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/pocfeedback/" + marineImportItem.FieldValues["ItemID"].ToString());
                            }

                        }
                    else if (marineImportItem.FieldValues["ifHodTaskUpdate"]?.ToString() == "false" && (marineImportItem.FieldValues["Exceptional"])?.ToString() == "true"
                        && (marineImportItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, marineImportItem.FieldValues["ItemID"].ToString(), "Marine Imports");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }

                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Marine Import Claim on HOD Approval", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/hodfeedback/" + marineImportItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Marine Import Claim on HOD Approval", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/hodfeedback/" + marineImportItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Marine Import Claim on HOD Approval", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/hodfeedback/" + marineImportItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    else if (marineImportItem.FieldValues["ifVendorTaskUpdate"]?.ToString() == "False" && (marineImportItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, marineImportItem.FieldValues["ItemID"].ToString(), "Marine Imports");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                
                                //tempList.Add("telcoRep.j@telco.com");
                                //VendorUserArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(VendorUserArray);
                                //VendorUserArray = EmailsSet.ToArray();
                                var EmailsSetUser = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSetUser.ToArray();
                                string[] VendorHOD = new string[] { "telcoRep.j@telco.com" };
                                SendMail(VendorHOD, "Pending Approval for Marine Import Claim on Vendor", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/financefeedback/" + marineImportItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(VendorUserArray, "Marine Import Claim on Vendor", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), VendorURL + "vendor/marineimport/" + marineImportItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Marine Import Claim on Vendor", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), VendorURL + "vendor/marineimport/" + marineImportItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Marine Import Claim on Vendor", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), VendorURL + "vendor/marineimport/" + marineImportItem.FieldValues["ItemID"].ToString());
                                }

                            }
                        }
                    else if (marineImportItem.FieldValues["financeTaskOutcome"] == null && marineImportItem.FieldValues["vendorTaskOutcome"]?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchFinanceArchivedDate(WF_ARCHIVE, marineImportItem.FieldValues["ItemID"].ToString(), "Marine Imports");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = FinanceHODArray.ToList();
                                //foreach (var item in FinanceUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //FinanceHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(FinanceHODArray);
                                //FinanceHODArray = EmailsSet.ToArray();
                                var EmailsSetHod = new HashSet<string>(FinanceHODArray);
                                var EmailsSetUser = new HashSet<string>(FinanceUserArray);
                                FinanceHODArray = EmailsSetHod.ToArray();
                                FinanceUserArray = EmailsSetUser.ToArray();
                                SendMail(FinanceHODArray, "Pending Approval for Marine Import Claim on Finance", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/financefeedback/" + marineImportItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(FinanceUserArray, "Marine Import Claim on Finance", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/financefeedback/" + marineImportItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Marine Import Claim on Finance", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/financefeedback/" + marineImportItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Marine Import Claim on Finance", "Marine Import Claim", marineImportItem.FieldValues["Title"].ToString(), FinHubURL + "marineimport/financefeedback/" + marineImportItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    }
                }
            if (vehicleItems != null)
                {
                POCUserArray = new string[] { };
                POCHODArray = new string[] { };
                VendorUserArray = new string[] { };
                FinanceUserArray = new string[] { };
                FinanceHODArray = new string[] { };
                List<Dictionary<string, string>> POCUserList = GetSharePointApproversList("0", "Motor Vehicles", "POC");
                foreach (var item in POCUserList)
                    {
                    var tempList = POCUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> POCHODList = GetSharePointApproversList("1", "Motor Vehicles", "POC");
                foreach (var item in POCHODList)
                    {
                    var tempList = POCHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCHODArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> VendorUserList = GetSharePointVendorsList("Motor Vehicles");
                foreach (var item in VendorUserList)
                    {
                    var tempList = VendorUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    VendorUserArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> FinanceUserList = GetSharePointApproversList("0", "Motor Vehicles", "Finance");
                foreach (var item in FinanceUserList)
                    {
                    var tempList = FinanceUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> FinanceHODList = GetSharePointApproversList("1", "Motor Vehicles", "Finance");
                foreach (var item in FinanceHODList)
                    {
                    var tempList = FinanceHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceHODArray = tempList.ToArray();
                    }
                foreach (ListItem vehicleItem in vehicleItems)
                    {
                    if (vehicleItem.FieldValues["PocDescription"] == null && vehicleItem.FieldValues["IsDraft"]?.ToString() == "False")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        DateTime CreatedDate = Convert.ToDateTime(vehicleItem.FieldValues["TestingCreatedDate"]).ToLocalTime();
                        decimal differvalue = (decimal)(CurrentDate - CreatedDate).TotalDays;
                        decimal TotalDays = Math.Floor(differvalue);
                        if (TotalDays == 15)
                            {
                            //var tempList = POCHODArray.ToList();
                            //foreach (var item in POCUserArray)
                            //    {
                            //    tempList.Add(item);
                            //    }
                            //POCHODArray = tempList.ToArray();
                            //var EmailsSet = new HashSet<string>(POCHODArray);
                            //POCHODArray = EmailsSet.ToArray();
                            var EmailsSetHod = new HashSet<string>(POCHODArray);
                            var EmailsSetUser = new HashSet<string>(POCUserArray);
                            POCHODArray = EmailsSetHod.ToArray();
                            POCUserArray = EmailsSetUser.ToArray();
                            SendMail(POCUserArray, "Motor Vehicle Claim on POC", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/pocfeedback/" + vehicleItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                            SendMail(POCHODArray, "Pending Approval for Motor Vehicle Claim on POC", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/financefeedback/" + vehicleItem.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                            }
                        else if (TotalDays == 10)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Motor Vehicle Claim on POC", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/pocfeedback/" + vehicleItem.FieldValues["ItemID"].ToString());
                            }
                        else if (TotalDays == 5)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Motor Vehicle Claim on POC", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/pocfeedback/" + vehicleItem.FieldValues["ItemID"].ToString());
                            }

                        }
                    else if (vehicleItem.FieldValues["ifHodTaskUpdate"]?.ToString() == "false" && (vehicleItem.FieldValues["Exceptional"])?.ToString() == "true"
                        && (vehicleItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, vehicleItem.FieldValues["ItemID"].ToString(), "Motor Vehicles");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }

                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Motor Vehicle Claim on HOD Approval", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/hodfeedback/" + vehicleItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Motor Vehicle Claim on HOD Approval", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/hodfeedback/" + vehicleItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Motor Vehicle Claim on HOD Approval", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/hodfeedback/" + vehicleItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    else if (vehicleItem.FieldValues["ifVendorTaskUpdate"]?.ToString() == "False" && (vehicleItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, vehicleItem.FieldValues["ItemID"].ToString(), "Motor Vehicles");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                
                                //tempList.Add("telcoRep.j@telco.com");
                                //VendorUserArray = tempList.ToArray();

                                //var EmailsSet = new HashSet<string>(VendorUserArray);
                                //VendorUserArray = EmailsSet.ToArray();
                                var EmailsSetUser = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSetUser.ToArray();
                                string[] VendorHOD = new string[] { "telcoRep.j@telco.com" };
                                SendMail(VendorHOD, "Pending Approval for Motor Vehicle Claim on Vendor", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/financefeedback/" + vehicleItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(VendorUserArray, "Motor Vehicle Claim on Vendor", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), VendorURL + "vendor/vehicle/" + vehicleItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Motor Vehicle Claim on Vendor", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), VendorURL + "vendor/vehicle/" + vehicleItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Motor Vehicle Claim on Vendor", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), VendorURL + "vendor/vehicle/" + vehicleItem.FieldValues["ItemID"].ToString());
                                }
                            }

                        }
                    else if (vehicleItem.FieldValues["financeTaskOutcome"] == null && vehicleItem.FieldValues["vendorTaskOutcome"]?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchFinanceArchivedDate(WF_ARCHIVE, vehicleItem.FieldValues["ItemID"].ToString(), "Motor Vehicles");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = FinanceHODArray.ToList();
                                //foreach (var item in FinanceUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //FinanceHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(FinanceHODArray);
                                //FinanceHODArray = EmailsSet.ToArray();
                                var EmailsSetHod = new HashSet<string>(FinanceHODArray);
                                var EmailsSetUser = new HashSet<string>(FinanceUserArray);
                                FinanceHODArray = EmailsSetHod.ToArray();
                                FinanceUserArray = EmailsSetUser.ToArray();
                                SendMail(FinanceHODArray, "Pending Approval for Motor Vehicle Claim on Finance", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/financefeedback/" + vehicleItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(FinanceUserArray, "Motor Vehicle Claim on Finance", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/financefeedback/" + vehicleItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Motor Vehicle Claim on Finance", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/financefeedback/" + vehicleItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Motor Vehicle Claim on Finance", "Motor Vehicle Claim", vehicleItem.FieldValues["Title"].ToString(), FinHubURL + "vehicle/financefeedback/" + vehicleItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    }
                }



            if (cashItems != null)
                {
                POCUserArray = new string[] { };
                POCHODArray = new string[] { };
                InsUserArray = new string[] { };
                InsHODArray = new string[] { };
                VendorUserArray = new string[] { };
                FinanceUserArray = new string[] { };
                FinanceHODArray = new string[] { };
                List<Dictionary<string, string>> POCUserList = GetSharePointApproversList("0", "Cash in Safe Claims", "POC");
                foreach (var item in POCUserList)
                    {
                    var tempList = POCUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> POCHODList = GetSharePointApproversList("1", "Cash in Safe Claims", "POC");
                foreach (var item in POCHODList)
                    {
                    var tempList = POCHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCHODArray = tempList.ToArray();
                    }
                /// 
                ///                 
                List<Dictionary<string, string>> InsUserList = GetSharePointApproversList("0", "Cash in Safe Claims", "FinanceInsurance");
                foreach (var item in InsUserList)
                    {
                    var tempList = InsUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    InsUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> InsHODList = GetSharePointApproversList("1", "Cash in Safe Claims", "FinanceInsurance");
                foreach (var item in InsHODList)
                    {
                    var tempList = InsHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    InsHODArray = tempList.ToArray();
                    }
                /// 
                ///
                List<Dictionary<string, string>> VendorUserList = GetSharePointVendorsList("Cash in Safe Claims");
                foreach (var item in VendorUserList)
                    {
                    var tempList = VendorUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    VendorUserArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> FinanceUserList = GetSharePointApproversList("0", "Cash in Safe Claims", "Finance");
                foreach (var item in FinanceUserList)
                    {
                    var tempList = FinanceUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> FinanceHODList = GetSharePointApproversList("1", "Cash in Safe Claims", "Finance");
                foreach (var item in FinanceHODList)
                    {
                    var tempList = FinanceHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceHODArray = tempList.ToArray();
                    }
                ///
                ///
                foreach (ListItem cashItem in cashItems)
                    {
                        {
                        if (cashItem.FieldValues["PocDescription"] == null && cashItem.FieldValues["IsDraft"]?.ToString() == "False")
                            {
                            Dictionary<string, string> newItem = new Dictionary<string, string>();
                            DateTime CurrentDate = DateTime.Now.ToLocalTime();
                            DateTime CreatedDate = Convert.ToDateTime(cashItem.FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - CreatedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(POCHODArray);
                                //POCHODArray = EmailsSet.ToArray();
                                var EmailsSetHod = new HashSet<string>(POCHODArray);
                                var EmailsSetUser = new HashSet<string>(POCUserArray);
                                POCHODArray = EmailsSetHod.ToArray();
                                POCUserArray = EmailsSetUser.ToArray();
                                SendMail(POCUserArray, "Cash in Safe Claim on POC", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/pocfeedback/" + cashItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                SendMail(POCHODArray, "Pending Approval for Cash in Safe Claim on POC", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/financefeedback/" + cashItem.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(POCUserArray);
                                POCUserArray = EmailsSet.ToArray();
                                SendMail(POCUserArray, "Cash in Safe Claim on POC", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/pocfeedback/" + cashItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(POCUserArray);
                                POCUserArray = EmailsSet.ToArray();
                                SendMail(POCUserArray, "Cash in Safe Claim on POC", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/pocfeedback/" + cashItem.FieldValues["ItemID"].ToString());
                                }

                            }
                        else if (cashItem.FieldValues["ifHodTaskUpdate"]?.ToString() == "false" && (cashItem.FieldValues["Exceptional"])?.ToString() == "true"
    && (cashItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                            {
                            Dictionary<string, string> newItem = new Dictionary<string, string>();
                            DateTime CurrentDate = DateTime.Now.ToLocalTime();
                            ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, cashItem.FieldValues["ItemID"].ToString(), "Cash in Safe Claims");
                            if (ArchiveItems.Count >= 1)
                                {
                                DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                                decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                                decimal TotalDays = Math.Floor(differvalue);
                                if (TotalDays == 15)
                                    {
                                    //var tempList = POCHODArray.ToList();
                                    //foreach (var item in POCUserArray)
                                    //    {
                                    //    tempList.Add(item);
                                    //    }

                                    //POCHODArray = tempList.ToArray();
                                    SendMail(POCHODArray, "Cash in Safe Claim on HOD Approval", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/hodfeedback/" + cashItem.FieldValues["ItemID"].ToString());
                                    }
                                else if (TotalDays == 10)
                                    {
                                    //var tempList = POCHODArray.ToList();
                                    //foreach (var item in POCUserArray)
                                    //    {
                                    //    tempList.Add(item);
                                    //    }
                                    //POCHODArray = tempList.ToArray();
                                    SendMail(POCHODArray, "Cash in Safe Claim on HOD Approval", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/hodfeedback/" + cashItem.FieldValues["ItemID"].ToString());
                                    }
                                else if (TotalDays == 5)
                                    {
                                    //var tempList = POCHODArray.ToList();
                                    //foreach (var item in POCUserArray)
                                    //    {
                                    //    tempList.Add(item);
                                    //    }
                                    //POCHODArray = tempList.ToArray();
                                    SendMail(POCHODArray, "Cash in Safe Claim on HOD Approval", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/hodfeedback/" + cashItem.FieldValues["ItemID"].ToString());
                                    }
                                }
                            }
                        else if (cashItem.FieldValues["financeTeamTaskOutcome"] == null && cashItem.FieldValues["PocTaskOutcome"]?.ToString() == "Approve")
                            {
                            Dictionary<string, string> newItem = new Dictionary<string, string>();
                            DateTime CurrentDate = DateTime.Now.ToLocalTime();
                            ListItemCollection ArchiveItems = FetchInsuranceArchivedDate(WF_ARCHIVE, cashItem.FieldValues["ItemID"].ToString(), "Cash in Safe Claims");
                            if (ArchiveItems.Count >= 1)
                                {
                                DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                                decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                                decimal TotalDays = Math.Floor(differvalue);
                                if (TotalDays == 15)
                                    {
                                    //var tempList = InsHODArray.ToList();
                                    //foreach (var item in InsUserArray)
                                    //    {
                                    //    tempList.Add(item);
                                    //    }
                                    //InsHODArray = tempList.ToArray();
                                    //var EmailsSet = new HashSet<string>(InsHODArray);
                                    //InsHODArray = EmailsSet.ToArray();
                                    var EmailsSetHod = new HashSet<string>(InsHODArray);
                                    var EmailsSetUser = new HashSet<string>(InsUserArray);
                                    InsHODArray = EmailsSetHod.ToArray();
                                    InsUserArray = EmailsSetUser.ToArray();
                                    SendMail(InsUserArray, "Cash in Safe Claim on Insurance Team Feadback", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/insurancefeedback/" + cashItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                    SendMail(InsHODArray, "Pending Approval for Cash in Safe Claim on Insurance Team Feadback", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/financefeedback/" + cashItem.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                                    }
                                else if (TotalDays == 10)
                                    {
                                    var EmailsSet = new HashSet<string>(InsUserArray);
                                    InsUserArray = EmailsSet.ToArray();
                                    SendMail(InsUserArray, "Cash in Safe Claim on Insurance Team Feadback", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/insurancefeedback/" + cashItem.FieldValues["ItemID"].ToString());
                                    }
                                else if (TotalDays == 5)
                                    {
                                    var EmailsSet = new HashSet<string>(InsUserArray);
                                    InsUserArray = EmailsSet.ToArray();
                                    SendMail(InsUserArray, "Cash in Safe Claim on Insurance Team Feadback", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/insurancefeedback/" + cashItem.FieldValues["ItemID"].ToString());
                                    }
                                }
                            }
                        else if (cashItem.FieldValues["ifVendorTaskUpdate"]?.ToString() == "False" && (cashItem.FieldValues["financeTeamTaskOutcome"])?.ToString() == "Approve")
                            {

                            Dictionary<string, string> newItem = new Dictionary<string, string>();
                            DateTime CurrentDate = DateTime.Now.ToLocalTime();
                            ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, cashItem.FieldValues["ItemID"].ToString(), "Cash in Safe Claims", "Finance Team");
                            if (ArchiveItems.Count >= 1)
                                {
                                DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                                decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                                decimal TotalDays = Math.Floor(differvalue);
                                if (TotalDays == 15)
                                    {
                                    
                                    //tempList.Add("telcoRep.j@telco.com");
                                    //VendorUserArray = tempList.ToArray();
                                    //var EmailsSet = new HashSet<string>(VendorUserArray);
                                    //VendorUserArray = EmailsSet.ToArray();
                                    var EmailsSetUser = new HashSet<string>(VendorUserArray);
                                    VendorUserArray = EmailsSetUser.ToArray();
                                    string[] VendorHOD = new string[] { "telcoRep.j@telco.com" };
                                    SendMail(VendorHOD, "Pending Approval for Cash in Safe Claim on Vendor", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/financefeedback/" + cashItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                    SendMail(VendorUserArray, "Cash in Safe Claim on Vendor", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), VendorURL + "vendor/cash/" + cashItem.FieldValues["ItemID"].ToString());
                                    }
                                else if (TotalDays == 10)
                                    {
                                    var EmailsSet = new HashSet<string>(VendorUserArray);
                                    VendorUserArray = EmailsSet.ToArray();
                                    SendMail(VendorUserArray, "Cash in Safe Claim on Vendor", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), VendorURL + "vendor/cash/" + cashItem.FieldValues["ItemID"].ToString());
                                    }
                                else if (TotalDays == 5)
                                    {
                                    var EmailsSet = new HashSet<string>(VendorUserArray);
                                    VendorUserArray = EmailsSet.ToArray();
                                    SendMail(VendorUserArray, "Cash in Safe Claim on Vendor", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), VendorURL + "vendor/cash/" + cashItem.FieldValues["ItemID"].ToString());
                                    }
                                }

                            }
                        else if (cashItem.FieldValues["financeTaskOutcome"] == null && cashItem.FieldValues["vendorTaskOutcome"]?.ToString() == "Approve")
                            {

                            Dictionary<string, string> newItem = new Dictionary<string, string>();
                            DateTime CurrentDate = DateTime.Now.ToLocalTime();
                            ListItemCollection ArchiveItems = FetchFinanceArchivedDate(WF_ARCHIVE, cashItem.FieldValues["ItemID"].ToString(), "Cash in Safe Claims");
                            if (ArchiveItems.Count >= 1)
                                {
                                DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                    //var tempList = FinanceHODArray.ToList();
                                    //foreach (var item in FinanceUserArray)
                                    //    {
                                    //    tempList.Add(item);
                                    //    }
                                    //    FinanceHODArray = tempList.ToArray();
                                    //var EmailsSet = new HashSet<string>(FinanceHODArray);
                                    //FinanceHODArray = EmailsSet.ToArray();
                                    var EmailsSetHod = new HashSet<string>(FinanceHODArray);
                                    var EmailsSetUser = new HashSet<string>(FinanceUserArray);
                                    FinanceHODArray = EmailsSetHod.ToArray();
                                    FinanceUserArray = EmailsSetUser.ToArray();
                                    SendMail(FinanceHODArray, "Pending Approval for Cash in Safe Claim on Finance", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/financefeedback/" + cashItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                    SendMail(FinanceUserArray, "Cash in Safe Claim on Finance", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/financefeedback/" + cashItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                    }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Cash in Safe Claim on Finance", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/financefeedback/" + cashItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Cash in Safe Claim on Finance", "Cash in Safe Claim", cashItem.FieldValues["Title"].ToString(), FinHubURL + "cash/financefeedback/" + cashItem.FieldValues["ItemID"].ToString());
                                }
                            }
                            }
                        }
                    }
                }
            if (marineInlandItems != null)
                {
                POCUserArray = new string[] { };
                POCHODArray = new string[] { };
                InsUserArray = new string[] { };
                InsHODArray = new string[] { };
                VendorUserArray = new string[] { };
                FinanceUserArray = new string[] { };
                FinanceHODArray = new string[] { };
                List<Dictionary<string, string>> POCUserList = GetSharePointApproversList("0", "Marine Inland", "POC");
                foreach (var item in POCUserList)
                    {
                    var tempList = POCUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> POCHODList = GetSharePointApproversList("1", "Marine Inland", "POC");
                foreach (var item in POCHODList)
                    {
                    var tempList = POCHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCHODArray = tempList.ToArray();
                    }
                /// 
                ///                 
                List<Dictionary<string, string>> InsUserList = GetSharePointApproversList("0", "Marine Inland", "FinanceInsurance");
                foreach (var item in InsUserList)
                    {
                    var tempList = InsUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    InsUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> InsHODList = GetSharePointApproversList("1", "Marine Inland", "FinanceInsurance");
                foreach (var item in InsHODList)
                    {
                    var tempList = InsHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    InsHODArray = tempList.ToArray();
                    }
                /// 
                ///
                List<Dictionary<string, string>> VendorUserList = GetSharePointVendorsList("Marine Inland");
                foreach (var item in VendorUserList)
                    {
                    var tempList = VendorUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    VendorUserArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> FinanceUserList = GetSharePointApproversList("0", "Marine Inland", "Finance");
                foreach (var item in FinanceUserList)
                    {
                    var tempList = FinanceUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> FinanceHODList = GetSharePointApproversList("1", "Marine Inland", "Finance");
                foreach (var item in FinanceHODList)
                    {
                    var tempList = FinanceHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceHODArray = tempList.ToArray();
                    }
                ///
                ///
                foreach (ListItem marineInlandItem in marineInlandItems)
                    {
                    if (marineInlandItem.FieldValues["PocDescription"] == null && marineInlandItem.FieldValues["IsDraft"]?.ToString() == "False")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        DateTime CreatedDate = Convert.ToDateTime(marineInlandItem.FieldValues["TestingCreatedDate"]).ToLocalTime();
                        decimal differvalue = (decimal)(CurrentDate - CreatedDate).TotalDays;
                        decimal TotalDays = Math.Floor(differvalue);
                        if (TotalDays == 15)
                            {
                            //var tempList = POCHODArray.ToList();
                            //foreach (var item in POCUserArray)
                            //    {
                            //    tempList.Add(item);
                            //    }
                            //POCHODArray = tempList.ToArray();
                            //var EmailsSet = new HashSet<string>(POCHODArray);
                            //POCHODArray = EmailsSet.ToArray();
                            var EmailsSetHod = new HashSet<string>(POCHODArray);
                            var EmailsSetUser = new HashSet<string>(POCUserArray);
                            POCHODArray = EmailsSetHod.ToArray();
                            POCUserArray = EmailsSetUser.ToArray();
                            SendMail(POCUserArray, "Marine Inland Claim on POC", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/pocfeedback/" + marineInlandItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                            SendMail(POCHODArray, "Pending Approval for Marine Inland Claim on POC", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/financefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                            }
                        else if (TotalDays == 10)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Marine Inland Claim on POC", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/pocfeedback/" + marineInlandItem.FieldValues["ItemID"].ToString());
                            }
                        else if (TotalDays == 5)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Marine Inland Claim on POC", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/pocfeedback/" + marineInlandItem.FieldValues["ItemID"].ToString());
                            }
                        }
                    else if (marineInlandItem.FieldValues["ifHodTaskUpdate"]?.ToString() == "false" && (marineInlandItem.FieldValues["Exceptional"])?.ToString() == "true"
   && (marineInlandItem.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, marineInlandItem.FieldValues["ItemID"].ToString(), "Marine Inland");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }

                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Marine Inland Claim on HOD Approval", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/hodfeedback/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Marine Inland Claim on HOD Approval", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/hodfeedback/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Marine Inland Claim on HOD Approval", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/hodfeedback/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    else if (marineInlandItem.FieldValues["financeTeamTaskOutcome"] == null && marineInlandItem.FieldValues["PocTaskOutcome"]?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchInsuranceArchivedDate(WF_ARCHIVE, marineInlandItem.FieldValues["ItemID"].ToString(), "Marine Inland");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = InsHODArray.ToList();
                                //foreach (var item in InsUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //InsHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(InsHODArray);
                                //InsHODArray = EmailsSet.ToArray();
                                //SendMail(InsHODArray, "Marine Inland Claim on Insurance Team Feadback", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/insurancefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                var EmailsSetHod = new HashSet<string>(InsHODArray);
                                var EmailsSetUser = new HashSet<string>(InsUserArray);
                                InsHODArray = EmailsSetHod.ToArray();
                                InsUserArray = EmailsSetUser.ToArray();
                                SendMail(InsUserArray, "Marine Inland Claim on Insurance Team Feadback", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/insurancefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                SendMail(InsHODArray, "Pending Approval for Marine Inland Claim on Insurance Team Feadback", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/financefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(InsUserArray);
                                InsUserArray = EmailsSet.ToArray();
                                SendMail(InsUserArray, "Marine Inland Claim on Insurance Team Feadback", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/insurancefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(InsUserArray);
                                InsUserArray = EmailsSet.ToArray();
                                SendMail(InsUserArray, "Marine Inland Claim on Insurance Team Feadback", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/insurancefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    else if (marineInlandItem.FieldValues["ifVendorTaskUpdate"]?.ToString() == "False" && (marineInlandItem.FieldValues["financeTeamTaskOutcome"])?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, marineInlandItem.FieldValues["ItemID"].ToString(), "Marine Inland", "Finance Team");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                
                                //tempList.Add("telcoRep.j@telco.com");
                                //VendorUserArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(VendorUserArray);
                                //VendorUserArray = EmailsSet.ToArray();
                                var EmailsSetUser = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSetUser.ToArray();
                                string[] VendorHOD = new string[] { "telcoRep.j@telco.com" };
                                SendMail(VendorHOD, "Pending Approval for Marine Inland Claim on Vendor", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/financefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(VendorUserArray, "Marine Inland Claim on Vendor", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), VendorURL + "vendor/marineinland/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Marine Inland Claim on Vendor", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), VendorURL + "vendor/marineinland/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSet.ToArray();
                                SendMail(VendorUserArray, "Marine Inland Claim on Vendor", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), VendorURL + "vendor/marineinland/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                }
                            }

                        }
                    else if (marineInlandItem.FieldValues["financeTaskOutcome"] == null && marineInlandItem.FieldValues["vendorTaskOutcome"]?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchFinanceArchivedDate(WF_ARCHIVE, marineInlandItem.FieldValues["ItemID"].ToString(), "Marine Inland");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = FinanceHODArray.ToList();
                                //foreach (var item in FinanceUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //FinanceHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(FinanceHODArray);
                                //FinanceHODArray = EmailsSet.ToArray();
                                var EmailsSetHod = new HashSet<string>(FinanceHODArray);
                                var EmailsSetUser = new HashSet<string>(FinanceUserArray);
                                FinanceHODArray = EmailsSetHod.ToArray();
                                FinanceUserArray = EmailsSetUser.ToArray();
                                SendMail(FinanceHODArray, "Pending Approval for Marine Inland Claim on Finance", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/financefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(FinanceUserArray, "Marine Inland Claim on Finance", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/financefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Marine Inland Claim on Finance", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/financefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Marine Inland Claim on Finance", "Marine Inland Claim", marineInlandItem.FieldValues["Title"].ToString(), FinHubURL + "marineinland/financefeedback/" + marineInlandItem.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    }
                }



            if (cellsites != null)
                {
                POCUserArray = new string[] { };
                POCHODArray = new string[] { };
                FixedAssetUserArray = new string[] { };
                FixedAssetHODArray = new string[] { };
                VendorUserArray = new string[] { };
                FinanceUserArray = new string[] { };
                FinanceHODArray = new string[] { };
                List<Dictionary<string, string>> POCUserList = GetSharePointApproversList("0", "Cell Site Claims", "POC");
                foreach (var item in POCUserList)
                    {
                    var tempList = POCUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> POCHODList = GetSharePointApproversList("1", "Cell Site Claims", "POC");
                foreach (var item in POCHODList)
                    {
                    var tempList = POCHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    POCHODArray = tempList.ToArray();
                    }
                ///
                ///
                ///                 
                List<Dictionary<string, string>> FixedAssetUserList = GetSharePointApproversList("0", "Cell Site Claims", "FixedAsset");
                foreach (var item in FixedAssetUserList)
                    {
                    var tempList = FixedAssetUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FixedAssetUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> FixedAssetHODList = GetSharePointApproversList("1", "Cell Site Claims", "FixedAsset");
                foreach (var item in FixedAssetHODList)
                    {
                    var tempList = FixedAssetHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FixedAssetHODArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> VendorUserList = GetSharePointVendorsList("Cell Site Claims");
                foreach (var item in VendorUserList)
                    {
                    var tempList = VendorUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    VendorUserArray = tempList.ToArray();
                    }
                ///
                ///
                List<Dictionary<string, string>> FinanceUserList = GetSharePointApproversList("0", "Cell Site Claims", "Finance");
                foreach (var item in FinanceUserList)
                    {
                    var tempList = FinanceUserArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceUserArray = tempList.ToArray();
                    }
                List<Dictionary<string, string>> FinanceHODList = GetSharePointApproversList("1", "Cell Site Claims", "Finance");
                foreach (var item in FinanceHODList)
                    {
                    var tempList = FinanceHODArray.ToList();
                    tempList.Add(item["Email"].ToString());
                    FinanceHODArray = tempList.ToArray();
                    }
                ///
                ///
                foreach (ListItem cellsite in cellsites)
                    {
                    if (cellsite.FieldValues["PocDescription"] == null && cellsite.FieldValues["IsDraft"]?.ToString() == "False")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        DateTime CreatedDate = Convert.ToDateTime(cellsite.FieldValues["TestingCreatedDate"]).ToLocalTime();
                        decimal differvalue = (decimal)(CurrentDate - CreatedDate).TotalDays;
                        decimal TotalDays = Math.Floor(differvalue);
                        if (TotalDays == 15)
                            {
                            //var tempList = POCHODArray.ToList();
                            //foreach (var item in POCUserArray)
                            //    {
                            //    tempList.Add(item);
                            //    }
                            //POCHODArray = tempList.ToArray();
                            //var EmailsSet = new HashSet<string>(POCHODArray);
                            //POCHODArray = EmailsSet.ToArray();
                            var EmailsSetHod = new HashSet<string>(POCHODArray);
                            var EmailsSetUser = new HashSet<string>(POCUserArray);
                            POCHODArray = EmailsSetHod.ToArray();
                            POCUserArray = EmailsSetUser.ToArray();
                            SendMail(POCUserArray, "Cell Site Claim on POC", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/pocfeedback/" + cellsite.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                            SendMail(POCHODArray, "Pending Approval for Cell Site Claim on POC", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/financefeedback/" + cellsite.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                            }
                        else if (TotalDays == 10)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Cell Site Claim on POC", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/pocfeedback/" + cellsite.FieldValues["ItemID"].ToString());
                            }
                        else if (TotalDays == 5)
                            {
                            var EmailsSet = new HashSet<string>(POCUserArray);
                            POCUserArray = EmailsSet.ToArray();
                            SendMail(POCUserArray, "Cell Site Claim on POC", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/pocfeedback/" + cellsite.FieldValues["ItemID"].ToString());
                            }
                        }
                    else if (cellsite.FieldValues["ifHodTaskUpdate"]?.ToString() == "false" && (cellsite.FieldValues["Exceptional"])?.ToString() == "true"
   && (cellsite.FieldValues["PocTaskOutcome"])?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, cellsite.FieldValues["ItemID"].ToString(), "Cell Site Claims");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }

                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Cell Site Claim on HOD Approval", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/hodfeedback/" + cellsite.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 10)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Cell Site Claim on HOD Approval", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/hodfeedback/" + cellsite.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                //var tempList = POCHODArray.ToList();
                                //foreach (var item in POCUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //POCHODArray = tempList.ToArray();
                                SendMail(POCHODArray, "Cell Site Claim on HOD Approval", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/hodfeedback/" + cellsite.FieldValues["ItemID"].ToString());
                                }
                            }
                        }
                    else if (cellsite.FieldValues["ifFixedAssetTaskUpdate"]?.ToString() == "False" && cellsite.FieldValues["PocTaskOutcome"]?.ToString() == "Approve")
                        {
                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchFixedAssetArchivedDate(WF_ARCHIVE, cellsite.FieldValues["ItemID"].ToString(), "Cell Site Claims");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = FixedAssetHODArray.ToList();
                                //foreach (var item in FixedAssetUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //FixedAssetHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(FixedAssetHODArray);
                                //FixedAssetHODArray = EmailsSet.ToArray();
                                var EmailsSetHod = new HashSet<string>(FixedAssetHODArray);
                                var EmailsSetUser = new HashSet<string>(FixedAssetUserArray);
                                FixedAssetHODArray = EmailsSetHod.ToArray();
                                FixedAssetUserArray = EmailsSetUser.ToArray();
                                SendMail(FixedAssetHODArray, "Pending Approval for Cell Site Claim on Fixed Asset", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/financefeedback/" + cellsite.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                                SendMail(FixedAssetUserArray, "Cell Site Claim on Fixed Asset", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/fixedasset/" + cellsite.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(FixedAssetUserArray);
                                FixedAssetUserArray = EmailsSet.ToArray();
                                SendMail(FixedAssetUserArray, "Cell Site Claim on Fixed Asset", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/fixedasset/" + cellsite.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(FixedAssetUserArray);
                                FixedAssetUserArray = EmailsSet.ToArray();
                                SendMail(FixedAssetUserArray, "Cell Site Claim on Fixed Asset", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/fixedasset/" + cellsite.FieldValues["ItemID"].ToString());
                                }
                            }

                        }
                    else if (cellsite.FieldValues["ifVendorTaskUpdate"]?.ToString() == "False" && cellsite.FieldValues["ifFixedAssetTaskUpdate"]?.ToString() == "True")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchVendorArchivedDate(WF_ARCHIVE, cellsite.FieldValues["ItemID"].ToString(), "Cell Site Claims","FixedAsset");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                                if (TotalDays == 15)
                                    {
                                
                                //tempList.Add("telcoRep.j@telco.com");
                                //VendorUserArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(VendorUserArray);
                                //VendorUserArray = EmailsSet.ToArray();
                                var EmailsSetUser = new HashSet<string>(VendorUserArray);
                                VendorUserArray = EmailsSetUser.ToArray();
                                string[] VendorHOD = new string[] { "telcoRep.j@telco.com" };
                                SendMail(VendorHOD, "Pending Approval for Cell Site Claim on Vendor", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/financefeedback/" + cellsite.FieldValues["ItemID"].ToString() + "?viewmode=viewonly");
                                SendMail(VendorUserArray, "Cell Site Claim on Vendor", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), VendorURL + "vendor/cellsite/" + cellsite.FieldValues["ItemID"].ToString());
                                    }
                                else if (TotalDays == 10)
                                    {
                                    var EmailsSet = new HashSet<string>(VendorUserArray);
                                    VendorUserArray = EmailsSet.ToArray();
                                    SendMail(VendorUserArray, "Cell Site Claim on Vendor", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), VendorURL + "vendor/cellsite/" + cellsite.FieldValues["ItemID"].ToString());
                                    }
                                else if (TotalDays == 5)
                                    {
                                    var EmailsSet = new HashSet<string>(VendorUserArray);
                                    VendorUserArray = EmailsSet.ToArray();
                                    SendMail(VendorUserArray, "Cell Site Claim on Vendor", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), VendorURL + "vendor/cellsite/" + cellsite.FieldValues["ItemID"].ToString());
                                    }
                            }
                        }
                    else if (cellsite.FieldValues["financeTaskOutcome"] == null && cellsite.FieldValues["vendorTaskOutcome"]?.ToString() == "Approve")
                        {

                        Dictionary<string, string> newItem = new Dictionary<string, string>();
                        DateTime CurrentDate = DateTime.Now.ToLocalTime();
                        ListItemCollection ArchiveItems = FetchFinanceArchivedDate(WF_ARCHIVE, cellsite.FieldValues["ItemID"].ToString(), "Cell Site Claims");
                        if (ArchiveItems.Count >= 1)
                            {
                            DateTime ArchivedDate = Convert.ToDateTime(ArchiveItems[0].FieldValues["TestingCreatedDate"]).ToLocalTime();
                            decimal differvalue = (decimal)(CurrentDate - ArchivedDate).TotalDays;
                            decimal TotalDays = Math.Floor(differvalue);
                            if (TotalDays == 15)
                                {
                                //var tempList = FinanceHODArray.ToList();
                                //foreach (var item in FinanceUserArray)
                                //    {
                                //    tempList.Add(item);
                                //    }
                                //FinanceHODArray = tempList.ToArray();
                                //var EmailsSet = new HashSet<string>(FinanceHODArray);
                                //FinanceHODArray = EmailsSet.ToArray();
                                var EmailsSetHod = new HashSet<string>(FinanceHODArray);
                                var EmailsSetUser = new HashSet<string>(FinanceUserArray);
                                FinanceHODArray = EmailsSetHod.ToArray();
                                FinanceUserArray = EmailsSetUser.ToArray();
                                SendMail(FinanceHODArray, "Pending Approval for Cell Site Claim on Finance", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/financefeedback/" + cellsite.FieldValues["ItemID"].ToString()+"?viewmode=viewonly");
                                SendMail(FinanceUserArray, "Cell Site Claim on Finance", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/financefeedback/" + cellsite.FieldValues["ItemID"].ToString(), "Your HOD is notified as well.");
                                }
                            else if (TotalDays == 10)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Cell Site Claim on Finance", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/financefeedback/" + cellsite.FieldValues["ItemID"].ToString());
                                }
                            else if (TotalDays == 5)
                                {
                                var EmailsSet = new HashSet<string>(FinanceUserArray);
                                FinanceUserArray = EmailsSet.ToArray();
                                SendMail(FinanceUserArray, "Cell Site Claim on Finance", "Cell Site Claim", cellsite.FieldValues["Title"].ToString(), FinHubURL + "cellsite/financefeedback/" + cellsite.FieldValues["ItemID"].ToString());
                                }
                            }
                        }

                    }
                }
            return Response;
            }

        }






    }
