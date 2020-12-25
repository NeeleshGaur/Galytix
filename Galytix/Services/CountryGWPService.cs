using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Galytix.Model;
using Galytix.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Galytix.Services
{
    public class CountryGWPService : ICountryGWPService
    {
        readonly ICountryGWPRepository gwpRepository;
        readonly ILogger<CountryGWPService> logger;
        readonly MemoryCacheEntryOptions cacheEntryOptions;


        public IMemoryCache cache { get; private set; }

        public CountryGWPService(ICountryGWPRepository gwpRepository, ILogger<CountryGWPService> logger, IMemoryCache memoryCache)
        {
            this.gwpRepository = gwpRepository;
            this.logger = logger;
            this.cache = memoryCache;
            cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
        }

        public async Task<Dictionary<string, double>> GetGWPDataResponse(string country, string[] lobs)
        {
            Dictionary<string, double> cachedData;
            Array.Sort(lobs);

            StringBuilder sb = new StringBuilder();
            string key = "";

            for (int i = 0; i < lobs.Length; i++)
            {
                logger.LogInformation("Addng to sb " + lobs[i]);
                sb.Append(lobs[i]);
            }

            logger.LogInformation("String Builder " + sb.ToString());
            key = sb.ToString() + country;

            if (cache.TryGetValue(key, out cachedData))
            {
                logger.LogInformation("Returning data from cache");
                return cachedData;
            }
            return await GetDataFromDB(country, lobs, key);
        }

        private async Task<Dictionary<string, double>> GetDataFromDB(string country, string[] lobs, string key)
        {
            List<DataRow> dataRows = await gwpRepository.GetGWPDataResponse(country, lobs);
            Dictionary<string, double> res = new Dictionary<string, double>();
            foreach (DataRow dataRow in dataRows)
            {
                string lob = dataRow[3].ToString();
                double sum = 0;
                int counter = 0;
                for (int i = 0; i < 8; i++)
                {
                    double d = 0;
                    if (Double.TryParse(dataRow[12 + i].ToString(), out d))
                    {
                        sum += d;
                        counter++;
                        logger.LogInformation("Mi dictionary values " + sum + " " + counter);
                    }

                }
                logger.LogInformation("After For " + sum + " " + counter);
                res[lob] = (sum / counter);


            }
            cache.Set(key, res);
            logger.LogInformation("Returning data from database");
            return res;
        }
    }
}
