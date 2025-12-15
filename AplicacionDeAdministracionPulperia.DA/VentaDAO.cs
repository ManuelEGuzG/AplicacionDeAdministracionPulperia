using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.DA
{
    /// <summary>
    /// DAO para la gestión de ventas
    /// </summary>
    public class VentaDAO : Conexion
    {
        #region CRUD Básico

        public int Insertar(Venta v)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Ventas (Fecha, Total, TipoPago, Cajero, NumeroRecibo, IdCliente)
                    OUTPUT INSERTED.IdVenta
                    VALUES (@Fecha, @Total, @TipoPago, @Cajero, @NumeroRecibo, @IdCliente)", con);
                cmd.Parameters.AddWithValue("@Fecha", v.Fecha);
                cmd.Parameters.AddWithValue("@Total", v.Total);
                cmd.Parameters.AddWithValue("@TipoPago", v.TipoPago);
                cmd.Parameters.AddWithValue("@Cajero", v.Cajero);
                cmd.Parameters.AddWithValue("@NumeroRecibo", v.NumeroRecibo);
                cmd.Parameters.AddWithValue("@IdCliente", (object)v.IdCliente ?? DBNull.Value);
                return (int)cmd.ExecuteScalar();
            }
        }

        public Venta ObtenerPorId(int id)
        {
            Venta v = null;
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Ventas WHERE IdVenta = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                        v = MapearVenta(dr);
                }
            }
            return v;
        }

        public List<Venta> ObtenerTodas()
        {
            List<Venta> lista = new List<Venta>();
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Ventas ORDER BY Fecha DESC", con);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        lista.Add(MapearVenta(dr));
                }
            }
            return lista;
        }

        public Venta ObtenerPorNumeroRecibo(string numeroRecibo)
        {
            Venta v = null;
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Ventas WHERE NumeroRecibo = @num", con);
                cmd.Parameters.AddWithValue("@num", numeroRecibo);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                        v = MapearVenta(dr);
                }
            }
            return v;
        }

        #endregion

        #region Ventas Detalladas

        public List<VentaDetallada> ObtenerVentasDetalladas(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            List<VentaDetallada> lista = new List<VentaDetallada>();
            string query = @"
                SELECT v.*, c.Nombre AS NombreCliente
                FROM Ventas v
                LEFT JOIN Clientes c ON v.IdCliente = c.IdCliente
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
            query += " ORDER BY v.Fecha DESC";

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddRange(parametros.ToArray());

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new VentaDetallada
                        {
                            IdVenta = Convert.ToInt32(dr["IdVenta"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Total = Convert.ToDecimal(dr["Total"]),
                            TipoPago = dr["TipoPago"].ToString(),
                            Cajero = dr["Cajero"].ToString(),
                            NumeroRecibo = dr["NumeroRecibo"].ToString(),
                            NombreCliente = dr["NombreCliente"]?.ToString(),
                            Detalles = new List<DetalleVentaExtendido>()
                        });
                    }
                }

                // Cargar detalles para cada venta
                foreach (var venta in lista)
                {
                    venta.Detalles = ObtenerDetallesExtendidos(con, venta.IdVenta);
                }
            }
            return lista;
        }

        private List<DetalleVentaExtendido> ObtenerDetallesExtendidos(SqlConnection con, int idVenta)
        {
            List<DetalleVentaExtendido> detalles = new List<DetalleVentaExtendido>();
            string query = @"
                SELECT dv.*, p.Codigo AS CodigoProducto, p.Nombre AS NombreProducto,
                       (dv.Subtotal - (p.PrecioCosto * dv.Cantidad)) AS Ganancia
                FROM DetalleVenta dv
                JOIN Productos p ON dv.IdProducto = p.IdProducto
                WHERE dv.IdVenta = @IdVenta";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@IdVenta", idVenta);

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    detalles.Add(new DetalleVentaExtendido
                    {
                        IdDetalle = Convert.ToInt32(dr["IdDetalle"]),
                        IdProducto = Convert.ToInt32(dr["IdProducto"]),
                        CodigoProducto = dr["CodigoProducto"].ToString(),
                        NombreProducto = dr["NombreProducto"].ToString(),
                        Cantidad = Convert.ToInt32(dr["Cantidad"]),
                        PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"]),
                        Subtotal = Convert.ToDecimal(dr["Subtotal"]),
                        Ganancia = Convert.ToDecimal(dr["Ganancia"])
                    });
                }
            }
            return detalles;
        }

        #endregion

        #region Estadísticas

        public int ContarTotal()
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Ventas", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        #endregion

        #region Auxiliares

        private Venta MapearVenta(SqlDataReader dr)
        {
            return new Venta
            {
                IdVenta = Convert.ToInt32(dr["IdVenta"]),
                Fecha = Convert.ToDateTime(dr["Fecha"]),
                Total = Convert.ToDecimal(dr["Total"]),
                TipoPago = dr["TipoPago"].ToString(),
                Cajero = dr["Cajero"].ToString(),
                NumeroRecibo = dr["NumeroRecibo"].ToString(),
                IdCliente = dr["IdCliente"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["IdCliente"])
            };
        }

        #endregion
    }
}