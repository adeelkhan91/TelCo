
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiService.Models
{
    public class NewInitiatorModel
    {
        public string Region { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Workflow { get; set; }
    }
}