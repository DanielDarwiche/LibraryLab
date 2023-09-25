using LibraryLab_MVC.Models;

namespace LibraryLab_MVC.Services
{
    public interface IBaseService : IDisposable
    {
        ResponseDTO responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
