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
        /*
                public async Task<UserDTOOut> AddUserFromCredentialsAsync(RegisterDTO register)
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        // Verificar si el email ya existe
                        string checkEmailQuery = "SELECT COUNT(Email) FROM Usuarios WHERE Email = @Email";
                        using (var comprobarEmail = new SqlCommand(checkEmailQuery, connection))
                        {
                            comprobarEmail.Parameters.AddWithValue("@Email", register.Email);

                            var count = await comprobarEmail.ExecuteScalarAsync();
                            if ((int)count > 0) // si el resultado es mayor q 0, significará que ya hay un registro con ese email en la bbdd
                            {
                                throw new HttpRequestException("Este email ya esta asociado a una cuenta");
                            }
                        }

                        string insertUserQuery = "INSERT INTO Usuarios (Nombre, Apellidos, Email, Contrasenia, IdRol) " +
                                                 "VALUES (@Nombre, @Apellidos, @Email, @Contrasenia, 2); SELECT SCOPE_IDENTITY();"; // el id de rol siempre será 2 que es cliente, ya que solo el administrador podrá registrar otros admins

                        using (var command = new SqlCommand(insertUserQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Nombre", register.Nombre);
                            command.Parameters.AddWithValue("@Apellidos", register.Apellidos);
                            command.Parameters.AddWithValue("@Email", register.Email);
                            command.Parameters.AddWithValue("@Contrasenia", register.Contrasenia);

                            var nuevoIdUsuario = await command.ExecuteScalarAsync();

                            if (nuevoIdUsuario == null)
                            {
                                throw new HttpRequestException("Error al crear el usuario");
                            }

                            string RolNombreQuery = "SELECT Nombre FROM Roles WHERE IdRol = IdRol"; // query para obtener el nombre del rol de la tabla roles
                            string rolNombre;

                            using (var rolCommand = new SqlCommand(RolNombreQuery, connection))
                            {
                                rolNombre = (string)await rolCommand.ExecuteScalarAsync(); // se asigna el nombre recibido de la bbdd a la variable
                            }

                            return new UserDTOOut
                            {
                                IdUsuario = Convert.ToInt32(nuevoIdUsuario),
                                Nombre = register.Nombre,
                                Apellidos = register.Apellidos,
                                Email = register.Email,
                                Rol = rolNombre
                            };
                        }
                    }
                }
                public async Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        // contraseña antigua comparativa
                        string comprobarContraseniaActual = "SELECT Contrasenia FROM Usuarios WHERE IdUsuario = @IdUsuario";
                        using (var command = new SqlCommand(comprobarContraseniaActual, connection))
                        {
                            command.Parameters.AddWithValue("@IdUsuario", changePasswordDTO.IdUsuario);
                            var contraseniaActual = (string)await command.ExecuteScalarAsync();

                            if (contraseniaActual == null || contraseniaActual != changePasswordDTO.OldPassword) // si es null o no coincide con la actual, dará error
                            {
                                throw new HttpRequestException("La contraseña actual es incorrecta.");
                            }
                        }

                        // Actualizar la contraseña
                        string updateContrasenia = "UPDATE Usuarios SET Contrasenia = @NewPassword WHERE IdUsuario = @IdUsuario";
                        using (var command = new SqlCommand(updateContrasenia, connection))
                        {
                            command.Parameters.AddWithValue("@NewPassword", changePasswordDTO.NewPassword);
                            command.Parameters.AddWithValue("@IdUsuario", changePasswordDTO.IdUsuario);

                            int rowsAffected = await command.ExecuteNonQueryAsync();
                            return rowsAffected > 0;
                        }
                    }
                }

                public async Task<bool> ChangeUserRoleAsync(string email)
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

                            if ((int)rol != 2) // si no es 2, solo puede ser rol 1 osea ya es admin
                            {
                                throw new HttpRequestException("El usuario no tiene el rol de cliente (IdRol = 2).");
                            }
                        }

                        string updateRoleQuery = "UPDATE Usuarios SET IdRol = 1 WHERE Email = @Email"; // id rol de 1 será el de admin
                        using (var command = new SqlCommand(updateRoleQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Email", email.ToLower());

                            int rowsAffected = await command.ExecuteNonQueryAsync();
                            return rowsAffected > 0;
                        }
                    }
                }


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
