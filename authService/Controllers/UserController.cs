using System;
using System.Threading.Tasks;
using authService.Data;
using authService.Models;
using authService.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace authService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        readonly IUserService _userServiceImpl;

        public UserController(IUserService userServiceImpl)
        {
            _userServiceImpl = userServiceImpl;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var response = await _userServiceImpl.Register(user);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Authenticate authenticate)
        {
            var response = await _userServiceImpl.Authenticate(authenticate);

            if (response == null)
                return BadRequest(new {message = "Username or password is incorrect"});

            return Ok(response);
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshToken refreshToken)
        {
            var response = await _userServiceImpl.RefreshToken(refreshToken);

            if (response == null)
                return Unauthorized(new {message = "Invalid token"});

            return Ok(response);
        }

        [Authorize]
        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeToken model)
        {
            if (string.IsNullOrEmpty(model.refreshToken))
                return BadRequest(new {message = "Token is required"});

            var response = await _userServiceImpl.RevokeRefreshToken(model);
            if (!response)
                return NotFound(new {message = "Token not found"});

            return Ok(new {message = "Token revoked"});
        }
        
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var users = _userServiceImpl.GetAll();
            return Ok(users);
        }
    }
}
