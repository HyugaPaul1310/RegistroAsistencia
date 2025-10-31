using RegistroAsistencia.Models;
using RegistroAsistencia.Services;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using Microsoft.Maui.Devices.Sensors;
using System.IO;

namespace RegistroAsistencia.ViewModels
{
    public class RegistroAsistenciaViewModel : BaseViewModel
    {
        private readonly AsistenciaService _service = new AsistenciaService();

        // Servicio para MySQL 
        private readonly AsistenciaMySqlService _dbService = new AsistenciaMySqlService(
            "Server=localhost;Port=3306;Database=registrodeasistencias;User=root;Password=;");

        public ICommand RegistrarEntradaCommand { get; }
        public ICommand RegistrarSalidaCommand { get; }

        public RegistroAsistenciaViewModel()
        {
            RegistrarEntradaCommand = new Command(async () => await RegistrarAsistencia(true));
            RegistrarSalidaCommand = new Command(async () => await RegistrarAsistencia(false));
        }

        private async Task RegistrarAsistencia(bool esEntrada)
        {
            // Solicitar permisos de ubicación
            var locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (locationStatus != PermissionStatus.Granted)
            {
                await Application.Current.MainPage.DisplayAlert("Permiso requerido", "Debes permitir el acceso a la ubicación.", "OK");
                return;
            }

            // Solicitar permisos de cámara
            var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
            if (cameraStatus != PermissionStatus.Granted)
            {
                await Application.Current.MainPage.DisplayAlert("Permiso requerido", "Debes permitir el acceso a la cámara.", "OK");
                return;
            }

            // Verificar si está en rango
            var establecimiento = await _service.ObtenerEstablecimientoCercanoAsync(1000);
            if (establecimiento == null)
            {
                await Application.Current.MainPage.DisplayAlert("Fuera de zona", "No estás dentro del rango de ningún establecimiento.", "OK");
                return;
            }

            // Tomar foto
            var foto = await MediaPicker.CapturePhotoAsync();
            if (foto == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo tomar la foto.", "OK");
                return;
            }

            // Crear el registro
            var registro = new AsistenciaRegistro
            {
                Fecha = DateTime.Now,
                Tipo = esEntrada ? "Entrada" : "Salida",
                EstablecimientoId = establecimiento.Id, 
                Latitud = establecimiento.Latitud,
                Longitud = establecimiento.Longitud,
                FotoPath = foto.FullPath
            };

            Console.WriteLine($"EstablecimientoId a guardar: {registro.EstablecimientoId}");


            // Convertir la foto a bytes
            using var fotoStream = await foto.OpenReadAsync();
            using var ms = new MemoryStream();
            await fotoStream.CopyToAsync(ms);
            var fotoBytes = ms.ToArray();

            // Guardar en la base de datos MySQL
            await _dbService.GuardarRegistroAsync(registro, fotoBytes);

            await Application.Current.MainPage.DisplayAlert("Registro exitoso", $"Asistencia registrada en {registro.EstablecimientoId} como {registro.Tipo}.", "OK");
        }
    }
}
