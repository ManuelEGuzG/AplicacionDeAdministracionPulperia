namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// DTO para top productos con análisis detallado
    /// </summary>
    public class TopProducto
    {
        /// <summary>
        /// ID del producto
        /// </summary>
        public int IdProducto { get; set; }

        /// <summary>
        /// Código del producto
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Precio de costo del producto
        /// </summary>
        public decimal PrecioCosto { get; set; }

        /// <summary>
        /// Cantidad total vendida
        /// </summary>
        public int CantidadVendida { get; set; }

        /// <summary>
        /// Total de ventas generadas
        /// </summary>
        public decimal TotalVentas { get; set; }

        /// <summary>
        /// Costo total de las unidades vendidas
        /// </summary>
        public decimal CostoTotal { get; set; }

        /// <summary>
        /// Ganancia total generada
        /// </summary>
        public decimal GananciaTotal { get; set; }

        /// <summary>
        /// Número de ventas en las que apareció este producto
        /// </summary>
        public int NumeroVentas { get; set; }

        /// <summary>
        /// Margen de ganancia en porcentaje
        /// </summary>
        public decimal MargenGanancia { get; set; }
    }
}