using APILibrary.Core.Attributes;
using APILibrary.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class Customer : ModelBase
    {
        //public int ID { get; set; }

        [NotJson]
        [Required(ErrorMessage = "l'email est obligatoire")]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Firstname { get; set; }

        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        
    }
}
