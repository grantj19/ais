using System;
using AISapi.BA;
using AISapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AISapi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PositionReportController : ControllerBase
	{

		private readonly PositionReportBA _positionReportBA;

		public PositionReportController(PositionReportBA positionReportBA)
        {
			_positionReportBA = positionReportBA;
        }

		[HttpGet]
		public async Task<IActionResult> Get()
        {
			(List<PositionReport> reports, string error) = await _positionReportBA.GetRecentPositionsAsync();

			if (string.IsNullOrEmpty(error))
				return Ok(reports);
			return BadRequest(error);
		}

		[HttpGet]
		[Route("GetByMMSI")]
		public async Task<IActionResult> GetByMMSI(int MMSI)
		{
			(PositionReport report, string error) = await _positionReportBA.GetPositionByMMSIAsync(MMSI);

			if (string.IsNullOrEmpty(error))
				return Ok(report);
			return BadRequest(error);
		}

	}
}

