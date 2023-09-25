namespace LibraryLab_Api.Services
{
    public interface IApiFunctionsRepository<TBook> // Repository interface for ApiFunctionsRepository
    {
        Task<IEnumerable<TBook>> GetAllAvailable();
        Task<IEnumerable<TBook>> GetAllUnavailable();
        Task<IEnumerable<TBook>> GetAllOrderedByYear();
        Task<IEnumerable<TBook>> GetByTitle(string title);
        Task<IEnumerable<TBook>> GetByAuthor(string author);
    }
}
