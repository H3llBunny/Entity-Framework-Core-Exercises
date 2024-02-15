using System.Xml.Serialization;

namespace ProductShopXML.DTO.Export
{
    public class UsersAndProductsExportDto
    {
        [XmlType("User")]
        public class UsersDto
        {
            [XmlElement("firstName")]
            public string FirstName { get; set; }

            [XmlElement("lastName")]
            public string LastName { get; set; }

            [XmlElement("age")]
            public int? Age { get; set; }

            public ProductsDto SoldProducts { get; set; }
        }

        public class ProductsDto
        {
            [XmlElement("count")]
            public int Count { get; set; }

            [XmlArray("products")]
            public List<ProductDto> Products { get; set; }
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
