using TaskManagement.API.DTOs;
using TaskManagement.API.Model;

namespace TaskManagement.API.Utility
{
    public class ManualMap
    {
       public static RegisteredUserResponseDto MapUserResponse(AppUser registerUser)
        {
            return new RegisteredUserResponseDto(registerUser.Id,registerUser.Username, registerUser.Email);
        }
    }
}
