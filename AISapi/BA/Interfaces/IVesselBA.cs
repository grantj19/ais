using System;
using AISapi.Models;
using MySql.Data.MySqlClient;

namespace AISapi.BA.Interfaces
{
	public interface IVesselBA
	{
        public Task<Tuple<List<Vessel>, string>> GetVesselsAsync();
    }
}

