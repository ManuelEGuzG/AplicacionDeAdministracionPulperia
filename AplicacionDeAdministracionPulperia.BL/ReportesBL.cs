using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class ReportesBL
    {
        private readonly ReportesDAO _dao = new ReportesDAO();

        public List<ResumenVentas> ObtenerResumenVentas()
        {
            return _dao.ObtenerResumenDiario();
        }

        public List<ProductoMasVendido> ObtenerProductosMasVendidos()
        {
            return _dao.ObtenerProductosMasVendidos();
        }
    }
}
