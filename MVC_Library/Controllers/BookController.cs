using LibraryLab_Api.Models;
using LibraryLab_Api.Models.DTO;
using LibraryLab_MVC.Models;
using LibraryLab_MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace LibraryLab_MVC.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            this._bookService = bookService;
        }
        public async Task<IActionResult> Available()
        {
            List<Book> list = new List<Book>();
            var response = await _bookService.GetAllBooks<ResponseDTO>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<Book>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        public async Task<IActionResult> Unavailable()
        {
            List<Book> list = new List<Book>();
            var response = await _bookService.GetAllBooks<ResponseDTO>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<Book>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        public async Task<IActionResult> Year()
        {
            List<Book> list = new List<Book>();
            var response = await _bookService.GetAllBooks<ResponseDTO>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<Book>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        public async Task<IActionResult> IndexBook()
        {
            List<Book> list = new List<Book>();
            var response = await _bookService.GetAllBooks<ResponseDTO>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<Book>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        public async Task<IActionResult> DetailsBook(int id)
        {
            var response = await _bookService.GetBookById<ResponseDTO>(id);
            if (response != null && response.IsSuccess)
            {
                Book model = JsonConvert.DeserializeObject<Book>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
        public async Task<IActionResult> CreateBook()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateBook(BookDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _bookService.CreateBookAsync<ResponseDTO>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexBook));
                }
            }
            return View(model);
        }
        public async Task<IActionResult> UpdateBook(int id)
        {
            var response = await _bookService.GetBookById<ResponseDTO>(id);
            if (response != null && response.IsSuccess)
            {
                Book model = JsonConvert.DeserializeObject<Book>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBook(Book model)
        {
            if (ModelState.IsValid)         //Kontrollerar om Required attributen är angivna! på Book klassen
            {
                var response = await _bookService.UpdateBookAsync<ResponseDTO>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexBook));
                }
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteBook(int id)
        {
            var response = await _bookService.GetBookById<ResponseDTO>(id);
            if (response != null && response.IsSuccess)
            {
                Book model = JsonConvert.DeserializeObject<Book>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteBook(Book model)
        {
            var response = await _bookService.DeleteBookAsync<ResponseDTO>(model.Id);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexBook));
            }
            return NotFound();
        }
        public async Task<IActionResult> SearchByAuthor(string author)
        {
            List<Book> list = new List<Book>();
            var response = await _bookService.GetByAuthorAsync<ResponseDTO>(author);
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<Book>>(Convert.ToString(response.Result));
            }
            return View(list.Where(b => b.Author.ToUpper() == author.ToUpper()));
        }
        public async Task<IActionResult> SearchByTitle(string title)
        {
            List<Book> list = new List<Book>();
            var response = await _bookService.GetByTitleAsync<ResponseDTO>(title);
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<Book>>(Convert.ToString(response.Result));
            }
            return View(list.Where(b => b.Title.ToUpper() == title.ToUpper()));
        }
    }
}
