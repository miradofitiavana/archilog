using APILibrary.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace APILibrary.Core.Controllers
{
    
    public abstract class ControllerBaseAPI<T> : ControllerBase //where T: new()
    {

        public IEnumerable<dynamic> ToJsonList(IEnumerable<T> tab)
        {
            var tabNew = tab.Select((x) => {
                var expo = new ExpandoObject() as IDictionary<string, object>;
                //var collectionType = tab.GetType().GenericTypeArguments[0];
                var collectionType = typeof(T);
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

        public dynamic ToJson(T item)
        {
            var expo = new ExpandoObject() as IDictionary<string, object>;
            foreach (var prop in typeof(T).GetProperties())
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
