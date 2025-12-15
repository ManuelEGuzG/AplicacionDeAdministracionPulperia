using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class DetalleVentaBL
    {
        private readonly DetalleVentaDAO _dao = new DetalleVentaDAO();

        /// <summary>
        /// Lista todos los detalles de una venta específica
        /// </summary>
        /// <param name="idVenta">ID de la venta</param>
        /// <returns>Lista de detalles de venta</returns>
        public List<DetalleVenta> ListarPorVenta(int idVenta)
        {
            return _dao.ObtenerPorVenta(idVenta);
        }
    }
}