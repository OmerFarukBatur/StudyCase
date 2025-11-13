using Core.DTOs;
using Core.DTOs.UserDtos;

namespace Core.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<ResponseMessageDto> UserCreateAsync(UserCreateDto userCreateDto);
        Task<Tuple<ResponseMessageDto, AuthenticationDto?>> Login(LoginDto loginDto);
        Task<ResponseMessageDto> PasswordReset(PasswordResetDto userCreateDto);
    }
}
