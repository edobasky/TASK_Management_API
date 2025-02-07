using Microsoft.EntityFrameworkCore;
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
                #region username and Email check
                var checkUsername = await _appDb.AppUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Username.Equals(userDto.username,StringComparison.CurrentCultureIgnoreCase));
                if (checkUsername is not null)
                {
                    return new GenericResponse<RegisteredUserResponseDto>
                    {
                        Successful = false,
                        Message = "Sorry, Username already exist.",
                        ResponseCode = "99"
                    };
                }
                var checkEmail = await _appDb.AppUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Email.Equals(userDto.username,StringComparison.CurrentCultureIgnoreCase));
                if (checkEmail is not null)
                {
                    return new GenericResponse<RegisteredUserResponseDto>
                    {
                        Successful = false,
                        Message = "Sorry, Email already exist.",
                        ResponseCode = "99"
                    };
                }
                #endregion

                string passwordHarsh = BC.EnhancedHashPassword(userDto.password,13);

                var userToCreate = new AppUser
                {
                    Username = userDto.username,
                    Email = userDto.email,
                    PasswordHash =  passwordHarsh,
                    Role = "Admin"
                };

                var addUser = _appDb.AppUsers.AddAsync(userToCreate);
                 await _appDb.SaveChangesAsync();



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

                throw;
            }


            throw new NotImplementedException();
        }
    }
}
