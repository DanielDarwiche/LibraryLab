using AutoMapper;
using LibraryLab.Models;
using LibraryLab.Models.DTO;

namespace LibraryLab
{
    public class AutoMapperConfig : Profile     //  Profile entered for AutoMapper
    {
        public AutoMapperConfig()
        {
            CreateMap<Book, BookDTO>().ReverseMap();  //Maps the classes. ReverseMap maps classes on both ways!
        }
    }
}
