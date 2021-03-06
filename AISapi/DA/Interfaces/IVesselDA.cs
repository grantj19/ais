using System;
using AISapi.Models;
using AISapi.Models.Requests;
using MySql.Data.MySqlClient;

namespace AISapi.DA.Interfaces
{
	public interface IVesselDA
	{
		Task<Tuple<Vessel, string>> GetVesselByIMOAsync(int IMO, MySqlConnection? connection = default);
		Task InsertVesselAsync(AISMessageRequest msg, MySqlConnection connection, MySqlTransaction transaction);
	}
}

