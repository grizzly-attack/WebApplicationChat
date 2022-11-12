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

            // Add services to the container.
            builder.Services.AddControllers();

            // Использование авторизации
            builder.Services
                .AddAuthentication(schema =>
            {
                schema.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;//тип токена
                schema.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//откуда берем (поле)
            })
                .AddJwtBearer(p =>//настройка различных параметров токена
            {
                p.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "chat_server",
                    ValidAudience = "chat_server",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_secret_key_server_long"))
                };
            });

            // Проверка авторизации
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();//Генерирует страницу для отображения методов, которые можно вызвать в приложении
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
            });//Генерирует страницу для отображения методов, которые можно вызвать в приложении
            builder.Services
                .AddDbContext<ChatDataContext>(
                    builder => builder.UseNpgsql(
                        "Host=localhost;Port=5432;Database=chatdb;Username=postgres;Password=1;"));//Подключение БД

            var app = builder.Build();//СОздает ядро приложения


            app.UseHttpsRedirection();//редиректит незащищенное подключение
            app.UseSwagger();//делает доступным файл с описанием методов приложения
            app.UseSwaggerUI();//делает технический интерфейс для запросов к приложению
            app.UseAuthentication();//добавляет аутонтификацию
            app.UseAuthorization();//добавляет авторизацию
            app.MapControllers();//делает обработку запросов в контроллерах
			
			MigrationDb(app.Services);
						
            app.Run();//запуск приложения
        }

        private static void MigrationDb(IServiceProvider appServices)
        {
            using(var scope = appServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ChatDataContext>();//контекст для работы с базой данных
                //dbContext.Database.EnsureCreated();//создаем базу данных
                dbContext.Database.Migrate();//обновляем бд
            }
        }
    }
}