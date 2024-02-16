using Newtonsoft.Json;
using System.Xml.Serialization;

namespace CarDealer.DTO.Import
{
    public class CarsImportDto
    {
        public string Make { get; set; }

        public string Model { get; set; }

        public long TravelledDistance { get; set; }

        [JsonProperty("partsId")]
        public List<int> Parts { get; set; }
    }
}
