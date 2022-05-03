using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AISapi.Controllers;
using AISapi.DA.Interfaces;
using AISapi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AISTests.ControllerTests
{
	public class PositionReportControllerTests
    {
        private readonly Random random = new();
		private readonly Mock<IPositionReportDA> _daStub = new();

        // Test GET /PositionReport with success
        [Fact]
        public async Task GetPositionReports_WithNoError_ReturnsOK()
        {
            var items = new List<PositionReport> { RandomPositionReport(), RandomPositionReport(), RandomPositionReport() };

            var response = new Tuple<List<PositionReport>, string>(items, string.Empty);

            _daStub.Setup(x => x.GetRecentPositionsAsync().Result).Returns(response);

            var controller = new PositionReportController(_daStub.Object);

            var result = await controller.Get();
            var resultObj = result as OkObjectResult;

            Assert.Equal(items, resultObj?.Value);
            Assert.Equal(200, resultObj?.StatusCode);
        }

        // Test GET /PositionReport with error
        [Fact]
        public async Task GetPositionReports_WithError_ReturnsBadRequest()
        {

            var response = new Tuple<List<PositionReport>, string>(new List<PositionReport>(), "Some Error");

            _daStub.Setup(x => x.GetRecentPositionsAsync().Result).Returns(response);

            var controller = new PositionReportController(_daStub.Object);

            var result = await controller.Get();
            var resultObj = result as BadRequestObjectResult;

            Assert.Equal("Some Error", resultObj?.Value);
            Assert.Equal(400, resultObj?.StatusCode);
        }

        // Test GET /PositionReport/GetByMMSI with success
        // This test is giving a null reference error, which is very confusing
        // since the version of this test that returns an error is passing
        [Fact]
        public async Task GetPositionReportsByMMSI_WithNoError_ReturnsOK()
        {
            var item = RandomPositionReport();

            var response = new Tuple<PositionReport, string>(item, string.Empty);

            _daStub.Setup(x => x.GetPositionByMMSIAsync(item.MMSI).Result).Returns(response);

            var controller = new PositionReportController(_daStub.Object);

            var result = await controller.Get();
            var resultObj = result as OkObjectResult;

            Assert.Equal(item, resultObj?.Value);
            Assert.Equal(200, resultObj?.StatusCode);
        }

        // Test GET /PositionReport/GetByMMSI with error
        [Fact]
        public async Task GetPositionReportsByMMSI_WithError_ReturnsBadRequest()
        {
            var response = new Tuple<PositionReport, string>(new PositionReport(), "Some Error");

            _daStub.Setup(x => x.GetPositionByMMSIAsync(It.IsAny<int>()).Result).Returns(response);

            var controller = new PositionReportController(_daStub.Object);

            var result = await controller.GetByMMSI(It.IsAny<int>());
            var resultObj = result as BadRequestObjectResult;

            Assert.Equal("Some Error", resultObj?.Value);
            Assert.Equal(400, resultObj?.StatusCode);
        }

        private PositionReport RandomPositionReport()
        {
            return new PositionReport()
            {
                MMSI = random.Next()
            };
        }
    }
}

