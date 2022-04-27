using System.Data;
using AISapi.Models;
using AISapi.Models.Requests;
using AISapi.Utilities;
using MySql.Data.MySqlClient;

namespace AISapi.BA
{
	public class AISMessageBA
	{
		private readonly MySqlConnection _connection;
        private readonly VesselBA _vesselBA;

        public AISMessageBA(MySqlConnection connection, VesselBA vesselBA)
		{
			_connection = connection;
            _vesselBA = vesselBA;
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


        public async Task<Tuple<int, string>> InsertAISMessagesAsync(AISMessageInsertRequest request)
        {
            var recordsInserted = 0;
            MySqlTransaction _transaction;

            await _connection.OpenAsync();

            _transaction = await _connection.BeginTransactionAsync(IsolationLevel.ReadUncommitted);

            try
            {
                foreach (var msg in request.AISMessages)
                {
                    recordsInserted += await InsertAISMessageAsync(msg, _transaction);

                    Enum.TryParse(msg.MsgType?.RemoveUnderscore(), true, out MsgType msgType);

                    if (msgType == MsgType.PositionReport)
                        await InsertPositionReportAsync(msg, _transaction);
                    else
                        await InsertStaticDataAsync(msg, _transaction);
                }

                await _transaction.CommitAsync();

                return new Tuple<int, string>(recordsInserted, string.Empty);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _transaction.RollbackAsync();

                return new Tuple<int, string>(0, ex.Message);
            }
            finally
            {
                await _transaction.DisposeAsync();
                await _connection.CloseAsync();
            }
        }

        private async Task<int> InsertAISMessageAsync(AISMessageRequest msg, MySqlTransaction _transaction)
        {
            var command = new MySqlCommand
            {
                Connection = _connection,
                Transaction = _transaction
            };

            try
            {
                var vesselIMO = msg.IMO?.ValueKind == System.Text.Json.JsonValueKind.Number ? int.Parse(msg.IMO.ToString()) : null;

                if (vesselIMO is not null)
                {
                    (Vessel existingVessel, string error) = (Tuple<Vessel, string>)await _vesselBA.GetVesselByIMOAsync(vesselIMO, _connection);

                    if (existingVessel.IMO is null)
                        await _vesselBA.InsertVesselAsync(msg, _connection, _transaction);
                }


                var query = "INSERT INTO AIS_MESSAGE (Timestamp, MMSI, Class, Vessel_IMO, MessageType) VALUES (@Timestamp, @MMSI, @Class, @Vessel_IMO, @MessageType)";

                command.CommandText = query;

                command.Parameters.AddWithValue("@Timestamp", msg.Timestamp);
                command.Parameters.AddWithValue("@MMSI", msg.MMSI);
                command.Parameters.AddWithValue("@Class", msg.Class);
                command.Parameters.AddWithValue("@Vessel_IMO", vesselIMO);
                command.Parameters.AddWithValue("@MessageType", msg.MsgType);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                await command.DisposeAsync();
            }
            
        }

        private async Task InsertPositionReportAsync(AISMessageRequest msg, MySqlTransaction _transaction)
        {

            var command = new MySqlCommand
            {
                Connection = _connection,
                Transaction = _transaction
            };

            try
            {

                var aisMessageId = await GetAISMessageIdAsync(msg.MMSI, "position_report");

                var query = "INSERT INTO POSITION_REPORT (AISMessage_Id, NavigationalStatus, Longitude, Latitude, RoT, SoG, CoG, Heading, LastStaticData_Id) VALUES" +
                    " (@AISMessageId, @Status, @Longitude, @Latitude, @RoT, @SoG, @CoG, @Heading, @LastStaticDataId)";

                command.CommandText = query;

                command.Parameters.AddWithValue("@AISMessageId", aisMessageId);
                command.Parameters.AddWithValue("@Status", msg.Status);
                command.Parameters.AddWithValue("@Longitude", msg.Position?.Coordinates?[0]);
                command.Parameters.AddWithValue("@Latitude", msg.Position?.Coordinates?[1]);
                command.Parameters.AddWithValue("@RoT", msg.RoT);
                command.Parameters.AddWithValue("@SoG", msg.SoG);
                command.Parameters.AddWithValue("@CoG", msg.CoG);
                command.Parameters.AddWithValue("@Heading", msg.Heading);
                command.Parameters.AddWithValue("@LastStaticDataId", await GetLastStaticDataIdAsync(aisMessageId)); //Calculated

                await command.ExecuteNonQueryAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                await command.DisposeAsync();
            }
        }

        private async Task InsertStaticDataAsync(AISMessageRequest msg, MySqlTransaction _transaction)
        {
            var command = new MySqlCommand
            {
                Connection = _connection,
                Transaction = _transaction
            };

            try
            {
                var aisMessageId = await GetAISMessageIdAsync(msg.MMSI, "static_data");

                var query = "INSERT INTO STATIC_DATA (AISMessage_Id, AISIMO, CallSign, Name, VesselType, CargoType, Length, Breadth, Draught, AISDestination, ETA, DestinationPort_Id) VALUES" +
                    " (@AISMessageId, @AISIMO, @CallSign, @Name, @VesselType, @CargoType, @Length, @Breadth, @Draught, @AISDestination, @ETA, @DestinationPort_Id)";

                command.CommandText = query;

                command.Parameters.AddWithValue("@AISMessageId", aisMessageId);
                command.Parameters.AddWithValue("@AISIMO", msg.IMO?.ValueKind == System.Text.Json.JsonValueKind.Number ? int.Parse(msg.IMO.ToString()) : null);
                command.Parameters.AddWithValue("@CallSign", msg.CallSign);
                command.Parameters.AddWithValue("@Name", msg.Name);
                command.Parameters.AddWithValue("@VesselType", msg.VesselType);
                command.Parameters.AddWithValue("@CargoType", msg.SoG);
                command.Parameters.AddWithValue("@Length", msg.Length);
                command.Parameters.AddWithValue("@Breadth", msg.Breadth);
                command.Parameters.AddWithValue("@Draught", msg.Draught);
                command.Parameters.AddWithValue("@AISDestination", msg.Destination);
                command.Parameters.AddWithValue("@ETA", msg.ETA);
                command.Parameters.AddWithValue("@DestinationPort_Id", await GetDestinationPortIdAsync(msg.Destination));

                await command.ExecuteNonQueryAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                await command.DisposeAsync();
            }
        }

        private async Task<int?> GetDestinationPortIdAsync(string destination)
        {
            try
            {
                if (string.IsNullOrEmpty(destination))
                    return null;

                var query = "SELECT Id FROM PORT WHERE " +
                    "Name = @Name";

                var command = new MySqlCommand(query, _connection);

                command.Parameters.AddWithValue("@Name", destination);

                var id = command.ExecuteScalarAsync().Result?.ToString();

                await command.DisposeAsync();

                if (!string.IsNullOrEmpty(id))
                    return int.Parse(id);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<int> GetAISMessageIdAsync(int mmsi, string messageType)
        {
            var command = _connection.CreateCommand();

            try
            {
                var query = "SELECT Id FROM AIS_MESSAGE WHERE " +
                    "MMSI = @MMSI " +
                    "AND MessageType = @MessageType " +
                    "ORDER BY Id DESC " +
                    "LIMIT 1";

                command.CommandText = query;

                command.Parameters.AddWithValue("@MMSI", mmsi);
                command.Parameters.AddWithValue("@MessageType", messageType);

                var id = command.ExecuteScalarAsync().Result?.ToString();

                return int.Parse(id);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new int();
            }
            finally
            {
                await command.DisposeAsync();
            }
        }

        private async Task<int?> GetLastStaticDataIdAsync(int aisMessageId)
        {
            var command = _connection.CreateCommand();

            try
            {
                if (aisMessageId <= 0)
                    return null;

                var query = "SELECT Id FROM AIS_MESSAGE WHERE " +
                    "MessageType = @MessageType " +
                    "AND MMSI = ( SELECT MMSI FROM AIS_MESSAGE WHERE Id = @aisMessageId) " +
                    "ORDER BY Id DESC " +
                    "LIMIT 1";

                command.CommandText = query;

                command.Parameters.AddWithValue("@MessageType", "static_data");
                command.Parameters.AddWithValue("@aisMessageId", aisMessageId);

                var id = command.ExecuteScalarAsync().Result?.ToString();

                if (!string.IsNullOrEmpty(id))
                    return int.Parse(id);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                await command.DisposeAsync();
            }
        }
    }
}

