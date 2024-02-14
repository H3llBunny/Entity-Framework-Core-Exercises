namespace ProductShop.DTO.Export
{
    public class SoldProductsExportDto
    {
        public class SellerDto
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        public class ProductDto
        {
            public string Name { get; set; }

            public decimal Price { get; set; }

            public string BuyerFistName { get; set; }

            public string BuyerLastName { get; set; }
        }

        public class SellerWithProductsDto
        {
            public SellerDto Seller { get; set; }

            public List<ProductDto> SoldProucts { get; set; }
        }
    }
}
