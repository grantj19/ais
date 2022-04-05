using System;
using AISapi.Models;

namespace AISapi.BA.Interfaces
{
	public interface IVesselBA
	{
        public Task<List<Vessel>> GetVessels();
    }
}

