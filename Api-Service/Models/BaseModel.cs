
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models;
public class BaseModel
{
    public string ReferenceNumber { get; set; }
    // public virtual Guid ReferenceNumber { get; set; }

    // [Key]
    // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public bool InitIsReject { get; set; }
    public bool Exceptional { get; set; }

    [DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }
    /*[DisplayFormat(DataFormatString="{0:00:00}", ApplyFormatInEditMode=true)]*/
    public TimeSpan Time { get; set; }
    /*[StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
    [Column("Claim_Type")]*/
    public string ClaimType { get; set; }
    /* [Required]
     [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]*/
    public string Region { get; set; }
    //[StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
    public string City { get; set; }
    public string InitiatedBy { get; set; }
}