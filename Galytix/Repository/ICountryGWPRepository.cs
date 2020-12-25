using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Galytix.Model;
using Microsoft.AspNetCore.Mvc;

namespace Galytix.Repository
{
    public interface ICountryGWPRepository
    {
        Task<List<DataRow>> GetGWPDataResponse(string country, string[] lob);
    }
}
