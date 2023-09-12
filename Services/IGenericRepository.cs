using LibraryLab.Models;

namespace LibraryLab.Services
{
    public interface IGenericRepository<T> //Generic Repository interface for Book
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> Create(T t);
        Task<T> Update(int id, T t);
        Task<T> Delete(int id);
    }
}
