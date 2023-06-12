using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReparacionMecanicoController : ControllerBase
    {
        private readonly string _connectionString;
        public ReparacionMecanicoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpPost]
        public Response<ReparacionMecanico> Post(ReparacionMecanico reparacionMecanico)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("AsociarMecanicoReparacion", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ReparacionId", reparacionMecanico.ReparacionId);
                command.Parameters.AddWithValue("@MecanicoId", reparacionMecanico.MecanicoId);
                command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                connection.Open();

                command.ExecuteNonQuery();

                return new Response<ReparacionMecanico>
                {
                    Data = reparacionMecanico,
                    Message = command.Parameters["@Mensaje"].Value?.ToString()
                };
            }
        }

        [HttpDelete]
        public Response<ReparacionMecanico> Delete(int reparacionId, int mecanicoId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM ReparacionMecanico WHERE ReparacionId = @ReparacionId AND MecanicoId = @MecanicoId", connection);
                command.Parameters.AddWithValue("@ReparacionId", reparacionId);
                command.Parameters.AddWithValue("@MecanicoId", mecanicoId);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return new Response<ReparacionMecanico>
                    {
                        Data = new ReparacionMecanico { ReparacionId = reparacionId, MecanicoId = mecanicoId },
                        Message = "Mecánico desasociado de la reparación correctamente."
                    };
                }
                else
                {
                    return new Response<ReparacionMecanico>
                    {
                        Data = null,
                        Message = "No se encontró ninguna asociación entre el mecánico y la reparación."
                    };
                }
            }
        }

        [HttpGet]
        public Response<List<ReparacionMecanico>> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ReparacionId, MecanicoId FROM ReparacionMecanico", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                List<ReparacionMecanico> reparacionMecanicos = new List<ReparacionMecanico>();

                while (reader.Read())
                {
                    ReparacionMecanico reparacionMecanico = new ReparacionMecanico
                    {
                        ReparacionId = Convert.ToInt32(reader["ReparacionId"]),
                        MecanicoId = Convert.ToInt32(reader["MecanicoId"])
                    };

                    reparacionMecanicos.Add(reparacionMecanico);
                }
                return new Response<List<ReparacionMecanico>>
                {
                    Data = reparacionMecanicos,
                    Message = "Registros de ReparacionMecanico encontrados."
                };
            }
        }
    }
}
