
using System.Text;
using EnterpriseServiceManagement.Data;
using EnterpriseServiceManagement.DTOs;
using EnterpriseServiceManagement.Identity;
using EnterpriseServiceManagement.Interfaces;
using EnterpriseServiceManagement.Middleware;
using EnterpriseServiceManagement.Repositories;
using EnterpriseServiceManagement.Services;
using EnterpriseServiceManagement.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;    
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/application-.log",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog
    builder.Host.UseSerilog();

    // Database
    builder.Services.AddDbContext<ESMDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "DefaultConnection")));

    // ASP.NET Core Identity
    builder.Services
        .AddIdentityCore<AppUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
        })
        .AddRoles<AppRole>()
        .AddEntityFrameworkStores<ESMDbContext>()
        .AddSignInManager();

    // JWT authentication
    builder.Services
        .AddAuthentication(
            JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT signing key is missing.");

            var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT issuer is missing.");

            var jwtAudience = builder.Configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT audience is missing.");

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,

                    ValidateAudience = true,
                    ValidAudience = jwtAudience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey))
                };
        });

    // Authorization
    builder.Services.AddAuthorization();

    // Department services
    builder.Services.AddScoped<
        IDepartmentRepository, DepartmentRepository>();

    builder.Services.AddScoped<
        IDepartmentService, DepartmentService>();

    builder.Services.AddScoped<IRoleService, RoleService>();

    // Employee services
    builder.Services.AddScoped<
        IEmployeeRepository, EmployeeRepository>();

    builder.Services.AddScoped<
        IEmployeeService, EmployeeService>();

    // Authentication service
    builder.Services.AddScoped<IAuthService, AuthService>();

    // Controllers
    builder.Services.AddControllers();

    // FluentValidation
    builder.Services.AddFluentValidationAutoValidation();

    builder.Services
        .AddValidatorsFromAssemblyContaining<
            CreateDepartmentValidator>();

    // Standardized validation error responses
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray());

            var response = new ErrorResponseDto
            {
                Success = false,
                Message = "Validation failed.",
                Errors = errors,
                TraceId = context.HttpContext.TraceIdentifier
            };

            return new BadRequestObjectResult(response);
        };
    });

    // Swagger with JWT Bearer authorization
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT access token."
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

    // HTTP request logging
    app.UseSerilogRequestLogging();

    // Global exception handling
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Swagger in development
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Authentication before authorization
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
