using VirusTotalASP.Services;

namespace VirusTotalASP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавление сервисов в контейнер
            builder.Services.AddControllers();
            
            // Настройка Swagger/OpenAPI для документации API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Настройка CORS для Vue.js фронтенда
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowVueApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:8080")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Добавление HttpClient для работы с VirusTotal API
            builder.Services.AddHttpClient<IVirusTotalService, VirusTotalService>(client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            // Регистрация сервиса VirusTotal
            builder.Services.AddScoped<IVirusTotalService, VirusTotalService>();

            var app = builder.Build();

            // Настройка конвейера обработки HTTP запросов
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Включение CORS
            app.UseCors("AllowVueApp");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
