using Newtonsoft.Json;

namespace ConcesionariaAPI
{
    public class ErrorHandler : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                // Invoca el siguiente middleware en el pipeline
                await next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                // Crear el objeto Response<string> con el mensaje de error
                var response = new Response<string>
                {
                    Data = ex.Message,
                    Message = "Ocurrió un error"
                };

                // Serializar el objeto Response<string> como JSON y enviarlo en la respuesta
                var responseJson = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(responseJson);
            }
        }
    }

}
