using System;

namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// DTO para el resumen de ventas diarias
    /// </summary>
    public class ResumenVentas
    {
        /// <summary>
        /// Día de las ventas
        /// </summary>
        public DateTime Dia { get; set; }

        /// <summary>
        /// Número de ventas realizadas en el día
        /// </summary>
        public int NumeroVentas { get; set; }

        /// <summary>
        /// Monto total vendido en el día
        /// </summary>
        public decimal TotalVendido { get; set; }

        /// <summary>
        /// Promedio de venta por transacción
        /// </summary>
        public decimal PromedioVenta => NumeroVentas > 0 ? TotalVendido / NumeroVentas : 0;
    }
}