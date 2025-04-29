using System.Threading.Tasks;
using System.Security.Claims;
using Models;
using CoWorking.DTO;

namespace CoWorking.Service
{
    public interface IAuthService
    {
        Task<string> Login(LoginDto login);
        Task<string> Register(RegisterDTO register);
        Task<string> GenerateToken(UserDTOOut userDTOOut);
        Task<bool> HasAccessToResource(int requestedUserID, ClaimsPrincipal user);
        Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, ClaimsPrincipal user);

    }
}
