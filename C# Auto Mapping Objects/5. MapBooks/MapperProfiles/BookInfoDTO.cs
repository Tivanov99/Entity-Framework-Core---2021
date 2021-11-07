using AutoMapper;
using BookShop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookShop.MapperProfiles
{
    public class BookInfoDTO : Profile
    {
        public BookInfoDTO()
        {
            this.CreateMap<Book, BookInfoDTO>()
                .ForMember(b => b.AuthorFullName,
                             p => p.MapFrom(
                                 b => $"{b.Author.FirstName} {b.Author.LastName}")
                             );
        }

        public string AuthorFullName { get; set; }

        public string Title { get; set; }
    }
}
