﻿using AISapi.BA;
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
		public async Task<IActionResult> Get(int messageId)
		{
			(AISMessage message, string error) = await _aisMessageBA.GetAISMessagesByIdAsync(messageId);

			if (message is not null)
				return Ok(message);
			else
				return NotFound(error);
		}
    }
}

