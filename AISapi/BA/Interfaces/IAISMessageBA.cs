using System;
using AISapi.Models;
using AISapi.Models.Requests;

namespace AISapi.BA.Interfaces
{
	public interface IAISMessageBA
	{
		Task<Tuple<int, string>> InsertAISMessagesAsync(AISMessageInsertRequest request);
		Task<Tuple<AISMessage, string>> GetAISMessagesByIdAsync(int messageId);
		Task<Tuple<int, string>> DeleteAISMessageAsync(DateTime timestamp = default);
	}
}

