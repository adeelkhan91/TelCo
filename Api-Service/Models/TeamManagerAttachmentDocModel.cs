
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models;
public class TeamManagerAttachmentDocModel : FilesModel
{
    
    //[Required]
    //[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
    public String PocName { get; set; }
    //[Required]
    //[RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
    public String PocContactNo { get; set; }
    public String IncidentReason { get; set; }
    //[Required]
    //[StringLength(15, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
    public String AssignToManager { get; set; }
    //[StringLength(15, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
    public String Action { get; set; }
    public String Comment { get; set; }
    public string Approver { get; set; }
}