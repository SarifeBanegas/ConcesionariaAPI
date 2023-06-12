using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutomovilExtraController : ControllerBase
    {
        private readonly string _connectionString;
        public AutomovilExtraController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpGet]
        public Response<List<AutomovilExtra>> Get()
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM AutomovilExtra", connection);
            List<AutomovilExtra> automovilExtras = new();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                AutomovilExtra automovilExtra = new()
                {
                    BastidorId = Convert.ToInt32(reader["BastidorId"]),
                    ExtraId = Convert.ToInt32(reader["ExtraId"])
                };

                automovilExtras.Add(automovilExtra);
            }

            return new Response<List<AutomovilExtra>>
            {
                Data = automovilExtras,
                Message = "Lista de AutomovilExtra obtenida."
            };
        }

        [HttpPost]
        public Response<AutomovilExtra> Post([FromBody] AutomovilExtra automovilExtra)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("AsociarExtraAutomovil", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@BastidorId", automovilExtra.BastidorId);
            command.Parameters.AddWithValue("@ExtraId", automovilExtra.ExtraId);

            SqlParameter mensajeParam = new("@Mensaje", SqlDbType.VarChar, 100)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(mensajeParam);

            command.ExecuteNonQuery();

            return new Response<AutomovilExtra>
            {
                Data = automovilExtra,
                Message = mensajeParam.Value?.ToString()
            };
        }

        [HttpDelete("{bastidorId}/{extraId}")]
        public Response<string> Delete(int bastidorId, int extraId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("DELETE FROM AutomovilExtra WHERE BastidorId = @BastidorId AND ExtraId = @ExtraId", connection);
            command.Parameters.AddWithValue("@BastidorId", bastidorId);
            command.Parameters.AddWithValue("@ExtraId", extraId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<string>
                {
                    Message = "Extra desasociado del automóvil correctamente."
                };
            }

            return new Response<string>
            {
                Message = "No se pudo desasociar el extra del automóvil."
            };
        }
    }
}
