using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SaboreIA.Database;
using SaboreIA.Interfaces.Repository;
using SaboreIA.Interfaces.Service;
using SaboreIA.Repositories;
using SaboreIA.Services;
using SaboreIA.Services.IA;
using SaboreIA.Integrations;
using SaboreIA.Integrations.Cloudinary;
using SaboreIA.Authorization.IsOwnerOrAdmin;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SaboreIA API",
        Version = "v1",
        Description = "API para recomendação inteligente de restaurantes com IA",
        Contact = new OpenApiContact
        {
            Name = "Estevão Alves",
            Email = "ealves1710@hotmail.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Cabeçalho de autorização JWT usando esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database Configuration - SQLite para testes, PostgreSQL para produção
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var isTestEnvironment = builder.Environment.EnvironmentName == "Test";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (isTestEnvironment || connectionString?.Contains(".db") == true)
    {
        // SQLite para ambiente de teste
        options.UseSqlite(connectionString);
    }
    else
    {
        // PostgreSQL para produção
        options.UseNpgsql(connectionString);
    }
    
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey não configurada no appsettings.json");

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
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsOwnerOrAdmin", policy =>
        policy.Requirements.Add(new IsOwnerOrAdminRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, IsOwnerOrAdminHandler>();

// Cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

// IA Service (Perplexity)
builder.Services.Configure<IAServiceConfig>(builder.Configuration.GetSection("IAServiceConfig"));
builder.Services.AddHttpClient<PerplexityClient>();
builder.Services.AddScoped<IIAService, IAService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:8080", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Aplicar migrations automaticamente em ambiente de teste
if (isTestEnvironment)
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureDeleted(); // Limpa o banco anterior
        db.Database.EnsureCreated(); // Cria o banco com o schema atualizado
    }
}

// Swagger
if (app.Environment.IsDevelopment() || isTestEnvironment)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SaboreIA API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();
app.UseCors("AllowAny");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
