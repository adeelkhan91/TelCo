using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TelcoAPIService.Models
{
    public class PocModel
    {
        [Required]
        public int Id { get; set; }

        public List<HttpPostedFile> Attachment { get; set; }

        public List<String> AttachmentBase64 { get; set; }

        [Required]
        public string Action { get; set; }
        public string Manager { get; set; }

        public string Comments { get; set; }

        public string InitiatedBy { get; set; }

        public string ClaimFor { get; internal set; }
    }
}