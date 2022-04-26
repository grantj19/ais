using System;

namespace AISapi.Models.Requests
{
	public class AISMessageInsertRequest
	{
        public List<AISMessageRequest>? AISMessages { get; set; }
    }

    public class AISMessageRequest
    {
        public DateTime? Timestamp { get; set; }
        public string? Class { get; set; }
        public int MMSI { get; set; }
        public dynamic? IMO { get; set; }
        public string? MsgType { get; set; }
        public string? Status { get; set; } //Mapped to nav status
        public Position? Position { get; set; }
        public double? RoT { get; set; }
        public double? SoG { get; set; }
        public double? CoG { get; set; }
        public int? Heading { get; set; }
        public string? CallSign { get; set; }
        public string? Name { get; set; }
        public string? VesselType { get; set; }
        public string? CargoType { get; set; }
        public int? Length { get; set; }
        public int? Breadth { get; set; }
        public double? Draught { get; set; }
        public string? Destination { get; set; }
        public DateTime? ETA { get; set; }
        public int? A { get; set; }
        public int? B { get; set; }
        public int? C { get; set; }
        public int? D { get; set; }
    }

    public class Position
    {
        public string? Type { get; set; }
        public double[]? Coordinates { get; set; }
    }
}