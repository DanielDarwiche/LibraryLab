using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation.Internal;
using LibraryLab.Data;
using LibraryLab.Models;
using LibraryLab.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace LibraryLab.Services
{
    public class BookRepository : IGenericRepository<BookDTO>   //Working with BookDTO, to conseal properties
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public BookRepository(DataContext database, IMapper mapper) //Dependency Injection of DB-Connection and IMapper for BookDTO
        {
            _mapper = mapper;
            _context = database;
        }
        public async Task<IEnumerable<BookDTO>> GetAll()
        {
            var booklist = await _context.Books.ProjectTo<BookDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return booklist;  //Db:s Books are mapping BookDTO:s and returning a list of the DTO:s
        }
        public async Task<BookDTO> GetById(int id)  //Finding databases book after entered id
        {
            var book = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);
            return _mapper.Map<BookDTO>(book);   //returns a mapped dto of the book
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
        public async Task<BookDTO> Update(int id, BookDTO bookdto)
        {
            //looking if  booktoupdate exists in database by this id
            var BookToUpdate = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (BookToUpdate != null)           //If book is found by id all the props are updated
            {
                BookToUpdate.Title = bookdto.Title;
                BookToUpdate.Author = bookdto.Author;
                BookToUpdate.Genre = bookdto.Genre;
                BookToUpdate.YearOfPublication = bookdto.YearOfPublication;
                BookToUpdate.Description = bookdto.Description;
                BookToUpdate.AvailableToBorrow = bookdto.AvailableToBorrow;
                await _context.SaveChangesAsync();
                return _mapper.Map<BookDTO>(BookToUpdate); //Saving update and returning dto of book!
            }
            return null;    //if the book is not found by its id in the list, null is returned
        }
        public async Task<BookDTO> Delete(int id)
        {
            var BookToDelete = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);
            if (BookToDelete != null) //if book id is found in database
            {
                _context.Books.Remove(BookToDelete);
                await _context.SaveChangesAsync();
                return _mapper.Map<BookDTO>(BookToDelete); //book deleted and dto returned
            }
            return null; //will be null if book is not found
        }
    }
}
