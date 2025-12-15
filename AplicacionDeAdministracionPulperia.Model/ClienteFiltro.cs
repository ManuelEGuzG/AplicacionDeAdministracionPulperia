namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// Clase para filtrar clientes con múltiples criterios
    /// </summary>
    public class ClienteFiltro
    {
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public DateTime? FechaCreacionDesde { get; set; }
        public DateTime? FechaCreacionHasta { get; set; }
        public bool? TieneCompras { get; set; } // Clientes que han realizado al menos una compra
        public decimal? MontoTotalComprasMin { get; set; }
        public decimal? MontoTotalComprasMax { get; set; }
        public int? NumeroComprasMin { get; set; }
        public int? NumeroComprasMax { get; set; }
        public string? OrdenarPor { get; set; } // "Nombre", "FechaCreacion", "TotalCompras"
        public bool Descendente { get; set; } = false;
    }
}