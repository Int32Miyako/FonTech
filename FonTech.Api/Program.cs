using FonTech.Api;
using FonTech.Api.Middlewares;
using FonTech.Application.DependencyInjection;
using FonTech.Consumer.DependencyInjection;
using FonTech.DataAccess.DependencyInjection;
using FonTech.Domain.Settings;
using FonTech.Producer.DependencyInjection;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Конфигурации
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSection));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(RabbitMqSettings.DefaultSection));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(RedisSettings.DefaultSection));

// Добавление сервисов
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddAuthenticationAndAuthorization(builder);
builder.Services.AddSwagger();
builder.Services.UseHttpClientMetrics();

// Логирование
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Configuration.AddUserSecrets<Program>();

// Подключение слоёв
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddProducer();
builder.Services.AddConsumer();



var app = builder.Build();
app.UseRouting();

// Подключение метрик HTTP
app.UseMetricServer();
app.UseHttpMetrics();

app.MapGet("/random-number", () =>
{
    var number = Random.Shared.Next(0, 10);
    return Results.Ok(number);
});

// Middleware для обработки исключений
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger UI
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FonTech Swagger v 1.0");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "FonTech Swagger v 2.0");
        c.RoutePrefix = string.Empty;
    });
}

// CORS
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


app.UseHttpsRedirection();

app.MapMetrics();
app.MapControllers();

app.Run();
