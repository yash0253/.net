
using EnterpriseServiceManagement.DTOs;

namespace EnterpriseServiceManagement.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);

    Task<LoginResponseDto> LoginAsync(LoginDto dto);
}
