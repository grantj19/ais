using AISapi.BA;
using AISapi.BA.Interfaces;
using AISapi.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AISapi.Controllers;

[ApiController]
[Route("[controller]")]
public class VesselController : ControllerBase
{
    private readonly ILogger<VesselController> _logger;
    private readonly IVesselBA _vesselBA;

    public VesselController(IVesselBA vesselBA,
        ILogger<VesselController> logger)
    {
        _logger = logger;
        _vesselBA = vesselBA;
    }

    [HttpGet(Name = "GetVesselData")]
    public async Task<IActionResult> Get()
    {
        (List<Vessel> vessels, string error) = await _vesselBA.GetVesselsAsync();

        if (vessels.Any())
            return Ok(vessels);
        else
            return NotFound(error);
    }
}

