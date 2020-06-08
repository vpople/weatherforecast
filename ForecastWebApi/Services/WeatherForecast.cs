using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainEntities = WeatherApp.Domain.Entities;

namespace ForecastWebApi.Services
{
    public class WeatherForecast : IWeatherForecast
    {
        private readonly IMetaWeatherClient _MetaWeatherClient;
        private readonly ILocationRepository _locationRepository;
        public WeatherForecast(IMetaWeatherClient boMetaWeatherClient, ILocationRepository locationRepository)
        {
            _boMetaWeatherClient = boMetaWeatherClient;
            _locationRepository = locationRepository;
        }
        public async Task<List<LocationSearch>> LocationSearch(string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    return null;
                }
                var uri = query.Contains(',') ? "search/?lattlong={0}" : "search/?query={0}";
                var metaWeatherClient = _boMetaWeatherClient.MetaWeatherClientInstance();
                var response = await metaWeatherClient.GetAsync(string.Format(uri, query));
                var apiResponse = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<List<LocationSearch>>(apiResponse);
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Location> Location(string WoeId)
        {
            try
            {
                if (string.IsNullOrEmpty(WoeId))
                {
                    return null;
                }
                var metaWeatherClient = _boMetaWeatherClient.MetaWeatherClientInstance();
                var response = await metaWeatherClient.GetAsync(WoeId);
                var apiResponse = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<Location>(apiResponse);
                UpdateWeatherWarnings(data);
                AddUpdateLocationAudit(WoeId, data);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void AddUpdateLocationAudit(string WoeId, Location location)
        {
            try
            {
                var intWoeId = Convert.ToInt32(WoeId);
                var currentDate = DateTime.Now.Date;
                var locationData = _locationRepository.GetLocations().Where(x => x.WoeId.Equals(intWoeId) && x.SearchDate.Equals(currentDate)).FirstOrDefault();
                if (locationData != null)
                {
                    locationData.SearchCount += 1;
                    _locationRepository.Update(locationData);
                    _locationRepository.Save();
                    return;
                }
                var newLocation = new DomainEntities.Location()
                {
                    WoeId = intWoeId,
                    LocationName = location.Title,
                    SearchDate = DateTime.Now.Date,
                    SearchCount = 1,
                    LastupdatedDateTime = DateTime.Now
                };
                _locationRepository.Insert(newLocation);
                _locationRepository.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateWeatherWarnings(Location location)
        {
            //ideally this should be configurable in appsettings.json
            if (location.ConsolidatedWeather != null)
            {
                location.ConsolidatedWeather.ForEach(x =>
                {
                    if (x.WindSpeed < 60 && x.WindSpeed >= 50)
                        x.Warning = "Yellow Warning";
                    else if (x.WindSpeed < 70 && x.WindSpeed >= 60)
                        x.Warning = "Amber Warning";
                    else if (x.WindSpeed >= 70)
                        x.Warning = "Red Warning";
                    else
                        x.Warning = "No Warning";
                });
            }
        }
    }
}
