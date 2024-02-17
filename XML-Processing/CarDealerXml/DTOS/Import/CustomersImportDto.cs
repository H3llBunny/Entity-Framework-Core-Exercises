using System.Xml.Serialization;

namespace CarDealerXml.DTOS.Import
{
    [XmlType("Customer")]
    public class CustomersImportDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("birthDate")]
        public DateTime BirthDate { get; set; }

        [XmlElement("isYoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}
