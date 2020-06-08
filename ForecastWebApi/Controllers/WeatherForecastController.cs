using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ForecastWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ForecastWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        //private readonly ILogger<WeatherForecastController> _logger;
        private readonly forcastContext _dbContext;

        //public WeatherForecastController(ILogger<WeatherForecastController> logger)
        //{
        //    _logger = logger;
        //}

        public WeatherForecastController(forcastContext dbContext)
        {
            _dbContext = dbContext;
        }



        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            Console.WriteLine("Response sent");
            var result = GetLocationDetails("london");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        //[HttpGet("{location}")]
        [HttpGet]
        [Route("GetLocationDetails/{location}")]
        public async Task<string> GetLocationDetails(string location)
        {
            var resLocationDetails = await GetLocation(location);

            //Add entry in Audit log table
            UpdateAuditlog(location);
            // Pass the data into the web app
            return (string)resLocationDetails;
        }


        [HttpGet]
        [Route("GetLocationForecast/{woeid}")]
        public async Task<string> GetLocationForecast(string woeid)
        {
            //GetWeatherForecasts
            var resLocationForecast = await GetWeatherForecast(woeid);
            // Pass the data into the web app
            return (string)resLocationForecast;
        }

        private async Task<string> GetLocation(string location)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("https://www.metaweather.com");
                    var response = await client.GetAsync($"api/location/search/?query={location}");
                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsStringAsync();
                    //return JsonConvert.DeserializeObject<WeatherForecast>(result);
                    return result;                    
                }
                catch (HttpRequestException httpRequestException)
                {
                    Console.WriteLine($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                }
                return null;
            }
        }

        private async Task<string> GetWeatherForecast(string woeid)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("https://www.metaweather.com");
                    var response = await client.GetAsync($"api/location/" + woeid);
                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsStringAsync();
                    //return JsonConvert.DeserializeObject<WeatherForecast>(result);
                    return result;                                        
                }
                catch (HttpRequestException httpRequestException)
                {
                    Console.WriteLine($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                }
                return null;
            }
        }

        private void UpdateAuditlog(string location)
        {
            var newAuditlog = new AuditLog();
            newAuditlog.SearchName = location;
            newAuditlog.SearchTime = DateTime.Now.ToString();

            _dbContext.AuditLog.Add(newAuditlog);
            _dbContext.SaveChanges();
        }
    }
}
