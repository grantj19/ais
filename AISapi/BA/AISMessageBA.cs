using AISapi.Models;
using MySql.Data.MySqlClient;

namespace AISapi.BA
{
	public class AISMessageBA
	{
		private readonly MySqlConnection _connection;

		public AISMessageBA(MySqlConnection connection)
		{
			_connection = connection;
		}

        public async Task<Tuple<AISMessage, string>> GetAISMessagesByIdAsync(int messageId)
        {
            try
            {
                await _connection.OpenAsync();

                var message = new AISMessage();

                var query = "SELECT * FROM AIS_MESSAGE" +
                    " WHERE Id = @Id";

                var command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue("@Id", messageId);

                var results = command.ExecuteReaderAsync().Result;

                if (await results.ReadAsync())
                {
                    message = new AISMessage
                    {
                        Id = results.GetInt32(0),
                        Timestamp = results.GetDateTime(1),
                        Class = results.GetString(2),
                        MMSI = results.GetInt32(3),
                        Vessel_IMO = results.IsDBNull(4) ? null : results.GetInt32(4)
                    };
                }
                    

                return new Tuple<AISMessage, string>(message, string.Empty);

            }
            catch (Exception ex)
            {
                return new Tuple<AISMessage, string>(new AISMessage(), ex.ToString());
            }
            finally
            {
                await _connection.CloseAsync();
            }

        }


        //public async Task<Tuple<List<AISMessage>, string>> InsertAISMessagesAsync()
        //{

        //}
	}
}

