using Dal.context;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class HouseServiceImpl : IHouseService
    {
        private FinalContext _context;
        public List<House> GetAllHouse(int userId)
        {
            List<House> houses = _context.Houses
                .Where(h => h.Users.Any(u => u.Id == userId))
                .ToList();

            return houses;
        }

        public bool AddHouse(House houseRequest, int userId)
        {
            // je cherche dans la db si la maison existe dejà
            House? house = _context.Houses.FirstOrDefault(h => h.Name == houseRequest.Name);
            if (house == null)
            {
                // si elle n'existe pas je l'enregistre dans la DB
                house = _context.Add(houseRequest
                ).Entity;
                _context.SaveChanges();
            }

            // je cherche l'utilisateur connecté et les maisons ainsi que 
            // qui lui sont attribuées 
            User user = _context.Users
                .Include(u => u.Houses)

                .FirstOrDefault(u => u.Id == userId)!;

            // j'ajoute à l'uilisateur la maison
            user.Houses.Add(house);
            _context.SaveChanges();

            //que faut il mettre comme verification
            return true;

        }
    }
}
