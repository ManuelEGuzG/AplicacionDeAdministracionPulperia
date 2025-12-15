using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.DA
{
    /// <summary>
    /// DAO especializado en reportes y análisis
    /// </summary>
    public class ReportesDAO : Conexion
    {
        #region Reportes Básicos

        public List<ResumenVentas> ObtenerResumenDiario()
        {
            List<ResumenVentas> lista = new List<ResumenVentas>();
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM VW_ResumenVentas ORDER BY Dia DESC", con);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ResumenVentas
                        {
                            Dia = Convert.ToDateTime(dr["Dia"]),
                            NumeroVentas = Convert.ToInt32(dr["NumeroVentas"]),
                            TotalVendido = Convert.ToDecimal(dr["TotalVendido"])
                        });
                    }
                }
            }
            return lista;
        }

        public List<ProductoMasVendido> ObtenerProductosMasVendidos()
        {
            List<ProductoMasVendido> lista = new List<ProductoMasVendido>();
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM VW_ProductosMasVendidos ORDER BY CantidadVendida DESC", con);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ProductoMasVendido
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Nombre = dr["Nombre"].ToString(),
                            CantidadVendida = Convert.ToInt32(dr["CantidadVendida"]),
                            MontoGenerado = Convert.ToDecimal(dr["MontoGenerado"])
                        });
                    }
                }
            }
            return lista;
        }

        #endregion

        #region Top Productos

        public List<TopProducto> ObtenerTopProductos(int top, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            List<TopProducto> lista = new List<TopProducto>();
            string query = $@"
                SELECT TOP {top} p.IdProducto, p.Codigo, p.Nombre,
                       SUM(dv.Cantidad) AS CantidadVendida,
                       SUM(dv.Subtotal) AS MontoGenerado,
                       SUM(dv.Subtotal - (p.PrecioCosto * dv.Cantidad)) AS GananciaGenerada,
                       COUNT(DISTINCT v.IdVenta) AS NumeroVentas
                FROM DetalleVenta dv
                JOIN Productos p ON dv.IdProducto = p.IdProducto
                JOIN Ventas v ON dv.IdVenta = v.IdVenta
                WHERE 1=1";

            List<SqlParameter> parametros = new List<SqlParameter>();
            if (fechaInicio.HasValue)
            {
                query += " AND v.Fecha >= @FechaInicio";
                parametros.Add(new SqlParameter("@FechaInicio", fechaInicio.Value));
            }
            if (fechaFin.HasValue)
            {
                query += " AND v.Fecha <= @FechaFin";
                parametros.Add(new SqlParameter("@FechaFin", fechaFin.Value));
            }
            query += " GROUP BY p.IdProducto, p.Codigo, p.Nombre ORDER BY CantidadVendida DESC";

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddRange(parametros.ToArray());
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new TopProducto
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Codigo = dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            CantidadVendida = Convert.ToInt32(dr["CantidadVendida"]),
                            MontoGenerado = Convert.ToDecimal(dr["MontoGenerado"]),
                            GananciaGenerada = Convert.ToDecimal(dr["GananciaGenerada"]),
                            NumeroVentas = Convert.ToInt32(dr["NumeroVentas"])
                        });
                    }
                }
            }
            return lista;
        }

        #endregion

        #region Análisis de Rentabilidad

        public AnalisisRentabilidad ObtenerAnalisisRentabilidad(DateTime fechaInicio, DateTime fechaFin)
        {
            AnalisisRentabilidad analisis = new AnalisisRentabilidad
            {
                PeriodoInicio = fechaInicio,
                PeriodoFin = fechaFin
            };

            string query = @"
                SELECT COUNT(DISTINCT v.IdVenta) AS NumeroVentas,
                       ISNULL(SUM(v.Total), 0) AS TotalVentas,
                       ISNULL(SUM(p.PrecioCosto * dv.Cantidad), 0) AS CostoTotal,
                       ISNULL(SUM(dv.Subtotal - (p.PrecioCosto * dv.Cantidad)), 0) AS GananciaBruta,
                       CASE WHEN SUM(v.Total) > 0 
                            THEN (SUM(dv.Subtotal - (p.PrecioCosto * dv.Cantidad)) / SUM(v.Total) * 100)
                            ELSE 0 END AS MargenBruto,
                       CASE WHEN COUNT(DISTINCT v.IdVenta) > 0 
                            THEN SUM(v.Total) / COUNT(DISTINCT v.IdVenta)
                            ELSE 0 END AS VentaPromedio
                FROM Ventas v
                JOIN DetalleVenta dv ON v.IdVenta = dv.IdVenta
                JOIN Productos p ON dv.IdProducto = p.IdProducto
                WHERE v.Fecha >= @FechaInicio AND v.Fecha <= @FechaFin";

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        analisis.NumeroVentas = Convert.ToInt32(dr["NumeroVentas"]);
                        analisis.TotalVentas = Convert.ToDecimal(dr["TotalVentas"]);
                        analisis.CostoTotal = Convert.ToDecimal(dr["CostoTotal"]);
                        analisis.GananciaBruta = Convert.ToDecimal(dr["GananciaBruta"]);
                        analisis.MargenBruto = Convert.ToDecimal(dr["MargenBruto"]);
                        analisis.VentaPromedio = Convert.ToDecimal(dr["VentaPromedio"]);
                    }
                }
            }
            return analisis;
        }

        #endregion

        #region Estadísticas Generales

        public Dictionary<string, object> ObtenerEstadisticasGenerales()
        {
            Dictionary<string, object> stats = new Dictionary<string, object>();
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Productos", con);
                stats["TotalProductos"] = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("SELECT COUNT(*) FROM Clientes", con);
                stats["TotalClientes"] = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("SELECT COUNT(*) FROM Proveedores", con);
                stats["TotalProveedores"] = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("SELECT COUNT(*) FROM Ventas", con);
                stats["TotalVentas"] = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("SELECT COUNT(*) FROM Productos WHERE Existencias <= PuntoReorden", con);
                stats["ProductosBajoStock"] = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("SELECT ISNULL(SUM(Existencias * PrecioCosto), 0) FROM Productos", con);
                stats["ValorInventario"] = (decimal)cmd.ExecuteScalar();

                cmd = new SqlCommand("SELECT ISNULL(SUM(Total), 0) FROM Ventas WHERE CAST(Fecha AS DATE) = CAST(GETDATE() AS DATE)", con);
                stats["VentasHoy"] = (decimal)cmd.ExecuteScalar();

                cmd = new SqlCommand("SELECT ISNULL(SUM(Total), 0) FROM Ventas WHERE YEAR(Fecha) = YEAR(GETDATE()) AND MONTH(Fecha) = MONTH(GETDATE())", con);
                stats["VentasMes"] = (decimal)cmd.ExecuteScalar();
            }
            return stats;
        }

        #endregion
    }
}