using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class MovimientoInventarioController : Controller
    {
        private readonly MovimientoInventarioBL _bl = new MovimientoInventarioBL();

        public IActionResult ByProducto(int idProducto)
        {
            var movimientos = _bl.ListarPorProducto(idProducto);
            return View(movimientos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MovimientoInventario mov)
        {
            if (!ModelState.IsValid) return View(mov);

            try
            {
                _bl.RegistrarMovimiento(mov);
                return RedirectToAction(nameof(ByProducto), new { idProducto = mov.IdProducto });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(mov);
            }
        }
    }
}
