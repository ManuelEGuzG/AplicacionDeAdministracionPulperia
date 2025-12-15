using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ClienteBL _bl = new ClienteBL();

        public IActionResult Index()
        {
            var clientes = _bl.Listar();
            return View(clientes);
        }

        public IActionResult Details(int id)
        {
            var cliente = _bl.ObtenerPorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente)
        {
            if (!ModelState.IsValid) return View(cliente);

            try
            {
                _bl.Crear(cliente);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(cliente);
            }
        }

        public IActionResult Edit(int id)
        {
            var cliente = _bl.ObtenerPorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Cliente cliente)
        {
            if (id != cliente.IdCliente) return NotFound();
            if (!ModelState.IsValid) return View(cliente);

            try
            {
                _bl.Actualizar(cliente);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(cliente);
            }
        }

        public IActionResult Delete(int id)
        {
            var cliente = _bl.ObtenerPorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
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
                var cliente = _bl.ObtenerPorId(id);
                return View(cliente);
            }
        }
    }
}
