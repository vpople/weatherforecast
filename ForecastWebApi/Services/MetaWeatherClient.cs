using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ForecastWebApi.Services
{
    public class MetaWeatherClient : IMetaWeatherClient
    {
        private HttpClient _instance;
        public HttpClient MetaWeatherClientInstance()
        {
            _instance = new HttpClient();
            _instance.BaseAddress = new Uri("https://www.metaweather.com/api/location/");
            _instance.DefaultRequestHeaders.Clear();
            _instance.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return _instance;
        }
    }
}
