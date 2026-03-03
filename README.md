NMH_POO
Projet NMH_POO – Blazor WebAssembly Hosted avec EF Core et Identity

Description
Ce projet est un prototype Blazor WebAssembly Hosted utilisant ASP.NET Core pour le backend, SQLite comme base de données et ASP.NET Core Identity pour la gestion des utilisateurs.
L'objectif principal est de créer une application modulable avec une API minimaliste pour gérer des films et préparer l'intégration future de fonctionnalités plus avancées.

🟨 Jour 1 – Workflow et avancement
Préparation du projet

Création du workspace dans Visual Studio Code.

Placement dans le dossier du projet C:/Projet/NMH_POO.

Création de la solution Blazor Hosted :

dotnet new blazorwasm -n NMH.Client --hosted --framework net8.0

Vérification que le projet compile correctement.

Installation des packages nécessaires

Entity Framework Core avec SQLite :

dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
Création des classes principales

Movie dans NMH.Shared avec les propriétés :

Id (int)

Title (string)

ReleaseDate (DateTime)

ApplicationDbContext dans NMH.Server/Data héritant de IdentityDbContext et incluant DbSet<Movie>.

Configuration du serveur

Ajout des services DbContext et Identity dans Program.cs.

Configuration des endpoints minimal API et des Razor Pages.

Test de l’endpoint /api/test renvoyant :

"Hello NMH API is working 🚀"
Migration et création de la base SQLite
dotnet ef migrations add InitialCreate
dotnet ef database update

Vérification que le fichier nmh.db est créé et la base est prête.

Test final

Lancement du serveur et vérification que l’API fonctionne correctement via navigateur ou Postman.

Structure du projet

NMH.Client – Frontend Blazor WebAssembly

NMH.Server – Backend ASP.NET Core

NMH.Shared – Modèles partagés entre client et serveur

nmh.db – Base SQLite locale (exclue du dépôt via .gitignore)

🟨 Jour 2 – Authentification complète (JWT)

Objectif
Créer un système d’authentification complet côté serveur et client avec JWT, protection des routes et gestion des utilisateurs.

1️⃣ Installation des packages JWT
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
2️⃣ Configuration JWT dans Program.cs
var jwtSecretKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKeyPourJWT123!1234567890";

builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => 
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateLifetime = true
    };
});

// Middleware
app.UseAuthentication();
app.UseAuthorization();

Astuce : Stocker la clé JWT dans appsettings.json pour éviter le hardcoding.

3️⃣ Création du AuthController

Endpoints créés :

POST /api/auth/register → inscription

POST /api/auth/login → génération du JWT

POST /api/auth/logout → déconnexion (optionnelle)

Exemple minimal pour générer un JWT :

var tokenHandler = new JwtSecurityTokenHandler();
var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new Claim[]
    {
        new Claim(ClaimTypes.Name, user.UserName)
    }),
    Expires = DateTime.UtcNow.AddHours(1),
    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
};
var token = tokenHandler.CreateToken(tokenDescriptor);
return Ok(new { token = tokenHandler.WriteToken(token) });
4️⃣ Création des DTOs pour Auth
public class RegisterDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
5️⃣ Pages Blazor côté client

Register.razor → formulaire d’inscription via /api/auth/register

Login.razor → formulaire de connexion via /api/auth/login et stockage du JWT dans localStorage

Protection d’une page :

@attribute [Authorize]

Page protégée
Seuls les utilisateurs connectés peuvent voir ce contenu.

Program.cs côté client :

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
6️⃣ Protection des endpoints côté serveur
app.MapGet("/api/movies", [Authorize] async (ApplicationDbContext db) => await db.Movies.ToListAsync());

app.MapPost("/api/movies", [Authorize] async (Movie movie, ApplicationDbContext db) => 
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();
    return Results.Created($"/api/movies/{movie.Id}", movie);
});
7️⃣ Test complet du flow JWT

Inscription via Postman ou page Register

Connexion via Login → récupération du JWT

Accès aux endpoints protégés (/api/movies) avec le JWT → succès

Vérification que l’accès sans JWT renvoie 401 Unauthorized

Structure après Jour 2

NMH/ – Serveur et client Blazor Server

Shared/ – Modèles et DTOs (Movie, RegisterDto, LoginDto)

nmh.db – Base SQLite locale

Résultat

Authentification fonctionnelle avec JWT

Endpoints sécurisés via [Authorize]

JWT centralisé et stocké côté client

Flow complet testé et opérationnel

Prochaines étapes

Gestion des rôles et permissions

Expiration et refresh token

Intégration du CRUD films côté client

Amélioration UX/UI pour Register et Login
