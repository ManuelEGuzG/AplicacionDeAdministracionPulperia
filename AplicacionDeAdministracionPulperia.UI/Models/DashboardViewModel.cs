using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Models
{
    public class DashboardViewModel
    {
        /* ===============================
           MÉTRICAS GENERALES
           =============================== */

        public int TotalVentas { get; set; }
        public decimal MontoTotalVentas { get; set; }

        public int TotalClientes { get; set; }
        public int TotalProductos { get; set; }
        public int TotalProveedores { get; set; }

        /* ===============================
           INVENTARIO
           =============================== */

        public int ProductosBajoStock { get; set; }
        public List<Producto> ProductosBajoStockList { get; set; }

        /* ===============================
           CLIENTES Y PROVEEDORES
           =============================== */

        public List<Cliente> ClientesRecientes { get; set; }
        public List<Proveedor> Proveedores { get; set; }
    }
}
