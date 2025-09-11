using Api_Labo_Final.dto;
using Domain;

namespace Api_Labo_Final.Mapper
{
    public static class Usermappers
    {

        public static UserDTO ToUserDTO(this User u)
        {
            return new UserDTO()
            {
                Id = u.Id,
                role = u.Role.ToString(),
                Username = u.Username,
            };
        }

        public static User ToUser(this RegisterFormDTO form)
        {
            return new User()
            {
                Username = form.Username,
                Password = form.Password,
            };
        }

        public static User ToUser(this LoginFormDTO form)
        {
            return new User()
            {
                Username = form.Username,
                Password = form.Password,
            };
        }
    }
}
