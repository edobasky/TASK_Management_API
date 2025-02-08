using TaskManagement.API.DTOs;
using TaskManagement.API.Model;

namespace TaskManagement.API.Services.Interface
{
    public interface IAuthenticationService
    {
        Task<GenericResponse<RegisteredUserResponseDto>> CreateNewUser(RegisterUserDto userDto);

        Task<GenericResponse<UserDetailDto>> GetUserByIdAsync(int userId);
    }
}
