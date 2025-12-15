using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class DetalleVentaBL
    {
        private readonly DetalleVentaDAO _dao = new DetalleVentaDAO();

        public List<DetalleVenta> ListarPorVenta(int idVenta)
        {
            return _dao.ObtenerPorVenta(idVenta);
        }
    }
}
