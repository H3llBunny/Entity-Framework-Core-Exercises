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
                .ForMember(src => src.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForMember(src => src.IsImporter, opt => opt.MapFrom(dto => dto.IsImporter));

            CreateMap<PartsImportDto, Part>()
                .ForMember(src => src.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForMember(src => src.Price, opt => opt.MapFrom(dto => dto.Price))
                .ForMember(src => src.Quantity, opt => opt.MapFrom(dto => dto.Quantity))
                .ForMember(src => src.SupplierId, opt => opt.MapFrom(dto => dto.SupplierId));

            CreateMap<CarsImportDto, Car>()
                .ForMember(src => src.Make, opt => opt.MapFrom(dto => dto.Make))
                .ForMember(src => src.Model, opt => opt.MapFrom(dto => dto.Model))
                .ForMember(src => src.TravelledDistance, opt => opt.MapFrom(dto => dto.TravelledDistance))
                .ForMember(src => src.PartCars, opt => opt.MapFrom(src => src.Parts.Select(p => new PartCar { PartId = p })));

            CreateMap<CustomersImportDto, Customer>()
                .ForMember(src => src.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForMember(src => src.BirthDate, opt => opt.MapFrom(dto => dto.BirthDate))
                .ForMember(src => src.IsYoungDriver, opt => opt.MapFrom(dto => dto.IsYoungDriver));

            CreateMap<SalesImportDto, Sale>()
                .ForMember(src => src.Discount, opt => opt.MapFrom(dto => dto.Discount))
                .ForMember(src => src.CarId, opt => opt.MapFrom(dto => dto.CarId))
                .ForMember(src => src.CustomerId, opt => opt.MapFrom(dto => dto.CustomerId));

            CreateMap<Customer, CustomersExportDto>()
                .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dto => dto.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dto => dto.IsYoungDriver, opt => opt.MapFrom(src => src.IsYoungDriver));

            CreateMap<Car, CarsToyotaExportDto>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dto => dto.Make, opt => opt.MapFrom(src => src.Make))
                .ForMember(dto => dto.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dto => dto.TravelledDistance, opt => opt.MapFrom(src => src.TravelledDistance));

            CreateMap<Supplier, LocalSuppliersExportDto>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dto => dto.PartsCount, opt => opt.MapFrom(src => src.Parts.Count()));

            CreateMap<Car, CarsWithPartsExportDto.CarDto>()
                .ForMember(dto => dto.Make, opt => opt.MapFrom(src => src.Make))
                .ForMember(dto => dto.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dto => dto.TravelledDistance, opt => opt.MapFrom(src => src.TravelledDistance));

            CreateMap<Part, CarsWithPartsExportDto.PartDto>()
                .ForMember(dto => dto.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dto => dto.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<Car, CarsWithPartsExportDto.CarExportDto>()
                .ForMember(dto => dto.Car, opt => opt.MapFrom(src => src))
                .ForMember(dto => dto.Parts, opt => opt.MapFrom(src => src.PartCars.Select(pc => pc.Part)));

            CreateMap<Customer, TotalSalesCustomersExportDto>()
                .ForMember(dto => dto.FullName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dto => dto.BoughtCars, opt => opt.MapFrom(src => src.Sales.Count()))
                .ForMember(dto => dto.SpentMoney,
                opt => opt.MapFrom(src => src.Sales.SelectMany(s => s.Car.PartCars).Sum(p => p.Part.Price)));

            CreateMap<Car, SalesWithDiscountExportDto.CarDto>()
                .ForMember(dto => dto.Make, opt => opt.MapFrom(src => src.Make))
                .ForMember(dto => dto.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dto => dto.TravelledDistance, opt => opt.MapFrom(src => src.TravelledDistance));

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
