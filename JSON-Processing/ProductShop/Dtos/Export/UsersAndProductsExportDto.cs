namespace ProductShop.Dtos.Export
{
    public class UsersAndProductsExportDto
    {
        public int UsersCount { get; set; }
        public List<UsersDto> Users { get; set; }

        public class UsersDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public SoldProductsDto SoldProducts { get; set; }
        }

        public class SoldProductsDto
        {
            public int Count { get; set; }
            public List<ProductDto> Products { get; set; }
        }

        public class ProductDto
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
        }
    }
}
