using AISapi.BA;
using AISapi.BA.Interfaces;
using AISapi.Models;
using AISapi.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AISapi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AISMessageController : ControllerBase
	{
		private readonly IAISMessageBA _aisMessageBA;

		public AISMessageController(IAISMessageBA aisMessageBA)
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

		/// <summary> Insert Batch of AIS Messages </summary>
		/// <param name="request">AIS Message Insert Request</param>
		/// <returns>Number of inserted messages</returns>
		/// <response code="201">Returns number of inserted AIS Messages</response>
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost]
		[Route("Batch")]
		public async Task<IActionResult> InsertBatch(AISMessageInsertRequest request)
        {
			(int recordsInserted, string error) = await _aisMessageBA.InsertAISMessagesAsync(request);

			if (string.IsNullOrEmpty(error))
				return CreatedAtAction(nameof(InsertBatch), recordsInserted);
			return BadRequest(error);
        }

		/// <summary> Insert a single AIS Message </summary>
		/// <param name="request">AIS Message Request</param>
		/// <returns>Number of inserted messages</returns>
		/// <response code="201">Returns "1" if insert succeeded</response>
		/// <response code="400">Returns "0" if insert failed</response>
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
				return CreatedAtAction(nameof(Insert), 1);
			return BadRequest(0);
		}
	}
}

