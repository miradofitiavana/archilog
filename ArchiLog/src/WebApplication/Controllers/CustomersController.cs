using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APILibrary.Core.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    
    public class CustomersController : ControllerBaseAPI<Customer, EatDbContext>
    {

        public CustomersController(EatDbContext context):base(context)
        {
        }

        
    }
}
