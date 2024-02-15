using System.Xml.Serialization;

namespace ProductShopXML.DTO.Import
{
    [XmlType("User")]
    public class UsersImportDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }
    }
}
