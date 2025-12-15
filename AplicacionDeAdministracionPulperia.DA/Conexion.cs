using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace AplicacionDeAdministracionPulperia.DA
{
    public class Conexion
    {
        private readonly string _cadenaConexion;

        public Conexion()
        {
            _cadenaConexion = "Data Source=TECNHOLOGY;Initial Catalog=PulperiaDB;Integrated Security=True;TrustServerCertificate=True";
        }


        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(_cadenaConexion);
        }
    }
}
