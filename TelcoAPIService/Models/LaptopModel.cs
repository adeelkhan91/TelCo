namespace TelcoAPIService.Models
{
    public class LaptopModel : BaseModel
    {
        //[RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$", ErrorMessage = "Characters are not allowed.")]
        public string EmployeeName { get; set; }

        public int EmployeeId { get; set; }
        //[StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 10)]
        //[EmailAddress]
        public string EmployeeEmail { get; set; }

        //[RegularExpression(@"^((\+92)|(0092))-{0,1}\d{3}-{0,1}\d{7}$|^\d{11}$|^\d{4}-\d{7}$", ErrorMessage = "Only numeric value allowed.")]
        public string EmployeeContactNo { get; set; }

       // [RegularExpression(@"^[a-zA-Z0-9]{3,9}$", ErrorMessage = "Please use valid format for serial #.")]
        public string SerialNo { get; set; }

        //[RegularExpression(@"^[a-zA-Z0-9]{3,9}$", ErrorMessage = "Please use valid format for model #.")]
        public string ModelNo { get; set; }

        //[RegularExpression(@"^[a-zA-Z0-9]{3,9}$", ErrorMessage = "Please use valid format for stock code.")]
        public string StockCode { get; set; }

        //[StringLength(20, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Make { get; set; }

        public string Description { get; set; }
    }
}
