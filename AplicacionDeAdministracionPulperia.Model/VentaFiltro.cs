namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// Clase para filtrar ventas con múltiples criterios
    /// </summary>
    public class VentaFiltro
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public decimal? TotalMin { get; set; }
        public decimal? TotalMax { get; set; }
        public string? TipoPago { get; set; } // "Efectivo", "Tarjeta", "Transferencia", etc.
        public string? Cajero { get; set; }
        public string? NumeroRecibo { get; set; }
        public int? IdCliente { get; set; }
        public bool? TieneCliente { get; set; } // Ventas con o sin cliente asociado
        public string? OrdenarPor { get; set; } // "Fecha", "Total", "NumeroRecibo"
        public bool Descendente { get; set; } = true;
    }
}