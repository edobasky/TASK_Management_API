using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.DataAccess;
using TaskManagement.API.DTOs;
using TaskManagement.API.Model;
using TaskManagement.API.Services.Interface;
using TaskManagement.API.Services.Logger;
using TaskManagement.API.Utility;

namespace TaskManagement.API.Services.Implementation
{
    public class TaskServiceImp : ITaskService
    {
        private readonly IAuthenticationService _authentication;
        private readonly ILoggerServ _logger;
        private readonly AppDbContext _appDb;

        public TaskServiceImp(IAuthenticationService authentication, ILoggerServ logger, AppDbContext appDb)
        {
            _authentication = authentication;
            _logger = logger;
            _appDb = appDb;
        }

        public async Task<GenericResponse<dynamic>> CreateTaskItemAsync(TaskCreateDto taskCreate)
        {
            try
            {
                #region User verify
                if (!await _authentication.UserExist(taskCreate.userId))
                {
                    _logger.LogInfo($"Unable to find user with Id ==> {taskCreate.userId}");
                    return new GenericResponse<dynamic>
                    {
                        Successful = false,
                        ResponseCode = "98",
                        Message = "User does exist in our system"
                    };
                }
                #endregion

                var TaskToSave = ManualMap.MapFromTaskCreateDto(taskCreate);

                _appDb.TaskItems.Add(TaskToSave);
                int taskCreateStatus = await _appDb.SaveChangesAsync();

                if (taskCreateStatus == 0)
                {
                    return new GenericResponse<dynamic>
                    {
                        Successful = false,
                        Message = "Task was not created successfully",
                        ResponseCode = "99",

                    };
                }

                return new GenericResponse<dynamic>
                {
                    Successful = true,
                    Message = "Task was created successfully",
                    ResponseCode = "00"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception : An error occured with message : {ex.Message} and Trace : {ex.StackTrace}");
                return new GenericResponse<dynamic> { ResponseCode = "98", Message = "Unable to carry out this request at this time, please try again or contact support", Successful = false };
            }

        }

        public async Task<GenericResponse<IEnumerable<TaskToReturnDto>>> GetTasks(int userId)
        {
            try
            {
                #region User verify
                if (!await _authentication.UserExist(userId))
                {
                    _logger.LogInfo($"Unable to find user with Id ==> {userId}");
                    return new GenericResponse<IEnumerable<TaskToReturnDto>>
                    {
                        Successful = false,
                        ResponseCode = "98",
                        Message = "User does exist in our system"
                    };
                }
                #endregion

                var TaskFetch = await _appDb.TaskItems.AsNoTracking().ToListAsync();

                var TaskToReturn = ManualMap.MapToTaskReturn(TaskFetch);

                return new GenericResponse<IEnumerable<TaskToReturnDto>> { Successful = true, ResponseCode = "00", Message = "Task Fetched successfully", Data = TaskToReturn };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception : An error occured with message : {ex.Message} and Trace : {ex.StackTrace}");
                return new GenericResponse<IEnumerable<TaskToReturnDto>> { ResponseCode = "98", Message = "Unable to carry out this request at this time, please try again or contact support", Successful = false };
            }
        }

        public async Task<GenericResponse<dynamic>> RemoveTaskItemAsync(int TaskId,int userId)
        {
            try
            {
                var getTask = await _appDb.TaskItems.FirstOrDefaultAsync(x => x.Id == TaskId && x.UserId == userId);
                if (getTask == null)
                { // add log
                    return new GenericResponse<dynamic> { Successful = false, Message = "No Task with the Id found", ResponseCode = "99" };
                };

                _appDb.Remove(getTask);
                await _appDb.SaveChangesAsync();
                return new GenericResponse<dynamic> { Successful = true, Message = "Task Item deleted successfully", ResponseCode = "00" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception : An error occured with message : {ex.Message} and Trace : {ex.StackTrace}");
                return new GenericResponse<dynamic> { ResponseCode = "98", Message = "Unable to carry out this request at this time, please try again or contact support", Successful = false };
            }
        }
    }
}
