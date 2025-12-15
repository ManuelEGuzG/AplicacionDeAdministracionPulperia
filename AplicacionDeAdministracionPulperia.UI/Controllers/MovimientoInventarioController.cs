using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class MovimientoInventarioController : Controller
    {
        private readonly MovimientoInventarioBL _bl = new MovimientoInventarioBL();
        private readonly ProductoBL _productoBL = new ProductoBL();

        #region Vistas Principales

        /// <summary>
        /// Lista todos los movimientos de un producto específico
        /// </summary>
        public IActionResult Index(int? idProducto)
        {
            try
            {
                if (!idProducto.HasValue)
                {
                    TempData["Info"] = "Debe especificar un producto para ver sus movimientos.";
                    return RedirectToAction("Index", "Productos");
                }

                var movimientos = _bl.ListarPorProducto(idProducto.Value);
                var producto = _productoBL.ObtenerPorId(idProducto.Value);

                ViewBag.Producto = producto;
                ViewBag.IdProducto = idProducto.Value;

                return View(movimientos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar movimientos: {ex.Message}";
                return RedirectToAction("Index", "Productos");
            }
        }

        /// <summary>
        /// Muestra todos los movimientos del sistema
        /// </summary>
        public IActionResult Todos()
        {
            try
            {
                // Obtener todos los productos y sus movimientos
                var productos = _productoBL.Listar();
                var todosLosMovimientos = new List<MovimientoInventario>();

                foreach (var producto in productos)
                {
                    var movimientos = _bl.ListarPorProducto(producto.IdProducto);
                    todosLosMovimientos.AddRange(movimientos);
                }

                // Ordenar por fecha descendente
                todosLosMovimientos = todosLosMovimientos
                    .OrderByDescending(m => m.Fecha)
                    .ToList();

                return View(todosLosMovimientos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(new List<MovimientoInventario>());
            }
        }

        /// <summary>
        /// Muestra detalles de un movimiento específico
        /// </summary>
        public IActionResult Details(int id, int idProducto)
        {
            try
            {
                var movimientos = _bl.ListarPorProducto(idProducto);
                var movimiento = movimientos.FirstOrDefault(m => m.IdMovimiento == id);

                if (movimiento == null)
                {
                    TempData["Error"] = "Movimiento no encontrado.";
                    return RedirectToAction("Index", new { idProducto });
                }

                var producto = _productoBL.ObtenerPorId(idProducto);
                ViewBag.Producto = producto;

                return View(movimiento);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Productos");
            }
        }

        #endregion

        #region Crear Movimiento

        [HttpGet]
        public IActionResult Create(int? idProducto)
        {
            try
            {
                if (idProducto.HasValue)
                {
                    var producto = _productoBL.ObtenerPorId(idProducto.Value);
                    ViewBag.Producto = producto;
                    ViewBag.IdProducto = idProducto.Value;
                }

                CargarProductosEnViewBag();
                CargarTiposMovimiento();

                return View(new MovimientoInventario
                {
                    Fecha = DateTime.Now,
                    IdProducto = idProducto ?? 0
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Productos");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MovimientoInventario movimiento)
        {
            if (!ModelState.IsValid)
            {
                CargarProductosEnViewBag();
                CargarTiposMovimiento();
                return View(movimiento);
            }

            try
            {
                _bl.RegistrarMovimiento(movimiento);
                TempData["Success"] = "Movimiento registrado exitosamente.";
                return RedirectToAction("Index", new { idProducto = movimiento.IdProducto });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                CargarProductosEnViewBag();
                CargarTiposMovimiento();
                return View(movimiento);
            }
        }

        #endregion

        #region Ajuste Rápido

        /// <summary>
        /// Vista para ajuste rápido de inventario
        /// </summary>
        [HttpGet]
        public IActionResult AjusteRapido(int idProducto)
        {
            try
            {
                var producto = _productoBL.ObtenerPorId(idProducto);
                ViewBag.Producto = producto;
                CargarTiposMovimiento();

                return View(new MovimientoInventario
                {
                    IdProducto = idProducto,
                    Fecha = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Productos");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AjusteRapido(MovimientoInventario movimiento)
        {
            try
            {
                _bl.RegistrarMovimiento(movimiento);
                TempData["Success"] = "Ajuste realizado exitosamente.";
                return RedirectToAction("Details", "Productos", new { id = movimiento.IdProducto });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("AjusteRapido", new { idProducto = movimiento.IdProducto });
            }
        }

        #endregion

        #region Búsqueda y Filtros

        [HttpGet]
        public IActionResult Buscar(int? idProducto, string tipoMovimiento, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                List<MovimientoInventario> movimientos = new List<MovimientoInventario>();

                if (idProducto.HasValue)
                {
                    movimientos = _bl.ListarPorProducto(idProducto.Value);
                }
                else
                {
                    // Obtener todos los movimientos
                    var productos = _productoBL.Listar();
                    foreach (var producto in productos)
                    {
                        movimientos.AddRange(_bl.ListarPorProducto(producto.IdProducto));
                    }
                }

                // Aplicar filtros
                if (!string.IsNullOrEmpty(tipoMovimiento))
                {
                    movimientos = movimientos.Where(m => m.TipoMovimiento == tipoMovimiento).ToList();
                }

                if (fechaDesde.HasValue)
                {
                    movimientos = movimientos.Where(m => m.Fecha >= fechaDesde.Value).ToList();
                }

                if (fechaHasta.HasValue)
                {
                    movimientos = movimientos.Where(m => m.Fecha <= fechaHasta.Value).ToList();
                }

                ViewBag.IdProducto = idProducto;
                ViewBag.TipoMovimiento = tipoMovimiento;
                ViewBag.FechaDesde = fechaDesde;
                ViewBag.FechaHasta = fechaHasta;

                return View("Todos", movimientos.OrderByDescending(m => m.Fecha).ToList());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error en búsqueda: {ex.Message}";
                return RedirectToAction("Todos");
            }
        }

        #endregion

        #region Métodos Auxiliares

        private void CargarProductosEnViewBag()
        {
            var productos = _productoBL.Listar();
            ViewBag.Productos = new SelectList(productos, "IdProducto", "Nombre");
        }

        private void CargarTiposMovimiento()
        {
            var tipos = new List<string> { "Entrada", "Salida", "Ajuste", "Devolución" };
            ViewBag.TiposMovimiento = new SelectList(tipos);
        }

        #endregion
    }
}