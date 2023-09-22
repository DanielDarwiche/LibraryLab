using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation.Internal;
using LibraryLab_Api.Data;
using LibraryLab_Api.Models;
using LibraryLab_Api.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace LibraryLab_Api.Services
{
    public class BookRepository : IGenericRepository<Book, BookDTO>   //Working with BookDTO, to conseal properties
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public BookRepository(DataContext database, IMapper mapper) //Dependency Injection of DB-Connection and IMapper for BookDTO
        {
            _mapper = mapper;
            _context = database;
        }
        public async Task<IEnumerable<Book>> GetAll()
        {
            var booklist = await _context.Books.ToListAsync();
            return booklist;
        }
        public async Task<Book> GetById(int id)  //Finding databases book after entered id
        {
            var book = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);
            return book;
        }
        public async Task<BookDTO> Create(BookDTO bookdtoToAdd)
        {
            // controlling if book already exists by title and author
            var existingBook = await _context.Books.FirstOrDefaultAsync(
                b => b.Title.ToLower() == bookdtoToAdd.Title.ToLower() && b.Author.ToLower() == bookdtoToAdd.Author.ToLower());
            if (existingBook != null)
            {
                return null; // book with this id already exists
            }
            //no duplicate title/author => mapping book with dto and creating below
            var bookcreated = _mapper.Map<Book>(bookdtoToAdd);
            var addedBook = await _context.Books.AddAsync(bookcreated);
            await _context.SaveChangesAsync();
            return _mapper.Map<BookDTO>(addedBook.Entity);  //returning dto after adding to database
        }
        public async Task<Book> Update(int id, Book book)
        {
            //looking if  booktoupdate exists in database by this id
            var BookToUpdate = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (BookToUpdate != null)           //If book is found by id all the props are updated
            {
                BookToUpdate.Title = book.Title;
                BookToUpdate.Author = book.Author;
                BookToUpdate.Genre = book.Genre;
                BookToUpdate.YearOfPublication = book.YearOfPublication;
                BookToUpdate.Description = book.Description;
                BookToUpdate.AvailableToBorrow = book.AvailableToBorrow;
                await _context.SaveChangesAsync();
                return BookToUpdate; //Saving update and returning updated book
            }
            return null;    //if the book is not found by its id in the list, null is returned
        }
        public async Task<Book> Delete(int id)
        {
            var BookToDelete = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);
            if (BookToDelete != null) //if book id is found in database
            {
                _context.Books.Remove(BookToDelete);
                await _context.SaveChangesAsync();
                return BookToDelete; //book deleted and returned
            }
            return null; //will be null if book is not found
        }
    }
}
