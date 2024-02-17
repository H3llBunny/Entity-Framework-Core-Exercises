using AutoMapper;
using CarDealerXml.DTOS.Export;
using CarDealerXml.DTOS.Import;
using CarDealerXml.Models;

namespace CarDealerXml
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<SuppliersImportDto, Supplier>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsImporter, opt => opt.MapFrom(src => src.IsImporter));

            CreateMap<PartsImportDto, Part>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId));

            CreateMap<CarsImportDto, Car>()
                .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Make))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.TravelledDistance, opt => opt.MapFrom(src => src.TravelledDistance))
                .ForMember(dest => dest.PartCars, opt => opt.MapFrom(src => src.Parts.Select(p => new PartCar { PartId = p.Id })));

            CreateMap<CustomersImportDto, Customer>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.IsYoungDriver, opt => opt.MapFrom(src => src.IsYoungDriver));

            CreateMap<SalesImportDto, Sale>()
               .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
               .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
               .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount));

            CreateMap<Car, CarsWithDistanceExportDto>()
                .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Make))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.TravelledDistance, opt => opt.MapFrom(src => src.TravelledDistance));

            CreateMap<Car, BmwCarsExportDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
               .ForMember(dest => dest.TravelledDistance, opt => opt.MapFrom(src => src.TravelledDistance));

            CreateMap<Supplier, LocalSuppliersExportDto>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.PartsCount, opt => opt.MapFrom(src => src.Parts.Count()));

            CreateMap<Part, CarsAndPartsExportDto.PartDto>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<Car, CarsAndPartsExportDto.CarDto>()
               .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Make))
               .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
               .ForMember(dest => dest.TravelledDistance, opt => opt.MapFrom(src => src.TravelledDistance))
               .ForMember(dest => dest.Parts, opt => opt.MapFrom(src => src.PartCars.Select(pc => new CarsAndPartsExportDto.PartDto
               {
                   Name = pc.Part.Name,
                   Price = pc.Part.Price
               })));

            CreateMap<Customer, CustomersAndSalesExportDto>()
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.BoughtCars, opt => opt.MapFrom(src => src.Sales.Count()))
              .ForMember(dest => dest.SpendMoney,
              opt => opt.MapFrom(src => src.Sales.SelectMany(c => c.Car.PartCars).Sum(p => p.Part.Price)));

            CreateMap<Sale, SaleDto>()
                .ForMember(dest => dest.Car, opt => opt.MapFrom(src => src.Car))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Car.PartCars.Sum(pc => pc.Part.Price)))
                .ForMember(dest => dest.PriceWithDiscount, opt => opt.MapFrom(src => GetCarPriceWithDiscount(src.Car, src.Discount)));

            CreateMap<Car, CarDto>();
        }
        private decimal GetCarPriceWithDiscount(Car car, decimal discount)
        {
            decimal carPrice = car.PartCars.Sum(pc => pc.Part.Price);
            return carPrice * (1 - (discount / 100));
        }
    }
}
