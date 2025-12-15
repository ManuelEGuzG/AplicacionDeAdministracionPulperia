using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class VentasController : Controller
    {
        private readonly VentaBL _bl = new VentaBL();

        public IActionResult Index()
        {
            var ventas = _bl.Listar();
            return View(ventas);
        }

        public IActionResult Details(int id)
        {
            var venta = _bl.ObtenerPorId(id);
            if (venta == null) return NotFound();
            return View(venta);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Venta venta, List<DetalleVenta> detalles)
        {
            if (!ModelState.IsValid) return View(venta);

            try
            {
                _bl.Crear(venta, detalles);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(venta);
            }
        }
    }
}
