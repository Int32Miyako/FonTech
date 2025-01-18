namespace FonTech.Domain.Dto;

/// <summary>
/// эта dto будет возвращаться только после логина пользователя
/// но не регистрации
/// </summary>
public class TokenDto
{
    public string AccessToken { get; set; }
    
    public string RefreshToken { get; set; }
}