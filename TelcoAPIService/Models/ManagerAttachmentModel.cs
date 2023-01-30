
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TelcoAPIService.Models
{
    public class ManagerAttachmentModel
    {
        public int ID { get; set; }

        public string PocName { get; set; }

        public string PocContactNo { get; set; }
        public string StolenAmount { get; set; }

        public string Action { get; set; }

        public string Comments { get; set; }

        public string ClaimFor { get; internal set; }

        public string Approver { get; set; }

        public List<HttpPostedFile> Attachment { get; set; }

        public List<String> AttachmentBase64 { get; set; }
    }
}