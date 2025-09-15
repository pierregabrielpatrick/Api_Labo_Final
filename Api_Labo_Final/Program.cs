
using Api_Labo_Final.Utils;
using BLL;
using Dal;
using Dal.context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Api_Labo_Final
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<FinalContext>(o => o
    .UseSqlServer(builder.Configuration.GetConnectionString("Main")));

            builder.Services.AddSingleton<JwtUtils>();

            builder.Services.AddScoped<IHouseService, HouseServiceImpl>();
            builder.Services.AddScoped<IUserService , UserServiceImpl>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBatchDataService , BatchDataService>();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyProject", Version = "v1.0.0" });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };

                c.AddSecurityRequirement(securityRequirement);

            });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        builder.Configuration.GetSection("TokenInfo").GetSection("secret").Value!)),
                    ValidateIssuer = false,
                    ValidIssuer = builder.Configuration.GetSection("TokenInfo").GetSection("issuer").Value!,
                    ValidateAudience = false,
                    ValidAudience = builder.Configuration.GetSection("TokenInfo").GetSection("audience").Value!,
                    ValidateLifetime = false
                };
            });

            builder.Services.AddAuthorization(o =>
            {
                o.AddPolicy("Auth", p => p.RequireAuthenticatedUser());
                o.AddPolicy("Admin", p => p.RequireRole("Admin"));
            });

            builder.Services.AddCors(o =>
            {
                o.AddDefaultPolicy(p =>
                {
                    var audience = builder.Configuration.GetSection("TokenInfo").GetSection("audience").Value;
                    var origins = new List<string> { "http://localhost:4200" };
                    if (!string.IsNullOrWhiteSpace(audience))
                    {                        origins.Add(audience);
                    }
                    p.WithOrigins(origins.ToArray());
                    p.AllowAnyHeader();
                    p.AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();


            app.MapControllers();

            app.Run();
        }
    }
}
