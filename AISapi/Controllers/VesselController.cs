using AISapi.BA.Interfaces;
using AISapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AISapi.Controllers;

[ApiController]
[Route("[controller]")]
public class VesselController : ControllerBase
{
    private readonly IVesselBA _vesselBA;

    public VesselController(IVesselBA vesselBA)
    {
        _vesselBA = vesselBA;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        (List<Vessel> vessels, string error) = await _vesselBA.GetVesselsAsync();

        if (vessels.Any())
            return Ok(vessels);
        else
            return NotFound(error);
    }
}

