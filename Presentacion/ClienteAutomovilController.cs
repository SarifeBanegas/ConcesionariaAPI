using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteAutomovilController : ControllerBase
    {
        private readonly string _connectionString;
        public ClienteAutomovilController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpGet]
        public Response<List<ClienteAutomovil>> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM ClienteAutomovil", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                List<ClienteAutomovil> clienteAutomoviles = new List<ClienteAutomovil>();

                while (reader.Read())
                {
                    ClienteAutomovil clienteAutomovil = new ClienteAutomovil
                    {
                        Matricula = reader["Matricula"].ToString(),
                        ClienteId = Convert.ToInt32(reader["ClienteId"]),
                        BastidorId = Convert.ToInt32(reader["BastidorId"])
                    };

                    clienteAutomoviles.Add(clienteAutomovil);
                }

                return new Response<List<ClienteAutomovil>>
                {
                    Data = clienteAutomoviles,
                    Message = "Lista de asociaciones cliente-automóvil obtenida correctamente."
                };
            }
        }

        [HttpPost]
        public Response<ClienteAutomovil> Post(ClienteAutomovil clienteAutomovil)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("RegistrarClienteAutomovil", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ClienteId", clienteAutomovil.ClienteId);
                command.Parameters.AddWithValue("@BastidorId", clienteAutomovil.BastidorId);
                command.Parameters.AddWithValue("@Matricula", clienteAutomovil.Matricula);
                command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                connection.Open();

                command.ExecuteNonQuery();

                return new Response<ClienteAutomovil>
                {
                    Data = clienteAutomovil,
                    Message = command.Parameters["@Mensaje"].Value?.ToString()
                };
            }
        }

        [HttpDelete("{clienteId}/{bastidorId}")]
        public Response<ClienteAutomovil> Delete(int clienteId, int bastidorId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM ClienteAutomovil WHERE ClienteId = @ClienteId AND BastidorId = @BastidorId", connection);

                command.Parameters.AddWithValue("@ClienteId", clienteId);
                command.Parameters.AddWithValue("@BastidorId", bastidorId);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return new Response<ClienteAutomovil>
                    {
                        Message = "Cliente y automóvil desasociados correctamente."
                    };
                }

                return new Response<ClienteAutomovil>
                {
                    Message = "No se encontró la asociación entre el cliente y el automóvil."
                };
            }
        }
    }
}
