namespace ConcesionariaAPI.Negocio
{
    public class Reparacion
    {
        public int ReparacionId { get; set; }
        public int TallerId { get; set; }
        public string Matricula { get; set; }
        public DateTime FechaHoraEntrada { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public decimal ManoDeObra { get; set; }
    }
}
