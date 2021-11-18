using AutoMapper;
using CarDealer.Dto;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<Car, CarOutputDTO>();

            CreateMap<Car, BmwDTO>();

            CreateMap<Supplier, SuppliersDTO>()
                .ForMember(dest => dest.PartsCount, y => y.MapFrom(src => src.Parts.Count));
        }
    }
}
