using AISapi.Models;

namespace AISapi.BA.Interfaces
{
    public interface IVesselBA
	{
        public Task<Tuple<List<Vessel>, string>> GetVesselsAsync();
    }
}

