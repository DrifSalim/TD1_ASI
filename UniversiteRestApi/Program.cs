using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversiteCsvProvider.Services;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.JeuxDeDonnees;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.Repositories;
using UniversiteEFDataProvider.RepositoryFactories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Mis en place d'un annuaire des services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.AddConsole();
});

// Configuration des connexions à MySql
String connectionString = builder.Configuration.GetConnectionString("MySqlConnection") ?? throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");
// Création du contexte de la base de données en utilisant la connexion MySql que l'on vient de définir
// Ce contexte est rajouté dans les services de l'application, toujours prêt à être utilisé par injection de dépendances
builder.Services.AddDbContext<UniversiteDbContext>(options =>options.UseMySQL(connectionString));
// La factory est rajoutée dans les services de l'application, toujours prête à être utilisée par injection de dépendances
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
//Rajout des services
builder.Services.AddScoped<GenerateCsvService>();
builder.Services.AddScoped<UploadCsvService>();
builder.Services.AddScoped<IEtudiantRepository, EtudiantRepository>();
builder.Services.AddScoped<IUeRepository, UeRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

// Sécurisation
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<UniversiteUser>()
    .AddRoles<UniversiteRole>()
    .AddEntityFrameworkStores<UniversiteDbContext>() // Ici, on stocke les users dans la même bd que le reste
    .AddApiEndpoints();


// Création de tous les services qui sont stockés dans app
// app contient tous les objets de notre application
var app = builder.Build();

// Configuration du serveur Web
app.UseHttpsRedirection();
app.MapControllers();

// Configuration de Swagger.
// Commentez les deux lignes ci-dessous pour désactiver Swagger (en production par exemple)
app.UseSwagger();
app.UseSwaggerUI();
// Initisation de la base de données
ILogger logger = app.Services.GetRequiredService<ILogger<BdBuilder>>();
logger.LogInformation("Chargement des données de test");
using(var scope = app.Services.CreateScope())
{
    UniversiteDbContext context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    IRepositoryFactory repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();   
    // C'est ici que vous changez le jeu de données pour démarrer sur une base vide par exemple
    BdBuilder seedBD = new BasicBdBuilder(repositoryFactory);
    await seedBD.BuildUniversiteBdAsync();
}
// Sécurisation
app.UseAuthorization();
// Ajoute les points d'entrée dans l'API pour s'authentifier, se connecter et se déconnecter
app.MapIdentityApi<UniversiteUser>();

// Exécution de l'application
app.Run();