using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShopXML.DTO.Import
{
    [XmlType("Category")]
    public class CategoriesImportDto
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
