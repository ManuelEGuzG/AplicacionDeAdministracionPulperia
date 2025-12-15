using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.UI.Models;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class PrincipalController : Controller
    {
        private readonly ProductoBL _productoBL = new ProductoBL();
        private readonly ClienteBL _clienteBL = new ClienteBL();
        private readonly ProveedorBL _proveedorBL = new ProveedorBL();
        private readonly VentaBL _ventaBL = new VentaBL();
        private readonly ReportesBL _reportesBL = new ReportesBL();

        /// <summary>
        /// Dashboard principal del sistema
        /// </summary>
        public IActionResult Dashboard()
        {
            try
            {
                // Obtener estadísticas generales
                var estadisticasProducto = _productoBL.ObtenerEstadisticas();
                var totalClientes = _clienteBL.ContarTotal();
                var totalProveedores = _proveedorBL.ContarTotal();
                var totalVentas = _ventaBL.ContarTotal();

                // Obtener productos bajo stock
                var productosBajoStock = _productoBL.ObtenerProductosBajoStock()
                    .OrderBy(p => p.Existencias)
                    .Take(5)
                    .ToList();

                // Obtener clientes recientes (últimos 5)
                var todosLosClientes = _clienteBL.Listar();
                var clientesRecientes = todosLosClientes
                    .OrderByDescending(c => c.FechaCreacion)
                    .Take(5)
                    .ToList();

                // Obtener proveedores (primeros 5)
                var proveedores = _proveedorBL.Listar().Take(5).ToList();

                // Obtener resumen de ventas del día actual
                var resumenHoy = _ventaBL.ObtenerResumenHoy();

                // Obtener top 5 productos más vendidos del mes
                var topProductos = _reportesBL.ObtenerTopProductos(5);

                // Construir el ViewModel
                var model = new DashboardViewModel
                {
                    // Métricas generales
                    TotalVentas = totalVentas,
                    MontoTotalVentas = resumenHoy.TotalVentas,
                    TotalClientes = totalClientes,
                    TotalProductos = estadisticasProducto.TotalProductos,
                    TotalProveedores = totalProveedores,

                    // Inventario
                    ProductosBajoStock = estadisticasProducto.ProductosBajoStock,
                    ProductosBajoStockList = productosBajoStock,

                    // Listas
                    ClientesRecientes = clientesRecientes,
                    Proveedores = proveedores,

                    // Adicionales
                    ValorInventario = estadisticasProducto.ValorTotalInventario,
                    TopProductos = topProductos
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el dashboard: {ex.Message}";
                return View(new DashboardViewModel());
            }
        }

        /// <summary>
        /// Vista de estadísticas avanzadas
        /// </summary>
        public IActionResult Estadisticas()
        {
            try
            {
                var rentabilidadMes = _reportesBL.ObtenerRentabilidadMesActual();
                var rentabilidadAnio = _reportesBL.ObtenerRentabilidadAnioActual();
                var comparativoMeses = _reportesBL.CompararMesActualConAnterior();

                ViewBag.RentabilidadMes = rentabilidadMes;
                ViewBag.RentabilidadAnio = rentabilidadAnio;
                ViewBag.Comparativo = comparativoMeses;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar estadísticas: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
        }
    }
}