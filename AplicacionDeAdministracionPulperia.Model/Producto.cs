namespace AplicacionDeAdministracionPulperia.Model
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCosto { get; set; }
        public int Existencias { get; set; }
        public int PuntoReorden { get; set; }
        public int? IdProveedor { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}