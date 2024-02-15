using System.Xml.Serialization;

namespace ProductShopXML.DTO.Export
{
    public class SoldProductsExportDto
    {
        [XmlType("User")]
        public class UserDtop
        {
            [XmlElement("firstName")]
            public string FirstName { get; set; }

            [XmlElement("lastName")]
            public string LastName { get; set; }

            [XmlElement("soldProducts")]
            public List<ProductDto> SoldProducts { get; set; }
        }

        [XmlType("Product")]
        public class ProductDto
        {
            [XmlElement("name")]
            public string Name { get; set; }

            [XmlElement("price")]
            public decimal Price { get; set; }
        }
    }
}
