using APILibrary.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ControllerBaseAPI<TModel, TContext> : ControllerBase where TModel : class where TContext : DbContext
    {
        protected readonly TContext _context;

        public ControllerBaseAPI(TContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TModel>>> GetAllAsync()
        {
            var results = await _context.Set<TModel>().ToListAsync();
            return results;
        }

        [HttpPost]
        public async Task<ActionResult<TModel>> CreateItem([FromBody] TModel item)
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
        }


        protected IEnumerable<dynamic> ToJsonList(IEnumerable<TModel> tab)
        {
            var tabNew = tab.Select((x) =>
            {
                var expo = new ExpandoObject() as IDictionary<string, object>;
                //var collectionType = tab.GetType().GenericTypeArguments[0];
                var collectionType = typeof(TModel);
                foreach (var prop in collectionType.GetProperties())
                {
                    var isPresentAttribute = prop.CustomAttributes
                    .Any(x => x.AttributeType == typeof(NotJsonAttribute));
                    if (!isPresentAttribute)
                        expo.Add(prop.Name, prop.GetValue(x));
                }

                return expo;
            });
            return tabNew;
        }

        protected dynamic ToJson(TModel item)
        {
            var expo = new ExpandoObject() as IDictionary<string, object>;
            foreach (var prop in typeof(TModel).GetProperties())
            {
                var isPresentAttribute = prop.CustomAttributes
                .Any(x => x.AttributeType == typeof(NotJsonAttribute));
                if (!isPresentAttribute)
                    expo.Add(prop.Name, prop.GetValue(item));
            }
            return expo;
        }
    }
}
