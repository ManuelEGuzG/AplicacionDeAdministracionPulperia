namespace AplicacionDeAdministracionPulperia.Model
{
    public class MovimientoInventario
    {
        public int IdMovimiento { get; set; }
        public int IdProducto { get; set; }
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
        public string TipoMovimiento { get; set; }
        public string Notas { get; set; }
    }
}
