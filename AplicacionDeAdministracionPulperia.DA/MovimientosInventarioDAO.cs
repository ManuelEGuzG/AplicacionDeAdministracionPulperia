using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;
using Microsoft.Data.SqlClient;

namespace AplicacionDeAdministracionPulperia.DA
{
    public class MovimientoInventarioDAO : Conexion
    {
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
                cmd.Parameters.AddWithValue("@Notas", m.Notas);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<MovimientoInventario> ObtenerPorProducto(int idProducto)
        {
            List<MovimientoInventario> lista = new List<MovimientoInventario>();

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM MovimientosInventario WHERE IdProducto = @id ORDER BY Fecha DESC", con);
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
    }
}
