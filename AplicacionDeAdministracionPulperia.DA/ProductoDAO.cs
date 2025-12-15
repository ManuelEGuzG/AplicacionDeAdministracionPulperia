using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;
using Microsoft.Data.SqlClient;

namespace AplicacionDeAdministracionPulperia.DA
{
    public class ProductoDAO : Conexion
    {
        public List<Producto> ObtenerTodos()
        {
            List<Producto> lista = new List<Producto>();

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Productos", con);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Producto
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Codigo = dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                            PrecioCosto = Convert.ToDecimal(dr["PrecioCosto"]),
                            Existencias = Convert.ToInt32(dr["Existencias"]),
                            PuntoReorden = Convert.ToInt32(dr["PuntoReorden"]),
                            IdProveedor = dr["IdProveedor"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["IdProveedor"]),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        });
                    }
                }
            }

            return lista;
        }

        public Producto ObtenerPorId(int id)
        {
            Producto p = null;

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE IdProducto = @id", con);
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        p = new Producto
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Codigo = dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                            PrecioCosto = Convert.ToDecimal(dr["PrecioCosto"]),
                            Existencias = Convert.ToInt32(dr["Existencias"]),
                            PuntoReorden = Convert.ToInt32(dr["PuntoReorden"]),
                            IdProveedor = dr["IdProveedor"] == DBNull.Value ? null : (int?)Convert.ToInt32(dr["IdProveedor"]),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        };
                    }
                }
            }

            return p;
        }

        public bool Insertar(Producto p)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Productos
                    (Codigo, Nombre, Descripcion, PrecioVenta, PrecioCosto, Existencias, PuntoReorden, IdProveedor)
                    VALUES
                    (@Codigo, @Nombre, @Descripcion, @PrecioVenta, @PrecioCosto, @Existencias, @PuntoReorden, @IdProveedor)", con);

                cmd.Parameters.AddWithValue("@Codigo", p.Codigo);
                cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", p.Descripcion);
                cmd.Parameters.AddWithValue("@PrecioVenta", p.PrecioVenta);
                cmd.Parameters.AddWithValue("@PrecioCosto", p.PrecioCosto);
                cmd.Parameters.AddWithValue("@Existencias", p.Existencias);
                cmd.Parameters.AddWithValue("@PuntoReorden", p.PuntoReorden);
                cmd.Parameters.AddWithValue("@IdProveedor", (object)p.IdProveedor ?? DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Actualizar(Producto p)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Productos
                    SET Codigo = @Codigo,
                        Nombre = @Nombre,
                        Descripcion = @Descripcion,
                        PrecioVenta = @PrecioVenta,
                        PrecioCosto = @PrecioCosto,
                        Existencias = @Existencias,
                        PuntoReorden = @PuntoReorden,
                        IdProveedor = @IdProveedor
                    WHERE IdProducto = @IdProducto", con);

                cmd.Parameters.AddWithValue("@IdProducto", p.IdProducto);
                cmd.Parameters.AddWithValue("@Codigo", p.Codigo);
                cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", p.Descripcion);
                cmd.Parameters.AddWithValue("@PrecioVenta", p.PrecioVenta);
                cmd.Parameters.AddWithValue("@PrecioCosto", p.PrecioCosto);
                cmd.Parameters.AddWithValue("@Existencias", p.Existencias);
                cmd.Parameters.AddWithValue("@PuntoReorden", p.PuntoReorden);
                cmd.Parameters.AddWithValue("@IdProveedor", (object)p.IdProveedor ?? DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Eliminar(int id)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Productos WHERE IdProducto = @id", con);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
