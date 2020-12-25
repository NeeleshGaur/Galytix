using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Galytix.Controllers;
using Galytix.Model;
using Galytix.Repository;
using Galytix.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GalytixTest
{
    public class Tests
    {
        ILogger<CountryGWPController> logger { get;  set; }
        Mock<ICountryGWPService> gwpService;
        Dictionary<string, double> gwpData = new Dictionary<string, double>();

        [SetUp]
        public void Setup()
        {
            
            this.gwpService = new Mock<ICountryGWPService>();
            var mock = new Mock<ILogger<CountryGWPController>>();
            this.logger = mock.Object;
            gwpData["transport"] = 285137382.4714286;
            gwpData["freight"] = 267493063.6142857;
            gwpService.Setup(r => r.GetGWPDataResponse(It.IsAny<string>(), It.IsAny<string[]>())).ReturnsAsync(gwpData).Verifiable();

        }

        [Test]
        public void GetGWPDataPositiveTest ()
        {
            GWPModel gWPModel = new GWPModel();
            gWPModel.Country = "ae";
            gWPModel.LOB = "transport,freight";
            CountryGWPController ec = new CountryGWPController(logger, gwpService.Object);
            var res = ec.Post(gWPModel).Result as OkObjectResult;
            gwpService.Verify();
            Assert.AreEqual(res.StatusCode.Value, StatusCodes.Status200OK);
        }

        [Test]
        public void GetGWPDataNegativeTest1()
        {
            CountryGWPController ec = new CountryGWPController(logger, gwpService.Object);
            var res = ec.Post(null).Result as BadRequestObjectResult;
            Assert.AreEqual(res.StatusCode, StatusCodes.Status400BadRequest);
        }
/*        
        [Test]
        public void GetGWPDataNegativeTest2()
        {
            gwpService.Setup(r => r.GetGWPDataResponse(It.IsAny<string>(), It.IsAny<string[]>())).Returns(Task.FromResult( Enumerable.Empty<Dictionary<string,double>>()));
            CountryGWPController ec = new CountryGWPController(logger, gwpService.Object);
            var res = ec.Post(null).Result as BadRequestObjectResult;
            Assert.AreEqual(res.StatusCode, StatusCodes.Status400BadRequest);
        }
*/
    }
}