using AISapi.DA.Interfaces;
using AISapi.Models;
using AISapi.Models.Requests;
using MySql.Data.MySqlClient;

namespace AISapi.DA
{
    public class VesselBA : IVesselDA
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

        public async Task<Tuple<Vessel, string>> GetVesselByIMOAsync(int IMO, MySqlConnection? connection = null)
        {
            var closeConnection = false;

            if (connection is null)
            {
                connection = _connection;
                await connection.OpenAsync();
                closeConnection = true;
            }

            var command = new MySqlCommand
            {
                Connection = connection
            };

            var vessel = new Vessel();

            try
            {
                var query = "SELECT * FROM VESSEL WHERE IMO = @IMO LIMIT 1";

                command.CommandText = query;

                command.Parameters.AddWithValue("@IMO", IMO);

                var result = await command.ExecuteReaderAsync();

                while (await result.ReadAsync())
                {
                    vessel = new Vessel
                    {
                        IMO = result.IsDBNull(0) ? null : result.GetInt32(0),
                        Flag = result.IsDBNull(1) ? null : result.GetString(1),
                        Name = result.IsDBNull(2) ? null : result.GetString(2),
                        Built = result.IsDBNull(3) ? null : result.GetInt32(3),
                        CallSign = result.IsDBNull(4) ? null : result.GetString(4),
                        Length = result.IsDBNull(5) ? null : result.GetInt32(5),
                        Breadth = result.IsDBNull(6) ? null : result.GetInt32(6),
                        Tonnage = result.IsDBNull(7) ? null : result.GetInt32(7),
                        MMSI = result.IsDBNull(8) ? null : result.GetInt32(8),
                        Type = result.IsDBNull(9) ? null : result.GetString(9),
                        Status = result.IsDBNull(10) ? null : result.GetString(10),
                        Owner = result.IsDBNull(11) ? null : result.GetString(11),

                    };
                }

                await result.DisposeAsync();

                return new Tuple<Vessel, string>(vessel, string.Empty);
            }
            catch (Exception ex)
            {
                return new Tuple<Vessel, string>(new Vessel(), ex.Message);
            }
            finally
            {
                await command.DisposeAsync();

                if (closeConnection)
                    await connection.CloseAsync();
            }
        }

        public async Task InsertVesselAsync(AISMessageRequest msg, MySqlConnection connection, MySqlTransaction transaction)
        {
            var command = new MySqlCommand
            {
                Connection = connection,
                Transaction = transaction
            };

            try
            {
                var query = "INSERT INTO VESSEL (IMO, Name, CallSign, Length, Breadth, MMSI, Type, Status) VALUES " +
                    "(@IMO, @Name, @CallSign, @Length, @Breadth, @MMSI, @Type, @Status)";

                command.CommandText = query;

                command.Parameters.AddWithValue("@IMO", msg.IMO?.ValueKind == System.Text.Json.JsonValueKind.Number ? int.Parse(msg.IMO.ToString()) : null);
                command.Parameters.AddWithValue("@Name", msg.Name);
                command.Parameters.AddWithValue("@CallSign", msg.CallSign);
                command.Parameters.AddWithValue("@Length", msg.Length);
                command.Parameters.AddWithValue("@Breadth", msg.Breadth);
                command.Parameters.AddWithValue("@MMSI", msg.MMSI);
                command.Parameters.AddWithValue("@Type", msg.VesselType);
                command.Parameters.AddWithValue("@Status", msg.Status);

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
	}
}

