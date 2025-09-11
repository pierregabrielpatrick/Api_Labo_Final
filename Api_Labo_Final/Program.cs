
using Api_Labo_Final.Utils;
using BusinessLogicLayer;
using Dal.context;
using Microsoft.EntityFrameworkCore;

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

            builder.Services.AddScoped<HouseService>();
            builder.Services.AddScoped<HouseRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
