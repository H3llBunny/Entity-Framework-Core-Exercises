using System.Xml.Serialization;

namespace CarDealerXml.DTOS.Export
{
    [XmlType("car")]
    public class CarsWithDistanceExportDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}
