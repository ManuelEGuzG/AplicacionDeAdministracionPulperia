using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ReportesBL _bl = new ReportesBL();

        public IActionResult ResumenVentas()
        {
            var resumen = _bl.ObtenerResumenVentas();
            return View(resumen);
        }

        public IActionResult ProductosMasVendidos()
        {
            var productos = _bl.ObtenerProductosMasVendidos();
            return View(productos);
        }
    }
}
