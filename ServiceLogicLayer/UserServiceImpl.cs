using Dal.context;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer
{
    public class UserServiceImpl : UserService
    {
        private readonly FinalContext _context
public UserServiceImpl(FinalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Houses)
                .Select(u => new User
                {
                    Id = u.Id,
                    role = u.Role,
                    Username = u.Username,
                })
                .ToListAsync();

            return users;
        }
    }
}
