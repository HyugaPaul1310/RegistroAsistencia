using RegistroAsistencia.Models;
using Microsoft.Maui.Devices.Sensors;

namespace RegistroAsistencia.Services
{
    public class AsistenciaService
    {
        public List<Establecimiento> ObtenerEstablecimientos()
        {
            return new List<Establecimiento>
       {
           new Establecimiento { Id = 1, Nombre = "Establecimiento 1", Latitud = 32.56203672696053, Longitud = -115.30758388842013 },
           // ... otros establecimientos ...
       };
        }

        public async Task<Establecimiento?> ObtenerEstablecimientoCercanoAsync(double rangoMetros)
        {
            var ubicacion = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();
            if (ubicacion == null) return null;

            foreach (var est in ObtenerEstablecimientos())
            {
                double distancia = Location.CalculateDistance(
                    ubicacion.Latitude, ubicacion.Longitude,
                    est.Latitud, est.Longitud, DistanceUnits.Kilometers) * 1000;

                if (distancia <= rangoMetros)
                    return est;
            }
            return null;
        }
    }
}
