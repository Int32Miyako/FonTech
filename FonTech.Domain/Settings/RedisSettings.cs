namespace FonTech.Domain.Settings;

public class RedisSettings
{
    public const string DefaultSection = "RedisSettings";
    
    public string RedisUrl { get; init; }
    
    public string InstanceName { get; init; }
    
}