
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public class MarineModel : FilesModel
    {
        //[ForeignKey("WorkflowModel")]
       // [Column("Reference_No")]
       // public Guid ReferenceNumber { get; set; }
        public WorkflowModel WorkflowModel { get; set; }

        [Column("Incident_Reason")]
        public string IncidentReason { get; set; }
        [Required]
        //[StringLength(15, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
        [Column("Assign_To_Manager")]
        public string AssignToManager { get; set; }
        [Required]
       // [RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
        [Column("Poc_Name")]
        public string PocName { get; set; }
        [Required]
       // [RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
        [Column("Poc_Contact_No")]
        public string PocContactNo { get; set; }
    }
}