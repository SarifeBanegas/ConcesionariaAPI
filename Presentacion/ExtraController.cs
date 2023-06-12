using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtraController : ControllerBase
    {
        private readonly string _connectionString;
        public ExtraController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }
        [HttpGet]
        public Response<List<Extra>> Get()
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM Extra", connection);
            List<Extra> extras = new();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Extra extra = new()
                {
                    ExtraId = Convert.ToInt32(reader["ExtraId"]),
                    Descripcion = reader["Descripcion"].ToString()!,
                    Precio = Convert.ToDecimal(reader["Precio"])
                };

                extras.Add(extra);
            }

            return new Response<List<Extra>>
            {
                Data = extras,
                Message = "Lista de extras obtenida."
            };
        }

        [HttpGet("{extraId}")]
        public Response<Extra> Get(int extraId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM Extra WHERE ExtraId = @ExtraId", connection);
            command.Parameters.AddWithValue("@ExtraId", extraId);

            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Extra extra = new()
                {
                    ExtraId = Convert.ToInt32(reader["ExtraId"]),
                    Descripcion = reader["Descripcion"].ToString()!,
                    Precio = Convert.ToDecimal(reader["Precio"])
                };

                return new Response<Extra>
                {
                    Data = extra,
                    Message = "Extra encontrado."
                };
            }

            return new Response<Extra>
            {
                Message = "Extra no encontrado."
            };
        }

        [HttpPost]
        public Response<Extra> Post([FromBody] Extra extra)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("RegistrarExtra", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter extraIdParam = new("@ExtraId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(extraIdParam);

            command.Parameters.AddWithValue("@Descripcion", extra.Descripcion);
            command.Parameters.AddWithValue("@Precio", extra.Precio);

            SqlParameter mensajeParam = new("@Mensaje", SqlDbType.VarChar, 100)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(mensajeParam);

            command.ExecuteNonQuery();

            int newExtraId = Convert.ToInt32(extraIdParam.Value);

            if (newExtraId > 0)
            {
                extra.ExtraId = newExtraId;

                return new Response<Extra>
                {
                    Data = extra,
                    Message = mensajeParam.Value?.ToString()
                };
            }

            return new Response<Extra>
            {
                Message = mensajeParam.Value?.ToString()
            };
        }

        [HttpPut("{extraId}")]
        public Response<Extra> Put(int extraId, [FromBody] Extra extra)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("UPDATE Extra SET Descripcion = @Descripcion, Precio = @Precio WHERE ExtraId = @ExtraId", connection);
            command.Parameters.AddWithValue("@Descripcion", extra.Descripcion);
            command.Parameters.AddWithValue("@Precio", extra.Precio);
            command.Parameters.AddWithValue("@ExtraId", extraId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<Extra>
                {
                    Data = extra,
                    Message = "Extra actualizado correctamente."
                };
            }

            return new Response<Extra>
            {
                Message = "No se pudo actualizar el extra."
            };
        }

        [HttpDelete("{extraId}")]
        public Response<string> Delete(int extraId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("DELETE FROM Extra WHERE ExtraId = @ExtraId", connection);
            command.Parameters.AddWithValue("@ExtraId", extraId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<string>
                {
                    Data = "Extra eliminado correctamente.",
                    Message = "Extra eliminado correctamente."
                };
            }

            return new Response<string>
            {
                Message = "No se pudo eliminar el extra."
            };
        }
    }
}
