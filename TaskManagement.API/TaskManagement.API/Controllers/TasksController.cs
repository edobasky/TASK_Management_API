using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs;
using TaskManagement.API.Services.Interface;

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
            var createNewTask = await _taskService.CreateTaskItemAsync(taskCreate);
            return Ok(createNewTask);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllTasks()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
            var Tasks = await _taskService.GetTasks(userId);
            return Ok(Tasks);
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
