using System;
namespace AISapi.Models
{
    public class PositionReport : AISMessage
    {
        public string? NavigationalStatus { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public double? RoT { get; set; }
        public double? SoG { get; set; }
        public double? CoG { get; set; }
        public int? Heading { get; set; }
        public int? LastStaticDataId { get; set; }
        public int? MapViewId1 { get; set; }
        public int? MapViewId2 { get; set; }
        public int? MapViewId3 { get; set; }
    }
}

