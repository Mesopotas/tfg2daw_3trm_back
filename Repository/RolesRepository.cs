using Microsoft.Data.SqlClient;
using Models;
using CoWorking.DTO;
using CoWorking.Data;
using Microsoft.EntityFrameworkCore;


namespace CoWorking.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        private readonly CoworkingDBContext _context;

        public RolesRepository(CoworkingDBContext context)
        {
            _context = context;
        }



        public async Task<List<Roles>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync(); // el ToListAsync hará una sentencia que devuelva todos los datos de la tabla roles, equivalente a SELECT * FROM roles
        }

        public async Task<Roles> GetByIdAsync(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(rol => rol.IdRol == id); // funcion flecha, rol recoge todos los roles quer cumple que Idrol == id
        }



        public async Task AddAsync(Roles rol)
        {

            await _context.Roles.AddAsync(rol); // AddAsync es metodo propio de EF, no hace el insert en si, solo lo prepara
            await _context.SaveChangesAsync(); // otro metodo de EF, esto si hace el insert con los datos del add, ambos son imprescindibles para el insert
        }

        public async Task UpdateAsync(Roles rol)
        {
            _context.Roles.Update(rol); // igual que el add pero haciendo un update
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rol = await GetByIdAsync(id); // primero busca el id del rol
            if (rol != null)
            {// si existe, pasa a ejecutar

                _context.Roles.Remove(rol); // metodo de EF para eliminar registros (los prepara para eliminacion)
                await _context.SaveChangesAsync();
            }
        }
    }
}



/*

public async Task<List<Roles>> GetAllAsync()
{
var roles = new List<Roles>();

using (var connection = new SqlConnection(_connectionString))
{
    await connection.OpenAsync();

    string query = "SELECT IdRol, Nombre, Descripcion FROM Roles";
    using (var command = new SqlCommand(query, connection))
    {
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var rol = new Roles
                {
                    IdRol = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.GetString(2)
                };

                roles.Add(rol);
            }
        }
    }
}
return roles;
}

public async Task<Roles> GetByIdAsync(int id)
{
Roles rol = null;

using (var connection = new SqlConnection(_connectionString))
{
    await connection.OpenAsync();

    string query = "SELECT IdRol, Nombre, Descripcion FROM Roles WHERE idRol = @Id";
    using (var command = new SqlCommand(query, connection))
    {
        command.Parameters.AddWithValue("@Id", id);

        using (var reader = await command.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                rol = new Roles
                {
                    IdRol = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.GetString(2)
                };

            }
        }
    }
}
return rol;
}

public async Task AddAsync(Roles rol)
{
using (var connection = new SqlConnection(_connectionString))
{
    await connection.OpenAsync();

    string query = "INSERT INTO Roles (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";

    using (var command = new SqlCommand(query, connection))
    {
        command.Parameters.AddWithValue("@Nombre", rol.Nombre);
        command.Parameters.AddWithValue("@Descripcion", rol.Descripcion);
        await command.ExecuteNonQueryAsync();
    }
}
}

public async Task UpdateAsync(Roles rol)
{
using (var connection = new SqlConnection(_connectionString))
{
    await connection.OpenAsync();

    string query = "UPDATE Roles SET Nombre = @Nombre, Descripcion = @Descripcion WHERE idRol = @IdRol";

    using (var command = new SqlCommand(query, connection))
    {
        command.Parameters.AddWithValue("@IdRol", rol.IdRol);
        command.Parameters.AddWithValue("@Nombre", rol.Nombre);
        command.Parameters.AddWithValue("@Descripcion", rol.Descripcion);
        await command.ExecuteNonQueryAsync();
    }
}
}

public async Task DeleteAsync(int id)
{
using (var connection = new SqlConnection(_connectionString))
{
    await connection.OpenAsync();

    string query = "DELETE FROM Roles WHERE idRol = @IdRol";
    using (var command = new SqlCommand(query, connection))
    {
        command.Parameters.AddWithValue("@IdRol", id);

        await command.ExecuteNonQueryAsync();
    }
}
}
}
}
*/