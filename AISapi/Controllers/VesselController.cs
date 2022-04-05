using AISapi.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace AISapi.Controllers;

[ApiController]
[Route("[controller]")]
public class VesselController : ControllerBase
{
    private readonly ILogger<VesselController> _logger;
    //private readonly MySqlConnection _connection;

    public VesselController(MySqlConnection mySqlConnection,
        ILogger<VesselController> logger)
    {
        _logger = logger;
        //_connection = mySqlConnection;
    }

    [HttpGet(Name = "GetVesselData")]
    public IEnumerable<Vessel> Get()
    {
        
    }
}

