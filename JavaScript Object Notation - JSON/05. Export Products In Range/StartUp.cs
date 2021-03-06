using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTO_S;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static IMapper mapper;

        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureCreated(); 
            //string reader = File.ReadAllText("../../../Datasets/users.json");
            //Console.WriteLine(ImportUsers(context, reader));

            //string data = File.ReadAllText("../../../Datasets/products.json");
            //Console.WriteLine(ImportProducts(context, data));

            //string data1 = File.ReadAllText("../../../Datasets/categories.json");
            //Console.WriteLine(ImportCategories(context, data1));

            //string data2 = File.ReadAllText("../../../Datasets/categories-products.json");
            //Console.WriteLine(ImportCategoryProducts(context, data2));
            Console.WriteLine(GetProductsInRange(context));
        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            User[] users = JsonConvert.DeserializeObject<User[]>(inputJson);
            context.Users.AddRange(users);
            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            Product[] products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.Products.AddRange(products);

            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(inputJson)
                 .Where(x => x.Name != null)
                 .ToList();

            context.Categories.AddRange(categories);
            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            CategoryProduct[] categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
            context.CategoryProducts.AddRange(categoryProducts);
            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            Initialize();

            List<Product> products = context
                 .Products
                 .Include(p => p.Seller)
                 .Where(p => p.Price >= 500 && p.Price <= 1000)
                 .OrderBy(p => p.Price)
                 //.ProjectTo<ProductDTO>(mapper.ConfigurationProvider)
                 .ToList();

            List<ProductDTO> productsDTO = mapper.Map<List<ProductDTO>>(products);

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    OverrideSpecifiedNames = false
                }
            };

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = contractResolver
            };
            string result = JsonConvert.SerializeObject(productsDTO, serializerSettings);
            return result;
        }
        public static void Initialize()
        {
            var config = new MapperConfiguration(cnfg =>
            {
                cnfg.AddProfile<ProductShopProfile>();
            });
            mapper = new Mapper(config);
        }
    }
}