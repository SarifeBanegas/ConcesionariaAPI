namespace ConcesionariaAPI
{
    public class Response<T>
    {
        public T Data { get; set; } = default!;
        public string? Message { get; set; }
    }
}
