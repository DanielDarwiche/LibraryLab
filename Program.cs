using AutoMapper;
using FluentValidation;
using LibraryLab_Api;
using LibraryLab_Api.Data;
using LibraryLab_Api.Models;
using LibraryLab_Api.Models.DTO;
using LibraryLab_Api.Services;
using LibraryLab_Api.Validations;
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
builder.Services.AddScoped<IGenericRepository<Book, BookDTO>, BookRepository>();
//  registering repository pattern files as a service
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// reg. Validation using BookValidation
builder.Services.AddScoped<ApiFunctionsRepo>();
//Registering ApiFunctions  for extra features
builder.Services.AddScoped<IApiFunctionsRepository<Book>, ApiFunctionsRepo>();
//  registering repository pattern files as a service


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();


// Endpoint refencing Repositories and their logic. ILogger logs whats happening
app.MapGet("/api/books", async ([FromServices] IGenericRepository<Book, BookDTO> bookrepo, ILogger<Program> _programLoggaren) =>
{
    _programLoggaren.Log(LogLevel.Information, "\nGetting all books");

    ApiResponse response = new ApiResponse();
    response.Result = await bookrepo.GetAll(); //using GetAll to display books
    response.IsSuccess = true;
    response.StatusCode = System.Net.HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("GetBooks").Produces<IEnumerable<Book>>(200).Produces(400);

app.MapGet("/api/book/{id:int}", async ([FromServices] IGenericRepository<Book, BookDTO> bookrepo, int id, ILogger<Program> _programLoggaren) =>
{
    ApiResponse response = new ApiResponse();
    response.IsSuccess = true;
    response.StatusCode = System.Net.HttpStatusCode.OK;

    response.Result = await bookrepo.GetById(id);
    if (response.Result != null) //if the Id exists in database its not null and the book is returned
    {
        _programLoggaren.Log(LogLevel.Information, "\nSUCCEDING IN:   Fetching a book via book-ID ");
        return Results.Ok(response);
    }
    _programLoggaren.Log(LogLevel.Error, "\nFAILING IN:   Fetching a book via book-ID");
    response.IsSuccess = false; response.StatusCode = System.Net.HttpStatusCode.NotFound;
    return Results.NotFound(response);

}).WithName("GetSingleBook").Produces<Book>(200).Produces(400);

app.MapPost("/api/book", async ([FromServices] IGenericRepository<Book, BookDTO> bookrepo, BookDTO bookdto,
    ILogger<Program> _programLoggaren, IValidator<BookDTO> validator) =>
{
    ApiResponse response = new ApiResponse();
    response.IsSuccess = false;
    response.StatusCode = System.Net.HttpStatusCode.BadRequest;

    // Using Validation before creating book
    var validationResult = await validator.ValidateAsync(bookdto);
    if (!validationResult.IsValid) //if book is INVALID it results in Error and BadRequest
    {
        _programLoggaren.Log(LogLevel.Error, "\nValidation incorrect!");
        var errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
        return Results.BadRequest(response);
    }
    // If Validation is valid the following occurs 
    var addingBook = await bookrepo.Create(bookdto);
    if (addingBook == null)     //Logger information informs you of whats happening
    {
        _programLoggaren.Log(LogLevel.Error, "\nBook with the title and author already exists");
        return Results.BadRequest(response);
    }
    _programLoggaren.Log(LogLevel.Information, "\nCreated a book via MapPost");

    response.Result = addingBook;
    response.IsSuccess = true;
    response.StatusCode = System.Net.HttpStatusCode.Created;
    return Results.Ok(response);

}).WithName("CreateBook").Accepts<BookDTO>("application/json").Produces<Book>(201).Produces(400);

app.MapPut("/api/book/{id:int}", async ([FromServices] IGenericRepository<Book, BookDTO> bookrepo, Book book, int id
 , ILogger<Program> _programLoggaren, IValidator<Book> validator) =>
{
    ApiResponse response = new ApiResponse();
    response.IsSuccess = false;
    response.StatusCode = System.Net.HttpStatusCode.BadRequest;


    // Using Validation before updating book
    var validationResult = await validator.ValidateAsync(book);
    if (!validationResult.IsValid) //if book is INVALID it results in Error and BadRequest
    {
        _programLoggaren.Log(LogLevel.Error, "\nCreating a book failed ; MapPost");
        var errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
        return Results.BadRequest(response);
    }
    // If Validation is valid the following occurs 

    var updatebook = await bookrepo.Update(id, book);
    if (updatebook != null)     //Logger information informs you of whats happening
    {
        _programLoggaren.Log(LogLevel.Information, "\nUpdating a book succeesfully");
        // Creating and returning RequestResponse with message and following updated book
        response.Result = updatebook;
        response.IsSuccess = true;
        response.StatusCode = System.Net.HttpStatusCode.OK;
        return Results.Ok(response);
    }
    _programLoggaren.Log(LogLevel.Error, "\nUpdating a book failed, id not found");
    return Results.NotFound(response);

}).WithName("UpdateBook").Produces(200).Produces(400);

app.MapDelete("/api/book/{id:int}", async ([FromServices] IGenericRepository<Book, BookDTO> bookrepo, int id,
   ILogger<Program> _programLoggaren) =>
{
    ApiResponse response = new ApiResponse();
    response.IsSuccess = false;
    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
    var DeleteBook = await bookrepo.Delete(id);
    if (DeleteBook != null)      //Logger information informs you of whats happening
    {
        _programLoggaren.Log(LogLevel.Information, "\nDeleting a book succeeded");
        response.Result = DeleteBook;
        response.IsSuccess = true;
        response.StatusCode = System.Net.HttpStatusCode.OK;
        return Results.Ok(response);
    }
    _programLoggaren.Log(LogLevel.Error, "\nDeleting a book failed, book not found");
    return Results.NotFound(response);

}).WithName("DeleteBook").Produces(200).Produces(400);
// ----------------------------------------------------------------------------------------------------
// --------------------  Endpoints for additional ApiFunctions below  ---------------------------------
// ----------------------------------------------------------------------------------------------------
app.MapGet("/api/book/available", async ([FromServices] IApiFunctionsRepository<Book> apifunctions, ILogger<Program> _programLoggaren) =>
{
    _programLoggaren.LogInformation("\nGetting all Available books");

    ApiResponse response = new ApiResponse();
    response.IsSuccess = true;
    response.Result = await apifunctions.GetAllAvailable();
    response.StatusCode = System.Net.HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("GetAvailableBooks").Produces<IEnumerable<Book>>(200).Produces(400);

app.MapGet("/api/book/unavailable", async ([FromServices] IApiFunctionsRepository<Book> apifunctions, ILogger<Program> _programLoggaren) =>
{
    _programLoggaren.LogInformation("\nGetting all Unavailable books");

    ApiResponse response = new ApiResponse();
    response.IsSuccess = true;
    response.Result = await apifunctions.GetAllUnavailable();
    response.StatusCode = System.Net.HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("GetUnavailableBooks").Produces<IEnumerable<Book>>(200).Produces(400);

app.MapGet("/api/book/year", async ([FromServices] IApiFunctionsRepository<Book> apifunctions, ILogger<Program> _programLoggaren) =>
{
    _programLoggaren.LogInformation("\nGetting all books ordered after year");

    ApiResponse response = new ApiResponse();
    response.IsSuccess = true;
    response.Result = await apifunctions.GetAllOrderedByYear();
    response.StatusCode = System.Net.HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("GetAllOrderedByYear").Produces<IEnumerable<Book>>(200).Produces(400);

app.MapGet("/api/book/author/{author}", async ([FromServices] IApiFunctionsRepository<Book> apifunctions, string author, ILogger<Program> _programLoggaren) =>
{
    ApiResponse response = new ApiResponse();
    response.IsSuccess = false;
    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
    response.Result = await apifunctions.GetByAuthor(author);

    if (response.Result != null)
    {
        _programLoggaren.LogInformation("\nSearching author: success");
        response.StatusCode = System.Net.HttpStatusCode.OK; response.IsSuccess = true;
        return Results.Ok(response);
    }
    _programLoggaren.LogError("\nSearching author: failed. No author found");
    return Results.NotFound(response);
}).WithName("GetByAuthor").Produces<IEnumerable<Book>>(200).Produces(400);

app.MapGet("/api/book/title/{title}", async ([FromServices] IApiFunctionsRepository<Book> apifunctions, string title, ILogger<Program> _programLoggaren) =>
{
    ApiResponse response = new ApiResponse();
    response.IsSuccess = true;
    response.StatusCode = System.Net.HttpStatusCode.OK;
    response.Result = await apifunctions.GetByTitle(title);

    if (response.Result != null)
    {
        _programLoggaren.LogInformation("\nSearching title: success");
        return Results.Ok(response);
    }
    _programLoggaren.LogError("\nSearching title: failed");
    response.IsSuccess = false; response.StatusCode = System.Net.HttpStatusCode.BadRequest;
    return Results.NotFound(response);
}).WithName("GetByTitle").Produces<IEnumerable<Book>>(200).Produces(400);

app.Run();