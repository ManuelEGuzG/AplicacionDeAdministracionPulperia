using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.DA
{
    /// <summary>
    /// DAO para la gestión de movimientos de inventario
    /// </summary>
    public class MovimientoInventarioDAO : Conexion
    {
        /// <summary>
        /// Inserta un nuevo movimiento de inventario
        /// </summary>
        public bool Insertar(MovimientoInventario m)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO MovimientosInventario (IdProducto, Fecha, Cantidad, TipoMovimiento, Notas)
                    VALUES (@IdProducto, @Fecha, @Cantidad, @TipoMovimiento, @Notas)", con);
                cmd.Parameters.AddWithValue("@IdProducto", m.IdProducto);
                cmd.Parameters.AddWithValue("@Fecha", m.Fecha);
                cmd.Parameters.AddWithValue("@Cantidad", m.Cantidad);
                cmd.Parameters.AddWithValue("@TipoMovimiento", m.TipoMovimiento);
                cmd.Parameters.AddWithValue("@Notas", m.Notas ?? (object)DBNull.Value);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Obtiene todos los movimientos de un producto específico
        /// </summary>
        public List<MovimientoInventario> ObtenerPorProducto(int idProducto)
        {
            List<MovimientoInventario> lista = new List<MovimientoInventario>();
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM MovimientosInventario WHERE IdProducto = @id ORDER BY Fecha DESC", con);
                cmd.Parameters.AddWithValue("@id", idProducto);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new MovimientoInventario
                        {
                            IdMovimiento = Convert.ToInt32(dr["IdMovimiento"]),
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            TipoMovimiento = dr["TipoMovimiento"].ToString(),
                            Notas = dr["Notas"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Obtiene todos los movimientos dentro de un rango de fechas
        /// </summary>
        public List<MovimientoInventario> ObtenerPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            List<MovimientoInventario> lista = new List<MovimientoInventario>();
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM MovimientosInventario 
                    WHERE Fecha >= @FechaInicio AND Fecha <= @FechaFin 
                    ORDER BY Fecha DESC", con);
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new MovimientoInventario
                        {
                            IdMovimiento = Convert.ToInt32(dr["IdMovimiento"]),
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            TipoMovimiento = dr["TipoMovimiento"].ToString(),
                            Notas = dr["Notas"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
    }
}