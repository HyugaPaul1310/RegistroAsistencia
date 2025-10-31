using MySqlConnector;
using RegistroAsistencia.Models;

namespace RegistroAsistencia.Services
{
    public class AsistenciaMySqlService
    {
        private readonly string _connectionString;

        public AsistenciaMySqlService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task GuardarRegistroAsync(AsistenciaRegistro registro, byte[] fotoBytes)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO asistencia_registro 
            (fecha, tipo, establecimiento_id, latitud, longitud, foto, foto_path)
            VALUES (@fecha, @tipo, @establecimiento_id, @latitud, @longitud, @foto, @foto_path)";
            cmd.Parameters.AddWithValue("@fecha", registro.Fecha);
            cmd.Parameters.AddWithValue("@tipo", registro.Tipo);
            cmd.Parameters.AddWithValue("@establecimiento_id", registro.EstablecimientoId); 
            cmd.Parameters.AddWithValue("@latitud", registro.Latitud);
            cmd.Parameters.AddWithValue("@longitud", registro.Longitud);
            cmd.Parameters.AddWithValue("@foto", fotoBytes);
            cmd.Parameters.AddWithValue("@foto_path", registro.FotoPath);



            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<AsistenciaRegistro>> ObtenerTodosAsync()
        {
            var registros = new List<AsistenciaRegistro>();
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT fecha, tipo, establecimiento_id, latitud, longitud, foto_path FROM asistencia_registro";
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                registros.Add(new AsistenciaRegistro
                {
                    Fecha = reader.GetDateTime(0),
                    Tipo = reader.GetString(1),
                    EstablecimientoId = reader.GetInt32(2),
                    Latitud = reader.GetDouble(3),
                    Longitud = reader.GetDouble(4),
                    FotoPath = reader.GetString(5)
                });
            }
            return registros;
        }

    }
}
