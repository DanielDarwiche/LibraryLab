using AutoMapper;
using LibraryLab_Api.Models;
using LibraryLab_Api.Models.DTO;

namespace LibraryLab_Api
{
    public class AutoMapperConfig : Profile     //  Profile entered for AutoMapper
    {
        public AutoMapperConfig()
        {
            CreateMap<Book, BookDTO>().ReverseMap();  //Maps the classes. ReverseMap maps classes on both ways!
        }
    }
}
