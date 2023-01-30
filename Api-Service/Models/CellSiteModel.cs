
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models;
public class CellSiteModel : FilesModel
{

    public string SiteCode { get; set; }
    public string SiteName { get; set; }

    // [Required]
    //  [Column("Damanage_Item")]
    public string DamanageItem { get; set; }
  //  [Column("Site_Downtime")]
    public String SiteDowntime { get; set; }
  //  [Required]
  //  [RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
  //  [Column("Poc_Name")]
    public string PocName { get; set; }
   // [Required]
   // [RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
   // [Column("Poc_Contact_No")]
    public string PocContactNo { get; set; }
   // [Required]
   // [Column("Incident_Reason")]
    public string IncidentReason { get; set; }
    public string Entity { get; set; }

}
