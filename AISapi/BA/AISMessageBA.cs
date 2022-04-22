using AISapi.Models;
using AISapi.Models.Requests;
using AISapi.Utilities;
using MySql.Data.MySqlClient;

namespace AISapi.BA
{
	public class AISMessageBA
	{
		private readonly MySqlConnection _connection;
        private MySqlTransaction _transaction;

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
                        MMSI = results.GetInt32(2),
                        Class = results.GetString(3),
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


        public async Task InsertAISMessagesAsync(AISMessageInsertRequest request)
        {
            try
            {

                var _commands = new List<MySqlCommand>();
                
                foreach (var msg in request.AISMessages)
                {

                    _commands = CreateInsertAISMessageCommand(msg, _commands);

                    Enum.TryParse(msg.Type.ToString().RemoveUnderscore(), true, out MsgType msgType);

                    if (msgType == MsgType.PositionReport)
                    {
                        _commands = CreateInsertPositionReportCommand(msg, _commands);
                    }
                    else
                    {
                        var messages = new List<AISMessage>();

                        var query = "SELECT * FROM AIS_MESSAGE" +
                            " WHERE Id = @Id";

                        var command = new MySqlCommand(query, _connection);

                        //command.Parameters.AddWithValue("@Id", );

                        var results = command.ExecuteReaderAsync().Result;

                    }
                }

                await _connection.OpenAsync();
                _transaction = _connection.BeginTransaction();

                //run commands
                foreach (var cmd in _commands)
                {
                    cmd.Transaction = _transaction;

                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                //return new Tuple<List<AISMessage>, string>>(new List<AISMessage>(), ex.ToString());
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        private List<MySqlCommand> CreateInsertAISMessageCommand(AISMessageRequest msg, List<MySqlCommand> _commands)
        {
            var query = "INSERT INTO AIS_MESSAGE VALUES (@Timestamp, @MMSI, @Class, @Vessel_IMO)";

            var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@Timestamp", msg.Timestamp);
            command.Parameters.AddWithValue("@MMSI", msg.MMSI);
            command.Parameters.AddWithValue("@Class", msg.Class);
            command.Parameters.AddWithValue("@Vessel_IMO", msg.IMO);

            _commands.Add(command);

            return _commands;
        }

        private List<MySqlCommand> CreateInsertPositionReportCommand(AISMessageRequest msg, List<MySqlCommand> _commands)
        {
            var query = "INSERT INTO POSITION_REPORT VALUES" +
                " (@AISMessageId, @Status, @Longitude, @Latitude, @RoT, @SoG, @CoG, @Heading, @LastStaticDataId)";

            var command = new MySqlCommand(query, _connection);

            command.Parameters.AddWithValue("@AISMessageId", GetAISMessageId(msg.Timestamp ?? new DateTime(), msg.MMSI ?? 0)); //Calculated
            command.Parameters.AddWithValue("@Status", msg.Status);
            command.Parameters.AddWithValue("@Longitude", msg.Position.Coordinates[0]);
            command.Parameters.AddWithValue("@Latitude", msg.Position.Coordinates[1]);
            command.Parameters.AddWithValue("@RoT", msg.RoT);
            command.Parameters.AddWithValue("@SoG", msg.SoG);
            command.Parameters.AddWithValue("@CoG", msg.CoG);
            command.Parameters.AddWithValue("@Heading", msg.Heading);
            command.Parameters.AddWithValue("@LastStaticDataId", null); //Calculated

            _commands.Add(command);

            return _commands;
        }

        private async Task<int> GetAISMessageId(DateTime timestamp, int MMSI)
        {
            try
            {
                await _connection.OpenAsync();

                var query = "SELECT Id FROM AIS_MESSAGE WHERE " +
                    " Timestamp = @Timestamp" +
                    " AND MMSI = @MMSI";

                var command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue("@Timestamp", timestamp);
                command.Parameters.AddWithValue("@MMSI", MMSI);

                return int.Parse(command.ExecuteScalarAsync().Result.ToString() ?? "0");
            }
            finally
            {
                await _connection.CloseAsync();
            }


        }
    }
}

