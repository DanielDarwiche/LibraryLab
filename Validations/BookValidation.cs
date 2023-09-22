
using FluentValidation;
using LibraryLab_Api.Models;
using LibraryLab_Api.Models.DTO;

namespace LibraryLab_Api.Validations
{
    public class BookValidation : AbstractValidator<Book>
    {
        public BookValidation()
        {
            RuleFor(book => book.Title).NotEmpty();
            RuleFor(book => book.Author).NotEmpty();
            RuleFor(book => book.YearOfPublication).NotEmpty().InclusiveBetween(-3000, 2023);
        }
    }
    public class BookDTOValidation : AbstractValidator<BookDTO>
    {
        public BookDTOValidation()
        {
            RuleFor(bookDTO => bookDTO.Title).NotEmpty();
            RuleFor(bookDTO => bookDTO.Author).NotEmpty();
            RuleFor(bookDTO => bookDTO.YearOfPublication).NotEmpty().InclusiveBetween(-3000, 2023);
        }
    }

}