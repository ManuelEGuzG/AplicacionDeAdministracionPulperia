using System;

namespace AplicacionDeAdministracionPulperia.Model
{
    /// <summary>
    /// DTO para análisis de rentabilidad en un período específico
    /// Contiene métricas de ventas, costos y márgenes de ganancia
    /// </summary>
    public class AnalisisRentabilidad
    {
        // ===== MÉTRICAS DE VENTAS =====

        /// <summary>
        /// Número total de ventas realizadas en el período
        /// </summary>
        public int NumeroVentas { get; set; }

        /// <summary>
        /// Monto total vendido en el período (suma de todos los totales de venta)
        /// </summary>
        public decimal TotalVentas { get; set; }

        /// <summary>
        /// Venta promedio por transacción
        /// Calculado como: TotalVentas / NumeroVentas
        /// </summary>
        public decimal VentaPromedio { get; set; }

        // ===== MÉTRICAS DE COSTOS =====

        /// <summary>
        /// Costo total de los productos vendidos en el período
        /// Calculado como: Suma(Cantidad * PrecioCosto) de todos los productos vendidos
        /// </summary>
        public decimal CostoTotal { get; set; }

        /// <summary>
        /// Alias para CostoTotal (para compatibilidad con vistas)
        /// </summary>
        public decimal TotalCostos => CostoTotal;

        // ===== MÉTRICAS DE GANANCIA =====

        /// <summary>
        /// Ganancia bruta del período
        /// Calculada como: TotalVentas - CostoTotal
        /// Representa la ganancia antes de gastos operativos
        /// </summary>
        public decimal GananciaBruta { get; set; }

        /// <summary>
        /// Ganancia neta del período (igual a ganancia bruta en este contexto)
        /// Para compatibilidad con vistas que esperan esta propiedad
        /// </summary>
        public decimal GananciaNeta => GananciaBruta;

        // ===== MÉTRICAS DE MARGEN =====

        /// <summary>
        /// Margen bruto en porcentaje
        /// Calculado como: (GananciaBruta / TotalVentas) * 100
        /// </summary>
        public decimal MargenBruto { get; set; }

        /// <summary>
        /// Margen de ganancia en porcentaje (alias para compatibilidad)
        /// </summary>
        public decimal MargenGanancia => MargenBruto;

        // ===== MÉTRICAS ADICIONALES (OPCIONALES) =====

        /// <summary>
        /// Período de análisis en formato descriptivo (opcional)
        /// Ejemplo: "Enero 2024", "Q1 2024", etc.
        /// </summary>
        public string Periodo { get; set; }

        /// <summary>
        /// Fecha de inicio del período analizado (opcional)
        /// </summary>
        public DateTime? FechaInicio { get; set; }

        /// <summary>
        /// Fecha de fin del período analizado (opcional)
        /// </summary>
        public DateTime? FechaFin { get; set; }
    }
}