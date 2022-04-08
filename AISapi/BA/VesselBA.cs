using AISapi.Models;
using MySql.Data.MySqlClient;

namespace AISapi.BA
{
    public class VesselBA
	{
		private readonly MySqlConnection _connection;

		public VesselBA(MySqlConnection connection)
        {
			_connection = connection;
        }

		public async Task<Tuple<List<Vessel>, string>> GetVesselsAsync()
        {
            try
            {
                await _connection.OpenAsync();

                var vesselList = new List<Vessel>();

                var query = "SELECT * FROM VESSEL;";

                var command = new MySqlCommand(query, _connection);

                var vessels = await command.ExecuteReaderAsync();

                while (await vessels.ReadAsync())
                {
                    vesselList.Add(new Vessel
                    {
                        IMO = vessels.GetInt32(0),
                        Flag = vessels.GetString(1),
                        Name = vessels.GetString(2)
                    });
                }

                return new Tuple<List<Vessel>, string>(vesselList, string.Empty);

            }
            catch (Exception ex)
            {
                return new Tuple<List<Vessel>, string>(new List<Vessel>(), ex.ToString());
            }
            finally
            {
                await _connection.CloseAsync();
            }

        }
	}
}

