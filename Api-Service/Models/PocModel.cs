
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models;
public class PocModel : FilesModel
{


    [Required]
    public string Action { get; set; }

    public string Comments { get; set; }
    public string Manager { get; set; }


    public string InitiatedBy { get; set; }
    // public TechTeamPocModel TechTeamPoc { get; set; }
}
