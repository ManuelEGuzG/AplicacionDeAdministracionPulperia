using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoBL _bl = new ProductoBL();

        public IActionResult Index()
        {
            var productos = _bl.Listar();
            return View(productos);
        }

        public IActionResult Details(int id)
        {
            var producto = _bl.ObtenerPorId(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {
            if (!ModelState.IsValid) return View(producto);

            try
            {
                _bl.Crear(producto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(producto);
            }
        }

        public IActionResult Edit(int id)
        {
            var producto = _bl.ObtenerPorId(id);
            if (producto == null) return NotFound();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Producto producto)
        {
            if (id != producto.IdProducto) return NotFound();
            if (!ModelState.IsValid) return View(producto);

            try
            {
                _bl.Actualizar(producto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(producto);
            }
        }

        public IActionResult Delete(int id)
        {
            var producto = _bl.ObtenerPorId(id);
            if (producto == null) return NotFound();
            return View(producto);
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
                var producto = _bl.ObtenerPorId(id);
                return View(producto);
            }
        }
    }
}
