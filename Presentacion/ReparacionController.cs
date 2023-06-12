using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReparacionController : ControllerBase
    {
        private readonly string _connectionString;
        public ReparacionController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpPost]
        public Response<Reparacion> Post(Reparacion reparacion)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("RegistrarReparacion", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@TallerId", reparacion.TallerId);
                command.Parameters.AddWithValue("@Matricula", reparacion.Matricula);
                command.Parameters.AddWithValue("@FechaHoraEntrada", reparacion.FechaHoraEntrada);
                command.Parameters.AddWithValue("@FechaHoraSalida", reparacion.FechaHoraSalida);
                command.Parameters.AddWithValue("@ManoDeObra", reparacion.ManoDeObra);
                command.Parameters.AddWithValue("@ReparacionId", 0).Direction = ParameterDirection.Output;
                command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                connection.Open();

                command.ExecuteNonQuery();

                int reparacionId = (int)command.Parameters["@ReparacionId"].Value;
                string mensaje = command.Parameters["@Mensaje"].Value.ToString();

                reparacion.ReparacionId = reparacionId;

                return new Response<Reparacion>
                {
                    Data = reparacion,
                    Message = mensaje
                };
            }
        }

        [HttpGet]
        public Response<List<Reparacion>> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Reparacion", connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                List<Reparacion> reparaciones = new List<Reparacion>();

                while (reader.Read())
                {
                    Reparacion reparacion = new Reparacion
                    {
                        ReparacionId = (int)reader["ReparacionId"],
                        TallerId = (int)reader["TallerId"],
                        Matricula = reader["Matricula"].ToString(),
                        FechaHoraEntrada = (DateTime)reader["FechaHoraEntrada"],
                        FechaHoraSalida = (DateTime)reader["FechaHoraSalida"],
                        ManoDeObra = (decimal)reader["ManoDeObra"]
                    };

                    reparaciones.Add(reparacion);
                }

                return new Response<List<Reparacion>>
                {
                    Data = reparaciones
                };
            }
        }

        [HttpGet("{reparacionId}")]
        public Response<Reparacion> Get(int reparacionId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Reparacion WHERE ReparacionId = @ReparacionId", connection);
                command.Parameters.AddWithValue("@ReparacionId", reparacionId);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Reparacion reparacion = new Reparacion
                    {
                        ReparacionId = (int)reader["ReparacionId"],
                        TallerId = (int)reader["TallerId"],
                        Matricula = reader["Matricula"].ToString(),
                        FechaHoraEntrada = (DateTime)reader["FechaHoraEntrada"],
                        FechaHoraSalida = (DateTime)reader["FechaHoraSalida"],
                        ManoDeObra = (decimal)reader["ManoDeObra"]
                    };

                    return new Response<Reparacion>
                    {
                        Data = reparacion
                    };
                }

                return new Response<Reparacion>
                {
                    Message = "No se encontró ninguna reparación con el ID especificado."
                };
            }
        }

        [HttpPut("{reparacionId}")]
        public Response<bool> Put(int reparacionId, Reparacion reparacion)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("UPDATE Reparacion SET TallerId = @TallerId, Matricula = @Matricula, " +
                    "FechaHoraEntrada = @FechaHoraEntrada, FechaHoraSalida = @FechaHoraSalida, ManoDeObra = @ManoDeObra " +
                    "WHERE ReparacionId = @ReparacionId", connection);

                command.Parameters.AddWithValue("@TallerId", reparacion.TallerId);
                command.Parameters.AddWithValue("@Matricula", reparacion.Matricula);
                command.Parameters.AddWithValue("@FechaHoraEntrada", reparacion.FechaHoraEntrada);
                command.Parameters.AddWithValue("@FechaHoraSalida", reparacion.FechaHoraSalida);
                command.Parameters.AddWithValue("@ManoDeObra", reparacion.ManoDeObra);
                command.Parameters.AddWithValue("@ReparacionId", reparacionId);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                return new Response<bool>
                {
                    Data = rowsAffected > 0,
                    Message = rowsAffected > 0 ? "Reparación actualizada correctamente." : "No se encontró ninguna reparación con el ID especificado."
                };
            }
        }

        [HttpDelete("{reparacionId}")]
        public Response<bool> Delete(int reparacionId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM Reparacion WHERE ReparacionId = @ReparacionId", connection);
                command.Parameters.AddWithValue("@ReparacionId", reparacionId);

                connection.Open();

                int rowsAffected = command.ExecuteNonQuery();

                return new Response<bool>
                {
                    Data = rowsAffected > 0,
                    Message = rowsAffected > 0 ? "Reparación eliminada correctamente." : "No se encontró ninguna reparación con el ID especificado."
                };
            }
        }
    }
}
