namespace ProductShop
{
    using AutoMapper;
    using ProductShop.DTO_S;
    using ProductShop.Models;

    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.Seller, y => y.MapFrom(src => $"{src.Seller.FirstName} {src.Seller.LastName}"));

            CreateMap<Product, SoldProductDTO>()
                .ForMember(dest => dest.BuyerFirstName, y => y.MapFrom(src => src.Buyer.FirstName))
                .ForMember(dest => dest.BuyerLastName, y => y.MapFrom(src => src.Buyer.LastName));

            CreateMap<User, UserOutputDTO>()
                .ForMember(dest => dest.SoldProducts, y => y.MapFrom(src => src.ProductsSold));
        }
    }
}
