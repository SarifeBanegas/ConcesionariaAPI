namespace ConcesionariaAPI.Negocio
{
    public class Venta
    {
        public int VentaId { get; set; }
        public int BastidorId { get; set; }
        public int VendedorId { get; set; }
        public DateTime FechaVenta { get; set; }
        public string ModoPago { get; set; }
        public decimal PrecioFinal { get; set; }
    }
}
