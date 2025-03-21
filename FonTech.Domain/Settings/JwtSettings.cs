﻿namespace FonTech.Domain.Settings;

public class JwtSettings
{
    // из какой секции тянем jwt токен в AppSettings
    public const string DefaultSection = "JwtSettings";

    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Authority { get; set; }
    public string JwtKey { get; set; }
    public int Lifetime { get; set; }
    public int RefreshTokenValidityInDays { get; set; }
}