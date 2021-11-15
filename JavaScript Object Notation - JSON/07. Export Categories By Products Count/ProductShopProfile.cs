namespace ProductShop
{
    using AutoMapper;
    using ProductShop.DTO_S;
    using ProductShop.Models;
    using System.Linq;

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

            CreateMap<Category, CategoryOutputDTO>()
                .ForMember(dest=>dest.Category,y=>y.MapFrom(src=>src.Name))
                .ForMember(dest => dest.ProductsCount, y => y.MapFrom(src => src.CategoryProducts.Count))
                .ForMember(dest => dest.AveragePrice,
                     y => y
                    .MapFrom(src => (src.CategoryProducts.Sum(x => x.Product.Price)/src.CategoryProducts.Count).ToString("f2"))
                   )
                .ForMember(dest => dest.TotalRevenue,
                    y => y
                    .MapFrom(src => src.CategoryProducts
                    .Select(x => x.Product.Price)
                    .Sum().ToString("f2")));
        }
    }
}
