using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelcoAPIService.Models
    {
    public class HODModel
        {
        //[Required]
        public int ID { get; set; }
        public string Approver { get; set; }
        public string hodcomments { get; set; }
        public string ifHodTaskUpdate { get; set; }
        public string hodTaskOutcome { get; set; }
        public string ClaimFor { get; internal set; }
        public bool Exceptional { get; set; }

    }
    }