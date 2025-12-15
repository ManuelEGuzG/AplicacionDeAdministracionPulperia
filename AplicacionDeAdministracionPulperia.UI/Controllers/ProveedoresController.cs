using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly ProveedorBL _bl = new ProveedorBL();

        public IActionResult Index()
        {
            var proveedores = _bl.Listar();
            return View(proveedores);
        }

        public IActionResult Details(int id)
        {
            try
            {
                var proveedor = _bl.ObtenerPorId(id);
                return View(proveedor);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Detalles()
        {
            var proveedoresDetalle = _bl.ListarConDetalles();
            return View(proveedoresDetalle);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Proveedor proveedor)
        {
            if (!ModelState.IsValid) return View(proveedor);

            try
            {
                _bl.Crear(proveedor);
                TempData["Success"] = "Proveedor creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(proveedor);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var proveedor = _bl.ObtenerPorId(id);
                return View(proveedor);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Proveedor proveedor)
        {
            if (id != proveedor.IdProveedor) return NotFound();
            if (!ModelState.IsValid) return View(proveedor);

            try
            {
                _bl.Actualizar(proveedor);
                TempData["Success"] = "Proveedor actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(proveedor);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                var proveedor = _bl.ObtenerPorId(id);
                return View(proveedor);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _bl.Eliminar(id);
                TempData["Success"] = "Proveedor eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}