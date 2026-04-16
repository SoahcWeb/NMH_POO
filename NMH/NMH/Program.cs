using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;

using NMH.Data;
using System.Text;
using NMH;

var builder = WebApplication.CreateBuilder(args);

// ===================== DB =====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=nmh.db"
    )
);

// ===================== IDENTITY =====================
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

// ===================== AUTH STATE =====================
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, NMH.Services.CustomAuthStateProvider>();
builder.Services.AddScoped<NMH.Services.CustomAuthStateProvider>();

// ===================== LOCAL STORAGE =====================
builder.Services.AddBlazoredLocalStorage();

// ===================== SERVICES =====================
builder.Services.AddScoped<NMH.Services.FavoritesService>();
builder.Services.AddHttpClient<NMH.Services.TmdbService>();

// ===================== CONTROLLERS =====================
builder.Services.AddControllers();

// ===================== BLAZOR WEB APP (.NET 8) =====================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ===================== HTTP CLIENT FIX (IMPORTANT) =====================
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(nav.BaseUri)
    };
});

// ===================== AUTH JWT =====================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings.GetValue<string>("Key");
var jwtIssuer = jwtSettings.GetValue<string>("Issuer");
var jwtAudience = jwtSettings.GetValue<string>("Audience");

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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey ?? "")
        ),
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// ===================== PIPELINE =====================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();

// ===================== BLazor .NET 8 =====================
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();