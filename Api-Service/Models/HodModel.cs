
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models;
public class HODModel
{
    //[Required]
    public int ID { get; set; }
    public string Approver { get; set; }
    public string hodcomments { get; set; }
    public string ifHodTaskUpdate { get; set; }
    public string hodTaskOutcome { get; set; }
    public bool Exceptional { get; set; }


}