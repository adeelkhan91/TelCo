
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models;
public class HandsetModel : FilesModel
{
    //[ForeignKey("WorkflowModel")]
    //[Column("Reference_No")]
    //public Guid ReferenceNumber { get; set; }
    //public WorkflowModel WorkflowModel { get; set; }

    //[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
    [Column("Emp_Name")]
    public string EmployeeName { get; set; }
    //[Required]
    [Column("Emp_Id")]
    public string EmployeeId { get; set; }
    //[Required]
    //[StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 10)]
    //[EmailAddress]
    [Column("Emp_Email")]
    public string EmployeeEmail { get; set; }
    //[Required]
    //[RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
    [Column("Emp_Contact_No")]
    public string EmployeeContactNo { get; set; }
    //[Required]
    //[RegularExpression(@"^[a-zA-Z0-9]{3,9}$", ErrorMessage = "Please use valid format for serial #.")]
    [Column("Serial_No")]
    public string SerialNo { get; set; }
    //[Required]
    //[RegularExpression(@"^[a-zA-Z0-9]{3,9}$", ErrorMessage = "Please use valid format for model #.")]
    [Column("Model_No")]
    public string ModelNo { get; set; }
    //[Required]
    //[StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
    public string Make { get; set; }
    //[Required]
    public string Description { get; set; }
}
