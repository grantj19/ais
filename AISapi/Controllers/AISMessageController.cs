using AISapi.BA;
using AISapi.Models;
using AISapi.Models.Requests;
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
		public async Task<IActionResult> Get(int messageId)
		{
			(AISMessage message, string error) = await _aisMessageBA.GetAISMessagesByIdAsync(messageId);

			if (message.Id > 0)
				return Ok(message);
			else
				return NoContent();
		}

		[HttpPost]
		public async Task<IActionResult> Insert(AISMessageInsertRequest request)
        {
			(int recordsInserted, string error) = await _aisMessageBA.InsertAISMessagesAsync(request);

			if (string.IsNullOrEmpty(error))
				return Ok(recordsInserted);
			return BadRequest(error);
        }
    }
}

