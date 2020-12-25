using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Galytix.Model;

namespace Galytix.Services
{
    public interface ICountryGWPService
    {
        Task<Dictionary<string, double>> GetGWPDataResponse(string country, string[] lob);
        
    }
}
