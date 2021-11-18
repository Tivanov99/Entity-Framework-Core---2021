using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<Product, ProductOutputDTO>()
                .ForMember(dest => dest.BuyerFullName, y => y.MapFrom(src => $"{src.Buyer.FirstName} {src.Buyer.LastName}"));

            CreateMap<User, UserOutputDTO>()
                .ForMember(dest => dest.SoldProducts, y => y.MapFrom(src => src.ProductsSold));
        }
    }
}
