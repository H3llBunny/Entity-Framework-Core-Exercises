namespace ProductShop.Dtos.Export
{
    public class CategoryProductExportDto
    {
        public string Category { get; set; }

        public int ProductCount { get; set; }

        public decimal AveragePrice { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}
