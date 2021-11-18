using AutoMapper;
using CarDealer.Dto;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<Car, CarOutputDTO>();
        }
    }
}
