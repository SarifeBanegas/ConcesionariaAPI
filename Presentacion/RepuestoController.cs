using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepuestoController : ControllerBase
    {
        private readonly string _connectionString;
        public RepuestoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpPost]
        public Response<Repuesto> Post(Repuesto repuesto)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("RegistrarRepuesto", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Descripcion", repuesto.Descripcion);
                command.Parameters.AddWithValue("@Precio", repuesto.Precio);
                command.Parameters.AddWithValue("@RepuestoId", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                connection.Open();

                command.ExecuteNonQuery();

                repuesto.RepuestoId = Convert.ToInt32(command.Parameters["@RepuestoId"].Value);
                string mensaje = command.Parameters["@Mensaje"].Value.ToString();

                return new Response<Repuesto>
                {
                    Data = repuesto,
                    Message = mensaje
                };
            }
        }

        [HttpGet("{id}")]
        public Response<Repuesto> Get(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT RepuestoId, Descripcion, Precio FROM Repuesto WHERE RepuestoId = @RepuestoId", connection);
                command.Parameters.AddWithValue("@RepuestoId", id);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Repuesto repuesto = new Repuesto
                    {
                        RepuestoId = Convert.ToInt32(reader["RepuestoId"]),
                        Descripcion = reader["Descripcion"].ToString(),
                        Precio = Convert.ToDecimal(reader["Precio"])
                    };

                    return new Response<Repuesto>
                    {
                        Data = repuesto,
                        Message = "Repuesto encontrado."
                    };
                }
                else
                {
                    return new Response<Repuesto>
                    {
                        Data = null,
                        Message = "No se encontró ningún repuesto con el ID especificado."
                    };
                }
            }
        }

        [HttpGet]
        public Response<List<Repuesto>> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT RepuestoId, Descripcion, Precio FROM Repuesto", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                List<Repuesto> repuestos = new List<Repuesto>();

                while (reader.Read())
                {
                    Repuesto repuesto = new Repuesto
                    {
                        RepuestoId = Convert.ToInt32(reader["RepuestoId"]),
                        Descripcion = reader["Descripcion"].ToString(),
                        Precio = Convert.ToDecimal(reader["Precio"])
                    };

                    repuestos.Add(repuesto);
                }

                if (repuestos.Count > 0)
                {
                    return new Response<List<Repuesto>>
                    {
                        Data = repuestos,
                        Message = "Registros de repuestos encontrados."
                    };
                }
                else
                {
                    return new Response<List<Repuesto>>
                    {
                        Data = null,
                        Message = "No se encontraron registros de repuestos."
                    };
                }
            }
        }
        [HttpPut("{id}")]
        public Response<Repuesto> Put(int id, Repuesto repuesto)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("UPDATE Repuesto SET Descripcion = @Descripcion, Precio = @Precio WHERE RepuestoId = @RepuestoId", connection);
                command.Parameters.AddWithValue("@Descripcion", repuesto.Descripcion);
                command.Parameters.AddWithValue("@Precio", repuesto.Precio);
                command.Parameters.AddWithValue("@RepuestoId", id);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return new Response<Repuesto>
                    {
                        Data = repuesto,
                        Message = "Repuesto actualizado correctamente."
                    };
                }
                else
                {
                    return new Response<Repuesto>
                    {
                        Data = null,
                        Message = "No se encontró ningún repuesto con el ID especificado."
                    };
                }
            }
        }

        [HttpDelete("{id}")]
        public Response<Repuesto> Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM Repuesto WHERE RepuestoId = @RepuestoId", connection);
                command.Parameters.AddWithValue("@RepuestoId", id);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return new Response<Repuesto>
                    {
                        Data = null,
                        Message = "Repuesto eliminado correctamente."
                    };
                }
                else
                {
                    return new Response<Repuesto>
                    {
                        Data = null,
                        Message = "No se encontró ningún repuesto con el ID especificado."
                    };
                }
            }
        }
    }
}
