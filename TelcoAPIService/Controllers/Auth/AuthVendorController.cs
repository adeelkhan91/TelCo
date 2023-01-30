using Microsoft.SharePoint.Client;
using TelcoAPIService.Models;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;
using Config = System.Configuration.ConfigurationManager;
using System;
using System.Net.Http;
using System.Net;

namespace TelcoAPIService.Controllers
{
    [System.Web.Http.RoutePrefix("api/vendorauth")]
    public class VendorAuthController : BaseController
    {

        private readonly string VENDOR_LOGIN = Config.AppSettings["VENDOR_LOGIN"];

        #region ACTIONS

        [HttpPost]
        [Route("signup")]
        public HttpResponseMessage VendorLogin(VendorAuthModel vendor)
            {
            try
                {
                ClientContext clientContext = GetSharePointAuth();
                var SPClaims = clientContext.Web.Lists.GetById(new Guid(VENDOR_LOGIN));
                ListItem SPClaim = SPClaims.AddItem(new ListItemCreationInformation());
                SPClaim["Title"] = vendor.UserName;
                SPClaim["VendorName"] = vendor.Name;
                SPClaim["VendorEmail"] = vendor.Email;
                SPClaim["VendorPassword"] = EnryptString(vendor.Password);

                SPClaim.Update();
                clientContext.ExecuteQuery();
                return Request.CreateResponse(HttpStatusCode.OK, "Record inserted");
                }
            catch (Exception ex)
                {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed, please try again.");
                }
            }

        [HttpPost]
        [Route("login")]
        public HttpResponseMessage VendorSignup(VendorAuthModel vendor)
            {
            try
                {
                ClientContext clientContext = GetSharePointAuth();
                bool IsValid = ValidateUser(clientContext, vendor);
                if (IsValid)
                    {
                    return Request.CreateResponse(HttpStatusCode.OK, "Record inserted");
                    }
                else
                    {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed, please try again.");
                    }
                }
            catch (Exception ex)
                {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Request failed, please try again.");
                }
            }

        #endregion

        #region CUSTOM_METHODS

        private bool ValidateUser(ClientContext clientContext, VendorAuthModel vendor)
            {
            List vendorList = clientContext.Web.Lists.GetById(new Guid(VENDOR_LOGIN));
            var camlQuery = new CamlQuery
                {
               
                ViewXml = @"<View Scope='RecursiveAll'>
                    <Query>
                        <Where>
                           <Eq><FieldRef Name='VendorEmail'/><Value Type='Text'>" + vendor.Email + @"</Value></Eq>
                        </Where>
                    </Query>
                    <ViewFields>
                        <FieldRef Name='VendorPassword' />
                    </ViewFields>
                    </View>"
                };

            ListItemCollection listItems = vendorList.GetItems(camlQuery);

            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            if (!listItems.AreItemsAvailable)
                {
                return false;
                }

            bool response = false;
            foreach (ListItem item in listItems)
                {
                string VendorPassword = DecryptString(item["VendorPassword"].ToString());
                if (VendorPassword == vendor.Password)
                    {
                    response = true;
                    }
                else
                    response = false;
                }
            return response;
        }


        private string EnryptString(string strEncrypted)
        {
            byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }


        public string DecryptString(string encrString)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(encrString);
                decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (FormatException fe)
            {
                decrypted = "";
            }
            return decrypted;
        }
        #endregion
        }
    }
