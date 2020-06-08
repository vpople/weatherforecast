using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ForecastWebApi.Services
{
    public interface IMetaWeatherClient
    {
        HttpClient MetaWeatherClientInstance();
    }
}
