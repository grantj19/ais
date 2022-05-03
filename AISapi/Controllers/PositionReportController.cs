using System;
using AISapi.DA;
using AISapi.DA.Interfaces;
using AISapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AISapi.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PositionReportController : ControllerBase
	{

		private readonly IPositionReportDA _positionReportDA;

		public PositionReportController(IPositionReportDA positionReportBA)
        {
			_positionReportDA = positionReportBA;
        }

		/// <summary> Get all most recent ship positions </summary>
		/// <returns>Array of ship documents</returns>
		/// <response code="200">
        /// Returns number of inserted AIS Messages
        ///
        /// Sample response:
        ///
        ///		{
        ///			"MMSI": 0,
        ///			"Latitude": 0.0,
        ///			"Longitude": 0.0,
        ///			"IMO": 0
        ///		}
        ///
        /// Note: Properties are dropped from the response if they have null values
        /// </response>
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpGet]
		public async Task<IActionResult> Get()
        {
			(List<PositionReport> reports, string error) = await _positionReportDA.GetRecentPositionsAsync();

			if (string.IsNullOrEmpty(error))
				return Ok(reports);
			return BadRequest(error);
		}

		/// <summary> Get all most recent ship positions by MMSI</summary>
		/// <returns>A single ship document</returns>
		/// <response code="200">
		/// Returns number of inserted AIS Messages
		///
		/// Sample response:
		///
		///		{
		///			"MMSI": 0,
		///			"Latitude": 0.0,
		///			"Longitude": 0.0,
		///			"IMO": 0
		///		}
		///
		/// Note: Properties are dropped from the response if they have null values
		/// </response>
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpGet]
		[Route("GetByMMSI")]
		public async Task<IActionResult> GetByMMSI(int MMSI)
		{
			(PositionReport report, string error) = await _positionReportDA.GetPositionByMMSIAsync(MMSI);

			if (string.IsNullOrEmpty(error))
				return Ok(report);
			return BadRequest(error);
		}

	}
}

