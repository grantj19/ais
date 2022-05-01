using System;
using System.Collections.Generic;
using AISapi.Models;
using AISapi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Threading.Tasks;
using AISapi.BA;
using AISapi.Models.Requests;
using AISapi.BA.Interfaces;

namespace AISTests.ControllerTests;

public class AISMessageControllerTests
{
    private readonly Random random = new();
    private readonly Mock<IAISMessageBA> _baStub = new();

    [Fact]
    public async Task Insert_With5Items_Returns5()
    {
        var items = new AISMessageInsertRequest
        {
            AISMessages = new List<AISMessageRequest>
            {
                CreateRandomAISMessageRequest(),
                CreateRandomAISMessageRequest(),
                CreateRandomAISMessageRequest(),
                CreateRandomAISMessageRequest(),
                CreateRandomAISMessageRequest()
            }
        };

        var response = new Tuple<int, string>(5, string.Empty);

        _baStub.Setup(x => x.InsertAISMessagesAsync(items).Result).Returns(response);

        var controller = new AISMessageController(_baStub.Object);

        var result = await controller.InsertBatch(items);
        var resultObj = result as OkObjectResult;

        Assert.Equal(5, resultObj?.Value);
        Assert.Equal(200, resultObj?.StatusCode);
    }

    [Fact]
    public async Task Get_WithNoItems_ReturnsNotFound()
    {
        Mock<VesselBA> baStub = new();
        var items = new Tuple<List<Vessel>, string>(new List<Vessel>(), string.Empty);

        baStub.Setup(v => v.GetVesselsAsync()).ReturnsAsync(items);

        var controller = new VesselController(baStub.Object);

        var result = await controller.Get();
        var notFoundResult = result as NotFoundObjectResult;

        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    private AISMessageRequest CreateRandomAISMessageRequest()
    {
        return new AISMessageRequest
        {
            MMSI = random.Next()
        };
    }
}
