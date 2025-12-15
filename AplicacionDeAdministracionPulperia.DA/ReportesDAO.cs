using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;
using Microsoft.Data.SqlClient;

namespace AplicacionDeAdministracionPulperia.DA
{
    /// <summary>
    /// Capa de acceso a datos para reportes
    /// Maneja todas las consultas SQL relacionadas con reportes y análisis
    /// </summary>
    public class ReportesDAO : Conexion
    {
        #region Reportes Básicos

        /// <summary>
        /// Obtiene el resumen de ventas diarias desde la vista VW_ResumenVentas
        /// </summary>
        /// <returns>Lista de resumen de ventas por día</returns>
        public List<ResumenVentas> ObtenerResumenDiario()
        {
            var lista = new List<ResumenVentas>();

            try
            {
                using SqlConnection conn = new SqlConnection(_cadenaConexion);
                conn.Open();
                string query = @"
                        SELECT 
                            Dia,
                            NumeroVentas,
                            TotalVendido
                        FROM VW_ResumenVentas
                        ORDER BY Dia DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ResumenVentas
                            {
                                Dia = reader.GetDateTime(0),
                                NumeroVentas = reader.GetInt32(1),
                                TotalVendido = reader.GetDecimal(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el resumen diario de ventas", ex);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene los productos más vendidos desde la vista VW_ProductosMasVendidos
        /// </summary>
        /// <returns>Lista de productos más vendidos</returns>
        public List<ProductoMasVendido> ObtenerProductosMasVendidos()
        {
            var lista = new List<ProductoMasVendido>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_cadenaConexion))
                {
                    conn.Open();
                    string query = @"
                        SELECT TOP 20
                            IdProducto,
                            Nombre,
                            CantidadVendida,
                            MontoGenerado
                        FROM VW_ProductosMasVendidos
                        ORDER BY CantidadVendida DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new ProductoMasVendido
                                {
                                    IdProducto = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    CantidadVendida = reader.GetInt32(2),
                                    MontoGenerado = reader.GetDecimal(3)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los productos más vendidos", ex);
            }

            return lista;
        }

        #endregion

        #region Top Productos

        /// <summary>
        /// Obtiene los productos más vendidos con filtros de fecha y cantidad
        /// </summary>
        /// <param name="top">Cantidad de productos a retornar (1-100)</param>
        /// <param name="fechaInicio">Fecha de inicio (opcional)</param>
        /// <param name="fechaFin">Fecha de fin (opcional)</param>
        /// <returns>Lista de top productos con análisis detallado</returns>
        public List<TopProducto> ObtenerTopProductos(int top = 10, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var lista = new List<TopProducto>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_cadenaConexion))
                {
                    conn.Open();

                    string query = $@"
                        SELECT TOP {top}
                            p.IdProducto,
                            p.Codigo,
                            p.Nombre,
                            SUM(dv.Cantidad) AS CantidadVendida,
                            SUM(dv.Subtotal) AS MontoGenerado,
                            SUM(dv.Subtotal - (dv.Cantidad * p.PrecioCosto)) AS GananciaGenerada,
                            COUNT(DISTINCT v.IdVenta) AS NumeroVentas
                        FROM DetalleVenta dv
                        INNER JOIN Productos p ON dv.IdProducto = p.IdProducto
                        INNER JOIN Ventas v ON dv.IdVenta = v.IdVenta
                        WHERE 1=1";

                    if (fechaInicio.HasValue)
                        query += " AND v.Fecha >= @FechaInicio";

                    if (fechaFin.HasValue)
                        query += " AND v.Fecha < @FechaFin";

                    query += @"
                        GROUP BY p.IdProducto, p.Codigo, p.Nombre
                        ORDER BY CantidadVendida DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (fechaInicio.HasValue)
                            cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value);

                        if (fechaFin.HasValue)
                            cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new TopProducto
                                {
                                    IdProducto = reader.GetInt32(0),
                                    Codigo = reader.GetString(1),
                                    Nombre = reader.GetString(2),
                                    CantidadVendida = reader.GetInt32(3),
                                    MontoGenerado = reader.GetDecimal(4),
                                    GananciaGenerada = reader.GetDecimal(5),
                                    NumeroVentas = reader.GetInt32(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los top productos", ex);
            }

            return lista;
        }

        #endregion

        #region Análisis de Rentabilidad

        /// <summary>
        /// Obtiene el análisis de rentabilidad para un período específico
        /// Calcula métricas de ventas, costos y márgenes
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del período</param>
        /// <param name="fechaFin">Fecha de fin del período</param>
        /// <returns>Análisis de rentabilidad con todas las métricas calculadas</returns>
        public AnalisisRentabilidad ObtenerAnalisisRentabilidad(DateTime fechaInicio, DateTime fechaFin)
        {
            AnalisisRentabilidad analisis = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_cadenaConexion))
                {
                    conn.Open();

                    string query = @"
                        WITH VentasDelPeriodo AS (
                            SELECT 
                                v.IdVenta,
                                v.Total,
                                SUM(dv.Cantidad * p.PrecioCosto) AS CostoVenta
                            FROM Ventas v
                            INNER JOIN DetalleVenta dv ON v.IdVenta = dv.IdVenta
                            INNER JOIN Productos p ON dv.IdProducto = p.IdProducto
                            WHERE v.Fecha >= @FechaInicio AND v.Fecha < @FechaFin
                            GROUP BY v.IdVenta, v.Total
                        )
                        SELECT 
                            COUNT(*) AS NumeroVentas,
                            ISNULL(SUM(Total), 0) AS TotalVentas,
                            ISNULL(SUM(CostoVenta), 0) AS CostoTotal,
                            ISNULL(SUM(Total - CostoVenta), 0) AS GananciaBruta,
                            CASE 
                                WHEN SUM(Total) > 0 THEN 
                                    (SUM(Total - CostoVenta) / SUM(Total)) * 100
                                ELSE 0 
                            END AS MargenBruto,
                            CASE 
                                WHEN COUNT(*) > 0 THEN 
                                    SUM(Total) / COUNT(*)
                                ELSE 0 
                            END AS VentaPromedio
                        FROM VentasDelPeriodo";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                analisis = new AnalisisRentabilidad
                                {
                                    NumeroVentas = reader.GetInt32(0),
                                    TotalVentas = reader.GetDecimal(1),
                                    CostoTotal = reader.GetDecimal(2),
                                    GananciaBruta = reader.GetDecimal(3),
                                    MargenBruto = reader.GetDecimal(4),
                                    VentaPromedio = reader.GetDecimal(5),
                                    PeriodoInicio = fechaInicio,
                                    PeriodoFin = fechaFin
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el análisis de rentabilidad", ex);
            }

            if (analisis == null)
            {
                analisis = new AnalisisRentabilidad
                {
                    NumeroVentas = 0,
                    TotalVentas = 0,
                    CostoTotal = 0,
                    GananciaBruta = 0,
                    MargenBruto = 0,
                    VentaPromedio = 0,
                    PeriodoInicio = fechaInicio,
                    PeriodoFin = fechaFin
                };
            }

            return analisis;
        }

        #endregion

        #region Estadísticas Generales

        /// <summary>
        /// Obtiene estadísticas generales del sistema
        /// Retorna un diccionario con métricas clave
        /// </summary>
        /// <returns>Diccionario con todas las estadísticas del sistema</returns>
        public Dictionary<string, object> ObtenerEstadisticasGenerales()
        {
            var stats = new Dictionary<string, object>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_cadenaConexion))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Productos", conn))
                    {
                        stats["TotalProductos"] = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Clientes", conn))
                    {
                        stats["TotalClientes"] = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Proveedores", conn))
                    {
                        stats["TotalProveedores"] = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Ventas", conn))
                    {
                        stats["TotalVentas"] = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Productos WHERE Existencias <= PuntoReorden", conn))
                    {
                        stats["ProductosBajoStock"] = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(Existencias * PrecioCosto), 0) FROM Productos", conn))
                    {
                        stats["ValorInventario"] = (decimal)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT ISNULL(SUM(Total), 0) 
                        FROM Ventas 
                        WHERE CAST(Fecha AS DATE) = CAST(GETDATE() AS DATE)", conn))
                    {
                        stats["VentasHoy"] = (decimal)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT ISNULL(SUM(Total), 0) 
                        FROM Ventas 
                        WHERE YEAR(Fecha) = YEAR(GETDATE()) 
                        AND MONTH(Fecha) = MONTH(GETDATE())", conn))
                    {
                        stats["VentasMes"] = (decimal)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las estadísticas generales", ex);
            }

            return stats;
        }

        #endregion
    }
}
