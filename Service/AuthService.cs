using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CoWorking.Repositories;
using System.Threading.Tasks;

namespace CoWorking.Service
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuariosRepository _repository;
        private readonly IEmailService _emailService;


        public AuthService(IConfiguration configuration, IUsuariosRepository repository,  IEmailService emailService)
        {
            _configuration = configuration;
            _repository = repository;
            _emailService = emailService;

        }


 public async Task<string> GenerateToken(UserDTOOut userDTOOut)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, userDTOOut.IdUsuario.ToString()),
            new Claim(ClaimTypes.Email, userDTOOut.Email),
            new Claim(ClaimTypes.Name, userDTOOut.Nombre),
            new Claim(ClaimTypes.Surname, userDTOOut.Apellidos),
            new Claim(ClaimTypes.Role, userDTOOut.Rol),
            new Claim("myCustomClaim", "myCustomClaimValue")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<string> Login(LoginDto login)
        {
            var user =  await _repository.GetUserFromCredentialsAsync(login);
            if (user == null) return null; // Error al recibir el usuario / no existe
            return await GenerateToken(user);
        }

        public async Task<string> Register(RegisterDTO register)
        {
            var user = await _repository.AddUserFromCredentialsAsync(register);
            if (user == null) return null; // Error al registrar usuario

            await _emailService.SendWelcomeEmailAsync(user.Email, user.Nombre); // manda un correo de bienvenida

            return await GenerateToken(user);

            }

       

        public Task<bool> HasAccessToResource(int requestedUserID, ClaimsPrincipal user)
        {
            //  obtener ID del usuario autenticado
            var idUsuarioClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idUsuarioClaim is null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                return Task.FromResult(false);
            }

            bool esElUsuario = idUsuario == requestedUserID;

            var rolUsuario = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (rolUsuario is null)
            {
                return Task.FromResult(false);
            }

            bool esAdmin = rolUsuario.Value == "Admin";

            return Task.FromResult(esElUsuario || esAdmin);
        }
            public async Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, ClaimsPrincipal user)
    {
        // Obtener el ID del usuario en base a su JWT
        var idUsuarioClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario)) // si es null o no lo puede parsear al valor de su id (osea el jwt no esta bien formado), retornará false
        {
            return false;
        }

        bool tieneAcceso = await HasAccessToResource(changePasswordDTO.IdUsuario, user); // llamamos a funcion para validar si es el propio usuario o si es un admin, sino no dejará
        if (!tieneAcceso) return false;

        return await _repository.ChangePasswordAsync(changePasswordDTO);
    }

    }
}