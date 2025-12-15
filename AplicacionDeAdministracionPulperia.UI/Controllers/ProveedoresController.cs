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
            var proveedor = _bl.ObtenerPorId(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Proveedor proveedor)
        {
            if (!ModelState.IsValid) return View(proveedor);

            try
            {
                _bl.Crear(proveedor);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(proveedor);
            }
        }

        public IActionResult Edit(int id)
        {
            var proveedor = _bl.ObtenerPorId(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
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
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(proveedor);
            }
        }

        public IActionResult Delete(int id)
        {
            var proveedor = _bl.ObtenerPorId(id);
            if (proveedor == null) return NotFound();
            return View(proveedor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _bl.Eliminar(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var proveedor = _bl.ObtenerPorId(id);
                return View(proveedor);
            }
        }
    }
}
