using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<Supplier, SupplierDTO>();

            CreateMap<Customer, CustomerDTOOutput>()
                .ForMember(dest=>dest.BirthDate,y=>y.MapFrom(src=>src.BirthDate.ToString(@"dd\/MM\/yyyy")));
        }
    }
}
