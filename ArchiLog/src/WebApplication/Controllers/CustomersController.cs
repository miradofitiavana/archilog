using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APILibrary.core.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    //  [Route("api/[controller]")]
    //[ApiController]
    public class CustomersController : ControllerBaseAPI<Customer, EatDbContext>
    {
        //  private readonly EatDbContext _context;

        public CustomersController(EatDbContext context) : base(context)
        {
            //this._context = context;
        }

        /*    //async : executé en parallèle avec d'autres methodes asynchrones
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Customer>>> GetAllAsync()
            {
                //await ne peut être utilisé que dans une methode async
                var results = await _context.Customers.ToListAsync();
                return results;
            }

            [HttpPost]
            public async Task<ActionResult<Customer>> CreateCustomer([FromBody]Customer item)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(item);
                    await _context.SaveChangesAsync();
                    return item;
                } else
                {
                 //   ModelState: retourne {clé: nom du champ // valeur : ce qui ne va pas sur le champ}
                    return BadRequest(ModelState);
                }
            }
        */
    }
}
