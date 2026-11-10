
using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Services;
using Microsoft.EntityFrameworkCore;

namespace HouseholdManager
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
            builder.Services.AddDbContext<HouseholdContext>(options =>
            {
                options.UseSeeding((x, v) =>
                {
                    var household = new Household { Name = "Test household" };
                    x.Set<User>().Add(new User { UserName = "Kristine", Household = household });
                    x.SaveChanges();
                });

                options.UseInMemoryDatabase("household");
            });

            builder.Services.AddScoped<IHouseholdService, HouseholdService>();
            builder.Services.AddScoped<IChoreService, ChoreService>();
            builder.Services.AddScoped<IChoreInstanceService, ChoreInstanceService>();
            builder.Services.AddScoped<IRecurrenceRuleService, RecurrenceRuleService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IShoppingItemService, ShoppingItemService>();
            builder.Services.AddScoped<IRecipeService, RecipeService>();
            builder.Services.AddScoped<IUserService, UserService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<HouseholdContext>();
                context.Database.EnsureCreated();
            }

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                }

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
