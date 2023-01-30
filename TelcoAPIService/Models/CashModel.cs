
namespace TelcoAPIService.Models
{
    public class CashModel : BaseModel
    {
       
        //[Required]
        //[RegularExpression(@"^[a-zA-Z0-9]{3,9}$", ErrorMessage = "Please use valid format for petty id.")]
        //public string PettyId { get; set; }
        
        public string IncidentReason { get; set; }
        
        //[Required]
        //[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
        public string PocName { get; set; }
       
        //[Required]
        //[RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
        public string POCNumber { get; set; }

        // [Required]
        // [RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
        public string FranchiseID { get; set; }

        // [Required]
        public int PettyAmountStolen { get; set; }
    }
}
