namespace RegistroAsistencia.Models
{
    public class AsistenciaRegistro
    {

        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public int EstablecimientoId { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string FotoPath { get; set; }
    }
}

