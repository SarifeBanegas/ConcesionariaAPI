using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReparacionRepuestoController : ControllerBase
    {
        private readonly string _connectionString;
        public ReparacionRepuestoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpPost]
        public Response<ReparacionRepuesto> Post(ReparacionRepuesto reparacionRepuesto)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("AsociarRepuestoReparacion", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ReparacionId", reparacionRepuesto.ReparacionId);
                command.Parameters.AddWithValue("@RepuestoId", reparacionRepuesto.RepuestoId);
                command.Parameters.AddWithValue("@Cantidad", reparacionRepuesto.Cantidad);
                command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                connection.Open();
                command.ExecuteNonQuery();

                return new Response<ReparacionRepuesto>
                {
                    Data = reparacionRepuesto,
                    Message = command.Parameters["@Mensaje"].Value?.ToString()
                };
            }
        }

        [HttpDelete("{reparacionId}/{repuestoId}")]
        public Response<string> Delete(int reparacionId, int repuestoId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM ReparacionRepuesto WHERE ReparacionId = @ReparacionId AND RepuestoId = @RepuestoId", connection);
                command.Parameters.AddWithValue("@ReparacionId", reparacionId);
                command.Parameters.AddWithValue("@RepuestoId", repuestoId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return new Response<string>
                    {
                        Data = "Repuesto eliminado de la reparación correctamente.",
                        Message = "Eliminación exitosa."
                    };
                }
                else
                {
                    return new Response<string>
                    {
                        Data = null,
                        Message = "No se encontró la combinación de ReparacionId y RepuestoId en la tabla ReparacionRepuesto."
                    };
                }
            }
        }

        [HttpGet]
        public Response<List<ReparacionRepuesto>> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ReparacionId, RepuestoId, Cantidad FROM ReparacionRepuesto", connection);

                List<ReparacionRepuesto> reparacionesRepuestos = new List<ReparacionRepuesto>();

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ReparacionRepuesto reparacionRepuesto = new ReparacionRepuesto
                    {
                        ReparacionId = Convert.ToInt32(reader["ReparacionId"]),
                        RepuestoId = Convert.ToInt32(reader["RepuestoId"]),
                        Cantidad = Convert.ToInt32(reader["Cantidad"])
                    };

                    reparacionesRepuestos.Add(reparacionRepuesto);
                }

                return new Response<List<ReparacionRepuesto>>
                {
                    Data = reparacionesRepuestos,
                    Message = "Lista de reparaciones y repuestos obtenida correctamente."
                };
            }
        }
    }
}
