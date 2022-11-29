using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebApplication.Chat.Database;
using System.Text;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;

namespace WebApplication.Chat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IdentityModelEventSource.ShowPII = true;
            var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

            builder.Logging
                .ClearProviders()
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole();

            builder.Host.UseNLog();

            builder.Services.AddControllers();

            builder.Services
                .AddAuthentication(schema =>
            {
                schema.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                schema.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(p =>
            {
                p.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "chat_server",
                    ValidAudience = "chat_server",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_secret_key_server_long"))
                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "CorsPolicy",
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader();
                                  });
            });


            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
            builder.Services
                .AddDbContext<ChatDataContext>(
                    dbbuilder => dbbuilder.UseNpgsql(builder.Configuration.GetConnectionString("chatdb")));

            var app = builder.Build();


            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseCors("CorsPolicy");

            MigrationDb(app.Services);

            app.Run();
        }

        private static void MigrationDb(IServiceProvider appServices)
        {
            using (var scope = appServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ChatDataContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}