using Newtonsoft.Json;

namespace CarDealer.DTO.Export
{
    public class SalesWithDiscountExportDto
    {
        public class CarDto
        {
            public string Make { get; set; }

            public string Model { get; set; }

            public int TravelledDistance { get; set; }
        }

        public class CustomerDto
        {
            [JsonProperty("customerName")]
            public string CustomerName { get; set; }

            public decimal Discount { get; set; }

            [JsonProperty("price")]
            public decimal Price { get; set; }

            [JsonProperty("priceWithDiscount")]
            public decimal PriceWithDiscount { get; set; }
        }

        public class SalesWithDiscountDto
        {
            [JsonProperty("car")]
            public CarDto Car { get; set; }

            [JsonProperty("customer")]
            public CustomerDto Customer { get; set; }
        }
    }
}
