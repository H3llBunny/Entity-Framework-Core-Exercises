﻿using System.Xml.Serialization;

namespace CarDealerXml.DTOS.Export
{
    [XmlRoot("sales")]
    public class SalesWithDiscountsExportDto
    {
        [XmlElement("sale")]
        public List<SaleDto> Sales { get; set; }
    }

    [XmlType("sale")]
    public class SaleDto
    {
        [XmlElement("car")]
        public CarDto Car { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }
    }

    [XmlType("car")]
    public class CarDto
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }
    }
}

