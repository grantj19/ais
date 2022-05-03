using System;
using AISapi.Models;
using AISapi.Models.Requests;

namespace AISapi.DA.Interfaces
{
	public interface IAISMessageDA
	{
		Task<Tuple<int, string>> InsertAISMessagesAsync(AISMessageInsertRequest request);
		Task<Tuple<int, string>> DeleteAISMessageAsync(DateTime timestamp = default);
	}
}

