using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IHouseService
    {

        public List<House> GetAllHouse(int Id);
        bool AddHouse(House dto, int userId);
    }
}
