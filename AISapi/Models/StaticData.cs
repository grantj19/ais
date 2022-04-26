using System;
namespace AISapi.Models
{
	public class StaticData : AISMessage
	{
        public string? CallSign { get; set; }
        public string? Name { get; set; }
        public string? VesselType { get; set; }
        public string? CargoType { get; set; }
        public int Length { get; set; }
        public int Breadth { get; set; }
        public int Draught { get; set; }
        public string? AISDestination { get; set; }
        public DateTime ETA { get; set; }
        public int DestinationPortId { get; set; }
    }
}

