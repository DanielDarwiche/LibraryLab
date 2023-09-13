namespace LibraryLab.Services
{
    public interface IApiFunctionsRepository<T> // Repository interface for ApiFunctionsRepository
    {
        Task<IEnumerable<T>> GetAllAvailable();
        Task<IEnumerable<T>> GetAllUnavailable();
        Task<IEnumerable<T>> GetAllOrderedByYear();
        Task<IEnumerable<T>> GetByTitle(string title);
        Task<IEnumerable<T>> GetByAuthor(string author);
    }
}
