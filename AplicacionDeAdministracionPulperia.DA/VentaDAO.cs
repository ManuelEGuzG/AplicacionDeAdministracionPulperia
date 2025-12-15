using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;
using Microsoft.Data.SqlClient;

namespace AplicacionDeAdministracionPulperia.DA
{
    public class VentaDAO : Conexion
    {
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
                    {
                        v = new Venta
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
                    {
                        lista.Add(new Venta
                        {
                            IdVenta = Convert.ToInt32(dr["IdVenta"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Total = Convert.ToDecimal(dr["Total"]),
                            TipoPago = dr["TipoPago"].ToString(),
                            Cajero = dr["Cajero"].ToString(),
                            NumeroRecibo = dr["NumeroRecibo"].ToString(),
                            IdCliente = dr["IdCliente"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["IdCliente"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
