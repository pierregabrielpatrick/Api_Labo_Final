using Api_Labo_Final.dto;
using Api_Labo_Final.Mapper;
using Api_Labo_Final.Utils;
using BLL;
using Dal.context;
using Domain;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api_Labo_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly JwtUtils _jwtUtils;

        public AuthController(IAuthService _authService, JwtUtils jwtUtils)
        {
            this._authService = _authService;
            this._jwtUtils = jwtUtils;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterFormDTO form)
        {
            try
            {
                if (this._authService == null)
                {
                    return StatusCode(500, new { Content = "Service difficle not available" });
                }
                // Validation du DTO
                if (form == null || string.IsNullOrEmpty(form.Username))
                {
                    return BadRequest(new { Content = "Invalid form data" });
                }
                bool valid2 = this._authService.CheckExistence(form.Username);
                Console.WriteLine(valid2);
                if (valid2)
                {
                    return BadRequest(new { Content = $"User with username {form.Username} already exist" });
                }
                User user = form.ToUser();
                user.Password = Argon2.Hash(form.Password);
                user.Role = UserRole.USER;
                int neoId = this._authService.Insert(user);
                return Ok(neoId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Register: {ex.Message}");
                return StatusCode(500, new { Content = "Internal server error" });
            }

        }

        [HttpPost("login")]
        public ActionResult<UserTokenDTO> Login([FromBody] LoginFormDTO form)
        {
            User? user = this._authService.CheckValidity(form.Username);               
               

            if (user == null)
            {
                return BadRequest(new { Content = $"User with username {form.Username} doesn't exist" });
            }

            if (!Argon2.Verify(user.Password, form.Password))
            {
                return BadRequest(new { Content = "Wrong password" });
            }

            UserDTO dto = user.ToUserDTO();
            string token = _jwtUtils.GenerateToken(user);

            UserTokenDTO result = new UserTokenDTO()
            {
                User = dto,
                Token = token
            };

            return Ok(result);
        }
    }
}
