using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class VentasController : Controller
    {
        private readonly VentaBL _bl = new VentaBL();
        private readonly ClienteBL _clienteBL = new ClienteBL();
        private readonly ProductoBL _productoBL = new ProductoBL();

        public IActionResult Index()
        {
            var ventas = _bl.Listar();
            return View(ventas);
        }

        public IActionResult Details(int id)
        {
            try
            {
                var venta = _bl.ObtenerVentaDetallada(id);
                return View(venta);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Detalladas()
        {
            var ventasDetalladas = _bl.ListarVentasDetalladas();
            return View(ventasDetalladas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            CargarDatosParaCreacion();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Venta venta, List<DetalleVenta> detalles)
        {
            if (!ModelState.IsValid)
            {
                CargarDatosParaCreacion();
                return View(venta);
            }

            try
            {
                var idVenta = _bl.Crear(venta, detalles);
                TempData["Success"] = $"Venta creada. Recibo: {venta.NumeroRecibo}";
                return RedirectToAction(nameof(Details), new { id = idVenta });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                CargarDatosParaCreacion();
                return View(venta);
            }
        }

        public IActionResult Resumen(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var resumen = _bl.ObtenerResumenPeriodo(fechaInicio, fechaFin);
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;
            return View(resumen);
        }

        private void CargarDatosParaCreacion()
        {
            ViewBag.Clientes = new SelectList(_clienteBL.Listar(), "IdCliente", "Nombre");
            ViewBag.Productos = new SelectList(_productoBL.Listar(), "IdProducto", "Nombre");
        }
    }
}