using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ForecastWebApi.Services
{
    public interface IWeatherForecast
    {
        ////public Task<List<LocationSearch>> LocationSearch(string query);
        public Task<Location> Location(string WoeId);
    }
}
