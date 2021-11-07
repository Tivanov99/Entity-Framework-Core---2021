using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop
{
    public class BookDTO
    {
        public string Title { get; set; }

        public decimal Price { get; set; }

        public string AuthorName { get; set; }
    }
}
