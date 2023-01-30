
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models;
public class VehicleModel : FilesModel
{
   /* [ForeignKey("WorkflowModel")]
    [Column("Reference_No")]
    public Guid ReferenceNumber { get; set; }
    public WorkflowModel WorkflowModel { get; set; }*/

    //[Required]
    //[StringLength(15, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
    public string Company { get; set; }
    //[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
    [Column("Emp_Name")]
    public string EmployeeName { get; set; }
    //[Required]
    [Column("Emp_Id")]
    public string EmployeeId { get; set; }
    //[Required]
    //[RegularExpression(@"^[a-zA-Z0-9]{3,8}$", ErrorMessage = "Please use valid format for engine #.")]
    [Column("Engine_No")]
    public string EngineNo { get; set; }
    //[Required]
    //[RegularExpression(@"^[a-zA-Z0-9]{2,8}$", ErrorMessage = "Please use valid format for chassis #.")]
    [Column("Chassis_No")]
    public string ChassisNo { get; set; }
    //[Required]
    //[RegularExpression(@"^[a-zA-Z0-9]{3,12}$", ErrorMessage = "Please use valid format for registration #.")]
    [Column("Registration_No")]
    public string RegistrationNo { get; set; }
    //[Required]
    //[RegularExpression(@"^[a-zA-Z0-9]{2,6}$", ErrorMessage = "Please use valid format for model #.")]
    [Column("Model_No")]
    public string ModelNo { get; set; }
    //[Required]
    //[StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
    public string Make { get; set; }
    [Required]
    public string Description { get; set; }
    //[Required]
    //[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
    [Column("Poc_Name")]
    public string PocName { get; set; }
    //[Required]
    //[RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
    [Column("Poc_Contact_No")]
    public string PocContactNo { get; set; }
}
