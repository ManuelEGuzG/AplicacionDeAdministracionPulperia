using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.UI.Models
{
    public class DashboardViewModel
    {
        // Métricas Generales
        public int TotalVentas { get; set; }
        public decimal MontoTotalVentas { get; set; }
        public int TotalClientes { get; set; }
        public int TotalProductos { get; set; }
        public int TotalProveedores { get; set; }

        // Inventario
        public int ProductosBajoStock { get; set; }
        public decimal ValorInventario { get; set; }
        public List<Producto> ProductosBajoStockList { get; set; } = new List<Producto>();

        // Listas
        public List<Cliente> ClientesRecientes { get; set; } = new List<Cliente>();
        public List<Proveedor> Proveedores { get; set; } = new List<Proveedor>();

        // Top Productos
        public List<TopProducto> TopProductos { get; set; } = new List<TopProducto>();
    }
}