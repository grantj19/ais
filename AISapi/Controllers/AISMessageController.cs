using AISapi.BA;
using AISapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AISapi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AISMessageController : ControllerBase
	{
		private readonly AISMessageBA _aisMessageBA;

		public AISMessageController(AISMessageBA aisMessageBA)
		{
			_aisMessageBA = aisMessageBA;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			(List<AISMessage> vessels, string error) = await _aisMessageBA.GetAISMessagesAsync();

			if (vessels.Any())
				return Ok(vessels);
			else
				return NotFound(error);
		}
    }
}

