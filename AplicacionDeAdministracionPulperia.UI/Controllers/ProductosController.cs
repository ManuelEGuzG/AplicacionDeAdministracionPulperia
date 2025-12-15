using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoBL _bl = new ProductoBL();
        private readonly ProveedorBL _proveedorBL = new ProveedorBL();

        #region Vistas Principales

        /// <summary>
        /// Lista todos los productos
        /// </summary>
        public IActionResult Index()
        {
            try
            {
                var productos = _bl.Listar();
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar productos: {ex.Message}";
                return View(new List<Producto>());
            }
        }

        /// <summary>
        /// Muestra detalles de un producto específico
        /// </summary>
        public IActionResult Details(int id)
        {
            try
            {
                var producto = _bl.ObtenerPorId(id);
                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Lista de productos con detalles extendidos
        /// </summary>
        public IActionResult Detalles()
        {
            try
            {
                var productosConDetalles = _bl.ListarConDetalles();
                return View(productosConDetalles);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar detalles: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Inventario con alertas de stock
        /// </summary>
        public IActionResult Inventario()
        {
            try
            {
                var inventario = _bl.ObtenerInventarioConAlertas();
                return View(inventario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar inventario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Crear Producto

        [HttpGet]
        public IActionResult Create()
        {
            CargarProveedoresEnViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                CargarProveedoresEnViewBag();
                return View(producto);
            }

            try
            {
                _bl.Crear(producto);
                TempData["Success"] = $"Producto '{producto.Nombre}' creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                CargarProveedoresEnViewBag();
                return View(producto);
            }
        }

        #endregion

        #region Editar Producto

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var producto = _bl.ObtenerPorId(id);
                CargarProveedoresEnViewBag();
                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Producto producto)
        {
            if (id != producto.IdProducto)
            {
                TempData["Error"] = "ID de producto no coincide.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                CargarProveedoresEnViewBag();
                return View(producto);
            }

            try
            {
                _bl.Actualizar(producto);
                TempData["Success"] = $"Producto '{producto.Nombre}' actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                CargarProveedoresEnViewBag();
                return View(producto);
            }
        }

        #endregion

        #region Eliminar Producto

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                var producto = _bl.ObtenerPorId(id);
                return View(producto);
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
                var producto = _bl.ObtenerPorId(id);
                _bl.Eliminar(id);
                TempData["Success"] = $"Producto '{producto.Nombre}' eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        #endregion

        #region Gestión de Inventario

        /// <summary>
        /// Vista para ajustar inventario
        /// </summary>
        [HttpGet]
        public IActionResult AjustarInventario(int id)
        {
            try
            {
                var producto = _bl.ObtenerPorId(id);
                ViewBag.Producto = producto;
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AjustarInventario(int id, int cantidad, string tipoMovimiento, string notas)
        {
            try
            {
                _bl.AjustarInventario(id, cantidad, tipoMovimiento, notas);
                TempData["Success"] = "Inventario ajustado exitosamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(AjustarInventario), new { id });
            }
        }

        #endregion

        #region Búsqueda y Filtros

        [HttpGet]
        public IActionResult Buscar(string codigo, string nombre, bool? bajoStock)
        {
            try
            {
                var filtro = new ProductoFiltro
                {
                    Codigo = codigo,
                    Nombre = nombre,
                    BajoStock = bajoStock
                };

                var productos = _bl.ListarConFiltros(filtro);
                ViewBag.Codigo = codigo;
                ViewBag.Nombre = nombre;
                ViewBag.BajoStock = bajoStock;

                return View("Index", productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error en la búsqueda: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Métodos Auxiliares

        private void CargarProveedoresEnViewBag()
        {
            var proveedores = _proveedorBL.Listar();
            ViewBag.Proveedores = new SelectList(proveedores, "IdProveedor", "Nombre");
        }

        #endregion
    }
}