namespace TelcoAPIService.Models
{
    public class VehicleModel : BaseModel
    {
        //[Required]
        //[StringLength(15, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 4)]
        public string Company { get; set; }

        //[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
        public string EmployeeName { get; set; }

        //[Required]
        public int EmployeeId { get; set; }

        //[Required]
        //[RegularExpression(@"^[a-zA-Z0-9]{3,8}$", ErrorMessage = "Please use valid format for engine #.")]
        public string EngineNo { get; set; }

        //[Required]
        //[RegularExpression(@"^[a-zA-Z0-9]{2,8}$", ErrorMessage = "Please use valid format for chassis #.")]
        public string ChassisNo { get; set; }

        //[Required]
        //[RegularExpression(@"^[a-zA-Z0-9]{3,12}$", ErrorMessage = "Please use valid format for registration #.")]
        public string RegistrationNo { get; set; }

        //[Required]
        //[RegularExpression(@"^[a-zA-Z0-9]{2,6}$", ErrorMessage = "Please use valid format for model #.")]
        public string ModelNo { get; set; }

        //[Required]
        //[StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Make { get; set; }

        //[Required]
        public string Description { get; set; }

        //[Required]
        //[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
        public string PocName { get; set; }

        //[Required]
        //[RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
        public string PocContactNo { get; set; }
    }
}
