using ConcesionariaAPI.Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConcesionariaAPI.Presentacion
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendedorController : ControllerBase
    {
        private readonly string _connectionString;
        public VendedorController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConcesionariaDb")!;
        }

        [HttpGet]
        public Response<List<Vendedor>> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Vendedor", connection))
                {
                    List<Vendedor> vendedores = new List<Vendedor>();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Vendedor vendedor = new Vendedor
                            {
                                VendedorId = Convert.ToInt32(reader["VendedorId"]),
                                Nombre = reader["Nombre"].ToString(),
                                NIT = reader["NIT"].ToString(),
                                Domicilio = reader["Domicilio"].ToString(),
                                ConcesionariaId = Convert.ToInt32(reader["ConcesionariaId"])
                            };

                            vendedores.Add(vendedor);
                        }
                    }

                    return new Response<List<Vendedor>>
                    {
                        Data = vendedores,
                        Message = "Lista de vendedores obtenida."
                    };
                }
            }
        }

        [HttpGet("{vendedorId}")]
        public Response<Vendedor> Get(int vendedorId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT * FROM Vendedor WHERE VendedorId = @VendedorId", connection))
                {
                    command.Parameters.AddWithValue("@VendedorId", vendedorId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Vendedor vendedor = new Vendedor
                            {
                                VendedorId = Convert.ToInt32(reader["VendedorId"]),
                                Nombre = reader["Nombre"].ToString(),
                                NIT = reader["NIT"].ToString(),
                                Domicilio = reader["Domicilio"].ToString(),
                                ConcesionariaId = Convert.ToInt32(reader["ConcesionariaId"])
                            };

                            return new Response<Vendedor>
                            {
                                Data = vendedor,
                                Message = "Vendedor encontrado."
                            };
                        }
                    }
                }

                return new Response<Vendedor>
                {
                    Message = "Vendedor no encontrado."
                };
            }
        }

        [HttpPost]
        public Response<Vendedor> Post([FromBody] Vendedor vendedor)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("RegistrarVendedor", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    SqlParameter vendedorIdParam = new SqlParameter("@VendedorId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(vendedorIdParam);

                    command.Parameters.AddWithValue("@Nombre", vendedor.Nombre);
                    command.Parameters.AddWithValue("@NIT", vendedor.NIT);
                    command.Parameters.AddWithValue("@Domicilio", vendedor.Domicilio);
                    command.Parameters.AddWithValue("@ConcesionariaId", vendedor.ConcesionariaId);

                    SqlParameter mensajeParam = new SqlParameter("@Mensaje", SqlDbType.NVarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(mensajeParam);

                    command.ExecuteNonQuery();

                    object newVendedorIdObj = command.Parameters["@VendedorId"].Value;
                    int newVendedorId = newVendedorIdObj != DBNull.Value ? Convert.ToInt32(newVendedorIdObj) : 0;

                    vendedor.VendedorId = newVendedorId;

                    return new Response<Vendedor>
                    {
                        Data = vendedor,
                        Message = command.Parameters["@Mensaje"].Value.ToString()
                    };
                }
            }
        }

        [HttpPut("{vendedorId}")]
        public Response<Vendedor> Put(int vendedorId, [FromBody] Vendedor vendedor)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("UPDATE Vendedor SET Nombre = @Nombre, NIT = @NIT, Domicilio = @Domicilio, ConcesionariaId = @ConcesionariaId WHERE VendedorId = @VendedorId", connection))
                {
                    command.Parameters.AddWithValue("@VendedorId", vendedorId);
                    command.Parameters.AddWithValue("@Nombre", vendedor.Nombre);
                    command.Parameters.AddWithValue("@NIT", vendedor.NIT);
                    command.Parameters.AddWithValue("@Domicilio", vendedor.Domicilio);
                    command.Parameters.AddWithValue("@ConcesionariaId", vendedor.ConcesionariaId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return new Response<Vendedor>
                        {
                            Data = vendedor,
                            Message = "Vendedor actualizado correctamente."
                        };
                    }

                    return new Response<Vendedor>
                    {
                        Message = "No se pudo actualizar el vendedor."
                    };
                }
            }
        }

        [HttpDelete("{vendedorId}")]
        public Response<Vendedor> Delete(int vendedorId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("DELETE FROM Vendedor WHERE VendedorId = @VendedorId", connection))
                {
                    command.Parameters.AddWithValue("@VendedorId", vendedorId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return new Response<Vendedor>
                        {
                            Message = "Vendedor eliminado correctamente."
                        };
                    }

                    return new Response<Vendedor>
                    {
                        Message = "No se pudo eliminar el vendedor."
                    };
                }
            }
        }
    }
}
