
using FluentValidation;
using LibraryLab.Models.DTO;

namespace LibraryLab.Validations
{
    public class BookValidation : AbstractValidator<BookDTO>   //Validating book 
    {
        public BookValidation()
        {
            RuleFor(book => book.Title).NotEmpty();
            RuleFor(book => book.Author).NotEmpty();
            RuleFor(book => book.YearOfPublication).NotEmpty().InclusiveBetween(0, 2023);
        }
    }
}