namespace AplicacionDeAdministracionPulperia.Model
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string TipoPago { get; set; }
        public string Cajero { get; set; }
        public string NumeroRecibo { get; set; }
        public int? IdCliente { get; set; }
    }
}