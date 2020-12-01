using APILibrary.core.Attributes;
using APILibrary.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class User : UserBase
    {
        [AuthenticateResponse]
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
