using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TaskManagement.API.DTOs;
using TaskManagement.API.Model;
using TaskManagement.API.Services.Interface;
using TaskManagement.API.Services.Logger;
using TaskManagement.API.Validations;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authentication;
        private readonly ILoggerServ _logger;

        public AuthController(IAuthenticationService authentication, ILoggerServ logger)
        {
            _authentication = authentication;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateNewUser([FromBody]RegisterUserDto registerUserDto)
        {
            var check = ModelValidationCheck(registerUserDto);
            if (!check.Successful) return Ok(check);

            _logger.LogInfo($"Request received with payload : {JsonConvert.SerializeObject(registerUserDto)} at {DateTime.UtcNow}");
            var createUserRes = await _authentication.CreateNewUser(registerUserDto);

            return Ok(createUserRes);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin([FromBody]LoginDto loginDto)
        {
            // validation goes here

            var userLogin = await _authentication.LoginUserAsync(loginDto);

            return Ok(userLogin);
        }



        #region user registration model validation
        private static GenericResponse<dynamic> ModelValidationCheck(RegisterUserDto registerUserDto)
        {
            UserValidation _validator = new UserValidation();
            var validResult = _validator.Validate(registerUserDto);
            if (!validResult.IsValid)
            {
                return new GenericResponse<dynamic>()
                {
                    ResponseCode = "99",
                    Successful = false,
                    Message = String.Join(", ", validResult.Errors)
                };
            }
            return new GenericResponse<dynamic>() { Successful = true, ResponseCode = "00" };
        }
        #endregion

        [Authorize]
        [HttpGet("UserDetailById")]
        public async Task<IActionResult> GetUserDetail(int userId)
        {
            _logger.LogInfo($"{nameof(GetUserDetail)} coontroller call || Request received with payload : {userId} at {DateTime.UtcNow}");
            var userResult = await _authentication.GetUserByIdAsync(userId);
            _logger.LogInfo($"GetUserByIdAsync Method call || Response returned from call at {DateTime.UtcNow} =====> {JsonConvert.SerializeObject(userResult)}");
            return Ok(userResult);
        }
        
    }
}
