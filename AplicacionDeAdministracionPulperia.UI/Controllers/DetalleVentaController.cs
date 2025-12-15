using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class DetalleVentaController : Controller
    {
        private readonly DetalleVentaBL _bl = new DetalleVentaBL();
        private readonly VentaBL _ventaBL = new VentaBL();
        private readonly ProductoBL _productoBL = new ProductoBL();

        /// <summary>
        /// Lista todos los detalles de una venta específica
        /// </summary>
        public IActionResult Index(int? idVenta)
        {
            try
            {
                if (!idVenta.HasValue)
                {
                    TempData["Info"] = "Debe especificar un ID de venta para ver los detalles.";
                    return RedirectToAction("Index", "Ventas");
                }

                var detalles = _bl.ListarPorVenta(idVenta.Value);

                // Obtener información de la venta
                var venta = _ventaBL.ObtenerPorId(idVenta.Value);
                ViewBag.Venta = venta;
                ViewBag.IdVenta = idVenta.Value;

                return View(detalles);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar detalles: {ex.Message}";
                return RedirectToAction("Index", "Ventas");
            }
        }

        /// <summary>
        /// Vista detallada con información extendida del producto
        /// </summary>
        public IActionResult Detalles(int idVenta)
        {
            try
            {
                var detalles = _bl.ListarPorVenta(idVenta);
                var venta = _ventaBL.ObtenerPorId(idVenta);

                ViewBag.Venta = venta;
                ViewBag.IdVenta = idVenta;

                // Enriquecer detalles con información del producto
                var detallesExtendidos = new List<DetalleVentaExtendido>();
                foreach (var detalle in detalles)
                {
                    var producto = _productoBL.ObtenerPorId(detalle.IdProducto);
                    detallesExtendidos.Add(new DetalleVentaExtendido
                    {
                        IdDetalle = detalle.IdDetalle,
                        IdProducto = detalle.IdProducto,
                        CodigoProducto = producto.Codigo,
                        NombreProducto = producto.Nombre,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Subtotal = detalle.Subtotal,
                        Ganancia = detalle.Subtotal - (producto.PrecioCosto * detalle.Cantidad)
                    });
                }

                return View(detallesExtendidos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Ventas");
            }
        }

        /// <summary>
        /// Muestra el detalle de un item específico
        /// </summary>
        public IActionResult Details(int id)
        {
            try
            {
                // Buscar el detalle en todas las ventas (no es eficiente pero funcional)
                // En una implementación real, deberías tener un método en el BL para obtener por ID
                var todasLasVentas = _ventaBL.Listar();
                DetalleVenta detalleEncontrado = null;
                Venta ventaAsociada = null;

                foreach (var venta in todasLasVentas)
                {
                    var detalles = _bl.ListarPorVenta(venta.IdVenta);
                    detalleEncontrado = detalles.FirstOrDefault(d => d.IdDetalle == id);
                    if (detalleEncontrado != null)
                    {
                        ventaAsociada = venta;
                        break;
                    }
                }

                if (detalleEncontrado == null)
                {
                    TempData["Error"] = "Detalle no encontrado.";
                    return RedirectToAction("Index", "Ventas");
                }

                var producto = _productoBL.ObtenerPorId(detalleEncontrado.IdProducto);

                ViewBag.Producto = producto;
                ViewBag.Venta = ventaAsociada;

                return View(detalleEncontrado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Ventas");
            }
        }

        /// <summary>
        /// Buscar detalles por criterios
        /// </summary>
        public IActionResult Buscar(int? idVenta, int? idProducto)
        {
            try
            {
                List<DetalleVenta> detalles = new List<DetalleVenta>();

                if (idVenta.HasValue)
                {
                    detalles = _bl.ListarPorVenta(idVenta.Value);

                    if (idProducto.HasValue)
                    {
                        detalles = detalles.Where(d => d.IdProducto == idProducto.Value).ToList();
                    }
                }

                ViewBag.IdVenta = idVenta;
                ViewBag.IdProducto = idProducto;

                return View("Index", detalles);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error en búsqueda: {ex.Message}";
                return RedirectToAction("Index", "Ventas");
            }
        }
    }
}