using System;
using AISapi.BA.Interfaces;
using AISapi.Models;
using MySql.Data.MySqlClient;

namespace AISapi.BA
{
	public class VesselBA : IVesselBA
	{
		private MySqlConnection _connection;

		public VesselBA(MySqlConnection mySqlConnection)
		{
			_connection = mySqlConnection;
		}

		public async string GetVessels()
        {

			var query = "SELECT * FROM VESSELS";

			var command = new MySqlCommand(query, _connection);

			var vessels = command.ExecuteNonQuery().ToString();

			return vessels;

        }
	}
}

