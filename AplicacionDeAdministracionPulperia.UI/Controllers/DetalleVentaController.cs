using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class DetalleVentaController : Controller
    {
        private readonly DetalleVentaBL _bl = new DetalleVentaBL();

        public IActionResult ByVenta(int idVenta)
        {
            var detalles = _bl.ListarPorVenta(idVenta);
            return View(detalles);
        }
    }
}
