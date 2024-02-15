using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShopXML.DTO.Import
{
    [XmlType("CategoryProduct")]
    public class CategoryProductsImportDto
    {
        public int CategoryId { get; set; }
     
        public int ProductId { get; set; }
    }
}
