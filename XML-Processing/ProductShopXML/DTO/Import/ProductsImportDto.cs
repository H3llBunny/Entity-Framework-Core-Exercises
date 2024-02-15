using ProductShopXML.Models;
using System.Xml.Serialization;

namespace ProductShopXML.DTO.Import
{
    [XmlType("Product")]
    public class ProductsImportDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("sellerId")]
        public int SellerId { get; set; }

        [XmlElement("buyerId")]
        public int? BuyerId { get; set; }
    }
}
