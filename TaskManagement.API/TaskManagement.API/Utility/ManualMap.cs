using TaskManagement.API.DTOs;
using TaskManagement.API.Model;

namespace TaskManagement.API.Utility
{
    public class ManualMap
    {
       public static RegisteredUserResponseDto MapUserResponse(AppUser registerUser)
        {
            return new RegisteredUserResponseDto(registerUser.Id,registerUser.Username,registerUser.Email);
        }

        public static UserDetailDto MapUserDetail(AppUser user)
        {
            return new UserDetailDto(user.Id,user.Username,user.Email,user.Role);
        }

        public static LoginResponseDto MapUserLoginResponse(AppUser user)
        {
            return new LoginResponseDto
            {
                userId = user.Id,
                email = user.Email!,
                userName = user.Username!,
                Token = String.Empty,
                tokenExpireIn = 0

            };
        }

        public static TaskItem MapFromTaskCreateDto(TaskCreateDto taskCreateDto)
        {
            return new TaskItem
            {
                Title = taskCreateDto.Title,
                Description = taskCreateDto.Description,
                DueDate = taskCreateDto.DueDate,    
                Priority = Enum.GetName(taskCreateDto.Priority) is null ? throw new Exception("Priority choosen does not exist") : Enum.GetName(taskCreateDto.Priority)!,
                Status = Enum.GetName(taskCreateDto.Status)!,
                CreatedAt = DateTime.UtcNow,
                UserId = taskCreateDto.userId,
            };
        }

        public static IEnumerable<TaskToReturnDto> MapToTaskReturn(IEnumerable<TaskItem> taskItem)
        {
            var TaskToReturn = taskItem.Select(x => new TaskToReturnDto
            {
                Id = x.Id,
                Status = x.Status,
                Priority = x.Priority,
                DueDate = x.DueDate,
            });

            return TaskToReturn;
        }

        public static SingleTaskToReturnDto MapToTaskReturn(TaskItem taskItem)
        {
            return new SingleTaskToReturnDto
            {
                Id = taskItem.Id,
                Status = taskItem.Status,
                Priority = taskItem.Priority,
                DueDate = taskItem.DueDate,
                Description = taskItem.Description
            };
        }

        public static bool verifyEnum(Priority taskPriority)
        {
            if(Enum.IsDefined(typeof(Priority), taskPriority))
            {
                return true;
            }
            return false;
                   
        }
    }
}
