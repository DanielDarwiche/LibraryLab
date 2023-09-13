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
using System.ComponentModel.DataAnnotations;
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
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// reg. Validation using BookValidation
builder.Services.AddScoped<ApiFunctionsRepo>();
//Registering ApiFunctions  for extra features
builder.Services.AddScoped<IApiFunctionsRepository<BookDTO>, ApiFunctionsRepo>();
//  registering repository pattern files as a service


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

app.MapPost("/api/book", async (IGenericRepository<BookDTO> bookrepo, BookDTO bookdto,
    ILogger<Program> _programLoggaren, IValidator<BookDTO> validator) =>
{
    // Using Validation before creating book
    var validationResult = await validator.ValidateAsync(bookdto);
    if (!validationResult.IsValid) //if book is INVALID it results in Error and BadRequest
    {
        var errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
        return Results.BadRequest(errors);
    }
    // If Validation is valid the following occurs 

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
 , ILogger<Program> _programLoggaren, IValidator<BookDTO> validator) =>
{
    // Using Validation before updating book
    var validationResult = await validator.ValidateAsync(book);
    if (!validationResult.IsValid) //if book is INVALID it results in Error and BadRequest
    {
        var errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
        return Results.BadRequest(errors);
    }
    // If Validation is valid the following occurs 

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
// ----------------------------------------------------------------------------------------------------
// ----------------------------------------------------------------------------------------------------
// --------------------  Endpoints for additional ApiFunctions below  ---------------------------------
// ----------------------------------------------------------------------------------------------------
// ----------------------------------------------------------------------------------------------------
app.MapGet("/api/book/available", async (IApiFunctionsRepository<BookDTO> apifunctions, ILogger<Program> _programLoggaren) =>
{
    _programLoggaren.Log(LogLevel.Information, "Getting all Available books");
    var allBooks = await apifunctions.GetAllAvailable();
    return Results.Ok(allBooks);
}).WithName("GetAvailableBooks").Produces<IEnumerable<BookDTO>>(200).Produces(400);

app.MapGet("/api/book/unavailable", async (IApiFunctionsRepository<BookDTO> apifunctions, ILogger<Program> _programLoggaren) =>
{
    _programLoggaren.Log(LogLevel.Information, "Getting all Unavailable books");
    var allBooks = await apifunctions.GetAllUnavailable();
    return Results.Ok(allBooks);
}).WithName("GetUnavailableBooks").Produces<IEnumerable<BookDTO>>(200).Produces(400);

app.MapGet("/api/book/year", async (IApiFunctionsRepository<BookDTO> apifunctions, ILogger<Program> _programLoggaren) =>
{
    _programLoggaren.Log(LogLevel.Information, "Getting all books ordered after year");
    var allBooks = await apifunctions.GetAllOrderedByYear();
    return Results.Ok(allBooks);
}).WithName("GetAllOrderedByYear").Produces<IEnumerable<BookDTO>>(200).Produces(400);

app.MapGet("/api/book/author/{author}", async (IApiFunctionsRepository<BookDTO> apifunctions, string author, ILogger<Program> _programLoggaren) =>
{
    var booksToFind = await apifunctions.GetByAuthor(author);
    if (booksToFind != null)
    {
        _programLoggaren.Log(LogLevel.Information, "Searching author: success");
        return Results.Ok(booksToFind);
    }
    _programLoggaren.Log(LogLevel.Information, "Searching author: failed");
    return Results.NotFound($"A book with author '{author}' could not be found");
}).WithName("GetByAuthor").Produces<IEnumerable<BookDTO>>(200).Produces(400);

app.MapGet("/api/book/title/{title}", async (IApiFunctionsRepository<BookDTO> apifunctions, string title, ILogger<Program> _programLoggaren) =>
{
    var booksToFind = await apifunctions.GetByTitle(title);
    if (booksToFind != null)
    {
        _programLoggaren.Log(LogLevel.Information, "Searching title: success");
        return Results.Ok(booksToFind);
    }
    _programLoggaren.Log(LogLevel.Information, "Searching title: failed");
    return Results.NotFound($"A book with author '{title}' could not be found");
}).WithName("GetByTitle").Produces<IEnumerable<BookDTO>>(200).Produces(400);



app.Run();