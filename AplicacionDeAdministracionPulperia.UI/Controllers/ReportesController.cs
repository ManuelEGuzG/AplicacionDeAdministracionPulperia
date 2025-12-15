using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ReportesBL _bl = new ReportesBL();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ResumenVentas()
        {
            var resumen = _bl.ObtenerResumenVentas();
            return View(resumen);
        }

        public IActionResult ProductosMasVendidos(int top = 10)
        {
            var productos = _bl.ObtenerTopProductos(top);
            ViewBag.Top = top;
            return View(productos);
        }

        public IActionResult Rentabilidad(DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (!fechaInicio.HasValue || !fechaFin.HasValue)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
                fechaFin = hoy;
            }

            var analisis = _bl.ObtenerAnalisisRentabilidad(fechaInicio.Value, fechaFin.Value);
            return View(analisis);
        }

        public IActionResult Dashboard()
        {
            var dashboard = _bl.GenerarDashboard();
            return View(dashboard);
        }

        public IActionResult Comparativo()
        {
            var comparativo = _bl.CompararMesActualConAnterior();
            return View(comparativo);
        }
    }
}