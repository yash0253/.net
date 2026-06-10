using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VisaManagementSystem.Data;
using VisaManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// Database
// ==========================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// JWT Authentication
// ==========================================
var key = Encoding.ASCII.GetBytes(
    builder.Configuration["JwtSettings:Secret"]
    ?? "ThisIsMySuperSecretKeyForVisaSystem1234567890");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = false,
        ValidateAudience = false,

        ClockSkew = TimeSpan.Zero
    };
});

// ==========================================
// Memory Cache
// ==========================================
builder.Services.AddMemoryCache();

// ==========================================
// Services
// ==========================================
builder.Services.AddScoped<IMessageService>(provider =>
{
    var dbOptions =
        provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();

    var cache =
        provider.GetRequiredService<IMemoryCache>();

    var config =
        provider.GetRequiredService<IConfiguration>();

    return new MessageService(dbOptions, cache, config);
});

// IMPORTANT:
// If your class is named VisaService
builder.Services.AddScoped<IVisaApplicationService, VisaApplicationServices>();
builder.Services.AddScoped<IAdminService, AdminService>();
// ==========================================
// CORS
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ==========================================
// Controllers
// ==========================================
builder.Services.AddControllers();

// ==========================================
// Swagger + JWT Support
// ==========================================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Visa Management System API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter JWT Token. Example: Bearer eyJhbGciOiJIUzI1NiIs..."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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

var app = builder.Build();

// ==========================================
// Middleware
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// ==========================================
// Create Database
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    dbContext.Database.EnsureCreated();
}

app.Run();