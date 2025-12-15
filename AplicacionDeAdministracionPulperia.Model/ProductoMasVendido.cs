namespace AplicacionDeAdministracionPulperia.Model
{
    public class ProductoMasVendido
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public int CantidadVendida { get; set; }
        public decimal MontoGenerado { get; set; }
    }
}
