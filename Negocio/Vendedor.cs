namespace ConcesionariaAPI.Negocio
{
    public class Vendedor
    {
        public int VendedorId { get; set; }
        public string Nombre { get; set; }
        public string NIT { get; set; }
        public string Domicilio { get; set; }
        public int ConcesionariaId { get; set; }
    }
}
