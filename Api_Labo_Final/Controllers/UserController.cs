using Api_Labo_Final.dto;
using Api_Labo_Final.Mapper;
using BusinessLogicLayer;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Labo_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            this._userService = userService;
        }

        [HttpGet("User")]
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
