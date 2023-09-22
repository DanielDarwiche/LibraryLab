using LibraryLab_Api.Models;

namespace LibraryLab_Api.Services
{
    public interface IGenericRepository<TBook, TBookDTO> //Generic Repository interface for Book
    {
        Task<IEnumerable<TBook>> GetAll();
        Task<TBook> GetById(int id);
        Task<TBookDTO> Create(TBookDTO t);
        Task<TBook> Update(int id, TBook t);
        Task<TBook> Delete(int id);
    }
}