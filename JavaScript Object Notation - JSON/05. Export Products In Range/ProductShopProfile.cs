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
        }
    }
}
