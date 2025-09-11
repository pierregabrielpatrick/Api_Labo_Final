using System.ComponentModel.DataAnnotations;

namespace Api_Labo_Final.dto
{
    public class UserTokenDTO
    {
        public UserDTO User { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string role { get; set; } = null!;
    }

    public class HouseAddDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public string IPV4 { get; set; }
        public Boolean IsActive { get; set; }
    }
}
