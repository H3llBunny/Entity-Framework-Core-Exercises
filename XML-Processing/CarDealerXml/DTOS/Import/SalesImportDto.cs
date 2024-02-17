using System.Xml.Serialization;

namespace CarDealerXml.DTOS.Import
{
    [XmlType("Sale")]
    public class SalesImportDto
    {
        [XmlElement("carId")]
        public int CarId { get; set; }

        [XmlElement("customerId")]
        public int CustomerId { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }
    }
}
