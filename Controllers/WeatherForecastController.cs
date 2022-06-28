using CustomAuthorizeAttribute.Authorization;
using CustomAuthorizeAttribute.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomAuthorizeAttribute.Controllers
{
    [ApiController]
    [Route("weatherforecast")]
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

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var summaries = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            return summaries;
        }

        [AuthorizationCustom]
        [HttpGet("name/{name}", Name = "GetWeatherForecastByName")]
        public string GetByName(string name)
        {
            var summary = Summaries.Where(x => x.ToLower().Equals(name)).FirstOrDefault() ?? string.Empty;
            return summary;
        }
    }
}