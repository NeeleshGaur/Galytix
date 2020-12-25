using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Galytix.Model;
using Galytix.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Galytix.Controllers
{
    [ApiController]
    [Route("server/api/gwp")]
    
    public class CountryGWPController : ControllerBase
    {
        
        public CountryGWPController(ILogger<CountryGWPController> logger, ICountryGWPService gwpService)
        {
            this.gwpService = gwpService;
            this.logger = logger;
            
        }

        private readonly ICountryGWPService gwpService;
        private readonly ILogger<CountryGWPController> logger;

        

        [HttpPost]
        [Route("avg")]
        public async Task<IActionResult> Post([FromBody] GWPModel gwpModel)
        {
            try
            {
                if (gwpModel == null) {
                    return BadRequest("Data is in incorect format");
                }
                logger.LogInformation(String.Format("Calculating response for Country: {0} and LOB: {1}", gwpModel.Country,gwpModel.LOB));
                Dictionary<string,double> res = await gwpService.GetGWPDataResponse(gwpModel.Country, gwpModel.LOB.Split(new char[] { ',' }));
                if (res == null) {
                    return NotFound(String.Format("Data not found for reuest with  Country: {0} and LOB: {1}", gwpModel.Country, gwpModel.LOB));
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.LogError(String.Format("Error occured with message {0}. Stack Trace {1}", ex.Message, ex.StackTrace));
                return StatusCode(500, String.Format("Unable to process the request. \nException : {0}", ex.Message));
            }
        }

        
    }
}
