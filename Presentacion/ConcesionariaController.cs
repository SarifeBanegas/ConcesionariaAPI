using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConcesionariaController : ControllerBase
    {
        private readonly string _connectionString;

        public ConcesionariaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpGet]
        public ActionResult<Response<List<Concesionaria>>> Get()
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM Concesionaria", connection);
            List<Concesionaria> concesionarias = new();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Concesionaria concesionaria = new()
                {
                    ConcesionariaId = Convert.ToInt32(reader["ConcesionariaId"]),
                    Nombre = reader["Nombre"].ToString()!,
                    Domicilio = reader["Domicilio"].ToString()!,
                    NIT = reader["NIT"].ToString()!
                };

                concesionarias.Add(concesionaria);
            }

            return new Response<List<Concesionaria>>
            {
                Data = concesionarias,
                Message = "Lista de concesionarias obtenida."
            };
        }

        [HttpGet("{concesionariaId:int}")]
        public ActionResult<Response<Concesionaria>> Get(int concesionariaId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM Concesionaria WHERE ConcesionariaId = @ConcesionariaId", connection);
            command.Parameters.AddWithValue("@ConcesionariaId", concesionariaId);

            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Concesionaria concesionaria = new()
                {
                    ConcesionariaId = Convert.ToInt32(reader["ConcesionariaId"]),
                    Nombre = reader["Nombre"].ToString()!,
                    Domicilio = reader["Domicilio"].ToString()!,
                    NIT = reader["NIT"].ToString()!
                };

                return new Response<Concesionaria>
                {
                    Data = concesionaria,
                    Message = "Concesionaria encontrada."
                };
            }

            return new Response<Concesionaria>
            {
                Message = "Concesionaria no encontrada."
            };
        }

        [HttpPost]
        public ActionResult<Response<Concesionaria>> Post([FromBody] Concesionaria concesionaria)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("RegistrarConcesionaria", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter concesionariaIdParam = new("@ConcesionariaId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(concesionariaIdParam);

            command.Parameters.AddWithValue("@Nombre", concesionaria.Nombre);
            command.Parameters.AddWithValue("@Domicilio", concesionaria.Domicilio);
            command.Parameters.AddWithValue("@NIT", concesionaria.NIT);

            SqlParameter mensajeParam = new("@Mensaje", SqlDbType.VarChar, 100)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(mensajeParam);

            command.ExecuteNonQuery();

            int newConcesionariaId = Convert.ToInt32(command.Parameters["@ConcesionariaId"].Value);

            if (newConcesionariaId > 0)
            {
                concesionaria.ConcesionariaId = newConcesionariaId;

                return new Response<Concesionaria>
                {
                    Data = concesionaria,
                    Message = command.Parameters["@Mensaje"].Value?.ToString()
                };
            }

            return new Response<Concesionaria>
            {
                Message = "No se pudo registrar la concesionaria."
            };
        }

        [HttpPut("{concesionariaId:int}")]
        public ActionResult<Response<Concesionaria>> Put(int concesionariaId, [FromBody] Concesionaria concesionaria)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("UPDATE Concesionaria SET Nombre = @Nombre, Domicilio = @Domicilio, NIT = @NIT WHERE ConcesionariaId = @ConcesionariaId", connection);
            command.Parameters.AddWithValue("@ConcesionariaId", concesionariaId);
            command.Parameters.AddWithValue("@Nombre", concesionaria.Nombre);
            command.Parameters.AddWithValue("@Domicilio", concesionaria.Domicilio);
            command.Parameters.AddWithValue("@NIT", concesionaria.NIT);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<Concesionaria>
                {
                    Data = concesionaria,
                    Message = "Concesionaria actualizada correctamente."
                };
            }

            return new Response<Concesionaria>
            {
                Message = "Concesionaria no encontrada."
            };
        }

        [HttpDelete("{concesionariaId:int}")]
        public ActionResult<Response<Concesionaria>> Delete(int concesionariaId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("DELETE FROM Concesionaria WHERE ConcesionariaId = @ConcesionariaId", connection);
            command.Parameters.AddWithValue("@ConcesionariaId", concesionariaId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<Concesionaria>
                {
                    Message = "Concesionaria eliminada correctamente."
                };
            }

            return new Response<Concesionaria>
            {
                Message = "Concesionaria no encontrada."
            };
        }
    }
}
