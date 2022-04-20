using System;
namespace AISapi.Models
{
	public class AISMessage
	{
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Class { get; set; }
        public long MMSI { get; set; }
        public int? Vessel_IMO { get; set; }
    }
}

