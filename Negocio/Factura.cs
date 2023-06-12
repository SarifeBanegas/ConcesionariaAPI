namespace ConcesionariaAPI.Negocio
{
    public class Factura
    {
        public int ReparacionId { get; set; }
        public int ClienteId { get; set; }
        public decimal Total { get; set; }
        public decimal IVA { get; set; }
        public decimal TotalFinal { get; set; }
    }
}
