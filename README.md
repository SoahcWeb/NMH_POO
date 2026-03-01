# NMH_POO

Projet NMH_POO – Blazor WebAssembly Hosted avec EF Core et Identity.

## Description

Ce projet est un **prototype Blazor WebAssembly Hosted** utilisant ASP.NET Core pour le backend, SQLite comme base de données et ASP.NET Core Identity pour la gestion des utilisateurs. L'objectif principal est de créer une application modulable avec une API minimaliste pour gérer des films et préparer l'intégration future de fonctionnalités plus avancées.

---

## 🟨 Jour 1 – Workflow et avancement

### Étapes réalisées :

1. **Préparation du projet**
   - Création du workspace dans Visual Studio Code.
   - Placement dans le dossier du projet `C:/Projet/NMH_POO`.

2. **Création de la solution Blazor Hosted**
   - Commande utilisée :  
     ```bash
     dotnet new blazorwasm -n NMH.Client --hosted --framework net8.0
     ```
   - Vérification que le projet compile correctement.

3. **Installation des packages nécessaires**
   - Entity Framework Core avec SQLite :
     ```bash
     dotnet add package Microsoft.EntityFrameworkCore.Sqlite
     dotnet add package Microsoft.EntityFrameworkCore.Tools
     dotnet add package Microsoft.EntityFrameworkCore.Design
     dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
     ```

4. **Création des classes principales**
   - `Movie` dans `NMH.Shared` avec les propriétés :
     - Id (int)
     - Title (string)
     - ReleaseDate (DateTime)
   - `ApplicationDbContext` dans `NMH.Server/Data` héritant de `IdentityDbContext` et incluant `DbSet<Movie>`.

5. **Configuration du serveur**
   - Ajout des services DbContext et Identity dans `Program.cs`.
   - Configuration des endpoints minimal API et des Razor Pages.
   - Test de l’endpoint `/api/test` renvoyant :
     ```
     "Hello NMH API is working 🚀"
     ```

6. **Migration et création de la base SQLite**
   - Commandes EF Core utilisées :
     ```bash
     dotnet ef migrations add InitialCreate
     dotnet ef database update
     ```
   - Vérification que le fichier `nmh.db` est créé et la base est prête.

7. **Test final**
   - Lancement du serveur et vérification que l’API fonctionne correctement via navigateur ou Postman.

---

## 📂 Structure du projet

- `NMH.Client` – Frontend Blazor WebAssembly
- `NMH.Server` – Backend ASP.NET Core avec API et Identity
- `NMH.Shared` – Modèles partagés entre client et serveur
- `nmh.db` – Base de données SQLite locale (exclue du dépôt via `.gitignore`)

---

## 🚀 Prochaines étapes

- Ajouter des endpoints CRUD pour les films.
- Implémenter l’authentification et la gestion des utilisateurs.
- Préparer le workflow GitHub pour les prochains commits et branches de développement.
