namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// DTO para comparación mensual de ventas
    /// Permite comparar el rendimiento del mes actual con el mes anterior
    /// </summary>
    public class ComparativoMensual
    {
        /// <summary>
        /// Nombre o identificador del mes actual
        /// </summary>
        public string MesActual { get; set; }

        /// <summary>
        /// Nombre o identificador del mes anterior
        /// </summary>
        public string MesAnterior { get; set; }

        // ===== ESTADÍSTICAS DEL MES ACTUAL =====

        /// <summary>
        /// Número de ventas realizadas en el mes actual
        /// </summary>
        public int VentasMesActual { get; set; }

        /// <summary>
        /// Monto total vendido en el mes actual
        /// </summary>
        public decimal MontoMesActual { get; set; }

        /// <summary>
        /// Ganancia total obtenida en el mes actual
        /// </summary>
        public decimal GananciaMesActual { get; set; }

        // ===== ESTADÍSTICAS DEL MES ANTERIOR =====

        /// <summary>
        /// Número de ventas realizadas en el mes anterior
        /// </summary>
        public int VentasMesAnterior { get; set; }

        /// <summary>
        /// Monto total vendido en el mes anterior
        /// </summary>
        public decimal MontoMesAnterior { get; set; }

        /// <summary>
        /// Ganancia total obtenida en el mes anterior
        /// </summary>
        public decimal GananciaMesAnterior { get; set; }

        // ===== VARIACIONES CALCULADAS =====

        /// <summary>
        /// Variación en el número de ventas (diferencia entre mes actual y anterior)
        /// </summary>
        public int VariacionVentas => VentasMesActual - VentasMesAnterior;

        /// <summary>
        /// Variación en el monto total (diferencia entre mes actual y anterior)
        /// </summary>
        public decimal VariacionMonto => MontoMesActual - MontoMesAnterior;

        /// <summary>
        /// Variación en la ganancia (diferencia entre mes actual y anterior)
        /// </summary>
        public decimal VariacionGanancia => GananciaMesActual - GananciaMesAnterior;

        // ===== PORCENTAJES DE VARIACIÓN =====

        /// <summary>
        /// Porcentaje de variación en ventas
        /// Maneja el caso de división por cero retornando 0
        /// </summary>
        public decimal PorcentajeVariacionVentas
        {
            get
            {
                if (VentasMesAnterior == 0) return 0;
                return (decimal)VariacionVentas / VentasMesAnterior * 100;
            }
        }

        /// <summary>
        /// Porcentaje de variación en monto
        /// Maneja el caso de división por cero retornando 0
        /// </summary>
        public decimal PorcentajeVariacionMonto
        {
            get
            {
                if (MontoMesAnterior == 0) return 0;
                return VariacionMonto / MontoMesAnterior * 100;
            }
        }

        /// <summary>
        /// Porcentaje de variación en ganancia
        /// Maneja el caso de división por cero retornando 0
        /// </summary>
        public decimal PorcentajeVariacionGanancia
        {
            get
            {
                if (GananciaMesAnterior == 0) return 0;
                return VariacionGanancia / GananciaMesAnterior * 100;
            }
        }
    }
}