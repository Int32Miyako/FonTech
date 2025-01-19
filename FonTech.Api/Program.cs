using FonTech.Api;
using FonTech.Api.Middlewares;
using FonTech.Application.DependencyInjection;
using FonTech.Consumer.DependencyInjection;
using FonTech.DataAccess.DependencyInjection;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Settings;
using FonTech.Producer.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>
    (builder.Configuration.GetSection(JwtSettings.DefaultSection));

builder.Services.Configure<RabbitMqSettings>
    (builder.Configuration.GetSection(RabbitMqSettings.DefaultSection));
    
builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization(builder);
builder.Services.AddSwagger();


builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Configuration.AddUserSecrets<Program>();



// Подключение слоёв
builder.Services.AddApplication();

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddProducer();
builder.Services.AddConsumer();


var app = builder.Build();


app.UseMiddleware<ExceptionHandlingMiddleware>();


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

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


app.UseHttpsRedirection();

app.MapControllers();

app.Run();