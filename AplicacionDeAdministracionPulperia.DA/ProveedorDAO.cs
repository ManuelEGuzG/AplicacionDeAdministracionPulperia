using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.DA
{
    /// <summary>
    /// DAO para la gestión de proveedores
    /// </summary>
    public class ProveedorDAO : Conexion
    {
        #region CRUD Básico

        public List<Proveedor> ObtenerTodos()
        {
            List<Proveedor> lista = new List<Proveedor>();
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Proveedores ORDER BY Nombre", con);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        lista.Add(MapearProveedor(dr));
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
                        p = MapearProveedor(dr);
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
                cmd.Parameters.AddWithValue("@Correo", p.Correo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", p.Telefono ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Direccion", p.Direccion ?? (object)DBNull.Value);
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
                    SET Nombre = @Nombre, Correo = @Correo, Telefono = @Telefono, Direccion = @Direccion
                    WHERE IdProveedor = @IdProveedor", con);
                cmd.Parameters.AddWithValue("@IdProveedor", p.IdProveedor);
                cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@Correo", p.Correo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono", p.Telefono ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Direccion", p.Direccion ?? (object)DBNull.Value);
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

        #endregion

        #region Análisis

        public List<ProveedorDetalle> ObtenerProveedoresConDetalles()
        {
            List<ProveedorDetalle> lista = new List<ProveedorDetalle>();
            string query = @"
                SELECT p.*, COUNT(prod.IdProducto) AS NumeroProductos,
                       ISNULL(SUM(prod.Existencias), 0) AS TotalExistencias,
                       ISNULL(SUM(prod.Existencias * prod.PrecioCosto), 0) AS ValorInventario
                FROM Proveedores p
                LEFT JOIN Productos prod ON p.IdProveedor = prod.IdProveedor
                GROUP BY p.IdProveedor, p.Nombre, p.Correo, p.Telefono, p.Direccion, p.FechaCreacion
                ORDER BY ValorInventario DESC";

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ProveedorDetalle
                        {
                            IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                            Nombre = dr["Nombre"].ToString(),
                            Correo = dr["Correo"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]),
                            NumeroProductos = Convert.ToInt32(dr["NumeroProductos"]),
                            TotalExistencias = Convert.ToInt32(dr["TotalExistencias"]),
                            ValorInventario = Convert.ToDecimal(dr["ValorInventario"])
                        });
                    }
                }
            }
            return lista;
        }

        #endregion

        #region Estadísticas

        public int ContarTotal()
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Proveedores", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        #endregion

        #region Auxiliares

        private Proveedor MapearProveedor(SqlDataReader dr)
        {
            return new Proveedor
            {
                IdProveedor = Convert.ToInt32(dr["IdProveedor"]),
                Nombre = dr["Nombre"].ToString(),
                Correo = dr["Correo"].ToString(),
                Telefono = dr["Telefono"].ToString(),
                Direccion = dr["Direccion"].ToString(),
                FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
            };
        }

        #endregion
    }
}