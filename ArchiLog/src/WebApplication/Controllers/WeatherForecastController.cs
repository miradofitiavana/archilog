using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APILibrary.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        private object test(object o)
        {
            return o;
        }

        [HttpGet]
        public Object Get(string propertyName)
        {
            var rng = new Random();
            var tab=  Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToList();

            string s = "";
            foreach(var prop in typeof(WeatherForecast).GetProperties())
            {
                var isPresentAttribute = prop.CustomAttributes
                    .Any(x => x.AttributeType == typeof(NotJsonAttribute));
                if (isPresentAttribute)
                    s += " " + prop.Name;
            }

            return s;
            return tab;

            //var test = new List<string> { "Mamadou", "Wassim", "Cylia" };
            //test.Sort();
            //return test;
           
            //tab.Sort();

            /*WeatherForecast w = new WeatherForecast();
            w.Summary = "test";
            w.Date = DateTime.Now;
            w.TemperatureC = 30;*/

            /*   if(propertyName == "Summary")
                   return w.Summary;// w.GetType();
               if (propertyName == "TemperatureC")
                   return w.TemperatureC;// w.GetType();*/


            /*var test2 = typeof(WeatherForecast)
                .GetProperty(propertyName, System.Reflection.BindingFlags.IgnoreCase 
                | System.Reflection.BindingFlags.Instance 
                | System.Reflection.BindingFlags.Public)
                .GetValue(w);
            return test2;*/
        }
    }
}
