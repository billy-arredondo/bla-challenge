namespace BlaChallenge.Api;

using System.Text;
using System.Text.Json.Serialization;
using BlaChallenge.Application.Users.Commands.LoginUser;
using BlaChallenge.Application.Users.Commands.RegisterUser;
using BlaChallenge.Application.Users.Queries.GetCurrentUser;
using BlaChallenge.Infrastructure;
using BlaChallenge.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.AddOpenApi();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddScoped<RegisterUserUseCase>();
        builder.Services.AddScoped<LoginUserUseCase>();
        builder.Services.AddScoped<GetCurrentUserUseCase>();
        builder.Services.AddScoped<CreateTaskUseCase>();
        builder.Services.AddScoped<GetTasksUseCase>();
        builder.Services.AddScoped<GetTaskByIdUseCase>();
        builder.Services.AddScoped<UpdateTaskUseCase>();
        builder.Services.AddScoped<DeleteTaskUseCase>();

        var jwtOptions = new JwtOptions
        {
            Issuer = builder.Configuration["Jwt:Issuer"] ?? "BlaChallenge",
            Audience = builder.Configuration["Jwt:Audience"] ?? "BlaChallenge.Client",
            Key = builder.Configuration["Jwt:Key"] ?? string.Empty,
            ExpirationMinutes = int.TryParse(builder.Configuration["Jwt:ExpirationMinutes"], out var expirationMinutes)
                ? expirationMinutes
                : 60
        };
        if (string.IsNullOrWhiteSpace(jwtOptions.Key) || jwtOptions.Key.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Key must contain at least 32 characters.");
        }

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        builder.Services.AddAuthorization();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        await app.Services.GetRequiredService<BlaChallenge.Infrastructure.Sqlite.SqliteDatabaseInitializer>().InitializeAsync();

        app.MapControllers();

        await app.RunAsync();
    }
}
