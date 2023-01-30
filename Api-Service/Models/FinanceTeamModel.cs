
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models;
public class FinanceTeamModel
{
    //[Required]
    public int ID { get; set; }

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
    public string NBV { get; set; }
    public string InsuranceCompanyDeductible { get; set; }
}