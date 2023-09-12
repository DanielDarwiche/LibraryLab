using AutoMapper;
using FluentValidation;
using LibraryLab;
using LibraryLab.Data;
using LibraryLab.Models;
using LibraryLab.Models.DTO;
using LibraryLab.Services;
using LibraryLab.Validations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Nuget packages: Automapper and AutoMapper.Extensions.Microsoft.DependencyInjection
// FluentValidation, fluentvalidation dependency extension and Entity Framework Core  

builder.Services.AddDbContext<DataContext>();
//  adding database connection
builder.Services.AddAutoMapper(typeof(AutoMapperConfig).Assembly);
//  Adding AutMapper referencing file AutoMapperConfig
builder.Services.AddScoped<IGenericRepository<BookDTO>, BookRepository>();
//  registering repository pattern files as a service


// BookAddValitation   BookUpdateValidation
//builder.Services.AddValidatorsFromAssemblyContaining<Program>(); //från programklassen, här



var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();


// Endpoint refencing Repositories and their logic. ILogger logs whats happening
app.MapGet("/api/book", async (IGenericRepository<BookDTO> bookrepo, ILogger<Program> _programLoggaren) =>
{
    _programLoggaren.Log(LogLevel.Information, "Getting all books");
    var allBooks = await bookrepo.GetAll(); //using GetAll to display BookDTO:s
    return Results.Ok(allBooks);
}).WithName("GetBooks").Produces<IEnumerable<BookDTO>>(200).Produces(400);

app.MapGet("/api/book/{id:int}", async (IGenericRepository<BookDTO> bookrepo, int id, ILogger<Program> _programLoggaren) =>
{
    var bookToFind = await bookrepo.GetById(id);
    if (bookToFind != null) //if the Id exists in database its not null and the book is returned
    {
        _programLoggaren.Log(LogLevel.Information, "SUCCEDING IN:   Fetching a book via book-ID ");
        return Results.Ok(bookToFind);
    }
    _programLoggaren.Log(LogLevel.Information, "FAILING IN:   Fetching a book via book-ID");
    return Results.NotFound($"A book with id '{id}' could not be found");
}).WithName("GetSingleBook").Produces<BookDTO>(200).Produces(400);

app.MapPost("/api/book", async (IGenericRepository<BookDTO> bookrepo, BookDTO bookdto, ILogger<Program> _programLoggaren) =>
{
    var addingBook = await bookrepo.Create(bookdto);
    if (addingBook == null)     //Logger information informs you of whats happening
    {
        _programLoggaren.Log(LogLevel.Information, "Creating a book failed ; MapPost");
        return Results.BadRequest("Book with the title and author already exists");
    }
    _programLoggaren.Log(LogLevel.Information, "Created a book via MapPost");
    return Results.Ok(addingBook);
}).WithName("CreateBook").Accepts<BookDTO>("application/json").Produces<Book>(201).Produces(400);

app.MapPut("/api/book/{id:int}", async (IGenericRepository<BookDTO> bookrepo, BookDTO book, int id
 , ILogger<Program> _programLoggaren) =>
{
    var updatebook = await bookrepo.Update(id, book);
    if (updatebook != null)     //Logger information informs you of whats happening
    {
        _programLoggaren.Log(LogLevel.Information, "Updating a book succeesfully");
        // Creating and returning RequestResponse with message and following updated book
        var RequestResponse = new
        {
            ResponseMessage = "The updated book:",
            Book = updatebook
        };
        return Results.Ok(RequestResponse);
    }
    _programLoggaren.Log(LogLevel.Information, "Updating a book failed");
    return Results.NotFound($"Book with id '{id}' not found");
}).WithName("UpdateBook").Produces(200).Produces(400);

app.MapDelete("/api/Book/{id:int}", async (IGenericRepository<BookDTO> bookrepo, int id,
   ILogger<Program> _programLoggaren) =>
{
    var DeleteBook = await bookrepo.Delete(id);
    if (DeleteBook != null)      //Logger information informs you of whats happening
    {
        _programLoggaren.Log(LogLevel.Information, "Deleting a book succeeded");
        // Creating and returning RequestResponse with message and following deleted book
        var RequestResponse = new
        {
            ResponseMessage = "The book that got deleted:",
            Book = DeleteBook
        };
        return Results.Ok(RequestResponse);
    }
    _programLoggaren.Log(LogLevel.Information, "Deleting a book failed");
    return Results.NotFound($"Book not found");
}).WithName("DeletingBook").Produces(200).Produces(400);


//app.MapPost("/api/coupon", async (
//    IValidator<BookDTO> validator,
//{
//    var validationResult = await validator.ValidateAsync(book_DTO);
//    if (!validationResult.IsValid)
//    {
//        _programLoggaren.Log(LogLevel.Information, "FAILING IN:   ADDING a book");
//        return Results.BadRequest();
//    }
//    if (Library.bookList.FirstOrDefault(c => c.Title.ToLower() == book_DTO.Title.ToLower()) != null)
//    {
//        _programLoggaren.Log(LogLevel.Information, "FAILING IN:   ADDING a book");
//        return Results.BadRequest($"A book with this Title ({book_DTO.Title}) already Exists");
//    }
//    _programLoggaren.Log(LogLevel.Information, "SUCCEEDING IN:   ADDING a book");

//    Book book = _mapper.Map<Book>(book_DTO);
//    book.Id = Library.bookList.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
//    Library.bookList.Add(book);

//    BookDTO bookdto = _mapper.Map<BookDTO>(book);
//    return Results.Ok(bookdto);

//}).WithName("CreateBook").Accepts<BookDTO>("application/json").Produces<Book>(201).Produces(400);

/*
 * 
 app.MapPost("/api/book", async ( 

IValidator<Book> _validator
{
    var bookToAdd = await bookrepo.CreateBook<Book>(book);
    if (bookToAdd == null)
    {
        return Results.BadRequest($"A book with this Id ({book.Id}) already Exists");
    }
    return Results.Ok(bookToAdd);

    //var validationResult = await _validator.ValidateAsync(dtoBok);  //validerar efter RuleFor kraven 
    ////if (book.Id != 0) { return Results.BadRequest("Invalid id! Id must be 0"); }
    //if (Library.bookList.FirstOrDefault(b => b.Title.ToLower() == dtoBok.Title.ToLower()) != null)
    //{
    //    return Results.BadRequest(response);
    //}
    //if (string.IsNullOrEmpty(dtoBok.Title)) { return Results.BadRequest("Title can´t be empty!"); }
    //VAlidator gör detta!
    //if (!validationResult.IsValid)
    //{
    //    return Results.BadRequest(response);
    //}
 
*/


//VALIDERA???????????   
//💡 **Extra Utmaning: **    //söka  böcker   //efter författare    //eller genre
app.Run();