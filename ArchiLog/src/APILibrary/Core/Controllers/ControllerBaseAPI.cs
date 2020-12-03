using APILibrary.core.Attributes;
using APILibrary.core.Helpers;
using APILibrary.core.Models;
using APILibrary.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ControllerBaseAPI<TModel, TContext> : ControllerBase where TModel : ModelBase where TContext : DbContext
    {
        public TContext _context;

        private const string LinkHeaderTemplate = "{0}; rel=\"{1}\"";
        public ControllerBaseAPI(TContext context)
        {
            this._context = context;
        }

        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("search")]
        public virtual async Task<ActionResult<IEnumerable<dynamic>>> SearchAsync([FromQuery] string fields, [FromQuery] string range, [FromQuery] string asc, [FromQuery] string desc)
        {
            IQueryCollection requestQuery = Request.Query;

            var query = _context.Set<TModel>().AsQueryable();

            // TRIS
            if (!string.IsNullOrWhiteSpace(asc) || !string.IsNullOrWhiteSpace(desc))
            {
                query = query.OrderByDynamic(asc, desc);
            }

            // FILTRES
            if (requestQuery != null && requestQuery.Count() > 0)
            {
                query = query.WhereDynamic(requestQuery, true);
            }

            // PAGINATION
            if (!string.IsNullOrWhiteSpace(range))
            {
                var tab = range.Split('-');
                query = query.TakePageResult(Int32.Parse(tab[0]), Int32.Parse(tab[1]));
            }

            // RENDU PARTIEL
            if (!string.IsNullOrWhiteSpace(fields))
            {
                var tab = fields.Split(',');
                query = query.SelectModel(tab);
                var results = await query.ToListAsync();
                return Ok(results.Select((x) => IQueryableExtensions.SelectObject(x, tab)).ToList());
            }
            else
            {
                return Ok(ToJsonList(await query.ToListAsync()));
            }
        }

        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<dynamic>>> GetAllAsync([FromQuery] string fields, [FromQuery] string range, [FromQuery] string asc, [FromQuery] string desc)
        {
            IQueryCollection requestQuery = Request.Query;

            var query = _context.Set<TModel>().AsQueryable();

            // TRIS
            if (!string.IsNullOrWhiteSpace(asc) || !string.IsNullOrWhiteSpace(desc))
            {
                query = query.OrderByDynamic(asc, desc);
            }

            // FILTRES
            if (requestQuery != null && requestQuery.Count() > 0)
            {
                query = query.WhereDynamic(requestQuery);
            }

            // PAGINATION
            if (!string.IsNullOrWhiteSpace(range))
            {
                var tab = range.Split('-');

                int total = query.Count();

                query = query.TakePageResult(Int32.Parse(tab[0]), Int32.Parse(tab[1]));

                var linkBuilder = new PageLinkBuilder(Url, "", null, Int32.Parse(tab[0]), Int32.Parse(tab[1]), total);

                List<string> links = new List<string>();
                if (linkBuilder.FirstPage != null)
                    links.Add(string.Format(LinkHeaderTemplate, linkBuilder.FirstPage, "first"));
                if (linkBuilder.PreviousPage != null)
                    links.Add(string.Format(LinkHeaderTemplate, linkBuilder.PreviousPage, "prev"));
                if (linkBuilder.NextPage != null)
                    links.Add(string.Format(LinkHeaderTemplate, linkBuilder.NextPage, "next"));
                if (linkBuilder.LastPage != null)
                    links.Add(string.Format(LinkHeaderTemplate, linkBuilder.LastPage, "last"));
                Response.Headers.Add("Link", string.Join(", ", links));

                Response.Headers.Add("Content-Range", linkBuilder.GetContentRange());
                Response.Headers.Add("Accept-Ranges", typeof(TModel).Name + " " + total);
            }

            // RENDU PARTIEL
            if (!string.IsNullOrWhiteSpace(fields))
            {
                var tab = fields.Split(',');
                query = query.SelectModel(tab);
                var results = await query.ToListAsync();
                return Ok(results.Select((x) => IQueryableExtensions.SelectObject(x, tab)).ToList());
            }
            else
            {
                return Ok(ToJsonList(await query.ToListAsync()));
            }
        }

        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TModel>> GetById([FromRoute] int id, [FromQuery] string fields)
        {
            var query = _context.Set<TModel>().AsQueryable();
            //solution 2: optimisation de la requete SQL

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var tab = new List<string>(fields.Split(','));
                if (!tab.Contains("id"))
                    tab.Add("id");
                var result = query.SelectModel(tab.ToArray()).SingleOrDefault(x => x.ID == id);
                if (result != null)
                {
                    var tabFields = fields.Split(',');
                    return Ok(IQueryableExtensions.SelectObject(result, tabFields));
                }
                else
                {
                    return NotFound(new { Message = $"ID {id} not found" });
                }
            }
            else
            {
                var result = query.SingleOrDefault(x => x.ID == id);
                if (result != null)
                {
                    return Ok(ToJson(result));
                }
                else
                {
                    return NotFound(new { Message = $"ID {id} not found" });
                }
            }
        }

        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPost]
        public virtual async Task<ActionResult<TModel>> CreateItem([FromBody] TModel item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return Created("", ToJson(item));
            }
            else
            {
                //   ModelState:  {clé: nom du champ // valeur : ce qui ne va pas sur le champ}
                return BadRequest(ModelState);
            }
        }

        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPut("{id}")]
        public async Task<ActionResult<TModel>> UpdateItem([FromRoute] int id, [FromBody] TModel item)
        {
            if (id != item.ID)
            {
                return BadRequest(ModelState);
            }

            bool result = await _context.Set<TModel>().AnyAsync(x => x.ID == id);
            if (!result)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update<TModel>(item);
                    await _context.SaveChangesAsync();
                    return Ok(ToJson(item));
                }
                catch (Exception e)
                {
                    return BadRequest(new { e.Message });
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<TModel>> RemoveItem([FromRoute] int id)
        {
            TModel item = await _context.Set<TModel>().FindAsync(id);
            if (item == null)
                return NotFound();
            _context.Remove<TModel>(item);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(ToJsonList(await _context.Set<TModel>().ToListAsync()));
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        protected IEnumerable<dynamic> ToJsonList(IEnumerable<object> tab)
        {
            var tabNew = tab.Select((x) => ToJson(x));
            return tabNew;
        }

        protected dynamic ToJson(object item)
        {
            var expandoDict = new ExpandoObject() as IDictionary<string, object>;
            var collectionType = typeof(TModel);
            IDictionary<string, object> dico = item as IDictionary<string, object>;

            if (dico != null)
            {
                foreach (var prop in dico)
                {
                    var propInTModel = collectionType.GetProperty(prop.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

                    var isPresentAttribute = propInTModel.CustomAttributes.
                        Any(x => x.AttributeType == typeof(NotJsonAttribute));
                    if (!isPresentAttribute)
                        expandoDict.Add(prop.Key, prop.Value);
                }
            }
            else
            {
                foreach (var prop in collectionType.GetProperties())
                {
                    var isPresentAttribute = prop.CustomAttributes.
                       Any(x => x.AttributeType == typeof(NotJsonAttribute));
                    if (!isPresentAttribute)
                        expandoDict.Add(prop.Name, prop.GetValue(item));
                }
            }
            return expandoDict;
        }
    }
}
