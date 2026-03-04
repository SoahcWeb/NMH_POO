using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NMH.Shared;
using NMH.Data;
using System.Text;
using MovieEntity = NMH.Data.Movie;
using NMH.Components;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Récupérer le JWT depuis la config
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings.GetValue<string>("Key");
var jwtIssuer = jwtSettings.GetValue<string>("Issuer");
var jwtAudience = jwtSettings.GetValue<string>("Audience");
var jwtExpireHours = jwtSettings.GetValue<int>("ExpireHours");

// Services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=nmh.db"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

// ✅ AJOUTÉ POUR BLazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 🔹 JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true
    };
});

// 🔹 Ajouter le service TMDB
builder.Services.AddHttpClient<NMH.Services.TmdbService>();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery(); 

// Endpoints API
app.MapGet("/api/test", () => "Hello NMH API is working 🚀");

app.MapGet("/api/movies", [Microsoft.AspNetCore.Authorization.Authorize] async (ApplicationDbContext db) =>
    await db.Movies.ToListAsync());

app.MapPost("/api/movies", [Microsoft.AspNetCore.Authorization.Authorize] async (MovieEntity movie, ApplicationDbContext db) =>
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();
    return Results.Created($"/api/movies/{movie.Id}", movie);
});

// 🔹 Endpoints TMDB
app.MapGet("/api/tmdb/movies", async (NMH.Services.TmdbService tmdb) =>
{
    var movies = await tmdb.GetTrendingMoviesAsync();
    return Results.Ok(movies);
});

app.MapGet("/api/tmdb/series", async (NMH.Services.TmdbService tmdb) =>
{
    var series = await tmdb.GetTrendingSeriesAsync();
    return Results.Ok(series);
});

app.MapControllers();

// ✅ AJOUTÉ POUR BLazor (IMPORTANT)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();