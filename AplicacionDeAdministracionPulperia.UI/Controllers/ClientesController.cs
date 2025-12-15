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
            try
            {
                var clientes = _bl.Listar();
                return View(clientes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar clientes: {ex.Message}";
                return View(new List<Cliente>());
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var cliente = _bl.ObtenerPorId(id);
                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Detalles()
        {
            try
            {
                var clientesConDetalles = _bl.ListarConDetalles();
                return View(clientesConDetalles);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
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
                TempData["Success"] = $"Cliente '{cliente.Nombre}' creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(cliente);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var cliente = _bl.ObtenerPorId(id);
                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
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
                TempData["Success"] = $"Cliente '{cliente.Nombre}' actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(cliente);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                var cliente = _bl.ObtenerPorId(id);
                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var cliente = _bl.ObtenerPorId(id);
                _bl.Eliminar(id);
                TempData["Success"] = $"Cliente '{cliente.Nombre}' eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        public IActionResult Buscar(string nombre, string telefono)
        {
            try
            {
                var filtro = new ClienteFiltro
                {
                    Nombre = nombre,
                    Telefono = telefono
                };

                var clientes = _bl.ListarConFiltros(filtro);
                ViewBag.Nombre = nombre;
                ViewBag.Telefono = telefono;

                return View("Index", clientes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error en búsqueda: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}