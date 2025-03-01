using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs;
using TaskManagement.API.Model;
using TaskManagement.API.Services.Interface;
using TaskManagement.API.Utility.Paged;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("NewTask")]
        public async Task<IActionResult> CreateNewTask([FromBody] TaskCreateDto taskCreate)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
            if (userId != taskCreate.userId) return Ok(new GenericResponse<dynamic> { ResponseCode = "97", Message = "Invalid User access", Successful = false });
            var createNewTask = await _taskService.CreateTaskItemAsync(taskCreate);
            return Ok(createNewTask);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllTasks([FromQuery] TaskParameters parameters, CancellationToken ct)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
            var pagedTaskResult = await _taskService.GetTasks(userId,parameters,ct);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedTaskResult.metaData));
            return Ok(pagedTaskResult.Item1);
        }

        [HttpGet("Id")]
        public async Task<IActionResult> GetTaskById(int TaskId)
        {
            var UserId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
            var taskGet = await _taskService.GetTaskByIdAsync(TaskId, UserId);
            return Ok(taskGet);
        }

        [HttpPut("{TaskId}")]
        public async Task<IActionResult> UpdateTask(int TaskId, int userId, TaskToUpdateDto taskToUpdate)
        {
            var UserId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
            if (UserId != userId) return Ok(new GenericResponse<dynamic> { ResponseCode = "97", Message = "Invalid User access", Successful = false });
            var taskRemoval = await _taskService.UpdateTask(TaskId,userId,taskToUpdate);
            return Ok(taskRemoval);
        }

        [HttpPut("{TaskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int TaskId, TaskStatusToUpdateDto taskToUpdate)
        {
            var UserId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
            if (UserId != taskToUpdate.userId) return Ok(new GenericResponse<dynamic> { ResponseCode = "97", Message = "Invalid User access", Successful = false });
            var taskRemoval = await _taskService.UpdateTaskStatus(TaskId, taskToUpdate);
            return Ok(taskRemoval);
        }

        [HttpDelete("Id")]
        public async Task<IActionResult> RemoveTask(int TaskId)
        {
            var UserId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value); 
            var taskRemoval = await _taskService.RemoveTaskItemAsync(TaskId,UserId);
            return Ok(taskRemoval);
        }


    }
}
