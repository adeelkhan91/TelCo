using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TelcoAPIService.Models
{
    public class VendorModel
    {
        [Required]
        public int ID { get; set; }

        public List<HttpPostedFile> Attachment { get; set; }

        public List<String> AttachmentBase64 { get; set; }

        public string vendorComments { get; set; }

        [Required]
        public string vendorTaskOutcome { get; set; }

        public string Approver { get; set; }

        public string ClaimFor { get; internal set; }
    }
}