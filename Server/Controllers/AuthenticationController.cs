using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;
using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    
    
    public class AuthenticationController(IUserAccount accountInterface) : ControllerBase
    {
        [HttpPost("register")]
        
        public async Task<IActionResult> CreateAsync(Register user)
        {
            if (user == null) return BadRequest("Model is empty");
            var result = await accountInterface.CreateAsync(user);
            return Ok(result);
        }

        [HttpPost("login")]
        
        public async Task<IActionResult> SignInAsync(Login user)
        {
            if (user == null) return BadRequest("Model is empty");
            var result = await accountInterface.SignInAsync(user);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        
        public async Task<IActionResult> RefreshTokenAsync(RefreshToken token)
        {
            if (token == null) return BadRequest("Model is empty");
            var result = await accountInterface.RefreshTokenAsync(token);
            return Ok(result);
        }
        
        [HttpPost("get-claims")]
        
        public async Task<IActionResult> GetClaims([FromBody] RefreshToken refreshToken)
        {
            if (refreshToken == null || string.IsNullOrEmpty(refreshToken.Token))
            {
                return BadRequest("Refresh token is required.");
            }

            var claimsResponse = await accountInterface.GetClaimsAsync(refreshToken);
            if (claimsResponse == null || !claimsResponse.Flag)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            return Ok(claimsResponse);
        }
        [HttpGet("users")]
        
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await accountInterface.GetUsers();
            if (users == null) return NotFound();
            return Ok(users);
        }
        [HttpPut("update-user")]
        
        public async Task<IActionResult> UpdateUser(ManageUser manageUser)
        {
            var result = await accountInterface.UpdateUser(manageUser);
            return Ok(result);
        }
        [HttpGet("roles")]
        
        public async Task<IActionResult> GetRoles()
        {
            var users = await accountInterface.GetRoles();
            if (users == null) return NotFound();
            return Ok(users);
        }
        [HttpDelete("delete-user/{id}")]
        
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await accountInterface.DeleteUser(id);
            return Ok(result);

        }
        [HttpGet("user-image/{id}")]
        [Authorize]
        [Produces("text/plain")]  // or [Produces("application/json")]
        public async Task<IActionResult> GetUserImage(int id)
        {
            var result = await accountInterface.GetUserImage(id);
            return Content(result, "text/plain"); // or return Ok(result) if JSON
        }
        [HttpPut("update-profile")]
        [Authorize]
        
        public async Task<IActionResult> UpdateProfile(UserProfile profile)
        {
            var result = await accountInterface.UpdateProfile(profile);
            return Ok(result);
        }
        /*[HttpPost("upload-image/{userId}")]
        [Authorize]
        public async Task<IActionResult> UploadImage(int userId, [FromForm]IFormFile file)
        {
            var result = await accountInterface.UploadImage(userId, file);
            return Ok(result);
        }*/


    }
}