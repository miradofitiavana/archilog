using APILibrary.Core.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;

namespace WebApplication
{
    public class WeatherForecast : IComparable<WeatherForecast>
    {
      
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [NotJson]
        public string Summary { get; set; }

        public int CompareTo([AllowNull] WeatherForecast other)
        {
            return 0;
        }
    }
}
