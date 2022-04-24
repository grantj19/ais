namespace AISapi.Models
{

	public class Vessel
	{
        public int? IMO { get; set; }
        public string? Flag { get; set; }
        public string? Name { get; set; }
        public int? Built { get; set; }
        public string? CallSign { get; set; }
        public int? Length { get; set; }
        public int? Breadth { get; set; }
        public int? Tonnage { get; set; }
        public int? MMSI { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? Owner { get; set; }
    }
}

