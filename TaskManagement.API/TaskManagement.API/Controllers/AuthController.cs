using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TaskManagement.API.DTOs;
using TaskManagement.API.Model;
using TaskManagement.API.Services.Interface;
using TaskManagement.API.Services.Logger;

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
        public async Task<IActionResult> CreateNewUser(RegisterUserDto registerUserDto)
        {
            if(!ModelState.IsValid)
            {
               var userResponse = new GenericResponse<RegisterUserDto>();
                userResponse.Successful = false;
                userResponse.ResponseCode = "98";
                userResponse.Message = "Empty Properties";

                return Ok(userResponse);
            }
            _logger.LogInfo($"Request received with payload : {JsonConvert.SerializeObject(registerUserDto)} at {DateTime.UtcNow}");
            var createUserRes = await _authentication.CreateNewUser(registerUserDto);

            return Ok(createUserRes);
        }

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
