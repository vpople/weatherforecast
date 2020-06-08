using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
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

                    /*
                    // Call *metaweather*, and display its response in the page
                    var request = new System.Net.Http.HttpRequestMessage();
                    request.RequestUri = new Uri("https://www.metaweather.com/api/location/search/?query=" + location);
                    var result = await client.GetAsync(request.ToString());
                    if (result.IsSuccessStatusCode)
                    {
                        // Read all of the response and deserialise it into an instace of
                        // WeatherForecast class
                        var content = await result.Content.ReadAsStringAsync();
                        var locationDtls = JsonConvert.DeserializeObject<WeatherForecast>(content);

                        if (content.Length > 0)
                        {
                            request.RequestUri = new Uri("https://www.metaweather.com/api/location/" + locationDtls.Woeid);
                            var responseforecast = await client.GetAsync(request.ToString());
                            if (result.IsSuccessStatusCode)
                            {
                                var forecastcontent = await result.Content.ReadAsStringAsync();
                                return JsonConvert.DeserializeObject<ConsolidatedWeather>(content);
                            }
                        }
                    } 
                    */
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


    }
}
