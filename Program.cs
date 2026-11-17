using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using HouseholdManager.Infrastructure;
using HouseholdManager.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace HouseholdManager
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            });
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
            builder.Services.AddOpenApi();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton(TimeProvider.System);

            builder.Services.AddDbContext<HouseholdContext>(options =>
            {
                options.UseSeeding((context, _) =>
                {
                    context.Set<Household>().Add(new Household { Name = "Test household" });
                    context.SaveChanges();
                });

                options.UseInMemoryDatabase("household");
            });

            builder.Services.AddOptions<JwtOptions>()
                .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException("The Jwt section is not configured");

            builder.Services.AddIdentityCore<User>(options =>
                {
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<HouseholdContext>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience
                    };
                });

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy("login", httpContext => RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
            });

            builder.Services.AddScoped<ICurrentUser, CurrentUser>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IHouseholdService, HouseholdService>();
            builder.Services.AddScoped<IChoreService, ChoreService>();
            builder.Services.AddScoped<IChoreInstanceService, ChoreInstanceService>();
            builder.Services.AddScoped<IRecurrenceRuleService, RecurrenceRuleService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IShoppingItemService, ShoppingItemService>();
            builder.Services.AddScoped<IRecipeService, RecipeService>();
            builder.Services.AddScoped<IIngredientService, IngredientService>();
            builder.Services.AddScoped<ISchoolPlanService, SchoolPlanService>();

            builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection(FileStorageOptions.SectionName));
            builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

            builder.Services.Configure<RecurrenceGenerationOptions>(builder.Configuration.GetSection(RecurrenceGenerationOptions.SectionName));
            builder.Services.AddScoped<IChoreInstanceGenerator, ChoreInstanceGenerator>();
            builder.Services.AddHostedService<RecurrenceGenerationBackgroundService>();

            var app = builder.Build();

            app.UseExceptionHandler();

            await DatabaseSeeder.SeedAsync(app.Services, app.Configuration);

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
