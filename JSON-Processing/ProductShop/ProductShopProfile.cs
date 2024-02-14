using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UsersImportDto, User>()
                .ForMember(src => src.FirstName, opt => opt.MapFrom(dto => dto.FirstName))
                .ForMember(src => src.LastName, opt => opt.MapFrom(dto => dto.LastName))
                .ForMember(src => src.Age, opt => opt.MapFrom(dto => dto.Age));

            CreateMap<ProductsImportDto, Product>()
                .ForMember(src => src.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForMember(src => src.Price, opt => opt.MapFrom(dto => dto.Price))
                .ForMember(src => src.SellerId, opt => opt.MapFrom(dto => dto.SellerId))
                .ForMember(src => src.BuyerId, opt => opt.MapFrom(dto => dto.BuyerId));

            CreateMap<CategoriesImportDto, Category>()
                .ForMember(src => src.Name, opt => opt.MapFrom(dto => dto.Name));

            CreateMap<CategoryProductsImportDto, CategoryProduct>()
                .ForMember(src => src.CategoryId, opt => opt.MapFrom(dto => dto.CategoryId))
                .ForMember(src => src.ProductId, opt => opt.MapFrom(dto => dto.ProductId));

            CreateMap<Product, ProductsInRangeExportDto>()
                .ForMember(dto => dto.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dto => dto.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dto => dto.SellerFullname,
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
                .ForMember(dto => dto.Seller, opt => opt.MapFrom(src => src))
                .ForMember(dto => dto.SoldProucts, opt => opt.MapFrom(src => src.ProductsSold));

            CreateMap<Category, CategoryProductExportDto>()
                .ForMember(dto => dto.Category, opt => opt.MapFrom(src => src.Name))
                .ForMember(dto => dto.ProductCount, opt => opt.MapFrom(src => src.CategoryProducts.Count()))
                .ForMember(dto => dto.AveragePrice, opt => opt.MapFrom(src =>
                src.CategoryProducts.Average(p => p.Product.Price).ToString("F2")))
                .ForMember(dto => dto.TotalRevenue, opt => opt.MapFrom(src =>
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