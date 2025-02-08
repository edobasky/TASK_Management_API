﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TaskManagement.API.DataAccess;
using TaskManagement.API.DTOs;
using TaskManagement.API.Model;
using TaskManagement.API.Services.Interface;
using TaskManagement.API.Services.Logger;
using TaskManagement.API.Utility;

namespace TaskManagement.API.Services.Implementation
{
    public class AuthenticationServiceImp : IAuthenticationService
    {
        private readonly AppDbContext _appDb;
        private readonly ILoggerServ _logger;

        public AuthenticationServiceImp(AppDbContext appDb,ILoggerServ logger)
        {
            _appDb = appDb;
            _logger = logger;
        }

        public async Task<GenericResponse<RegisteredUserResponseDto>> CreateNewUser(RegisterUserDto userDto)
        {
            // first check for exising user mails and names, we cant have duplicatte
            // if above is false, then hash password and return details of user created
            try
            {
                _logger.LogInfo($"{nameof(CreateNewUser)} Method call || About to verify supplied detail exist with user email : {userDto.email} and userName : {userDto.username}");
                #region username and Email check
                var checkUsername = await _appDb.AppUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Username.Equals(userDto.username) || x.Email.Equals(userDto.email));
                if (checkUsername is not null)
                {
                    return new GenericResponse<RegisteredUserResponseDto>
                    {
                        Successful = false,
                        Message = "Sorry, Username/email already exist.",
                        ResponseCode = "98"
                    };
                }
                #endregion

                #region hash password and create user
                string passwordHarsh = BC.EnhancedHashPassword(userDto.password,13);

                var userToCreate = new AppUser
                {
                    Username = userDto.username,
                    Email = userDto.email,
                    PasswordHash =  passwordHarsh,
                    Role = "Regular"
                };

                _logger.LogInfo($"{nameof(CreateNewUser)} Method call || About to persist a new user with details : {JsonConvert.SerializeObject(userToCreate)}");
                var addUser = _appDb.AppUsers.AddAsync(userToCreate);
                int response =  await _appDb.SaveChangesAsync();
                #endregion

                if (response <= 0)
                {
                    _logger.LogInfo($"{nameof(CreateNewUser)} Method call || Failed to persist a new user with userName : {userDto.username}");
                    return new GenericResponse<RegisteredUserResponseDto>()
                    {
                        Successful = false,
                        ResponseCode = "01",
                        Message = "User creation not successful, please try again",
                    };
                }


                // automate a mail sent to usermail using hangfire fire and forget -- Production level app
                var userToReturn = ManualMap.MapUserResponse(userToCreate);
                return new GenericResponse<RegisteredUserResponseDto>()
                {
                    Successful = true,
                    ResponseCode = "00",
                    Message = "User created successfully",
                    Data = userToReturn
                };
            }
            catch (Exception ex)
            {

                _logger.LogError($"{nameof(CreateNewUser)} Method called || an error occured during method operation =======> {ex}");
                return new GenericResponse<RegisteredUserResponseDto> { Successful = false, ResponseCode = "99", Message = "We are unable to complete this request at this time,please try again" };
            }


            throw new NotImplementedException();
        }

        public async Task<GenericResponse<UserDetailDto>> GetUserByIdAsync(int userId)
        {
            try
            {
                _logger.LogInfo($"About to fetch user with id ======> {userId}");
                var getUser = await _appDb.AppUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);

                if (getUser is null) return new GenericResponse<UserDetailDto>() { Successful = false, ResponseCode = "97", Message = "User does not exist,please create one" };

                var UsertoReturn = ManualMap.MapUserDetail(getUser);

                _logger.LogInfo($"{nameof(GetUserByIdAsync)} Method called || user detail fetch success with response ======> {UsertoReturn}");
                return new GenericResponse<UserDetailDto>() { Successful = true, ResponseCode = "00", Message = "User fetch successful", Data = UsertoReturn };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetUserByIdAsync)} Method called || an error occured during method operation =======> {ex}");
                return new GenericResponse<UserDetailDto> { Successful = false, ResponseCode = "99", Message = "We are unable to complete this request at this time,please try again" };
                
            }
        }

    }
}
