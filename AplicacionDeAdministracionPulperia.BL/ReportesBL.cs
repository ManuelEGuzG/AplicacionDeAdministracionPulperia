using System;
using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class ReportesBL
    {
        private readonly ReportesDAO _dao = new ReportesDAO();
        private readonly ProductoDAO _productoDao = new ProductoDAO();
        private readonly ClienteDAO _clienteDao = new ClienteDAO();

        #region Reportes Básicos

        /// <summary>
        /// Obtiene el resumen de ventas diarias
        /// </summary>
        public List<ResumenVentas> ObtenerResumenVentas()
        {
            return _dao.ObtenerResumenDiario();
        }

        /// <summary>
        /// Obtiene los productos más vendidos (todos los tiempos)
        /// </summary>
        public List<ProductoMasVendido> ObtenerProductosMasVendidos()
        {
            return _dao.ObtenerProductosMasVendidos();
        }

        #endregion

        #region Top Productos

        /// <summary>
        /// Obtiene los productos más vendidos en un período específico
        /// </summary>
        /// <param name="top">Cantidad de productos a retornar (1-100)</param>
        /// <param name="fechaInicio">Fecha de inicio (opcional)</param>
        /// <param name="fechaFin">Fecha de fin (opcional)</param>
        /// <returns>Lista de top productos</returns>
        public List<TopProducto> ObtenerTopProductos(int top = 10, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            if (top <= 0 || top > 100)
                throw new ArgumentException("El número de top productos debe estar entre 1 y 100.", nameof(top));

            return _dao.ObtenerTopProductos(top, fechaInicio, fechaFin);
        }

        /// <summary>
        /// Obtiene los top 5 productos del día actual
        /// </summary>
        public List<TopProducto> ObtenerTopProductosHoy()
        {
            var hoy = DateTime.Today;
            var manana = hoy.AddDays(1);
            return _dao.ObtenerTopProductos(5, hoy, manana);
        }

        /// <summary>
        /// Obtiene los top 10 productos del mes actual
        /// </summary>
        public List<TopProducto> ObtenerTopProductosMes()
        {
            var hoy = DateTime.Today;
            var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
            return _dao.ObtenerTopProductos(10, primerDiaMes, ultimoDiaMes);
        }

        #endregion

        #region Análisis de Rentabilidad

        /// <summary>
        /// Obtiene el análisis de rentabilidad para un período específico
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del período</param>
        /// <param name="fechaFin">Fecha de fin del período</param>
        /// <returns>Análisis de rentabilidad</returns>
        public AnalisisRentabilidad ObtenerAnalisisRentabilidad(DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaInicio > fechaFin)
                throw new ArgumentException("La fecha de inicio no puede ser mayor a la fecha fin.");

            return _dao.ObtenerAnalisisRentabilidad(fechaInicio, fechaFin);
        }

        /// <summary>
        /// Obtiene el análisis de rentabilidad del mes actual
        /// </summary>
        public AnalisisRentabilidad ObtenerRentabilidadMesActual()
        {
            var hoy = DateTime.Today;
            var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

            return _dao.ObtenerAnalisisRentabilidad(primerDiaMes, ultimoDiaMes);
        }

        /// <summary>
        /// Obtiene el análisis de rentabilidad del año actual
        /// </summary>
        public AnalisisRentabilidad ObtenerRentabilidadAnioActual()
        {
            var hoy = DateTime.Today;
            var primerDiaAnio = new DateTime(hoy.Year, 1, 1);
            var ultimoDiaAnio = new DateTime(hoy.Year, 12, 31);

            return _dao.ObtenerAnalisisRentabilidad(primerDiaAnio, ultimoDiaAnio);
        }

        /// <summary>
        /// Obtiene el análisis de rentabilidad de los últimos N días
        /// </summary>
        public AnalisisRentabilidad ObtenerRentabilidadUltimosDias(int dias)
        {
            if (dias <= 0)
                throw new ArgumentException("El número de días debe ser mayor a cero.", nameof(dias));

            var hoy = DateTime.Today;
            var fechaInicio = hoy.AddDays(-dias);

            return _dao.ObtenerAnalisisRentabilidad(fechaInicio, hoy);
        }

        #endregion

        #region Dashboard

        /// <summary>
        /// Genera un dashboard completo con todas las estadísticas principales
        /// </summary>
        /// <returns>Dashboard con todas las métricas</returns>
        public DashboardReportes GenerarDashboard()
        {
            var stats = _dao.ObtenerEstadisticasGenerales();
            var rentabilidadMes = ObtenerRentabilidadMesActual();

            // Calcular ticket promedio
            int totalVentas = (int)stats["TotalVentas"];
            decimal montoVentasMes = (decimal)stats["VentasMes"];
            decimal ticketPromedio = totalVentas > 0 ? montoVentasMes / totalVentas : 0;

            // Calcular margen promedio
            decimal gananciaTotal = rentabilidadMes != null ? rentabilidadMes.GananciaBruta : 0;
            decimal totalVentasDecimal = rentabilidadMes != null ? rentabilidadMes.TotalVentas : 0;
            decimal margenPromedio = totalVentasDecimal > 0 ? (gananciaTotal / totalVentasDecimal * 100) : 0;

            return new DashboardReportes
            {
                FechaGeneracion = DateTime.Now,
                TotalProductos = (int)stats["TotalProductos"],
                TotalClientes = (int)stats["TotalClientes"],
                TotalVentas = totalVentasDecimal,
                GananciaTotal = gananciaTotal,
                ProductosBajoStock = (int)stats["ProductosBajoStock"],
                ValorInventario = (decimal)stats["ValorInventario"],
                VentasMes = totalVentas,
                TicketPromedio = ticketPromedio,
                MargenPromedio = margenPromedio,
                RotacionPromedio = 0 // Se puede calcular con más datos
            };
        }

        #endregion

        #region Reportes Comparativos

        /// <summary>
        /// Compara el mes actual con el mes anterior
        /// </summary>
        public ComparativoMensual CompararMesActualConAnterior()
        {
            var hoy = DateTime.Today;

            // Mes actual
            var inicioMesActual = new DateTime(hoy.Year, hoy.Month, 1);
            var finMesActual = hoy;

            // Mes anterior
            var inicioMesAnterior = inicioMesActual.AddMonths(-1);
            var finMesAnterior = inicioMesActual.AddDays(-1);

            // Obtener datos de ambos períodos
            var rentabilidadActual = _dao.ObtenerAnalisisRentabilidad(inicioMesActual, finMesActual);
            var rentabilidadAnterior = _dao.ObtenerAnalisisRentabilidad(inicioMesAnterior, finMesAnterior);

            return new ComparativoMensual
            {
                MesActual = inicioMesActual.ToString("MMMM yyyy"),
                MesAnterior = inicioMesAnterior.ToString("MMMM yyyy"),
                VentasMesActual = rentabilidadActual.NumeroVentas,
                MontoMesActual = rentabilidadActual.TotalVentas,
                GananciaMesActual = rentabilidadActual.GananciaBruta,
                VentasMesAnterior = rentabilidadAnterior.NumeroVentas,
                MontoMesAnterior = rentabilidadAnterior.TotalVentas,
                GananciaMesAnterior = rentabilidadAnterior.GananciaBruta
            };
        }

        /// <summary>
        /// Genera un reporte comparativo entre dos períodos personalizados
        /// </summary>
        public ReporteComparativo GenerarReporteComparativo(
            DateTime fechaInicio1, DateTime fechaFin1,
            DateTime fechaInicio2, DateTime fechaFin2)
        {
            if (fechaInicio1 > fechaFin1)
                throw new ArgumentException("La fecha de inicio del período 1 no puede ser mayor a su fecha fin.");

            if (fechaInicio2 > fechaFin2)
                throw new ArgumentException("La fecha de inicio del período 2 no puede ser mayor a su fecha fin.");

            var periodo1 = _dao.ObtenerAnalisisRentabilidad(fechaInicio1, fechaFin1);
            var periodo2 = _dao.ObtenerAnalisisRentabilidad(fechaInicio2, fechaFin2);

            return new ReporteComparativo
            {
                Periodo1 = periodo1,
                Periodo2 = periodo2,
                DiferenciaTotalVentas = periodo2.TotalVentas - periodo1.TotalVentas,
                DiferenciaGanancia = periodo2.GananciaBruta - periodo1.GananciaBruta,
                DiferenciaNumeroVentas = periodo2.NumeroVentas - periodo1.NumeroVentas,
                PorcentajeCrecimientoVentas = periodo1.TotalVentas > 0
                    ? ((periodo2.TotalVentas - periodo1.TotalVentas) / periodo1.TotalVentas * 100)
                    : 0
            };
        }

        /// <summary>
        /// Compara el año actual con el año anterior
        /// </summary>
        public ReporteComparativo CompararAnioActualConAnterior()
        {
            var hoy = DateTime.Today;

            // Año actual
            var inicioAnioActual = new DateTime(hoy.Year, 1, 1);
            var finAnioActual = new DateTime(hoy.Year, 12, 31);

            // Año anterior
            var inicioAnioAnterior = new DateTime(hoy.Year - 1, 1, 1);
            var finAnioAnterior = new DateTime(hoy.Year - 1, 12, 31);

            return GenerarReporteComparativo(
                inicioAnioAnterior, finAnioAnterior,
                inicioAnioActual, finAnioActual
            );
        }

        #endregion
    }
}