using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;
using Microsoft.Data.SqlClient;

namespace AplicacionDeAdministracionPulperia.DA
{
    public class ReportesDAO : Conexion
    {
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
    }
}
