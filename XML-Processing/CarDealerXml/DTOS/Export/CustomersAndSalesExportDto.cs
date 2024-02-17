using System.Xml.Serialization;

namespace CarDealerXml.DTOS.Export
{
    [XmlType("customer")]
    public class CustomersAndSalesExportDto
    {
        [XmlAttribute("full-name")]
        public string Name { get; set; }

        [XmlAttribute("bought-cars")]
        public int BoughtCars { get; set; }

        [XmlAttribute("spent-money")]
        public decimal SpendMoney { get; set; }
    }
}
