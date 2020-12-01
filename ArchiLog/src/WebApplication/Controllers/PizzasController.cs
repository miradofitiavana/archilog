using APILibrary.core.Attributes;
using APILibrary.core.Controllers;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Authorize]
    public class PizzasController : ControllerBaseAPI<Pizza, EatDbContext>
    {
        public PizzasController(EatDbContext context) : base(context)
        {
        }

       
    }
}
