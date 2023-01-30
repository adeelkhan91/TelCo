
using System.ComponentModel.DataAnnotations;

namespace ApiService.Models;
public class VendorModel : FilesModel
{
    [Required]
    public int ID { get; set; }
    public string vendorComments { get; set; }
    public string vendorTaskOutcome { get; set; }

    public string Approver { get; set; }
}
