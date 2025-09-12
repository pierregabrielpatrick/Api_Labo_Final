using Api_Labo_Final.dto;
using Api_Labo_Final.Utils;
using BLL;

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

        private readonly IHouseService _houseservice;

        public HouseController(IHouseService houseservice)
        {
            _houseservice = houseservice;
        }

        [Authorize("auth")]
        [HttpGet]
        public IActionResult GetOwnedHouses()
        {
            int userId = User.GetId();

            var houses = this._houseservice.GetAllHouse(userId);

            if (houses == null)
            {
                return NotFound();
            }

            return Ok(houses);
        }

        [Authorize("Auth")]
        [HttpPost]
        public IActionResult AddHouse([FromBody] HouseAddDto dto)
        {
            // Vérifications recommandées
            if (dto == null)
                return BadRequest("Les données de la maison sont requises");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Le nom de la maison est requis");

            if (string.IsNullOrWhiteSpace(dto.IPV4))
                return BadRequest("L'adresse IPv4 est requise");
            try
            {
                //recommendations ajoutées
                House houseRequest =
                      new House
                      {
                          Name = dto.Name,
                          IPV4 = dto.IPV4,
                          IsActive = dto.IsActive
                      };

                bool valid = this._houseservice.AddHouse(houseRequest, User.GetId());

                if (!valid)
                    return StatusCode(500, "Erreur lors de la création de la maison");

                return Created($"/api/houses/{houseRequest.Id}", houseRequest);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log l'erreur
                return StatusCode(500, "Erreur interne du serveur");
            }


        }



    }
}
