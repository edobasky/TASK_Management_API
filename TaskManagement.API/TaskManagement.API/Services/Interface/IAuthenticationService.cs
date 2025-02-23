using TaskManagement.API.DTOs;
using TaskManagement.API.Model;

namespace TaskManagement.API.Services.Interface
{
    public interface IAuthenticationService
    {
        Task<GenericResponse<RegisteredUserResponseDto>> CreateNewUser(RegisterUserDto userDto);

        Task<GenericResponse<LoginResponseDto>> LoginUserAsync(LoginDto loginDto);

        Task<GenericResponse<UserDetailDto>> GetUserByIdAsync(int userId);

        Task<bool> UserExist(int UserId);
    }
}
