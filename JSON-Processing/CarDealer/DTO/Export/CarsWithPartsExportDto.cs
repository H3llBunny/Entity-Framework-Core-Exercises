using Newtonsoft.Json;

namespace CarDealer.DTO.Export
{
    public class CarsWithPartsExportDto
    {
        public class CarDto
        {
            public string Make { get; set; }

            public string Model { get; set; }

            public int TravelledDistance { get; set; }
        }

        public class PartDto
        {
            public string Name { get; set; }

            [JsonProperty("F2")]
            public decimal Price { get; set; }
        }

        public class CarExportDto
        {
            [JsonProperty("car")]
            public CarDto Car { get; set; }

            [JsonProperty("parts")]
            public List<PartDto> Parts { get; set; }
        }
    }
}
