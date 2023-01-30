using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web;

namespace TelcoAPIService.Models
{
    public class BaseModel
    {
        public int Id { get; set; }
        public bool InitIsReject { get; set; }
        public bool Exceptional { get; set; }
        public string ReferenceNumber { get; set; }

        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        //[DisplayFormat(DataFormatString = "{0:00:00}", ApplyFormatInEditMode = true)]
        //public TimeSpan Time { get; set; }
        //[StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
        public string ClaimType { get; set; }

        //[StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
        public string Region { get; set; }

        //[StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
        public string City { get; set; }

        public string InitiatedBy { get; set; }

        public List<HttpPostedFile> Attachment { get; set; }

        public List<string> AttachmentBase64 { get; set; }


        public List<HttpPostedFile> DamagedFilesAttachment { get; set; }

        public List<String> DamagedFilesAttachmentBase64 { get; set; }
        }
}