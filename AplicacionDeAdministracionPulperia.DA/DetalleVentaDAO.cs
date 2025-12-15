using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;
using Microsoft.Data.SqlClient;

namespace AplicacionDeAdministracionPulperia.DA
{
    public class DetalleVentaDAO : Conexion
    {
        public bool Insertar(DetalleVenta d)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO DetalleVenta (IdVenta, IdProducto, Cantidad, PrecioUnitario, Subtotal)
                    VALUES (@IdVenta, @IdProducto, @Cantidad, @PrecioUnitario, @Subtotal)", con);

                cmd.Parameters.AddWithValue("@IdVenta", d.IdVenta);
                cmd.Parameters.AddWithValue("@IdProducto", d.IdProducto);
                cmd.Parameters.AddWithValue("@Cantidad", d.Cantidad);
                cmd.Parameters.AddWithValue("@PrecioUnitario", d.PrecioUnitario);
                cmd.Parameters.AddWithValue("@Subtotal", d.Subtotal);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<DetalleVenta> ObtenerPorVenta(int idVenta)
        {
            List<DetalleVenta> lista = new List<DetalleVenta>();

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM DetalleVenta WHERE IdVenta = @id", con);
                cmd.Parameters.AddWithValue("@id", idVenta);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new DetalleVenta
                        {
                            IdDetalle = Convert.ToInt32(dr["IdDetalle"]),
                            IdVenta = Convert.ToInt32(dr["IdVenta"]),
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"]),
                            Subtotal = Convert.ToDecimal(dr["Subtotal"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
