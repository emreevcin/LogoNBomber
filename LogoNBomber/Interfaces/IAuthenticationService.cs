using LogoNBomber.Dtos;

namespace LogoNBomber.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> Login(UserDto user, HttpClient httpClient);
        Task Logout(UserDto user, HttpClient httpClient);
    }
}
