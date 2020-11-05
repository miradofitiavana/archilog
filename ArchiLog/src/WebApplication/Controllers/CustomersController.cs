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

        /*[HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Customer>>> SearchCustomersAsync()
        {
            var results = await _context.Customers.ToListAsync();
            return results;
        }*/
        /*
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody]Customer item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }*/
    }
}
