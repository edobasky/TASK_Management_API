
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TaskManagement.API.DataAccess;
using TaskManagement.API.DTOs;
using TaskManagement.API.Model;
using TaskManagement.API.Services.Interface;
using TaskManagement.API.Services.Logger;
using TaskManagement.API.Utility;
using TaskManagement.API.Utility.Paged;

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

        public async Task<GenericResponse<SingleTaskToReturnDto>> GetTaskByIdAsync(int taskId, int userId)
        {
            try
            {
                _logger.LogInfo($"GetTaskByIdAsync method call || About to fetch a single task with taskId : {taskId} for user with userId : {userId}");
                var fetchSingleTask = await _appDb.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);


                if (fetchSingleTask is null) return new GenericResponse<SingleTaskToReturnDto> { Successful = false, ResponseCode = "99", Message = "No task exist for the user at the moment" };


                var taskToReturn = ManualMap.MapToTaskReturn(fetchSingleTask);
                _logger.LogInfo($"Task fetched successful with response : {JsonConvert.SerializeObject(taskToReturn)}");
                return new GenericResponse<SingleTaskToReturnDto> { Successful = true, ResponseCode = "00", Message = "Task fetch successful", Data = taskToReturn };

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception : An error occured with message : {ex.Message} and Trace : {ex.StackTrace}");
                return new GenericResponse<SingleTaskToReturnDto> { ResponseCode = "98", Message = "Unable to carry out this request at this time, please try again or contact support", Successful = false };
            }
        }

        public async Task<(GenericResponse<PagedList<TaskToReturnDto>>, MetaData metaData)> GetTasks(int userId, TaskParameters taskParameters, CancellationToken ct = default)
        {
            try
            {
                #region User verify
                if (!await _authentication.UserExist(userId))
                {
                    _logger.LogInfo($"Unable to find user with Id ==> {userId}");
                    return (new GenericResponse<PagedList<TaskToReturnDto>>
                    {
                        Successful = false,
                        ResponseCode = "98",
                        Message = "User does exist in our system"
                    }, new MetaData());
                }
                #endregion

                var TaskQueryBuilder = TaskQuetBuilder(userId, taskParameters);

                var count = await TaskQueryBuilder.CountAsync(ct);

                var TaskFetch = await TaskQueryBuilder
                    .Skip((taskParameters.PageNumber - 1) * taskParameters.PageSize)
                    .Take(taskParameters.PageSize)
                    .ToListAsync(ct);

                

                var TaskToReturn = ManualMap.MapToTaskReturn(TaskFetch);

                var listToReturn = PagedList<TaskToReturnDto>.ToPagedList(TaskToReturn, count, taskParameters.PageNumber, taskParameters.PageSize);

                return (new GenericResponse<PagedList<TaskToReturnDto>> { Successful = true, ResponseCode = "00", Message = "Task Fetched successfully", Data = listToReturn }, listToReturn.MetaData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception : An error occured with message : {ex.Message} and Trace : {ex.StackTrace}");
                return (new GenericResponse<PagedList<TaskToReturnDto>> { ResponseCode = "98", Message = "Unable to carry out this request at this time, please try again or contact support", Successful = false }, new MetaData());
            }
        }

        private IOrderedQueryable<TaskItem> TaskQuetBuilder(int userId, TaskParameters taskParameters)
        {
            var TaskFetchQuery = _appDb.TaskItems.AsNoTracking()
                                .Where(x => x.UserId == userId)
                                .OrderBy(t => t.Id);
            if (taskParameters.Status > 0)
            {
                // 
                TaskFetchQuery = _appDb.TaskItems.AsNoTracking()
                                .Where(x => x.UserId == userId && x.Status == Enum.GetName(typeof(Status), taskParameters.Status))
                                .OrderBy(t => t.Id);
            }
            if (taskParameters.Priority > 0)
            {
                TaskFetchQuery = _appDb.TaskItems.AsNoTracking()
                                .Where(x => x.UserId == userId && x.Priority == Enum.GetName(typeof(Priority), taskParameters.Priority))
                                .OrderBy(t => t.Id);
            }
            if (taskParameters.Status > 0 && taskParameters.Priority > 0)
            {
                TaskFetchQuery = _appDb.TaskItems.AsNoTracking()
                                .Where(x => x.UserId == userId && x.Status == Enum.GetName(typeof(Status), taskParameters.Status) && x.Priority == Enum.GetName(typeof(Priority), taskParameters.Priority))
                                .OrderBy(t => t.Id);
            }

            return TaskFetchQuery;  
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

        public async Task<GenericResponse<dynamic>> UpdateTask(int taskId, int userId, TaskToUpdateDto toUpdate)
        {
            try
            {
                var TaskFetch = await _appDb.TaskItems.FirstOrDefaultAsync(x => x.Id == taskId && x.UserId == userId);

                if (TaskFetch == null) { return new GenericResponse<dynamic> { Successful = false, ResponseCode = "99", Message = "No task item found for user" }; }

                TaskFetch.Status = Enum.GetName(toUpdate.status)!;
                TaskFetch.Priority = Enum.GetName(toUpdate.priority)!;
                TaskFetch.Title = toUpdate.title;

                await _appDb.SaveChangesAsync();

                return new GenericResponse<dynamic> { Successful = true, Message = "Task updated successfully", ResponseCode = "00" };

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception : An error occured with message : {ex.Message} and Trace : {ex.StackTrace}");
                return new GenericResponse<dynamic> { ResponseCode = "98", Message = "Unable to carry out this request at this time, please try again or contact support", Successful = false };

            }
        }

        public async Task<GenericResponse<dynamic>> UpdateTaskStatus(int taskId, TaskStatusToUpdateDto taskStatusToUpdate)
        {
            try
            {
                if (taskStatusToUpdate.status != Status.Completed) { return new GenericResponse<dynamic> { ResponseCode = "98", Successful = false, Message = "Invalid status povided, please provide" }; }

                var FetchTasktoUpdateStatus = await _appDb.TaskItems.FirstOrDefaultAsync(x => x.Id == taskId && x.UserId == taskStatusToUpdate.userId);

                if (FetchTasktoUpdateStatus == null) { return new GenericResponse<dynamic> { Successful = false, ResponseCode = "99", Message = "Current Task does not exist for User" }; }

                FetchTasktoUpdateStatus.Status = Enum.GetName(taskStatusToUpdate.status)!;

                await _appDb.SaveChangesAsync() ;
                return new GenericResponse<dynamic> { Successful = true, ResponseCode = "00", Message = "Task Status Updated successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception : An error occured with message : {ex.Message} and Trace : {ex.StackTrace}");
                return new GenericResponse<dynamic> { ResponseCode = "98", Message = "Unable to carry out this request at this time, please try again or contact support", Successful = false };
            }
        }
    }
}
