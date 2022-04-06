using System;
using System.Collections.Generic;
using AISapi.BA.Interfaces;
using AISapi.Models;
using AISapi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace AISTests;

public class VesselControllerTests
{
    private readonly Random random = new();

    [Fact]
    public async Task Get_WithAnyItem_ReturnsOK()
    {
        Mock<IVesselBA> baStub = new();
        var items = new Tuple<List<Vessel>, string>(new List<Vessel>()
        {
            CreateRandomVessel(),
            CreateRandomVessel(),
            CreateRandomVessel()
        }, string.Empty);

        baStub.Setup(v => v.GetVesselsAsync()).ReturnsAsync(items);

        var controller = new VesselController(baStub.Object);

        var result = await controller.Get();
        var okResult = result as OkObjectResult;

        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Get_WithNoItems_ReturnsNotFound()
    {
        Mock<IVesselBA> baStub = new();
        var items = new Tuple<List<Vessel>, string>(new List<Vessel>(), string.Empty);

        baStub.Setup(v => v.GetVesselsAsync()).ReturnsAsync(items);

        var controller = new VesselController(baStub.Object);

        var result = await controller.Get();
        var notFoundResult = result as NotFoundObjectResult;

        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    private Vessel CreateRandomVessel()
    {
        return new Vessel
        {
            IMO = random.Next()
        };
    }
}
