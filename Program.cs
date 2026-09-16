
using KermAdhd.Database;
using KermAdhd.Database.DataModels;
using KermAdhd.Services;
using Microsoft.EntityFrameworkCore;

namespace KermAdhd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<AdhdContext>(options =>
            {
                options.UseSeeding((x, v) =>
                {
                    x.Set<Reminder>().Add(new Reminder { Name = "Test", IsCompleted = false });
                });

                options.UseInMemoryDatabase("adhdreminder");
            });

            builder.Services.AddScoped<IReminderService, ReminderService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
