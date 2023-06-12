namespace ConcesionariaAPI.Negocio
{
    public class Automovil
    {
        public int BastidorId { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public decimal Precio { get; set; }
        public decimal Descuento { get; set; }
        public decimal PotenciaFiscal { get; set; }
        public decimal Cilindrada { get; set; }
        public int ConcesionariaId { get; set; }
    }
}
