using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class MecanicoController : ControllerBase
    {
        private readonly string _connectionString;
        public MecanicoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpGet]
        public Response<List<Mecanico>> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Mecanico", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                List<Mecanico> mecanicos = new List<Mecanico>();

                while (reader.Read())
                {
                    Mecanico mecanico = new Mecanico
                    {
                        MecanicoId = Convert.ToInt32(reader["MecanicoId"]),
                        Nombre = reader["Nombre"].ToString(),
                        TallerId = Convert.ToInt32(reader["TallerId"])
                    };

                    mecanicos.Add(mecanico);
                }

                return new Response<List<Mecanico>>
                {
                    Data = mecanicos,
                    Message = "Mecánicos encontrados."
                };
            }
        }

        [HttpPost]
        public Response<Mecanico> Post(Mecanico mecanico)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("RegistrarMecanico", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Nombre", mecanico.Nombre);
                command.Parameters.AddWithValue("@TallerId", mecanico.TallerId);

                SqlParameter mecanicoIdParameter = new SqlParameter("@MecanicoId", SqlDbType.Int);
                mecanicoIdParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(mecanicoIdParameter);

                SqlParameter mensajeParameter = new SqlParameter("@Mensaje", SqlDbType.NVarChar, 100);
                mensajeParameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(mensajeParameter);

                connection.Open();

                command.ExecuteNonQuery();

                mecanico.MecanicoId = Convert.ToInt32(mecanicoIdParameter.Value);

                return new Response<Mecanico>
                {
                    Data = mecanico,
                    Message = mensajeParameter.Value?.ToString()
                };
            }
        }

        [HttpGet("{id}")]
        public Response<Mecanico> Get(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Mecanico WHERE MecanicoId = @MecanicoId", connection);
                command.Parameters.AddWithValue("@MecanicoId", id);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Mecanico mecanico = new Mecanico
                    {
                        MecanicoId = Convert.ToInt32(reader["MecanicoId"]),
                        Nombre = reader["Nombre"].ToString(),
                        TallerId = Convert.ToInt32(reader["TallerId"])
                    };

                    return new Response<Mecanico>
                    {
                        Data = mecanico,
                        Message = "Mecánico encontrado."
                    };
                }
                else
                {
                    return new Response<Mecanico>
                    {
                        Data = null,
                        Message = "Mecánico no encontrado."
                    };
                }
            }
        }

        [HttpPut("{id}")]
        public Response<Mecanico> Put(int id, Mecanico mecanico)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("UPDATE Mecanico SET Nombre = @Nombre, TallerId = @TallerId WHERE MecanicoId = @MecanicoId", connection);
                command.Parameters.AddWithValue("@Nombre", mecanico.Nombre);
                command.Parameters.AddWithValue("@TallerId", mecanico.TallerId);
                command.Parameters.AddWithValue("@MecanicoId", id);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return new Response<Mecanico>
                    {
                        Data = mecanico,
                        Message = "Mecánico actualizado correctamente."
                    };
                }
                else
                {
                    return new Response<Mecanico>
                    {
                        Data = null,
                        Message = "No se pudo actualizar el mecánico."
                    };
                }
            }
        }

        [HttpDelete("{id}")]
        public Response<Mecanico> Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM Mecanico WHERE MecanicoId = @MecanicoId", connection);
                command.Parameters.AddWithValue("@MecanicoId", id);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return new Response<Mecanico>
                    {
                        Data = null,
                        Message = "Mecánico eliminado correctamente."
                    };
                }
                else
                {
                    return new Response<Mecanico>
                    {
                        Data = null,
                        Message = "No se pudo eliminar el mecánico."
                    };
                }
            }
        }
    }
}
