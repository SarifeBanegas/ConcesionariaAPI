using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly string _connectionString;
        public VentaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpGet]
        public ActionResult<Response<List<Venta>>> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Venta";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                List<Venta> ventas = new List<Venta>();

                while (reader.Read())
                {
                    Venta venta = new Venta
                    {
                        VentaId = Convert.ToInt32(reader["VentaId"]),
                        BastidorId = Convert.ToInt32(reader["BastidorId"]),
                        VendedorId = Convert.ToInt32(reader["VendedorId"]),
                        FechaVenta = Convert.ToDateTime(reader["FechaVenta"]),
                        ModoPago = reader["ModoPago"].ToString(),
                        PrecioFinal = Convert.ToDecimal(reader["PrecioFinal"])
                    };

                    ventas.Add(venta);
                }

                return new Response<List<Venta>>
                {
                    Data = ventas,
                    Message = "Ventas obtenidas correctamente."
                };
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Response<Venta>> Get(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Venta WHERE VentaId = @VentaId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@VentaId", id);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Venta venta = new Venta
                    {
                        VentaId = Convert.ToInt32(reader["VentaId"]),
                        BastidorId = Convert.ToInt32(reader["BastidorId"]),
                        VendedorId = Convert.ToInt32(reader["VendedorId"]),
                        FechaVenta = Convert.ToDateTime(reader["FechaVenta"]),
                        ModoPago = reader["ModoPago"].ToString(),
                        PrecioFinal = Convert.ToDecimal(reader["PrecioFinal"])
                    };

                    return new Response<Venta>
                    {
                        Data = venta,
                        Message = "Venta obtenida correctamente."
                    };
                }

                return NotFound(new Response<Venta>
                {
                    Message = "Venta no encontrada."
                });
            }
        }

        [HttpPost]
        public ActionResult<Response<Venta>> Post(Venta venta)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("RegistrarVenta", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@VentaId", 0).Direction = ParameterDirection.Output;
                command.Parameters.AddWithValue("@BastidorId", venta.BastidorId);
                command.Parameters.AddWithValue("@VendedorId", venta.VendedorId);
                command.Parameters.AddWithValue("@FechaVenta", venta.FechaVenta);
                command.Parameters.AddWithValue("@ModoPago", venta.ModoPago);
                command.Parameters.AddWithValue("@PrecioFinal", 0).Direction = ParameterDirection.Output;
                command.Parameters.AddWithValue("@Mensaje", "").Direction = ParameterDirection.Output;

                connection.Open();

                command.ExecuteNonQuery();

                int newVentaId = Convert.ToInt32(command.Parameters["@VentaId"].Value);
                decimal precioFinal = Convert.ToDecimal(command.Parameters["@PrecioFinal"].Value);
                string mensaje = command.Parameters["@Mensaje"].Value.ToString();

                if (newVentaId > 0)
                {
                    venta.VentaId = newVentaId;
                    venta.PrecioFinal = precioFinal;

                    return new Response<Venta>
                    {
                        Data = venta,
                        Message = mensaje
                    };
                }

                return BadRequest(new Response<Venta>
                {
                    Message = mensaje
                });
            }
        }


        [HttpPut("{id}")]
        public ActionResult<Response<Venta>> Put(int id, Venta venta)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Venta SET BastidorId = @BastidorId, VendedorId = @VendedorId, FechaVenta = @FechaVenta, ModoPago = @ModoPago, PrecioFinal = @PrecioFinal WHERE VentaId = @VentaId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@VentaId", id);
                command.Parameters.AddWithValue("@BastidorId", venta.BastidorId);
                command.Parameters.AddWithValue("@VendedorId", venta.VendedorId);
                command.Parameters.AddWithValue("@FechaVenta", venta.FechaVenta);
                command.Parameters.AddWithValue("@ModoPago", venta.ModoPago);
                command.Parameters.AddWithValue("@PrecioFinal", venta.PrecioFinal);

                connection.Open();

                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows > 0)
                {
                    return new Response<Venta>
                    {
                        Data = venta,
                        Message = "Venta actualizada correctamente."
                    };
                }

                return NotFound(new Response<Venta>
                {
                    Message = "Venta no encontrada."
                });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<Response<Venta>> Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Venta WHERE VentaId = @VentaId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@VentaId", id);

                connection.Open();

                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows > 0)
                {
                    return new Response<Venta>
                    {
                        Message = "Venta eliminada correctamente."
                    };
                }

                return NotFound(new Response<Venta>
                {
                    Message = "Venta no encontrada."
                });
            }
        }
    }
}
