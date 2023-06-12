using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private readonly string _connectionString;
        public FacturaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpPost]
        public Response<Factura> Post(Factura factura)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("GenerarFactura", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ReparacionId", factura.ReparacionId);
                command.Parameters.Add("@Total", SqlDbType.Decimal, 18).Direction = ParameterDirection.Output;
                command.Parameters.Add("@IVA", SqlDbType.Decimal, 18).Direction = ParameterDirection.Output;
                command.Parameters.Add("@TotalFinal", SqlDbType.Decimal, 18).Direction = ParameterDirection.Output;
                command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                connection.Open();
                command.ExecuteNonQuery();

                factura.Total = Convert.ToDecimal(command.Parameters["@Total"].Value);
                factura.IVA = Convert.ToDecimal(command.Parameters["@IVA"].Value);
                factura.TotalFinal = Convert.ToDecimal(command.Parameters["@TotalFinal"].Value);

                return new Response<Factura>
                {
                    Data = factura,
                    Message = command.Parameters["@Mensaje"].Value?.ToString()
                };
            }
        }

        [HttpGet]
        public Response<List<Factura>> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ReparacionId, ClienteId, Total, IVA, TotalFinal FROM Factura", connection);

                List<Factura> facturas = new List<Factura>();

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Factura factura = new Factura
                    {
                        ReparacionId = Convert.ToInt32(reader["ReparacionId"]),
                        ClienteId = Convert.ToInt32(reader["ClienteId"]),
                        Total = Convert.ToDecimal(reader["Total"]),
                        IVA = Convert.ToDecimal(reader["IVA"]),
                        TotalFinal = Convert.ToDecimal(reader["TotalFinal"])
                    };

                    facturas.Add(factura);
                }

                return new Response<List<Factura>>
                {
                    Data = facturas,
                    Message = "Lista de facturas obtenida correctamente."
                };
            }
        }

        [HttpDelete("{reparacionId}")]
        public Response<string> Delete(int reparacionId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("DELETE FROM Factura WHERE ReparacionId = @ReparacionId", connection);
                command.Parameters.AddWithValue("@ReparacionId", reparacionId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return new Response<string>
                    {
                        Data = "Factura eliminada correctamente.",
                        Message = "Eliminación exitosa."
                    };
                }
                else
                {
                    return new Response<string>
                    {
                        Data = null,
                        Message = "No se encontró la factura con el FacturaId especificado."
                    };
                }
            }
        }

    }
}
