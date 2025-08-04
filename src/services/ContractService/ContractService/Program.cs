using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using ContractService.Data;
using ContractService.Services;
using ContractService.Mappings;
using ContractService.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database configuration
builder.Services.AddDbContext<ContractDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(3);
            npgsqlOptions.CommandTimeout(30);
        }));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "your-super-secret-key-that-is-at-least-32-characters-long");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "ContractManagementSystem",
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"] ?? "ContractManagementSystem",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateContractDtoValidator>();

// Business Services
builder.Services.AddScoped<IContractService, ContractService.Services.ContractService>();
builder.Services.AddScoped<IContractTypeService, ContractTypeService>();

// Temporary tenant context (replace with actual implementation later)
builder.Services.AddScoped<ITenantContext>(provider => new TenantContext
{
    TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Default tenant for testing
    UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),   // Default user for testing
    UserEmail = "admin@test.com",
    UserRoles = new[] { "Admin" },
    IsAuthenticated = true
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "");

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Contract Service API", 
        Version = "v1",
        Description = "Contract Management microservice API"
    });
    
    // JWT Authorization
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
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

    // Enable XML comments for Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    if (builder.Environment.IsDevelopment())
    {
        config.AddDebug();
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contract Service API v1");
        c.RoutePrefix = "swagger";
    });
}

// CORS
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Health Checks
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                exception = x.Value.Exception?.Message,
                duration = x.Value.Duration.ToString()
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

// Controllers
app.MapControllers();

// Error handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var error = new { message = "An error occurred processing your request." };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(error));
    });
});

// Run the application
try
{
    app.Logger.LogInformation("Starting ContractService on {Environment}", app.Environment.EnvironmentName);
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "ContractService failed to start");
    throw;
}

// Temporary classes until Shared library is working
namespace ContractService.Services
{
    public interface ITenantContext
    {
        Guid TenantId { get; }
        Guid UserId { get; }
        string UserEmail { get; }
        string[] UserRoles { get; }
        bool IsAuthenticated { get; }
    }

    public class TenantContext : ITenantContext
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string[] UserRoles { get; set; } = Array.Empty<string>();
        public bool IsAuthenticated { get; set; }
    }
}