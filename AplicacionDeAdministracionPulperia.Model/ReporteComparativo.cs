namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// DTO para reporte comparativo entre dos períodos
    /// </summary>
    public class ReporteComparativo
    {
        /// <summary>
        /// Análisis del primer período
        /// </summary>
        public AnalisisRentabilidad Periodo1 { get; set; }

        /// <summary>
        /// Análisis del segundo período
        /// </summary>
        public AnalisisRentabilidad Periodo2 { get; set; }

        /// <summary>
        /// Diferencia en total de ventas (Periodo2 - Periodo1)
        /// </summary>
        public decimal DiferenciaTotalVentas { get; set; }

        /// <summary>
        /// Diferencia en ganancia (Periodo2 - Periodo1)
        /// </summary>
        public decimal DiferenciaGanancia { get; set; }

        /// <summary>
        /// Diferencia en número de ventas (Periodo2 - Periodo1)
        /// </summary>
        public int DiferenciaNumeroVentas { get; set; }

        /// <summary>
        /// Porcentaje de crecimiento en ventas
        /// </summary>
        public decimal PorcentajeCrecimientoVentas { get; set; }
    }
}