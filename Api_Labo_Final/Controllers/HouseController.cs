using Api_Labo_Final.dto;
using Api_Labo_Final.Utils;
using BusinessLogicLayer;
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

        private readonly HouseService _houseservice;

        public HouseController(HouseService houseservice)
        {
            _houseservice = houseservice;
        }

        [Authorize("auth")]
        [HttpGet]
        public IActionResult GetOwnedHouses()
        {
            int userId = User.GetId();

            var houses  = this._houseservice.GetAllHouse(userId);

            return Ok(houses);
        }

        [Authorize("Auth")]
        [HttpPost]
        public IActionResult AddHouse([FromBody] HouseAddDto dto)
        {
            //// je cherche dans la db si la maison existe dejà
            //House? house = _context.Houses.FirstOrDefault(h => h.Name == dto.Name);
            //if (house == null)
            //{
            //    // si elle n'existe pas je l'enregistre dans la DB
            //    house = _context.Add(
            //        new House
            //        {
            //            Name = dto.Name,
            //            IPV4 = dto.IPV4,
            //            IsActive = dto.IsActive
            //        }
            //    ).Entity;
            //    _context.SaveChanges();
            //}

            //// je cherche l'utilisateur connecté et les maisons
            //// qui lui sont attribuées 
            //User user = _context.Users
            //    .Include(u => u.Houses)
            //    .FirstOrDefault(u => u.Id == User.GetId())!;

            //// j'ajoute à l'uilisateur la maison
            //user.Houses.Add(house);
            //_context.SaveChanges();

            this._houseservice.AddHouse(dto, User.GetId());
            return Created();
        }


    
    }
}
