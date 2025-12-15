namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// Clase para filtrar productos con múltiples criterios
    /// </summary>
    public class ProductoFiltro
    {
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public decimal? PrecioVentaMin { get; set; }
        public decimal? PrecioVentaMax { get; set; }
        public decimal? PrecioCostoMin { get; set; }
        public decimal? PrecioCostoMax { get; set; }
        public int? ExistenciasMin { get; set; }
        public int? ExistenciasMax { get; set; }
        public int? IdProveedor { get; set; }
        public bool? BajoStock { get; set; } // Productos con existencias <= punto de reorden
        public DateTime? FechaCreacionDesde { get; set; }
        public DateTime? FechaCreacionHasta { get; set; }
        public string? OrdenarPor { get; set; } // "Nombre", "PrecioVenta", "Existencias", etc.
        public bool Descendente { get; set; } = false;
    }
}