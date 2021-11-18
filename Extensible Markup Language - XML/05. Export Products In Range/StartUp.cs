using ProductShop.Data;
using System.IO;
using System.Xml.Linq;
using System;
using ProductShop.Models;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProductShop.Dtos.Export;
using AutoMapper.QueryableExtensions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{
    public class StartUp
    {
        public static IMapper mapper;

        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();
            //context.Database.EnsureCreated();

            //string users = File.ReadAllText("../../../Datasets/users.xml");
            //Console.WriteLine(ImportUsers(context, users));

            //string products = File.ReadAllText("../../../Datasets/products.xml");
            //Console.WriteLine(ImportProducts(context, products));

            //string categories = File.ReadAllText("../../../Datasets/categories.xml");
            //Console.WriteLine(ImportCategories(context, categories));

            //string categogiesPRoducts = File.ReadAllText("../../../Datasets/categories-products.xml");
            //Console.WriteLine(ImportCategoryProducts(context, categogiesPRoducts));

            Console.WriteLine(GetProductsInRange(context));
        }
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var root = XDocument.Parse(inputXml).Root.Elements()
                .Where(x => int.Parse(x.Element("age").Value) > 40);

            var users = XDocument.Parse(inputXml).Root.Elements();

            List<User> ParsedUserrs = new List<User>();

            foreach (var user in users)
            {
                string firstName = user.Element("firstName").Value;
                string lastName = user.Element("lastName").Value;
                int age = int.Parse(user.Element("age").Value);
                User user1 = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Age = age,
                };
                ParsedUserrs.Add(user1);
            }


            return $"Successfully imported {ParsedUserrs.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var products = XDocument.Parse(inputXml).Root.Elements();

            List<Product> ParsedProducts = new List<Product>();

            foreach (var product in products)
            {
                string name = product.Element("name").Value;
                decimal price = decimal.Parse(product.Element("price").Value);
                int sellerId = int.Parse(product.Element("sellerId").Value);

                Product TryProduct = product.Element("sellerId").NextNode != null ?
                        new Product()
                        {
                            Name = name,
                            Price = price,
                            SellerId = sellerId,
                            BuyerId = int.Parse(product.Element("buyerId").Value)
                        }
                        :
                         new Product()
                         {
                             Name = name,
                             Price = price,
                             SellerId = sellerId
                         };
                ParsedProducts.Add(TryProduct);
            }
            context.Products.AddRange(ParsedProducts);
            context.SaveChanges();
            return $"Successfully imported {ParsedProducts.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            List<Category> elements = XDocument.Parse(inputXml).Root.Elements()
                .Where(x => x.Elements().Count() >= 1)
                .Select(x => new Category() { Name = x.Element("name").Value })
                .ToList();

            //var test = XDocument.Parse(inputXml).Root.Elements();

            //List<Category> categories = new List<Category>();

            //foreach (var category in elements)
            //{
            //    string name = category.Element("name").Value;
            //    categories.Add(new Category() { Name = name });
            //}
            context
                .Categories
                .AddRange(elements);
            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            List<int> products = context
                .Products
                .Select(x => x.Id)
                .ToList();

            List<int> categories =
                context
                .Categories
                .Select(c => c.Id)
                .ToList();

            var result = XDocument.Parse(inputXml).Root.Elements();

            foreach (var categoryProduct in result)
            {
                int CategoryId = int.Parse(categoryProduct.Element("CategoryId").Value);
                int ProductId = int.Parse(categoryProduct.Element("ProductId").Value);
                if (products.Contains(ProductId) && categories.Contains(CategoryId))
                {
                    context.CategoryProducts
                         .Add(new CategoryProduct()
                         {
                             CategoryId = CategoryId,
                             ProductId = ProductId,
                         });
                }
            }
            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            Initialize();

            List<ProductOutputDTO> productOutputDTOs = context
                  .Products
                  .Include(p => p.Buyer)
                  .Where(p => p.Price >= 500 && p.Price <= 1000)
                  .ProjectTo<ProductOutputDTO>(mapper.ConfigurationProvider)
                  .OrderBy(p => p.Price)
                  .Take(10)
                  .ToList();

            var serializer = new XmlSerializer(typeof(List<ProductOutputDTO>), new XmlRootAttribute("Products"));


            var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();

            ns.Add("", "");

            serializer.Serialize(textWriter, productOutputDTOs, ns);

            return textWriter.ToString();
        }
        public static void Initialize()
        {
            MapperConfiguration configuration = new MapperConfiguration(cnfg =>
            {
                cnfg.AddProfile<ProductShopProfile>();
            });
            mapper = new Mapper(configuration);
        }
    }
}