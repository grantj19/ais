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
		/// <returns>
        ///	Number of inserted messages
        /// </returns>
        
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
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

