using AutoMapper;
using ProductShopXML.DTO.Export;
using ProductShopXML.DTO.ExportDtos;
using ProductShopXML.DTO.Import;
using ProductShopXML.Models;

namespace ProductShopXML
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

            CreateMap<Product, ProductExportDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.ToString("0.##")))
                .ForMember(dest => dest.BuyerFullName,
                opt => opt.MapFrom(src => $"{src.Buyer.FirstName} {src.Buyer.LastName}"))
                .ReverseMap();

            CreateMap<User, SoldProductsExportDto.UserDtop>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.SoldProducts,
                opt => opt.MapFrom(src => src.ProductsSold.Select(p => new SoldProductsExportDto.ProductDto
                {
                    Name = p.Name,
                    Price = p.Price
                })));

            CreateMap<Category, CategorisByProductCountExportDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.CategoryProducts.Select(p => p.Product).Count()))
                .ForMember(dest => dest.AveragePrice, opt => opt.MapFrom(src => src.CategoryProducts.Average(p => p.Product.Price).ToString("F2")))
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.CategoryProducts.Sum(p => p.Product.Price).ToString("F2")));

            CreateMap<User, UsersAndProductsExportDto.UsersDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.SoldProducts, opt => opt.MapFrom(src => new UsersAndProductsExportDto.ProductsDto
                {
                    Count = src.ProductsSold.Count,
                    Products = src.ProductsSold.Select(p => new UsersAndProductsExportDto.ProductDto
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).ToList()
                }));
        }
    }
}
