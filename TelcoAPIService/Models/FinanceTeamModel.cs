
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TelcoAPIService.Models
{
    public class FinanceTeamModel
    {
        //[Required]
        public int ID { get; set; }

        // [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfLodgement { get; set; }

        //[Required]
        public string ClaimAmount { get; set; }

        public string Recovery { get; set; }

        public string ChequeNumber { get; set; }

        // [Required]
        [DataType(DataType.Date)]
        public DateTime ChequeRecievedDate { get; set; }

        //[Required]
        public string DeductiblePolicy { get; set; }

        public string FtComments { get; set; }

        //[Required]
        public string financeTaskOutcome { get; set; }

        public string Approver { get; set; }

        public string ClaimFor { get; internal set; }

        public List<HttpPostedFile> Attachment { get; set; }

        public List<String> AttachmentBase64 { get; set; }
        public string NBV { get; set; }
        public string InsuranceCompanyDeductible { get; set; }
        }
}