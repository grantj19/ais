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

			if (string.IsNullOrEmpty(error))
				return Ok(message);
			return BadRequest(error);
		}

		[HttpPost]
		[Route("Batch")]
		public async Task<IActionResult> InsertBatch(AISMessageInsertRequest request)
        {
			(int recordsInserted, string error) = await _aisMessageBA.InsertAISMessagesAsync(request);

			if (string.IsNullOrEmpty(error))
				return Ok(recordsInserted);
			return BadRequest(error);
        }

		[HttpPost]
		public async Task<IActionResult> Insert(AISMessageRequest request)
		{
			(_ , string error) = await _aisMessageBA.InsertAISMessagesAsync(new AISMessageInsertRequest
			{
				AISMessages = new List<AISMessageRequest>
                {
					request
                }
			});

			if (string.IsNullOrEmpty(error))
				return Ok(1);
			return BadRequest(0);
		}
	}
}

