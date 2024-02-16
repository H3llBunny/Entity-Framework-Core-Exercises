using AutoMapper;
using ProductShop.DTO.Export;
using ProductShop.DTO.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UsersImportDto, User>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age));

            CreateMap<ProductsImportDto, Product>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.SellerId, opt => opt.MapFrom(src => src.SellerId))
                .ForMember(dest => dest.BuyerId, opt => opt.MapFrom(src => src.BuyerId));

            CreateMap<CategoriesImportDto, Category>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<CategoryProductsImportDto, CategoryProduct>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId));

            CreateMap<Product, ProductsInRangeExportDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.SellerFullname,
                opt => opt.MapFrom(src => $"{src.Seller.FirstName} {src.Seller.LastName}"));

            CreateMap<User, SoldProductsExportDto.SellerDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));

            CreateMap<Product, SoldProductsExportDto.ProductDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.BuyerFistName, opt => opt.MapFrom(src => src.Buyer.FirstName))
                .ForMember(dest => dest.BuyerLastName, opt => opt.MapFrom(src => src.Buyer.LastName));

            CreateMap<User, SoldProductsExportDto.SellerWithProductsDto>()
                .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.SoldProucts, opt => opt.MapFrom(src => src.ProductsSold));

            CreateMap<Category, CategoryProductExportDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.CategoryProducts.Count()))
                .ForMember(dest => dest.AveragePrice, opt => opt.MapFrom(src =>
                src.CategoryProducts.Average(p => p.Product.Price).ToString("F2")))
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src =>
                src.CategoryProducts.Sum(p => p.Product.Price).ToString("F2")));

            CreateMap<User, UsersAndProductsExportDto.UsersDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.SoldProducts,
                opt => opt.MapFrom(src => new UsersAndProductsExportDto.SoldProductsDto
                {
                    Count = src.ProductsSold.Count(),
                    Products = src.ProductsSold.Select(p => new UsersAndProductsExportDto.ProductDto
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).ToList()
                }));
        }
    }
}