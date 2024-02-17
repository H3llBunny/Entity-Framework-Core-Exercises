using System.Xml.Serialization;

namespace CarDealerXml.DTOS.Import
{
    [XmlType("Car")]
    public class CarsImportDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        [XmlArrayItem("partId")]
        public List<PartIdDto> Parts { get; set; }
    }

    [XmlType("partId")]
    public class PartIdDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
