using Core.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using MyBlog.WebApi.DTOs.Users;
using MyBlog.WebApi.Infrastructure.Mapper.Extensions;
using Services.Users;
using System;

namespace MyBlog.WebApi.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        public UsersController(IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// It is an authentication method.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///        "Username": "emreoz37",
        ///        "Password": "1234"
        ///     }
        ///
        /// </remarks>
        /// <param name="user">User for authenticate </param>
        /// <returns>user information</returns>
        /// <response code="200">If logged in</response>
        /// <response code="400">If bad request</response>   
        /// <response code="404">If the item is null</response>   
        /// <response code="500">If something went wrong</response>      
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserForAuthenticateDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object),StatusCodes.Status500InternalServerError)]
        public IActionResult Authenticate([FromBody]UserForAuthenticateDto user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogError("User object sent from client is null.");
                    return NotFound();
                }

                var userEntity = _userService.Authenticate(user.Username, user.Password);
                if (userEntity == null)
                    return BadRequest("Username or password is wrong!");

                var userDto = userEntity.ToDto<UserDto>();

                return Ok(userDto);

            }
            catch (Exception ex)
            {
                var logMessage = $"UsersController Authenticate Method. Something went wrong. Ex Message : { ex.Message }";
                _logger.LogError(ex, logMessage);
                return StatusCode(500, "Internal server error");
            }
           
        }
    }
}

