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

        public async Task<Tuple<List<AISMessage>, string>> GetAISMessagesAsync()
        {
            try
            {
                await _connection.OpenAsync();

                var messageList = new List<AISMessage>();

                var query = "SELECT * FROM AIS_MESSAGE";

                var command = new MySqlCommand(query, _connection);

                var messages = await command.ExecuteReaderAsync();

                while (await messages.ReadAsync())
                {
                    messageList.Add(new AISMessage
                    {
                        Timestamp = messages.GetDateTime(1),
                        MMSI = messages.GetInt32(2),
                        Class = messages.GetString(3),
                        Vessel_IMO = messages.IsDBNull(4) ? null : messages.GetInt32(4)
                    });
                }

                return new Tuple<List<AISMessage>, string>(messageList, string.Empty);

            }
            catch (Exception ex)
            {
                return new Tuple<List<AISMessage>, string>(new List<AISMessage>(), ex.ToString());
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

