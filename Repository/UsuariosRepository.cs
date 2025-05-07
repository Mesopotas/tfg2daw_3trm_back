using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;


namespace CoWorking.Repositories
{
    public class UsuariosRepository : IUsuariosRepository
    {
        private readonly CoworkingDBContext _context;


        public UsuariosRepository(CoworkingDBContext context) // referencia al data.CoworkingDBContext.cs en lugar de cadena de conexión, el EF hará las sentencias sin ponerlas explicitamente
        {
            _context = context;
        }


        public async Task<List<Usuarios>> GetAllAsync()
        {
            return await _context.Usuarios
            //  .Include(u => u.Rol) , con este inner sacaria tb los detalles de roles asociado a ese campo, sin el lo sacará como null de momento
            .ToListAsync(); // el ToListAsync hará una sentencia que devuelva todos los datos de la tabla Usuarios, equivalente a SELECT * FROM Usuarios
        }

        public async Task<Usuarios> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(usuario => usuario.IdUsuario == id); // funcion flecha, usuario recoge todos los usuarios quer cumple que IdUsuario == id
        }
        public async Task AddAsync(Usuarios usuario)
        {

            await _context.Usuarios.AddAsync(usuario); // AddAsync es metodo propio de EF, no hace el insert en si, solo lo prepara
            await _context.SaveChangesAsync(); // otro metodo de EF, esto si hace el insert con los datos del add, ambos son imprescindibles para el insert
        }


        public async Task UpdateAsync(Usuarios usuario)
        {
            _context.Usuarios.Update(usuario); // igual que el add pero haciendo un update
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await GetByIdAsync(id); // primero busca el id del usuario
            if (usuario != null)
            {// si existe, pasa a ejecutar

                _context.Usuarios.Remove(usuario); // metodo de EF para eliminar registros (los prepara para eliminacion)
                await _context.SaveChangesAsync();
            }
        }





        public async Task<List<UsuarioClienteDTO>> GetClientesByEmailAsync(string email)
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol) // Inner joi
                .Where(u => u.Email == email)
                .Select(u => new UsuarioClienteDTO
                {
                    Nombre = u.Nombre,
                    Apellidos = u.Apellidos,
                    Email = u.Email,
                    RolNombre = u.Rol.Nombre
                })
                .ToListAsync();

            return usuarios;
        }
        // se usa para el login en Auth para obtener el token de JWT, no va en el service de usuarios sino en AuthService
        public async Task<UserDTOOut> GetUserFromCredentialsAsync(LoginDto login)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol) // join a tabla Roles para el nombre
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (usuario == null)
            {
                throw new HttpRequestException("Email no encontrado");
            }

            if (usuario.Contrasenia != login.Contrasenia)
            {
                throw new Exception("Contraseña incorrecta");
            }

            if (usuario.Rol == null)
            {
                throw new Exception("Rol no encontrado");
            }

            return new UserDTOOut
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email,
                Rol = usuario.Rol.Nombre
            };
        }
        public async Task<UserDTOOut> AddUserFromCredentialsAsync(RegisterDTO register)
        {
            // revisar si existe ese correo
            var emailExisteYa = await _context.Usuarios.AnyAsync(u => u.Email == register.Email);
            if (emailExisteYa)
            {
                throw new HttpRequestException("Este email ya fue registrado");
            }

            // insert nuevo user
            var nuevoUsuario = new Usuarios
            {
                Nombre = register.Nombre,
                Apellidos = register.Apellidos,
                Email = register.Email,
                Contrasenia = register.Contrasenia,
                IdRol = 2 // este endpoint será el signup, asi q siempre será rol cliente (2)
            };

            // POST + guardado
            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // obtener nombre del rol
            var rolNombre = await _context.Roles
                .Where(r => r.IdRol == nuevoUsuario.IdRol)
                .Select(r => r.Nombre).FirstOrDefaultAsync();

            return new UserDTOOut
            {
                IdUsuario = nuevoUsuario.IdUsuario,
                Nombre = nuevoUsuario.Nombre,
                Apellidos = nuevoUsuario.Apellidos,
                Email = nuevoUsuario.Email,
                Rol = rolNombre
            };
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            // buscar usuario por su ID
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == changePasswordDTO.IdUsuario);

            if (usuario == null)
            {
                throw new HttpRequestException("Usuario no encontrado.");
            }

            // comprobar q coincidan
            if (usuario.Contrasenia != changePasswordDTO.OldPassword)
            {
                throw new HttpRequestException("La contraseña actual es incorrecta.");
            }

            // hacer y guardar el cambio en la bbdd
            usuario.Contrasenia = changePasswordDTO.NewPassword;
            await _context.SaveChangesAsync();

            return true;
        }

        // endpoint para nombrar nuevos admins
        public async Task<bool> ChangeUserRoleAsync(string email)
        {
            // evitar case sensitive
            var emailMinuscula = email.ToLower();

            // Buscar al usuario por email
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == emailMinuscula);

            if (usuario == null)
            {
                throw new HttpRequestException("Usuario no encontrado.");
            }

            if (usuario.IdRol != 2)
            {
                throw new HttpRequestException("El usuario no tiene el rol de cliente (IdRol = 2).");
            }

            // Cambiar el rol a administrador (IdRol = 1)
            usuario.IdRol = 1;
            await _context.SaveChangesAsync();

            return true;
        }


        /*
                        public async Task<bool> QuitarAdminAsync(string email)
                        {
                            using (var connection = new SqlConnection(_connectionString))
                            {
                                await connection.OpenAsync();

                                // Verificar si el usuario existe y tiene el rol de id 2
                                string checkRoleQuery = "SELECT IdRol FROM Usuarios WHERE Email = @Email";
                                using (var command = new SqlCommand(checkRoleQuery, connection))
                                {
                                    command.Parameters.AddWithValue("@Email", email.ToLower());

                                    var rol = await command.ExecuteScalarAsync();
                                    if (rol == null) // si no encuentra id rol es que no existe usuario con ese email registrado
                                    {
                                        throw new HttpRequestException("Usuario no encontrado.");
                                    }

                                    if ((int)rol != 1) // si no es 1, solo puede ser rol 2 osea ya es usuario
                                    {
                                        throw new HttpRequestException("El usuario no tiene el rol de admin (IdRol = 1).");
                                    }
                                }

                                string updateRoleQuery = "UPDATE Usuarios SET IdRol = 2 WHERE Email = @Email"; // id rol de 1 será el de admin
                                using (var command = new SqlCommand(updateRoleQuery, connection))
                                {
                                    command.Parameters.AddWithValue("@Email", email.ToLower());

                                    int rowsAffected = await command.ExecuteNonQueryAsync();
                                    return rowsAffected > 0;
                                }
                            }

                */
    }
}
