using Microsoft.AspNetCore.Mvc;
using AplicacionDeAdministracionPulperia.BL;
using AplicacionDeAdministracionPulperia.UI.Models;
using System.Linq;

namespace AplicacionDeAdministracionPulperia.UI.Controllers
{
    public class PrincipalController : Controller
    {
        private readonly VentaBL _ventaBL = new VentaBL();
        private readonly ClienteBL _clienteBL = new ClienteBL();
        private readonly ProductoBL _productoBL = new ProductoBL();
        private readonly ProveedorBL _proveedorBL = new ProveedorBL();

        public IActionResult Dashboard()
        {
            /* ===============================
               OBTENCIÓN DE DATOS
               =============================== */

            var ventas = _ventaBL.Listar();
            var clientes = _clienteBL.Listar();
            var productos = _productoBL.Listar();
            var proveedores = _proveedorBL.Listar();

            /* ===============================
               PRODUCTOS CON BAJO INVENTARIO
               =============================== */

            var productosBajoStockList = productos
                .Where(p => p.Existencias <= p.PuntoReorden)
                .OrderBy(p => p.Existencias)
                .Take(5)
                .ToList();

            /* ===============================
               CLIENTES RECIENTES
               =============================== */

            var clientesRecientes = clientes
                .OrderByDescending(c => c.FechaCreacion)
                .Take(5)
                .ToList();

            /* ===============================
               CONSTRUCCIÓN DEL VIEWMODEL
               =============================== */

            var model = new DashboardViewModel
            {
                TotalVentas = ventas.Count,
                MontoTotalVentas = ventas.Any() ? ventas.Sum(v => v.Total) : 0,

                TotalClientes = clientes.Count,
                TotalProductos = productos.Count,
                TotalProveedores = proveedores.Count,

                ProductosBajoStock = productosBajoStockList.Count,
                ProductosBajoStockList = productosBajoStockList,

                ClientesRecientes = clientesRecientes,
                Proveedores = proveedores.Take(5).ToList()
            };

            return View(model);
        }
    }
}
