using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelcoAPIService.Models
{
    public class BiometricModel : BaseModel
    {
        
        public string EmployeeName { get; set; }
        
        public string SerialNo { get; set; }

        // [Required]
        // [StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Make { get; set; }
        
        // [Required]
        // [StringLength(15, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
        public string AssignToManager { get; set; }
        
        // [Required]
        //[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
        public string PocName { get; set; }
        
        // [Required]
        // [RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
        public string PocContactNo { get; set; }
        
        // [Required]
        public string Description { get; set; }
    }
}