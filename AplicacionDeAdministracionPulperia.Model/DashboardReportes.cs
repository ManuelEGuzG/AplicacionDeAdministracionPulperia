using System;

namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// DTO para el dashboard completo de reportes
    /// Contiene todas las métricas principales del sistema
    /// </summary>
    public class DashboardReportes
    {
        // ===== MÉTRICAS PRINCIPALES =====

        /// <summary>
        /// Total de ventas realizadas en el sistema
        /// </summary>
        public decimal TotalVentas { get; set; }

        /// <summary>
        /// Total de productos activos en el inventario
        /// </summary>
        public int TotalProductos { get; set; }

        /// <summary>
        /// Total de clientes registrados
        /// </summary>
        public int TotalClientes { get; set; }

        /// <summary>
        /// Ganancia total acumulada
        /// </summary>
        public decimal GananciaTotal { get; set; }

        // ===== ESTADÍSTICAS DE INVENTARIO =====

        /// <summary>
        /// Valor total del inventario actual
        /// Calculado como: Suma(Existencias * PrecioCosto) de todos los productos
        /// </summary>
        public decimal ValorInventario { get; set; }

        /// <summary>
        /// Número de productos con existencias bajo el punto de reorden
        /// </summary>
        public int ProductosBajoStock { get; set; }

        /// <summary>
        /// Rotación promedio del inventario
        /// Indica cuántas veces se vende y reemplaza el inventario en un período
        /// </summary>
        public decimal RotacionPromedio { get; set; }

        // ===== ESTADÍSTICAS DE VENTAS =====

        /// <summary>
        /// Número de ventas realizadas en el mes actual
        /// </summary>
        public int VentasMes { get; set; }

        /// <summary>
        /// Ticket promedio de venta
        /// Calculado como: TotalVentas / NumeroVentas
        /// </summary>
        public decimal TicketPromedio { get; set; }

        /// <summary>
        /// Margen de ganancia promedio en porcentaje
        /// Calculado como: (GananciaTotal / TotalVentas) * 100
        /// </summary>
        public decimal MargenPromedio { get; set; }

        // ===== METADATA =====

        /// <summary>
        /// Fecha y hora en que se generó este dashboard
        /// </summary>
        public DateTime FechaGeneracion { get; set; }
    }
}