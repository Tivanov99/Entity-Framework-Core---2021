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
        }
    }
}
