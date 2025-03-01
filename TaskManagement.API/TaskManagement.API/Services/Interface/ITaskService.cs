using TaskManagement.API.DTOs;
using TaskManagement.API.Model;
using TaskManagement.API.Utility.Paged;

namespace TaskManagement.API.Services.Interface
{
    public interface ITaskService
    {
        Task<GenericResponse<dynamic>> CreateTaskItemAsync(TaskCreateDto taskCreate);

        Task<GenericResponse<dynamic>> RemoveTaskItemAsync(int TaskId,int userId);

        Task<(GenericResponse<PagedList<TaskToReturnDto>>, MetaData metaData
            )> GetTasks(int userId, TaskParameters taskParameters, CancellationToken ct);

        Task<GenericResponse<SingleTaskToReturnDto>> GetTaskByIdAsync(int taskId, int userId);

        Task<GenericResponse<dynamic>> UpdateTask(int taskId, int userId, TaskToUpdateDto toUpdate);

        Task<GenericResponse<dynamic>> UpdateTaskStatus(int taskId, TaskStatusToUpdateDto taskStatusToUpdate);
    }
}
