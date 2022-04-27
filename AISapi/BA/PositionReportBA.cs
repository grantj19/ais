using System;
using MySql.Data.MySqlClient;
using AISapi.Models;

namespace AISapi.BA
{
	public class PositionReportBA
	{
		private readonly MySqlConnection _connection;

		public PositionReportBA(MySqlConnection connection)
		{
			_connection = connection;
		}

        public async Task<Tuple<PositionReport, string>> GetPositionByMMSIAsync(int MMSI, MySqlConnection? connection = default)
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

            var positionReport = new PositionReport();

            try
            {
                var query = "SELECT  MMSI, Latitude, Longitude, IMO FROM AISMessage, POSITION_REPORT WHERE AISMessage_Id = @AISMessage_Id LIMIT 1";

                command.CommandText = query;

                command.Parameters.AddWithValue("@MMSI", MMSI);

                var result = await command.ExecuteReaderAsync();

                while (await result.ReadAsync())
                {
                    positionReport = new PositionReport

                    {

                        //How to inherit from parent class?
                        //MMSI = result.IsDBNull()


                        //IMO = result.IsDBNull(0) ? null : result.GetInt32(0),
                        //Flag = result.IsDBNull(1) ? null : result.GetString(1),
                        //Name = result.IsDBNull(2) ? null : result.GetString(2),
                        //Built = result.IsDBNull(3) ? null : result.GetInt32(3),
                        //CallSign = result.IsDBNull(4) ? null : result.GetString(4),
                        //Length = result.IsDBNull(5) ? null : result.GetInt32(5),
                        //Breadth = result.IsDBNull(6) ? null : result.GetInt32(6),
                        //Tonnage = result.IsDBNull(7) ? null : result.GetInt32(7),
                        //MMSI = result.IsDBNull(8) ? null : result.GetInt32(8),
                        //Type = result.IsDBNull(9) ? null : result.GetString(9),
                        //Status = result.IsDBNull(10) ? null : result.GetString(10),
                        //Owner = result.IsDBNull(11) ? null : result.GetString(11),

                    };
                }

                await result.DisposeAsync();

                return new Tuple<PositionReport, string>(positionReport, string.Empty);
            }
            catch (Exception ex)
            {
                return new Tuple<PositionReport, string>(new PositionReport(), ex.Message);
            }
            finally
            {
                await command.DisposeAsync();

                if (closeConnection)
                    await connection.CloseAsync();
            }
        }

        public async Task<Tuple<PositionReport, string>> GetRecentPositionsAsync(int IMO, MySqlConnection? connection = default)
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

            var positionReport = new PositionReport();

            try
            {
                var query = "SELECT a.Vessel_IMO, a.MMSI, p.Latitude, p.Longitude FROM ais_message AS a JOIN position_report p on p.aismessage_id = a.id WHERE(a.MMSI, a.Id) IN(SELECT MMSI, MIN(Id) FROM ais_message GROUP BY MMSI)";

                command.CommandText = query;

                var result = await command.ExecuteReaderAsync();

                while (await result.ReadAsync())
                {
                    positionReport = new PositionReport
                    {
                        Vessel_IMO = result.IsDBNull(0) ? null : result.GetInt32(0),
                        MMSI = result.IsDBNull(1) ? null : result.GetInt32(1),
                        Latitude = result.IsDBNull(2) ? null : result.GetDouble(2),
                        Longitude = result.IsDBNull(3) ? null : result.GetDouble(3),


                    };
                }

                await result.DisposeAsync();

                return new Tuple<PositionReport, string>(positionReport, string.Empty);
            }
            catch (Exception ex)
            {
                return new Tuple<PositionReport, string>(new PositionReport(), ex.Message);
            }
            finally
            {
                await command.DisposeAsync();

                if (closeConnection)
                    await connection.CloseAsync();
            }
        }


    }
}

