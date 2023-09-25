using LibraryLab_Api.Models;
using LibraryLab_Api.Models.DTO;

namespace LibraryLab_MVC.Services
{
    public interface IBookService
    {
        Task<T> GetAllBooks<T>();
        Task<T> GetBookById<T>(int id);
        Task<T> CreateBookAsync<T>(BookDTO book);
        Task<T> UpdateBookAsync<T>(Book book);
        Task<T> DeleteBookAsync<T>(int id);
        Task<T> GetAllAvailbleAsync<T>();
        Task<T> GetAllUnAvailbleAsync<T>();
        Task<T> GetAllOrderedByYearAsync<T>();
        Task<T> GetByTitleAsync<T>(string title);
        Task<T> GetByAuthorAsync<T>(string author);
    }
}
