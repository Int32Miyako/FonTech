namespace FonTech.Domain.Dto.User;
/// <summary>
/// если такой пользователь действительно будет
/// то ему будет выдаваться его токен и он сможет войти
/// </summary>
/// <param name="Login"></param>
/// <param name="Password"></param>
public record LoginUserDto(string Login, string Password);