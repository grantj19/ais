using System;
using System.Collections.Generic;
using AISapi.Models;
using AISapi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Threading.Tasks;
using AISapi.DA;
using AISapi.Models.Requests;
using AISapi.DA.Interfaces;

namespace AISTests.ControllerTests;

public class AISMessageControllerTests
{
    private readonly Random random = new();
    private readonly Mock<IAISMessageDA> _daStub = new();

    [Fact]
    public async Task InsertBatch_With5Items_Returns5()
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

        _daStub.Setup(x => x.InsertAISMessagesAsync(items).Result).Returns(response);

        var controller = new AISMessageController(_daStub.Object);

        var result = await controller.InsertBatch(items);
        var resultObj = result as CreatedAtActionResult;

        Assert.Equal(5, resultObj?.Value);
        Assert.Equal(201, resultObj?.StatusCode);
    }

    [Fact]
    public async Task InsertBatch_WithError_ReturnsBadRequest()
    {
        var items = new AISMessageInsertRequest();

        var response = new Tuple<int, string>(new int(), "Some Error");

        _daStub.Setup(x => x.InsertAISMessagesAsync(items).Result).Returns(response);

        var controller = new AISMessageController(_daStub.Object);

        var result = await controller.InsertBatch(items);
        var resultObj = result as BadRequestObjectResult;

        Assert.Equal("Some Error", resultObj?.Value);
        Assert.Equal(400, resultObj?.StatusCode);
    }

    [Fact]
    public async Task Insert_With1Item_Returns1()
    {
        var items = CreateRandomAISMessageRequest();

        var response = new Tuple<int, string>(1, string.Empty);

        _daStub.Setup(x => x.InsertAISMessagesAsync(It.IsAny<AISMessageInsertRequest>()).Result).Returns(response);

        var controller = new AISMessageController(_daStub.Object);

        var result = await controller.Insert(items);
        var resultObj = result as CreatedAtActionResult;

        Assert.Equal(1, resultObj?.Value);
        Assert.Equal(201, resultObj?.StatusCode);
    }

    [Fact]
    public async Task Insert_WithError_Returns0()
    {
        var items = new AISMessageRequest();

        var response = new Tuple<int, string>(new int(), "Some Error");

        _daStub.Setup(x => x.InsertAISMessagesAsync(It.IsAny<AISMessageInsertRequest>()).Result).Returns(response);

        var controller = new AISMessageController(_daStub.Object);

        var result = await controller.Insert(items);
        var resultObj = result as BadRequestObjectResult;

        Assert.Equal(0, resultObj?.Value);
        Assert.Equal(400, resultObj?.StatusCode);
    }

    [Fact]
    public async Task Delete_With5Items_Returns5()
    {

        var response = new Tuple<int, string>(5, string.Empty);

        _daStub.Setup(x => x.DeleteAISMessageAsync(new DateTime()).Result).Returns(response);

        var controller = new AISMessageController(_daStub.Object);

        var result = await controller.Delete();
        var resultObj = result as OkObjectResult;

        Assert.Equal(5, resultObj?.Value);
        Assert.Equal(200, resultObj?.StatusCode);
    }

    [Fact]
    public async Task Delete_WithError_ReturnsBadRequest()
    {
        var response = new Tuple<int, string>(new int(), "Some Error");

        _daStub.Setup(x => x.DeleteAISMessageAsync(new DateTime()).Result).Returns(response);

        var controller = new AISMessageController(_daStub.Object);

        var result = await controller.Delete();
        var resultObj = result as BadRequestObjectResult;

        Assert.Equal("Some Error", resultObj?.Value);
        Assert.Equal(400, resultObj?.StatusCode);
    }

    private AISMessageRequest CreateRandomAISMessageRequest()
    {
        return new AISMessageRequest
        {
            MMSI = random.Next()
        };
    }
}
