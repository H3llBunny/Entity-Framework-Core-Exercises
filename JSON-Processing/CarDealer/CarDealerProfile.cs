using AutoMapper;
using CarDealer.DTO.Export;
using CarDealer.DTO.Import;
using CarDealer.Models;
using System.Xml;

namespace CarDealer
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
                .ForMember(dest => dest.PartCars, opt => opt.MapFrom(src => src.Parts.Select(p => new PartCar { PartId = p })));

            CreateMap<CustomersImportDto, Customer>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.IsYoungDriver, opt => opt.MapFrom(src => src.IsYoungDriver));

            CreateMap<SalesImportDto, Sale>()
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId));

            CreateMap<Customer, CustomersExportDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.IsYoungDriver, opt => opt.MapFrom(src => src.IsYoungDriver));

            CreateMap<Car, CarsToyotaExportDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Make))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.TravelledDistance, opt => opt.MapFrom(src => src.TravelledDistance));

            CreateMap<Supplier, LocalSuppliersExportDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PartsCount, opt => opt.MapFrom(src => src.Parts.Count()));

            CreateMap<Car, CarsWithPartsExportDto.CarDto>()
                .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Make))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.TravelledDistance, opt => opt.MapFrom(src => src.TravelledDistance));

            CreateMap<Part, CarsWithPartsExportDto.PartDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<Car, CarsWithPartsExportDto.CarExportDto>()
                .ForMember(dest => dest.Car, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Parts, opt => opt.MapFrom(src => src.PartCars.Select(pc => pc.Part)));

            CreateMap<Customer, TotalSalesCustomersExportDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.BoughtCars, opt => opt.MapFrom(src => src.Sales.Count()))
                .ForMember(dest => dest.SpentMoney,
                opt => opt.MapFrom(src => src.Sales.SelectMany(s => s.Car.PartCars).Sum(p => p.Part.Price)));

            CreateMap<Car, SalesWithDiscountExportDto.CarDto>()
                .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Make))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.TravelledDistance, opt => opt.MapFrom(src => src.TravelledDistance));

            CreateMap<Customer, SalesWithDiscountExportDto.SalesWithDiscountDto>()
                .ForMember(dest => dest.Car, opt => opt.MapFrom(src => src.Sales.Select(s => s.Car).FirstOrDefault()))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new SalesWithDiscountExportDto.CustomerDto
                {
                    CustomerName = src.Name,
                    Discount = src.Sales.Select(s => s.Discount).FirstOrDefault(),
                    Price = Math.Round(src.Sales.Select(s => CalculateTotalPriceForCar(s.Car)).FirstOrDefault(), 2),
                    PriceWithDiscount = Math.Round(src.Sales.Select(s => CalculatePriceWithDiscount(s)).FirstOrDefault(), 2)
                }));
        }
        // Helper method to calculate total price for a car
        private decimal CalculateTotalPriceForCar(Car car)
        {
            decimal totalPrice = car.PartCars.Sum(pc => pc.Part.Price);
            return totalPrice;
        }

        // Helper method to calculate total price for a car with discount
        private decimal CalculatePriceWithDiscount(Sale sale)
        {
            decimal totalPrice = CalculateTotalPriceForCar(sale.Car);
            decimal discount = sale.Discount;
            decimal discountedPrice = totalPrice * (1 - (discount / 100));
            return discountedPrice;
        }
    }
}
