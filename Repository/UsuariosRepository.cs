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


public async Task UpdateAsync(UsuarioUpdateDTO usuario)
{
    var existingUsuario = await _context.Usuarios.FindAsync(usuario.IdUsuario);
    if (existingUsuario == null)
    {
        throw new InvalidOperationException("El usuario no existe.");
    }

    // validar que no exista ya
    var emailEnUso = await _context.Usuarios
        .AnyAsync(u => u.Email == usuario.Email && u.IdUsuario != usuario.IdUsuario);

    if (emailEnUso)
    {
        throw new InvalidOperationException("El email ya está en uso por otro usuario");
    }

    // actualizar los campos del endpoint
    existingUsuario.Nombre = usuario.Nombre;
    existingUsuario.Apellidos = usuario.Apellidos;
    existingUsuario.Email = usuario.Email;

    // guardar 
    _context.Usuarios.Update(existingUsuario);
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

    public async Task<bool> DeleteByEmailAsync(string email)
    {
        // buscar el email
        var userToDelete = await _context.Usuarios
                                     .FirstOrDefaultAsync(u => u.Email == email);

        if (userToDelete == null)
        {
            // no existe ese email
            return false;
        }

        // seleccionar lineas y reservas que tenga ese usuario, para evitar conflictos de FK
        var userReservas = await _context.Reservas
                                        .Where(r => r.IdUsuario == userToDelete.IdUsuario)
                                        .Include(r => r.Lineas) // para poder borrarlas tb
                                        .ToListAsync();

        if (userReservas.Any()) // si hay alguna reserva
        {
            foreach (var reserva in userReservas)
            {
                //  borrar lineas de cada reserva
                if (reserva.Lineas != null && reserva.Lineas.Any())
                {
                    _context.Lineas.RemoveRange(reserva.Lineas);
                }
            }
            // borrar reservas
            _context.Reservas.RemoveRange(userReservas);
        }

        // borrar usuario y guardar
        _context.Usuarios.Remove(userToDelete);
        await _context.SaveChangesAsync();

        return true; // devolver true
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


public async Task<bool> QuitarAdminAsync(string email)
{
    var emailMinuscula = email.ToLower();

    // buscar  por email
    var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == emailMinuscula);

    if (usuario == null)
    {
        throw new HttpRequestException("Usuario no encontrado.");
    }

    //checkear que el rol sea admin (Id = 1)
    if (usuario.IdRol != 1)
    {
        throw new HttpRequestException("El usuario no tiene el rol de admin (IdRol = 1).");
    }

    // cambiar el rol a cliente (Id = 2)
    usuario.IdRol = 2;
    await _context.SaveChangesAsync(); // guardar

    return true;
}

    }
}
