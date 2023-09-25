using LibraryLab_Api.Models;
using LibraryLab_Api.Models.DTO;

namespace LibraryLab_MVC.Services
{
    public class BookService : BaseService, IBookService
    {
        private readonly IHttpClientFactory _clientFactory;
        public BookService(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<T> CreateBookAsync<T>(BookDTO book)
        {
            return await this.SendAsync<T>(new Models.ApiRequest
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = book,
                Url = StaticDetails.BookApiBase + "/api/book",
                AccessToken = ""
            });
        }
        public async Task<T> DeleteBookAsync<T>(int id)
        {
            return await this.SendAsync<T>(new Models.ApiRequest
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = StaticDetails.BookApiBase + "/api/book/" + id,
                AccessToken = ""
            });
        }

        public Task<T> GetAllAvailbleAsync<T>()
        {
            return this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.BookApiBase + "/api/books/available",
                AccessToken = ""
            });
        }

        public Task<T> GetAllBooks<T>()
        {
            return this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.BookApiBase + "/api/books",
                AccessToken = ""
            });
        }

        public Task<T> GetAllOrderedByYearAsync<T>()
        {
            return this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.BookApiBase + "/api/books/year",
                AccessToken = ""
            });
        }

        public Task<T> GetAllUnAvailbleAsync<T>()
        {
            return this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.BookApiBase + "/api/books/unavailable",
                AccessToken = ""
            });
        }

        public async Task<T> GetBookById<T>(int id)
        {
            return await this.SendAsync<T>(new Models.ApiRequest
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.BookApiBase + "/api/book/" + id,
                AccessToken = ""
            });
        }
        public async Task<T> UpdateBookAsync<T>(Book book)
        {
            return await this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = book,
                Url = StaticDetails.BookApiBase + $"/api/book/{book.Id}",
                AccessToken = ""
            });
        }
        public async Task<T> GetByAuthorAsync<T>(string author)
        {
            return await this.SendAsync<T>(new Models.ApiRequest
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.BookApiBase + "/api/book/author/" + author,
                AccessToken = ""
            });
        }

        public async Task<T> GetByTitleAsync<T>(string title)
        {
            return await this.SendAsync<T>(new Models.ApiRequest
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.BookApiBase + "/api/book/title/" + title,
                AccessToken = ""
            });
        }
    }
}
