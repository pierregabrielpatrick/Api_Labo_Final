using Domain;
using System.Security.Claims;

namespace Api_Labo_Final.Utils
{
    public static class ExtendedUser
    {

        public static int GetId(this ClaimsPrincipal principal)
        {
            return int.Parse(principal.FindFirst(ClaimTypes.Sid)!.Value);
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Name)!.Value;
        }

        public static UserRole GetRole(this ClaimsPrincipal principal)
        {
            return Enum.Parse<UserRole>(principal.FindFirst(ClaimTypes.Role)!.Value);
        }
    }
}
