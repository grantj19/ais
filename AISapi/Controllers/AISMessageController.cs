using AISapi.DA;
using AISapi.DA.Interfaces;
using AISapi.Models;
using AISapi.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AISapi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AISMessageController : ControllerBase
	{
		private readonly IAISMessageDA _aisMessageDA;

		public AISMessageController(IAISMessageDA aisMessageDA)
		{
			_aisMessageDA = aisMessageDA;
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
			(int recordsInserted, string error) = await _aisMessageDA.InsertAISMessagesAsync(request);

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
			(int recordsInserted , string error) = await _aisMessageDA.InsertAISMessagesAsync(new AISMessageInsertRequest
			{
				AISMessages = new List<AISMessageRequest>
                {
					request
                }
			});

			if (string.IsNullOrEmpty(error))
				return CreatedAtAction(nameof(Insert), recordsInserted);
			return BadRequest(0);
		}

		/// <summary> Delete AIS Messages older than 5 minutes </summary>
		/// <returns>Number of deleted messages</returns>
		/// <response code="204">Returns number of deleted AIS Messages</response>
		[HttpDelete]
		public async Task<IActionResult> Delete(DateTime timestamp = default)
		{
			(int recordsDeleted, string error) = await _aisMessageDA.DeleteAISMessageAsync(timestamp);

			if (string.IsNullOrEmpty(error))
				return Ok(recordsDeleted);
			return BadRequest(error);
		}
	}
}

