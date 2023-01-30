
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public class BiometricModel : FilesModel
    {

        [Column("Emp_Name")]
        public string EmployeeName { get; set; }
        [Column("Serial_No")]
        public string SerialNo { get; set; }
        // [Required]
        // [StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Make { get; set; }
        // [Required]
        // [StringLength(15, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
        [Column("Assign_To_Manager")]
        public string AssignToManager { get; set; }
        // [Required]
        //[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
        [Column("Poc_Name")]
        public string PocName { get; set; }
        // [Required]
        // [RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
        [Column("Poc_Contact_No")]
        public string PocContactNo { get; set; }
        // [Required]
        public string Description { get; set; }

    }
}