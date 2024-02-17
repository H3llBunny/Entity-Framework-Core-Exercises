using System.Xml.Serialization;

namespace CarDealerXml.DTOS.Export
{
    [XmlType("car")]
    public class BmwCarsExportDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}
