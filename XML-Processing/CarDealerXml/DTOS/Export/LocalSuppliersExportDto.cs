using System.Xml.Serialization;

namespace CarDealerXml.DTOS.Export
{
    [XmlType("supplier")]
    public class LocalSuppliersExportDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("parts-count")]
        public int PartsCount { get; set; }
    }
}
