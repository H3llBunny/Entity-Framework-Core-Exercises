using System.Xml.Serialization;

namespace ProductShopXML.DTO.ExportDtos
{
    [XmlType("Product")]
    public class ProductExportDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("buyer")]
        public string?  BuyerFullName { get; set; }
    }
}
