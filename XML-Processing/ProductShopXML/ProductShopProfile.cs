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

            CreateMap<Product, ProductExportDto>()
                .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dto => dto.Price, opt => opt.MapFrom(src => src.Price.ToString("0.##")))
                .ForMember(dto => dto.BuyerFullName,
                opt => opt.MapFrom(src => $"{src.Buyer.FirstName} {src.Buyer.LastName}"))
                .ReverseMap();

            CreateMap<User, SoldProductsExportDto.UserDtop>()
                .ForMember(dto => dto.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dto => dto.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dto => dto.SoldProducts,
                opt => opt.MapFrom(src => src.ProductsSold.Select(p => new SoldProductsExportDto.ProductDto
                {
                    Name = p.Name,
                    Price = p.Price
                })));

            CreateMap<Category, CategorisByProductCountExportDto>()
                .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dto => dto.Count, opt => opt.MapFrom(src => src.CategoryProducts.Select(p => p.Product).Count()))
                .ForMember(dto => dto.AveragePrice, opt => opt.MapFrom(src => src.CategoryProducts.Average(p => p.Product.Price).ToString("F2")))
                .ForMember(dto => dto.TotalRevenue, opt => opt.MapFrom(src => src.CategoryProducts.Sum(p => p.Product.Price).ToString("F2")));

            CreateMap<User, UsersAndProductsExportDto.UsersDto>()
                .ForMember(dto => dto.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dto => dto.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dto => dto.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dto => dto.SoldProducts, opt => opt.MapFrom(src => new UsersAndProductsExportDto.ProductsDto
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
