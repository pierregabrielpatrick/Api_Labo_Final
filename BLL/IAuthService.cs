using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IAuthService
    {
        public bool CheckExistence(string formName);
        public User? CheckValidity(string userNameRequest);

        public int Insert(User user);

        public int Update(User user);

    }
}
