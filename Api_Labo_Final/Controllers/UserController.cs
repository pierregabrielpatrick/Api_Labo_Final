using Api_Labo_Final.dto;
using Api_Labo_Final.Mapper;
using BLL.user;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Labo_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> GetAllUsers()
        {
            List<User> users = await this._userService.GetAllUsers();
            if(users == null)
            {
                return NoContent();
            }           
            List<UserDTO> usersDto = users.Select( p => p.ToUserDTO()).ToList();
            return Ok(usersDto);
        }
    }
}
