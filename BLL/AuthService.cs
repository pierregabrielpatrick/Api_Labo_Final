using Dal.context;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AuthService: IAuthService
    {
        private readonly FinalContext _context;


        public AuthService(FinalContext context)
        {
            _context = context;
        }

        public bool CheckExistence(string formName)
        {
            return _context.Users.Any(u => u.Username == formName);
        }      

        public User? CheckValidity(string userNameRequest)
        {
            return _context.Users.FirstOrDefault(u => u.Username == userNameRequest);
        }

        public int Insert(User user)
        {
            _context.Users.Add(user);

            return _context.SaveChanges();
        }

        public int Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
