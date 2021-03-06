using System;
using MySql.Data.MySqlClient;
using AISapi.Models;
using AISapi.Utilities;
using AISapi.DA.Interfaces;

namespace AISapi.DA
{
	public class PositionReportDA : IPositionReportDA
	{
		private readonly MySqlConnection _connection;

		public PositionReportDA(MySqlConnection connection)
		{
			_connection = connection;
		}

        // Receive a list of all position reports.
        // Parameters: A MySql connection object.
        // Return: A tuple consisting of:
        //     -A list of position reports, with all associated non-map view attributes
        //     -An error message string
        public async Task<Tuple<List<PositionReport>, string>> GetPositionReport(MySqlConnection? connection = null)
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
                Connection = _connection
            };

            var positionReports = new List<PositionReport>();

            try
            {
                var query = "SELECT * FROM POSITION_REPORT";

                command.CommandText = query;

                var result = await command.ExecuteReaderAsync();

                while (await result.ReadAsync())
                {
                    var positionReport = new PositionReport

                    {
                        Id = result.GetInt32(0),
                        NavigationalStatus = result.IsDBNull(1) ? null : result.GetString(1),
                        Longitude = result.IsDBNull(2) ? null : result.GetFloat(2),
                        Latitude = result.IsDBNull(3) ? null : result.GetFloat(3),
                        RoT = result.IsDBNull(4) ? null : result.GetDouble(4),
                        SoG = result.IsDBNull(5) ? null : result.GetDouble(5),
                        CoG = result.IsDBNull(6) ? null : result.GetDouble(6),
                        Heading = result.IsDBNull(7) ? null : result.GetInt32(7),
                        LastStaticDataId = result.IsDBNull(8) ? null : result.GetInt32(8)

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

                if (closeConnection)
                    await _connection.CloseAsync();
            }
        }

        // Receive the most recent position of a vessel, given a specific MMSI.
        // First calls GetPositionReports() to filter down from all AIS messages for SQL query.
        // Parameters: The MMSI, an int, supplied by the user.
        // Return: A tuple consisting of:
        //     -A position report object, containing the vessel IMO, MMSI, latitude, and longitude
        //     -An error message string
        public async Task<Tuple<PositionReport, string>> GetPositionByMMSIAsync(int? MMSI)
        {
            await _connection.OpenAsync();

            var command = new MySqlCommand
            {
                Connection = _connection
            };

            var positionReport = new PositionReport();

            try
            {
                (List<PositionReport> currentPositions, string error) = await GetPositionReport(_connection);

                var query = "SELECT a.* FROM ais_message a " +
                    "INNER JOIN (SELECT mmsi, MAX(id) as id " +
                    "FROM ais_message WHERE mmsi = @MMSI) AS b " +
                    "ON a.mmsi = b.mmsi AND a.id = b.id; ";

                command.CommandText = query;

                command.Parameters.AddWithValue("@MMSI", MMSI);

                var result = await command.ExecuteReaderAsync();

                if (await result.ReadAsync())
                {
                    var currentId = result.GetInt32(0);
                    var currentLat = currentPositions.Where(p => p.Id == currentId).Select(p => p.Latitude).FirstOrDefault();
                    var currentLong = currentPositions.Where(p => p.Id == currentId).Select(p => p.Longitude).FirstOrDefault();

                    positionReport = new PositionReport
                    {
                        Vessel_IMO = result.IsDBNull(4) ? null : result.GetInt32(4),
                        MMSI = result.IsDBNull(2) ? null : result.GetInt32(2),
                        Latitude = currentLat,
                        Longitude = currentLong

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

        // Receive all most recent vessel positions.
        // First calls GetPositionReports() to filter down from all AIS messages for SQL query.
        // Parameters: None.
        // Return: A tuple consisting of:
        //     -A list of position report objects, containing the vessel IMO, MMSI, latitude, and longitude
        //     -An error message string
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
                (List<PositionReport> currentPositions, string error) = await GetPositionReport(_connection);

                var query = "SELECT a.* FROM ais_message a " +
                    "INNER JOIN (SELECT mmsi, MAX(id) as id " +
                    "FROM ais_message GROUP BY mmsi) AS b " +
                    "ON a.mmsi = b.mmsi AND a.id = b.id; ";

                command.CommandText = query;

                var result = await command.ExecuteReaderAsync();

                while (await result.ReadAsync())
                {
                    var currentId = result.GetInt32(0);
                    var currentLat = currentPositions.Where(p => p.Id == currentId).Select(p => p.Latitude).FirstOrDefault();
                    var currentLong = currentPositions.Where(p => p.Id == currentId).Select(p => p.Longitude).FirstOrDefault();

                    var positionReport = new PositionReport
                    {
                        Vessel_IMO = result.IsDBNull(4) ? null : result.GetInt32(4),
                        MMSI = result.IsDBNull(2) ? null : result.GetInt32(2),
                        Latitude = currentLat,
                        Longitude = currentLong

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

