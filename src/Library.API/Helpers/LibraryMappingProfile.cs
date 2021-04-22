using AutoMapper;
using Library.API.Entities;
using Library.API.Extensions;
using Library.API.Models;
using System;

namespace Library.API.Helpers
{
    public class LibraryMappingProfile : Profile
    {
        public LibraryMappingProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.Age, config => config.MapFrom(src => src.BirthDate.GetCurrentAge()));
            CreateMap<AuthorForCreationDto, Author>()
                .ForMember(dest => dest.BirthDate, config => config.MapFrom(src => DateTimeOffset.Now.AddYears(src.Age * -1)));

            CreateMap<Book, BookDto>();
            CreateMap<BookForCreationDto, Book>()
                .ReverseMap();
            CreateMap<BookForUpdateDto, Book>()
                .ReverseMap();
        }
    }
}
