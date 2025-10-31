using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using RegistroAsistencia.Models;
using RegistroAsistencia.Services;
using Microsoft.Maui.ApplicationModel;

namespace RegistroAsistencia.ViewModels
{
    public class AdminPanelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly AsistenciaMySqlService _asistenciaService;

        public AdminPanelViewModel()
        {
            _asistenciaService = new AsistenciaMySqlService(App.ConnectionString);

            FiltrarCommand = new Command(async () => await OnFiltrar());
            ExportarCommand = new Command(OnExportar);
            GestionarEstablecimientosCommand = new Command(OnGestionarEstablecimientos);
            VerFotoCommand = new Command<string>(OnVerFoto);

            ListaEstablecimientos = new List<string>();
            _ = CargarDatosIniciales();
        }

        private async Task CargarDatosIniciales()
        {
            var registros = await _asistenciaService.ObtenerTodosAsync();
            RegistrosAsistencia = registros;

           
            var lista = new List<string>();
            foreach (var reg in registros)
            {
                var id = reg.EstablecimientoId.ToString();
                if (!lista.Contains(id))
                    lista.Add(id);
            }
            ListaEstablecimientos = lista; 

            FechaInicio = DateTime.Today.AddDays(-7);
            FechaFin = DateTime.Today;

            ActualizarEstadisticas(registros);
        }

        private DateTime _fechaInicio;
        public DateTime FechaInicio
        {
            get => _fechaInicio;
            set { _fechaInicio = value; OnPropertyChanged(nameof(FechaInicio)); }
        }

        private DateTime _fechaFin;
        public DateTime FechaFin
        {
            get => _fechaFin;
            set { _fechaFin = value; OnPropertyChanged(nameof(FechaFin)); }
        }

        private List<string> _listaEstablecimientos = new List<string>();
        public List<string> ListaEstablecimientos
        {
            get => _listaEstablecimientos;
            set { _listaEstablecimientos = value; OnPropertyChanged(nameof(ListaEstablecimientos)); }
        }

        private string _establecimientoSeleccionado;
        public string EstablecimientoSeleccionado
        {
            get => _establecimientoSeleccionado;
            set { _establecimientoSeleccionado = value; OnPropertyChanged(nameof(EstablecimientoSeleccionado)); }
        }

        private int _totalAsistencias;
        public int TotalAsistencias
        {
            get => _totalAsistencias;
            set { _totalAsistencias = value; OnPropertyChanged(nameof(TotalAsistencias)); }
        }

        private int _entradasHoy;
        public int EntradasHoy
        {
            get => _entradasHoy;
            set { _entradasHoy = value; OnPropertyChanged(nameof(EntradasHoy)); }
        }

        private int _salidasHoy;
        public int SalidasHoy
        {
            get => _salidasHoy;
            set { _salidasHoy = value; OnPropertyChanged(nameof(SalidasHoy)); }
        }

        private List<AsistenciaRegistro> _registrosAsistencia;
        public List<AsistenciaRegistro> RegistrosAsistencia
        {
            get => _registrosAsistencia;
            set { _registrosAsistencia = value; OnPropertyChanged(nameof(RegistrosAsistencia)); }
        }

        public ICommand FiltrarCommand { get; }
        public ICommand ExportarCommand { get; }
        public ICommand GestionarEstablecimientosCommand { get; }
        public ICommand VerFotoCommand { get; }

        private async Task OnFiltrar()
        {
            var registros = await _asistenciaService.ObtenerTodosAsync();
            var fechaFinInclusiva = FechaFin.Date.AddDays(1);

            var filtrados = registros.FindAll(r =>
                r.Fecha.Date >= FechaInicio.Date &&
                r.Fecha < fechaFinInclusiva &&
                (string.IsNullOrEmpty(EstablecimientoSeleccionado) ||
                 r.EstablecimientoId.ToString() == EstablecimientoSeleccionado)
            );
            RegistrosAsistencia = filtrados;

            // Actualiza las estadísticas con los registros filtrados
            ActualizarEstadisticas(filtrados);

            await Application.Current.MainPage.DisplayAlert("Filtro Aplicado",
                $"Buscando desde: {FechaInicio:d}\nHasta: {FechaFin:d}\nEstablecimiento: {EstablecimientoSeleccionado ?? "Todos"}",
                "OK");
        }

        private void ActualizarEstadisticas(List<AsistenciaRegistro> registros)
        {
            TotalAsistencias = registros.Count;
            EntradasHoy = registros.FindAll(r => r.Fecha.Date == DateTime.Today && r.Tipo == "Entrada").Count;
            SalidasHoy = registros.FindAll(r => r.Fecha.Date == DateTime.Today && r.Tipo == "Salida").Count;
        }

        private void OnExportar()
        {
            Application.Current.MainPage.DisplayAlert("Acción", "Preparando datos para exportación a Excel...", "Entendido");
        }

        private void OnGestionarEstablecimientos()
        {
            Application.Current.MainPage.DisplayAlert("Acción", "Navegando a la sección de gestión.", "Entendido");
        }

        private async void OnVerFoto(string fotoPath)
        {
            if (!string.IsNullOrEmpty(fotoPath))
            {
                try
                {
                    await Launcher.Default.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(fotoPath)
                    });
                }
                catch
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo abrir la foto.", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Sin foto", "No hay foto para este registro.", "OK");
            }
        }
    }
}
