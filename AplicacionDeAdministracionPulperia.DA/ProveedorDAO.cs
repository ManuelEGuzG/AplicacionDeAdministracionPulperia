using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;
using Microsoft.Data.SqlClient;

namespace AplicacionDeAdministracionPulperia.DA
{
    public class ProveedorDAO : Conexion
    {
        public List<Proveedor> ObtenerTodos()
        {
            List<Proveedor> lista = new List<Proveedor>();

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Proveedores", con);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Proveedor
                        {
                            IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                            Nombre = dr["Nombre"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        });
                    }
                }
            }

            return lista;
        }

        public Proveedor ObtenerPorId(int id)
        {
            Proveedor p = null;

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Proveedores WHERE IdProveedor = @id", con);
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        p = new Proveedor
                        {
                            IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                            Nombre = dr["Nombre"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        };
                    }
                }
            }

            return p;
        }

        public bool Insertar(Proveedor p)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Proveedores (Nombre, Correo, Telefono, Direccion)
                    VALUES (@Nombre, @Correo, @Telefono, @Direccion)", con);

                cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@Correo", p.Correo);
                cmd.Parameters.AddWithValue("@Telefono", p.Telefono);
                cmd.Parameters.AddWithValue("@Direccion", p.Direccion);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Actualizar(Proveedor p)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Proveedores
                    SET Nombre = @Nombre,
                        Correo = @Correo,
                        Telefono = @Telefono,
                        Direccion = @Direccion
                    WHERE IdProveedor = @IdProveedor", con);

                cmd.Parameters.AddWithValue("@IdProveedor", p.IdProveedor);
                cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@Correo", p.Correo);
                cmd.Parameters.AddWithValue("@Telefono", p.Telefono);
                cmd.Parameters.AddWithValue("@Direccion", p.Direccion);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Eliminar(int id)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Proveedores WHERE IdProveedor = @id", con);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

      
    }
}
