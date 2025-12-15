namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// Clase para filtrar proveedores con múltiples criterios
    /// </summary>
    public class ProveedorFiltro
    {
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public DateTime? FechaCreacionDesde { get; set; }
        public DateTime? FechaCreacionHasta { get; set; }
        public bool? TieneProductos { get; set; } // Proveedores con productos asociados
        public int? NumeroProductosMin { get; set; }
        public int? NumeroProductosMax { get; set; }
        public string? OrdenarPor { get; set; } // "Nombre", "FechaCreacion", "NumeroProductos"
        public bool Descendente { get; set; } = false;
    }
}