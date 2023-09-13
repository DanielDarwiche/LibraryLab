using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryLab.Data;
using LibraryLab.Models;
using LibraryLab.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryLab.Services
{
    public class ApiFunctionsRepo : IApiFunctionsRepository<BookDTO>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public ApiFunctionsRepo(DataContext context, IMapper mapper) //DI for db-connection and mapping
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDTO>> GetAllAvailable()
        {
            var availablebooklist = await _context.Books.ProjectTo<BookDTO>(_mapper.ConfigurationProvider)
                .Where(b => b.AvailableToBorrow == true).ToListAsync();
            return availablebooklist;
        }
        public async Task<IEnumerable<BookDTO>> GetAllUnavailable()
        {
            var unavailablebooklist = await _context.Books.ProjectTo<BookDTO>(_mapper.ConfigurationProvider)
                .Where(b => b.AvailableToBorrow == false).ToListAsync();
            return unavailablebooklist;
        }
        public async Task<IEnumerable<BookDTO>> GetAllOrderedByYear()
        {
            var booklist = await _context.Books.ProjectTo<BookDTO>(_mapper.ConfigurationProvider)
                .OrderByDescending(b => b.YearOfPublication).Reverse()
                .ToListAsync();
            return booklist;
        }

        public async Task<IEnumerable<BookDTO>> GetByTitle([FromRoute] string title)
        {
            var booksWithSameTitle = await _context.Books.ProjectTo<BookDTO>(_mapper.ConfigurationProvider)
                .Where(book => book.Title.ToUpper() == title.ToUpper()).ToListAsync();
            if (booksWithSameTitle.Count > 0)
            {
                return _mapper.Map<IEnumerable<BookDTO>>(booksWithSameTitle);
            }
            return null;
        }

        public async Task<IEnumerable<BookDTO>> GetByAuthor([FromRoute] string author)
        {
            var booksWithSameAuthor = await _context.Books.ProjectTo<BookDTO>(_mapper.ConfigurationProvider)
                .Where(book => book.Author.ToUpper() == author.ToUpper()).ToListAsync();
            if (booksWithSameAuthor.Count > 0)
            {
                return _mapper.Map<IEnumerable<BookDTO>>(booksWithSameAuthor);
            }
            return null;
        }
    }
}

