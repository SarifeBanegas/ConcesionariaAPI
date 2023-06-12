using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly string _connectionString;
        public ClienteController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpGet]
        public Response<List<Cliente>> Get()
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM Cliente", connection);
            List<Cliente> clientes = new();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Cliente cliente = new()
                {
                    ClienteId = Convert.ToInt32(reader["ClienteId"]),
                    CI = reader["CI"].ToString()!,
                    Nombre = reader["Nombre"].ToString()!,
                    Apellido = reader["Apellido"].ToString()!,
                    Direccion = reader["Direccion"].ToString()!,
                    Telefono = reader["Telefono"].ToString()!
                };

                clientes.Add(cliente);
            }

            return new Response<List<Cliente>>
            {
                Data = clientes,
                Message = "Lista de clientes obtenida."
            };
        }

        [HttpGet("{clienteId:int}")]
        public Response<Cliente> Get(int clienteId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM Cliente WHERE ClienteId = @ClienteId", connection);
            command.Parameters.AddWithValue("@ClienteId", clienteId);

            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Cliente cliente = new()
                {
                    ClienteId = Convert.ToInt32(reader["ClienteId"]),
                    CI = reader["CI"].ToString()!,
                    Nombre = reader["Nombre"].ToString()!,
                    Apellido = reader["Apellido"].ToString()!,
                    Direccion = reader["Direccion"].ToString()!,
                    Telefono = reader["Telefono"].ToString()!
                };

                return new Response<Cliente>
                {
                    Data = cliente,
                    Message = "Cliente encontrado."
                };
            }

            return new Response<Cliente>
            {
                Message = "Cliente no encontrado."
            };
        }

        [HttpPost]
        public Response<Cliente> Post([FromBody] Cliente cliente)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("INSERT INTO Cliente (CI, Nombre, Apellido, Direccion, Telefono) VALUES (@CI, @Nombre, @Apellido, @Direccion, @Telefono); SELECT SCOPE_IDENTITY();", connection);
            command.Parameters.AddWithValue("@CI", cliente.CI);
            command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
            command.Parameters.AddWithValue("@Apellido", cliente.Apellido);
            command.Parameters.AddWithValue("@Direccion", cliente.Direccion);
            command.Parameters.AddWithValue("@Telefono", cliente.Telefono);

            int newClienteId = Convert.ToInt32(command.ExecuteScalar());

            if (newClienteId > 0)
            {
                cliente.ClienteId = newClienteId;

                return new Response<Cliente>
                {
                    Data = cliente,
                    Message = "Cliente registrado correctamente."
                };
            }

            return new Response<Cliente>
            {
                Message = "No se pudo registrar el cliente."
            };
        }

        [HttpPut("{clienteId:int}")]
        public Response<Cliente> Put(int clienteId, [FromBody] Cliente cliente)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("UPDATE Cliente SET CI = @CI, Nombre = @Nombre, Apellido = @Apellido, Direccion = @Direccion, Telefono = @Telefono WHERE ClienteId = @ClienteId", connection);
            command.Parameters.AddWithValue("@ClienteId", clienteId);
            command.Parameters.AddWithValue("@CI", cliente.CI);
            command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
            command.Parameters.AddWithValue("@Apellido", cliente.Apellido);
            command.Parameters.AddWithValue("@Direccion", cliente.Direccion);
            command.Parameters.AddWithValue("@Telefono", cliente.Telefono);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<Cliente>
                {
                    Data = cliente,
                    Message = "Cliente actualizado correctamente."
                };
            }

            return new Response<Cliente>
            {
                Message = "Cliente no encontrado."
            };
        }

        [HttpDelete("{clienteId:int}")]
        public Response<Cliente> Delete(int clienteId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("DELETE FROM Cliente WHERE ClienteId = @ClienteId", connection);
            command.Parameters.AddWithValue("@ClienteId", clienteId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<Cliente>
                {
                    Message = "Cliente eliminado correctamente."
                };
            }

            return new Response<Cliente>
            {
                Message = "Cliente no encontrado."
            };
        }
    }
}
