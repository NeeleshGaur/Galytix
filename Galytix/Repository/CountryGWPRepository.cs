using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Galytix.Model;
using Microsoft.Extensions.Logging;

namespace Galytix.Repository
{
    public class CountryGWPRepository : ICountryGWPRepository
    {
        private readonly ILogger<CountryGWPRepository> logger;
        
        DataTable gwpData;
        public CountryGWPRepository(ILogger<CountryGWPRepository> logger)
        {
            this.logger = logger;
            gwpData = new DataTable();
            PopulateDataInMemory();
        }

        public Task<List<DataRow>> GetGWPDataResponse(string country, string[] lobs)
        {
            List<DataRow> result = new List<DataRow>();
            Dictionary<string, double> res = new Dictionary<string, double>();
            var dataRows = gwpData.AsEnumerable().Where(r => r.Field<string>("country").ToString() == country);

            foreach (DataRow dataRow in dataRows)
            {
                string lob = dataRow[3].ToString();
                if (lobs.Contains(lob))
                {
                    result.Add(dataRow);
                }
            }

            return Task.FromResult(result);
            
        }

        private void PopulateDataInMemory()
        {
            StreamReader sr = new StreamReader(@"gwpByCountry.csv");
            string headerr = sr.ReadLine();
            headerr = sr.ReadLine();

            string[] headers = headerr.Split(',');
            foreach (string header in headers)
            {
                gwpData.Columns.Add(header);
            }
            while (!sr.EndOfStream)
            {
                string str = sr.ReadLine();
                string[] rows = Regex.Split(str, ",");
                DataRow dr = gwpData.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }
                gwpData.Rows.Add(dr);

            }
        }
    }
}
