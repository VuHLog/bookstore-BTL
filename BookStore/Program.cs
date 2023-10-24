using BookStore.Middleware;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration 
// .SetBasePath(Directory.GetCurrentDirectory()).Add JsonFile("Secrets.json"); 
//Đăng ký SchoolContext là một DbContext của ứng dụng 
builder.Services.AddDbContext<BookstoreContext>(options => options
.UseSqlServer(builder.Configuration.GetConnectionString("SchoolContext")));


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache(); // Sử dụng bộ nhớ cache mặc định cho session
builder.Services.AddSession(); // Thêm dịch vụ Session

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

app.UseSession(); // Sử dụng Session Middleware

app.UseRouting();
app.UseMiddleware<RoleMiddleware>(); // dua vao pipeline RoleMiddleware

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
