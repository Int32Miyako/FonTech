﻿using FluentValidation;
using FonTech.Application.Mapping;
using FonTech.Application.Services;
using FonTech.Application.Validations;
using FonTech.Application.Validations.FluentValidation.Report;
using FonTech.Domain.Dto.Report;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Interfaces.Validations;
using FonTech.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FonTech.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(ReportMapping));

        
        var options = configuration.GetSection(nameof(RedisSettings));
        var redisUrl = options["RedisUrl"];
        var instanceName = options["InstanceName"];
        
        services.AddStackExchangeRedisCache(redisCacheOptions =>
        {
            redisCacheOptions.Configuration = redisUrl;
            redisCacheOptions.InstanceName = instanceName;
        });
        
        
        InitServices(services);
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IReportValidator, ReportValidator>();
        services.AddScoped<IValidator<CreateReportDto>, CreateReportValidator>();
        services.AddScoped<IValidator<UpdateReportDto>, UpdateReportValidator>();

        
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRoleService, RoleService>();
    }
}