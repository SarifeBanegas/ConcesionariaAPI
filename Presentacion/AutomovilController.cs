using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutomovilController : ControllerBase
    {
        private readonly string _connectionString;
        public AutomovilController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpGet]
        public Response<List<Automovil>> Get()
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM Automovil", connection);
            List<Automovil> automoviles = new();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Automovil automovil = new()
                {
                    BastidorId = Convert.ToInt32(reader["BastidorId"]),
                    Marca = reader["Marca"].ToString()!,
                    Modelo = reader["Modelo"].ToString()!,
                    Precio = Convert.ToDecimal(reader["Precio"]),
                    Descuento = Convert.ToDecimal(reader["Descuento"]),
                    PotenciaFiscal = Convert.ToDecimal(reader["PotenciaFiscal"]),
                    Cilindrada = Convert.ToDecimal(reader["Cilindrada"]),
                    ConcesionariaId = Convert.ToInt32(reader["ConcesionariaId"])
                };

                automoviles.Add(automovil);
            }

            return new Response<List<Automovil>>
            {
                Data = automoviles,
                Message = "Lista de automóviles obtenida."
            };
        }

        [HttpGet("{bastidorId}")]
        public Response<Automovil> Get(int bastidorId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM Automovil WHERE BastidorId = @BastidorId", connection);
            command.Parameters.AddWithValue("@BastidorId", bastidorId);

            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Automovil automovil = new()
                {
                    BastidorId = Convert.ToInt32(reader["BastidorId"]),
                    Marca = reader["Marca"].ToString()!,
                    Modelo = reader["Modelo"].ToString()!,
                    Precio = Convert.ToDecimal(reader["Precio"]),
                    Descuento = Convert.ToDecimal(reader["Descuento"]),
                    PotenciaFiscal = Convert.ToDecimal(reader["PotenciaFiscal"]),
                    Cilindrada = Convert.ToDecimal(reader["Cilindrada"]),
                    ConcesionariaId = Convert.ToInt32(reader["ConcesionariaId"])
                };

                return new Response<Automovil>
                {
                    Data = automovil,
                    Message = "Automóvil encontrado."
                };
            }

            return new Response<Automovil>
            {
                Message = "Automóvil no encontrado."
            };
        }

        [HttpPost]
        public Response<Automovil> Post([FromBody] Automovil automovil)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("RegistrarAutomovil", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter bastidorIdParam = new("@BastidorId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(bastidorIdParam);

            command.Parameters.AddWithValue("@Marca", automovil.Marca);
            command.Parameters.AddWithValue("@Modelo", automovil.Modelo);
            command.Parameters.AddWithValue("@Precio", automovil.Precio);
            command.Parameters.AddWithValue("@Descuento", automovil.Descuento);
            command.Parameters.AddWithValue("@PotenciaFiscal", automovil.PotenciaFiscal);
            command.Parameters.AddWithValue("@Cilindrada", automovil.Cilindrada);
            command.Parameters.AddWithValue("@ConcesionariaId", automovil.ConcesionariaId);

            SqlParameter mensajeParam = new("@Mensaje", SqlDbType.VarChar, 100)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(mensajeParam);

            command.ExecuteNonQuery();

            int newBastidorId = Convert.ToInt32(bastidorIdParam.Value);

            if (newBastidorId > 0)
            {
                automovil.BastidorId = newBastidorId;

                return new Response<Automovil>
                {
                    Data = automovil,
                    Message = mensajeParam.Value?.ToString()
                };
            }

            return new Response<Automovil>
            {
                Message = mensajeParam.Value?.ToString()
            };
        }

        [HttpPut("{bastidorId}")]
        public Response<Automovil> Put(int bastidorId, [FromBody] Automovil automovil)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("UPDATE Automovil SET Marca = @Marca, Modelo = @Modelo, Precio = @Precio, Descuento = @Descuento, PotenciaFiscal = @PotenciaFiscal, Cilindrada = @Cilindrada, ConcesionariaId = @ConcesionariaId WHERE BastidorId = @BastidorId", connection);
            command.Parameters.AddWithValue("@Marca", automovil.Marca);
            command.Parameters.AddWithValue("@Modelo", automovil.Modelo);
            command.Parameters.AddWithValue("@Precio", automovil.Precio);
            command.Parameters.AddWithValue("@Descuento", automovil.Descuento);
            command.Parameters.AddWithValue("@PotenciaFiscal", automovil.PotenciaFiscal);
            command.Parameters.AddWithValue("@Cilindrada", automovil.Cilindrada);
            command.Parameters.AddWithValue("@ConcesionariaId", automovil.ConcesionariaId);
            command.Parameters.AddWithValue("@BastidorId", bastidorId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<Automovil>
                {
                    Data = automovil,
                    Message = "Automóvil actualizado correctamente."
                };
            }

            return new Response<Automovil>
            {
                Message = "No se pudo actualizar el automóvil."
            };
        }

        [HttpDelete("{bastidorId}")]
        public Response<string> Delete(int bastidorId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("DELETE FROM Automovil WHERE BastidorId = @BastidorId", connection);
            command.Parameters.AddWithValue("@BastidorId", bastidorId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<string>
                {
                    Data = "Automóvil eliminado correctamente.",
                    Message = "Automóvil eliminado correctamente."
                };
            }

            return new Response<string>
            {
                Message = "No se pudo eliminar el automóvil."
            };
        }
    }
}
