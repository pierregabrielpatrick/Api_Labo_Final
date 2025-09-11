using Api_Labo_Final.dto;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Labo_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseController : ControllerBase
    {

        public HouseController(MyDbContext context)
        {
            _context = context;
        }

        [Authorize("auth")]
        [HttpGet]
        public IActionResult GetOwnedHouses()
        {
            int userId = User.GetId();

            List<House> houses = _context.Houses
                .Where(h => h.Users.Any(u => u.Id == userId))
                .ToList();

            return Ok(houses);
        }

        [Authorize("Auth")]
        [HttpPost]
        public IActionResult AddHouse([FromBody] HouseAddDto dto)
        {
            // je cherche dans la db si la maison existe dejà
            House? house = _context.Houses.FirstOrDefault(h => h.Name == dto.Name);
            if (house == null)
            {
                // si elle n'existe pas je l'enregistre dans la DB
                house = _context.Add(
                    new House
                    {
                        Name = dto.Name,
                        IPV4 = dto.IPV4,
                        IsActive = dto.IsActive
                    }
                ).Entity;
                _context.SaveChanges();
            }

            // je cherche l'utilisateur connecté et les maisons
            // qui lui sont attribuées 
            User user = _context.Users
                .Include(u => u.Houses)
                .FirstOrDefault(u => u.Id == User.GetId())!;

            // j'ajoute à l'uilisateur la maison
            user.Houses.Add(house);
            _context.SaveChanges();
            return Created();
        }


        [HttpGet("User")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Houses)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    role = u.Role.ToString(),
                    Username = u.Username,
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
