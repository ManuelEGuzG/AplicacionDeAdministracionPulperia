namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// DTO para productos más vendidos
    /// </summary>
    public class ProductoMasVendido
    {
        /// <summary>
        /// ID del producto
        /// </summary>
        public int IdProducto { get; set; }

        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Cantidad total vendida del producto
        /// </summary>
        public int CantidadVendida { get; set; }

        /// <summary>
        /// Monto total generado por este producto
        /// </summary>
        public decimal MontoGenerado { get; set; }
    }
}