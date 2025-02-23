using TaskManagement.API.DTOs;
using TaskManagement.API.Model;

namespace TaskManagement.API.Services.Interface
{
    public interface ITaskService
    {
        Task<GenericResponse<dynamic>> CreateTaskItemAsync(TaskCreateDto taskCreate);

        Task<GenericResponse<dynamic>> RemoveTaskItemAsync(int TaskId,int userId);

        Task<GenericResponse<IEnumerable<TaskToReturnDto>>> GetTasks(int userId);
    }
}
