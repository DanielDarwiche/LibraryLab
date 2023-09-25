using LibraryLab_Api.Models;
using LibraryLab_Api.Services;
using LibraryLab_MVC;
using LibraryLab_MVC.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add Services 
builder.Services.AddHttpClient<IBookService, BookService>();
builder.Services.AddScoped<IBookService, BookService>();
StaticDetails.BookApiBase = builder.Configuration["ServiceUrls:BooksDbApi"];

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Book}/{action=IndexBook}/{id?}");

app.Run();