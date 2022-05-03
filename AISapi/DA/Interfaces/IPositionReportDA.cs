using System;
using AISapi.Models;

namespace AISapi.DA.Interfaces
{
	public interface IPositionReportDA
	{
		Task<Tuple<List<PositionReport>, string>> GetRecentPositionsAsync();
		Task<Tuple<PositionReport, string>> GetPositionByMMSIAsync(int? MMSI);
	}
}

