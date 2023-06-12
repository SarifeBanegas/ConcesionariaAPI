using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class TallerController : ControllerBase
    {
        private readonly string _connectionString;

        public TallerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpGet]
        public ActionResult<Response<List<Taller>>> Get()
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM Taller", connection);
            List<Taller> talleres = new();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Taller taller = new()
                {
                    TallerId = Convert.ToInt32(reader["TallerId"]),
                    Nombre = reader["Nombre"].ToString()!,
                    Domicilio = reader["Domicilio"].ToString()!,
                    NIT = reader["NIT"].ToString()!,
                    ConcesionariaId = Convert.ToInt32(reader["ConcesionariaId"])
                };

                talleres.Add(taller);
            }

            return new Response<List<Taller>>
            {
                Data = talleres,
                Message = "Lista de talleres obtenida."
            };
        }

        [HttpGet("{tallerId:int}")]
        public ActionResult<Response<Taller>> Get(int tallerId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("SELECT * FROM Taller WHERE TallerId = @TallerId", connection);
            command.Parameters.AddWithValue("@TallerId", tallerId);

            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Taller taller = new()
                {
                    TallerId = Convert.ToInt32(reader["TallerId"]),
                    Nombre = reader["Nombre"].ToString()!,
                    Domicilio = reader["Domicilio"].ToString()!,
                    NIT = reader["NIT"].ToString()!,
                    ConcesionariaId = Convert.ToInt32(reader["ConcesionariaId"])
                };

                return new Response<Taller>
                {
                    Data = taller,
                    Message = "Taller encontrado."
                };
            }

            return new Response<Taller>
            {
                Message = "Taller no encontrado."
            };
        }

        [HttpPost]
        public ActionResult<Response<Taller>> Post([FromBody] Taller taller)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("RegistrarTaller", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter tallerIdParam = new("@TallerId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(tallerIdParam);

            command.Parameters.AddWithValue("@Nombre", taller.Nombre);
            command.Parameters.AddWithValue("@Domicilio", taller.Domicilio);
            command.Parameters.AddWithValue("@NIT", taller.NIT);
            command.Parameters.AddWithValue("@ConcesionariaId", taller.ConcesionariaId);

            SqlParameter mensajeParam = new("@Mensaje", SqlDbType.VarChar, 100)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(mensajeParam);

            command.ExecuteNonQuery();

            object newTallerIdObj = command.Parameters["@TallerId"].Value;
            int newTallerId = newTallerIdObj != DBNull.Value ? Convert.ToInt32(newTallerIdObj) : 0;

            taller.TallerId = newTallerId;

            return new Response<Taller>
            {
                Data = taller,
                Message = command.Parameters["@Mensaje"].Value?.ToString()
            };
        }

        [HttpPut("{tallerId:int}")]
        public ActionResult<Response<Taller>> Put(int tallerId, [FromBody] Taller taller)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("UPDATE Taller SET Nombre = @Nombre, Domicilio = @Domicilio, NIT = @NIT, ConcesionariaId = @ConcesionariaId WHERE TallerId = @TallerId", connection);
            command.Parameters.AddWithValue("@TallerId", tallerId);
            command.Parameters.AddWithValue("@Nombre", taller.Nombre);
            command.Parameters.AddWithValue("@Domicilio", taller.Domicilio);
            command.Parameters.AddWithValue("@NIT", taller.NIT);
            command.Parameters.AddWithValue("@ConcesionariaId", taller.ConcesionariaId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<Taller>
                {
                    Data = taller,
                    Message = "Taller actualizado correctamente."
                };
            }

            return new Response<Taller>
            {
                Message = "Taller no encontrado."
            };
        }

        [HttpDelete("{tallerId:int}")]
        public ActionResult<Response<Taller>> Delete(int tallerId)
        {
            using SqlConnection connection = new(_connectionString);
            connection.Open();

            using SqlCommand command = new("DELETE FROM Taller WHERE TallerId = @TallerId", connection);
            command.Parameters.AddWithValue("@TallerId", tallerId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return new Response<Taller>
                {
                    Message = "Taller eliminado correctamente."
                };
            }

            return new Response<Taller>
            {
                Message = "Taller no encontrado."
            };
        }
    }
}
