using System.Xml.Serialization;

namespace CarDealerXml.DTOS.Import
{
    [XmlType("Supplier")]
    public class SuppliersImportDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("isImporter")]
        public bool IsImporter { get; set; }
    }
}
