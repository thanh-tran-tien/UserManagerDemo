using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UserManagerDemo.Api.Extensions;
using UserManagerDemo.Application.Auth.Dtos;
using UserManagerDemo.Application.Auth.Validators;
using UserManagerDemo.Application.Interface.Auth;
using UserManagerDemo.Application.Users.Dtos;
using UserManagerDemo.Application.Users.Mappings;
using UserManagerDemo.Application.Users.Validators;
using UserManagerDemo.Domain.Entity;
using UserManagerDemo.Infrastructure;
using UserManagerDemo.Infrastructure.Persistence;
using UserManagerDemo.Infrastructure.Services;

namespace UserManagerDemo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "User Management API",
                Version = "v1",
                Description = "Demo CRUD User API with Clean Architecture"
            });

            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower(), // "bearer"
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Nhập 'Bearer {token}' vào đây"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });

            c.EnableAnnotations();
        });

        builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        builder.Services.AddScoped<IValidator<UserDto>, UserDtoValidator>();
        builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
                )
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("jwt"))
                    {
                        context.Token = context.Request.Cookies["jwt"];
                    }
                    return Task.CompletedTask;
                }
            };
        });

        var app = builder.Build();
        DbInitializer.InitializeDatabase(app.Services);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
                c.RoutePrefix = string.Empty; // Swagger UI at root "/"
            });
        }

        // Add UoW middleware global
        app.UseUnitOfWork();

        app.UseCors("AllowAngularApp");

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}