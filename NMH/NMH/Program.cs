using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using NMH.Shared;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=nmh.db"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Endpoint test
app.MapGet("/api/test", () => "Hello NMH API is working 🚀");

// Endpoint Movies
app.MapGet("/api/movies", async (ApplicationDbContext db) =>
    await db.Movies.ToListAsync());

app.MapPost("/api/movies", async (Movie movie, ApplicationDbContext db) =>
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();
    return Results.Created($"/api/movies/{movie.Id}", movie);
});

app.MapRazorPages();
app.MapControllers();

app.Run();