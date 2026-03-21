using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Blazored.LocalStorage;

using NMH.Shared;
using NMH.Data;
using System.Text;
using MovieEntity = NMH.Data.Movie;
using NMH.Components;

var builder = WebApplication.CreateBuilder(args);

// 🔹 JWT (pour API uniquement)
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings.GetValue<string>("Key");
var jwtIssuer = jwtSettings.GetValue<string>("Issuer");
var jwtAudience = jwtSettings.GetValue<string>("Audience");

// 🔹 DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=nmh.db"));

// 🔹 Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 🔥 Blazor auth state
builder.Services.AddCascadingAuthenticationState();

// ✅ LocalStorage (AJOUT)
builder.Services.AddBlazoredLocalStorage();

// ✅ FavoritesService (AJOUT)
builder.Services.AddScoped<NMH.Services.FavoritesService>();

// ✅ CustomAuthStateProvider (déjà bon)
builder.Services.AddScoped<AuthenticationStateProvider, NMH.Services.CustomAuthStateProvider>();
builder.Services.AddScoped<NMH.Services.CustomAuthStateProvider>();

// 🔹 Cookie config
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
});

// 🔹 Razor + Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

// 🔹 HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5244")
});

builder.Services.AddControllers();

// 🔹 Auth JWT
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? "")),
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

// 🔹 Service TMDB
builder.Services.AddHttpClient<NMH.Services.TmdbService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();

// Blazor
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();