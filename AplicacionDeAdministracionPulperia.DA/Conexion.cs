using Microsoft.Data.SqlClient;

namespace AplicacionDeAdministracionPulperia.DA
{
    /// <summary>
    /// Clase base para la conexión a la base de datos
    /// </summary>
    public class Conexion
    {
        private readonly string _cadenaConexion;

        public Conexion()
        {
            // IMPORTANTE: Actualiza esta cadena de conexión según tu configuración
            // Data Source: nombre de tu servidor SQL Server
            // Initial Catalog: nombre de tu base de datos
            _cadenaConexion = "Data Source=TECNHOLOGY;Initial Catalog=PulperiaDB;Integrated Security=True;TrustServerCertificate=True";
        }

        /// <summary>
        /// Obtiene una nueva conexión a la base de datos
        /// </summary>
        /// <returns>Conexión SQL configurada</returns>
        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(_cadenaConexion);
        }
    }
}