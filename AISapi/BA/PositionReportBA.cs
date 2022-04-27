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

        public async Task<Tuple<PositionReport, string>> GetPositionByMMSIAsync(int MMSI)
        {

            await _connection.OpenAsync();

            var command = new MySqlCommand
            {
                Connection = _connection
            };

            var positionReport = new PositionReport();

            try
            {
                var query = "SELECT a.MMSI, a.Vessel_IMO, p.Latitude, p.Longitude FROM AIS_Message a JOIN POSITION_REPORT p ON p.AISMessage_Id = a.Id WHERE a.MMSI = @MMSI ORDER BY a.Timestamp DESC LIMIT 1";

                command.CommandText = query;

                command.Parameters.AddWithValue("@MMSI", MMSI);

                var result = await command.ExecuteReaderAsync();

                if (await result.ReadAsync())
                {
                    positionReport = new PositionReport

                    {
                        MMSI = result.IsDBNull(0) ? null : result.GetInt32(0),
                        Vessel_IMO = result.IsDBNull(1) ? null : result.GetInt32(1),
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

                await _connection.CloseAsync();
            }
        }

        public async Task<Tuple<List<PositionReport>, string>> GetRecentPositionsAsync()
        {
            await _connection.OpenAsync();

            var command = new MySqlCommand
            {
                Connection = _connection
            };

            var positionReports = new List<PositionReport>();

            try
            {
                var query = "SELECT a.Vessel_IMO, a.MMSI, p.Latitude, p.Longitude FROM ais_message AS a JOIN position_report p on p.aismessage_id = a.id WHERE(a.MMSI, a.Id) IN(SELECT MMSI, MIN(Id) FROM ais_message GROUP BY MMSI)";

                command.CommandText = query;

                var result = await command.ExecuteReaderAsync();

                while (await result.ReadAsync())
                {
                    var positionReport = new PositionReport
                    {
                        Vessel_IMO = result.IsDBNull(0) ? null : result.GetInt32(0),
                        MMSI = result.IsDBNull(1) ? null : result.GetInt32(1),
                        Latitude = result.IsDBNull(2) ? null : result.GetDouble(2),
                        Longitude = result.IsDBNull(3) ? null : result.GetDouble(3),
                    };

                    positionReports.Add(positionReport);
                }

                await result.DisposeAsync();

                return new Tuple<List<PositionReport>, string>(positionReports, string.Empty);
            }
            catch (Exception ex)
            {
                return new Tuple<List<PositionReport>, string>(new List<PositionReport>(), ex.Message);
            }
            finally
            {
                await command.DisposeAsync();

                await _connection.CloseAsync();
            }
        }
    }
}

