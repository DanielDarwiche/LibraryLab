using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryLab_Api.Data;
using LibraryLab_Api.Models;
using LibraryLab_Api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryLab_Api.Services
{
    public class ApiFunctionsRepo : IApiFunctionsRepository<Book>
    {
        private readonly DataContext _context;
        public ApiFunctionsRepo(DataContext context) //DI for db-connection 
        {
            _context = context;
        }
        public async Task<IEnumerable<Book>> GetAllAvailable()
        {
            var availablebooklist = await _context.Books.Where(b => b.AvailableToBorrow == true).ToListAsync();
            return availablebooklist;
        }
        public async Task<IEnumerable<Book>> GetAllUnavailable()
        {
            var unavailablebooklist = await _context.Books.Where(b => b.AvailableToBorrow == false).ToListAsync();
            return unavailablebooklist;
        }
        public async Task<IEnumerable<Book>> GetAllOrderedByYear()
        {
            var booklist = await _context.Books.OrderByDescending(b => b.YearOfPublication).Reverse()
                .ToListAsync();
            return booklist;
        }
        public async Task<IEnumerable<Book>> GetByTitle([FromRoute] string title)
        {
            var booksWithSameTitle = await _context.Books.Where(book => book.Title.ToUpper() == title.ToUpper()).ToListAsync();
            if (booksWithSameTitle.Count > 0)
            {
                return booksWithSameTitle;
            }
            return null;
        }
        public async Task<IEnumerable<Book>> GetByAuthor([FromRoute] string author)
        {
            var booksWithSameAuthor = await _context.Books.Where(book => book.Author.ToUpper() == author.ToUpper()).ToListAsync();
            if (booksWithSameAuthor.Count > 0)
            {
                return booksWithSameAuthor;
            }
            return null;
        }
    }
}

