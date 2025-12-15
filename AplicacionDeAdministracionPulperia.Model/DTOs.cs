namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// DTO para información detallada del cliente con estadísticas
    /// </summary>
    public class ClienteDetalle
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int NumeroCompras { get; set; }
        public decimal TotalCompras { get; set; }
        public decimal PromedioCompra { get; set; }
        public DateTime? UltimaCompra { get; set; }
        public string ProductoMasComprado { get; set; }
    }

    /// <summary>
    /// DTO para información detallada del producto con estadísticas
    /// </summary>
    public class ProductoDetalle
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCosto { get; set; }
        public int Existencias { get; set; }
        public int PuntoReorden { get; set; }
        public string NombreProveedor { get; set; }
        public int TotalVendido { get; set; }
        public decimal MontoGenerado { get; set; }
        public decimal MargenGanancia { get; set; }
        public DateTime? UltimaVenta { get; set; }
        public DateTime? UltimaEntrada { get; set; }
    }

    /// <summary>
    /// DTO para información detallada de proveedor con estadísticas
    /// </summary>
    public class ProveedorDetalle
    {
        public int IdProveedor { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int NumeroProductos { get; set; }
        public int TotalExistencias { get; set; }
        public decimal ValorInventario { get; set; }
    }

    /// <summary>
    /// DTO para venta con detalles completos
    /// </summary>
    public class VentaDetallada
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string TipoPago { get; set; }
        public string Cajero { get; set; }
        public string NumeroRecibo { get; set; }
        public string? NombreCliente { get; set; }
        public List<DetalleVentaExtendido> Detalles { get; set; }
    }

    /// <summary>
    /// DTO para detalle de venta extendido con información del producto
    /// </summary>
    public class DetalleVentaExtendido
    {
        public int IdDetalle { get; set; }
        public int IdProducto { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Ganancia { get; set; } // Subtotal - (PrecioCosto * Cantidad)
    }

    /// <summary>
    /// DTO para análisis de rentabilidad
    /// </summary>
    public class AnalisisRentabilidad
    {
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFin { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal GananciaBruta { get; set; }
        public decimal MargenBruto { get; set; }
        public int NumeroVentas { get; set; }
        public decimal VentaPromedio { get; set; }
    }

    /// <summary>
    /// DTO para inventario con alertas
    /// </summary>
    public class InventarioConAlerta
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int Existencias { get; set; }
        public int PuntoReorden { get; set; }
        public string EstadoStock { get; set; } // "Agotado", "Crítico", "Bajo", "Normal", "Alto"
        public int DiasInventarioEstimado { get; set; }
        public decimal ValorInventario { get; set; }
    }

    /// <summary>
    /// DTO para análisis de ventas por período
    /// </summary>
    public class VentasPorPeriodo
    {
        public string Periodo { get; set; } // Mes, Semana, Día
        public int NumeroVentas { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal Ganancia { get; set; }
        public int ProductosVendidos { get; set; }
        public decimal VentaPromedio { get; set; }
    }

    /// <summary>
    /// DTO para top productos
    /// </summary>
    public class TopProducto
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int CantidadVendida { get; set; }
        public decimal MontoGenerado { get; set; }
        public decimal GananciaGenerada { get; set; }
        public int NumeroVentas { get; set; }
    }

    /// <summary>
    /// DTO para movimientos de inventario con detalles
    /// </summary>
    public class MovimientoInventarioDetalle
    {
        public int IdMovimiento { get; set; }
        public int IdProducto { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
        public string TipoMovimiento { get; set; }
        public string Notas { get; set; }
        public int ExistenciaAnterior { get; set; }
        public int ExistenciaNueva { get; set; }
    }

    /// <summary>
    /// DTO para resumen de ventas en un período
    /// </summary>
    public class ResumenVentasPeriodo
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int NumeroVentas { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal VentaPromedio { get; set; }
        public decimal VentaMinima { get; set; }
        public decimal VentaMaxima { get; set; }
    }

    /// <summary>
    /// DTO para estadísticas de productos
    /// </summary>
    public class EstadisticasProducto
    {
        public int TotalProductos { get; set; }
        public int ProductosBajoStock { get; set; }
        public decimal ValorTotalInventario { get; set; }
    }

    /// <summary>
    /// DTO para dashboard con estadísticas principales
    /// </summary>
    public class ReporteDashboard
    {
        public DateTime FechaGeneracion { get; set; }
        public int TotalProductos { get; set; }
        public int TotalClientes { get; set; }
        public int TotalProveedores { get; set; }
        public int TotalVentas { get; set; }
        public int ProductosBajoStock { get; set; }
        public decimal ValorInventario { get; set; }
        public decimal VentasHoy { get; set; }
        public decimal VentasMes { get; set; }
        public List<TopProducto> TopProductos { get; set; }
        public AnalisisRentabilidad RentabilidadMes { get; set; }
    }

    /// <summary>
    /// DTO para reportes comparativos entre dos períodos
    /// </summary>
    public class ReporteComparativo
    {
        public AnalisisRentabilidad Periodo1 { get; set; }
        public AnalisisRentabilidad Periodo2 { get; set; }
        public decimal DiferenciaTotalVentas { get; set; }
        public decimal DiferenciaGanancia { get; set; }
        public int DiferenciaNumeroVentas { get; set; }
        public decimal PorcentajeCrecimientoVentas { get; set; }
    }
}