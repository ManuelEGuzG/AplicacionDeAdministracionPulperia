using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.DA
{
    /// <summary>
    /// DAO para la gestión de clientes con operaciones CRUD y análisis
    /// </summary>
    public class ClienteDAO : Conexion
    {
        #region Métodos Básicos CRUD

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>
        public List<Cliente> ObtenerTodos()
        {
            List<Cliente> lista = new List<Cliente>();

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Clientes ORDER BY Nombre", con);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(MapearCliente(dr));
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un cliente por su ID
        /// </summary>
        public Cliente ObtenerPorId(int id)
        {
            Cliente c = null;

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Clientes WHERE IdCliente = @id", con);
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        c = MapearCliente(dr);
                    }
                }
            }

            return c;
        }

        /// <summary>
        /// Inserta un nuevo cliente
        /// </summary>
        public bool Insertar(Cliente c)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Clientes (Nombre, Telefono)
                    VALUES (@Nombre, @Telefono)", con);

                cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
                cmd.Parameters.AddWithValue("@Telefono", c.Telefono ?? (object)DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Actualiza un cliente existente
        /// </summary>
        public bool Actualizar(Cliente c)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    UPDATE Clientes
                    SET Nombre = @Nombre,
                        Telefono = @Telefono
                    WHERE IdCliente = @IdCliente", con);

                cmd.Parameters.AddWithValue("@IdCliente", c.IdCliente);
                cmd.Parameters.AddWithValue("@Nombre", c.Nombre);
                cmd.Parameters.AddWithValue("@Telefono", c.Telefono ?? (object)DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Elimina un cliente
        /// </summary>
        public bool Eliminar(int id)
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Clientes WHERE IdCliente = @id", con);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion

        #region Métodos con Filtros

        /// <summary>
        /// Obtiene clientes aplicando filtros dinámicos
        /// </summary>
        public List<Cliente> ObtenerConFiltros(ClienteFiltro filtro)
        {
            List<Cliente> lista = new List<Cliente>();
            StringBuilder query = new StringBuilder(@"
                SELECT DISTINCT c.* 
                FROM Clientes c
                LEFT JOIN Ventas v ON c.IdCliente = v.IdCliente
                WHERE 1=1");

            List<SqlParameter> parametros = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(filtro.Nombre))
            {
                query.Append(" AND c.Nombre LIKE @Nombre");
                parametros.Add(new SqlParameter("@Nombre", $"%{filtro.Nombre}%"));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Telefono))
            {
                query.Append(" AND c.Telefono LIKE @Telefono");
                parametros.Add(new SqlParameter("@Telefono", $"%{filtro.Telefono}%"));
            }

            if (filtro.FechaCreacionDesde.HasValue)
            {
                query.Append(" AND c.FechaCreacion >= @FechaDesde");
                parametros.Add(new SqlParameter("@FechaDesde", filtro.FechaCreacionDesde.Value));
            }

            if (filtro.FechaCreacionHasta.HasValue)
            {
                query.Append(" AND c.FechaCreacion <= @FechaHasta");
                parametros.Add(new SqlParameter("@FechaHasta", filtro.FechaCreacionHasta.Value));
            }

            if (filtro.TieneCompras.HasValue)
            {
                if (filtro.TieneCompras.Value)
                    query.Append(" AND v.IdVenta IS NOT NULL");
                else
                    query.Append(" AND v.IdVenta IS NULL");
            }

            // Filtros de estadísticas requieren GROUP BY
            bool necesitaAgrupacion = filtro.MontoTotalComprasMin.HasValue ||
                                     filtro.MontoTotalComprasMax.HasValue ||
                                     filtro.NumeroComprasMin.HasValue ||
                                     filtro.NumeroComprasMax.HasValue;

            if (necesitaAgrupacion)
            {
                query = new StringBuilder(@"
                    SELECT c.* 
                    FROM Clientes c
                    LEFT JOIN Ventas v ON c.IdCliente = v.IdCliente
                    WHERE 1=1");

                // Repetir filtros básicos
                if (!string.IsNullOrWhiteSpace(filtro.Nombre))
                    query.Append(" AND c.Nombre LIKE @Nombre");
                if (!string.IsNullOrWhiteSpace(filtro.Telefono))
                    query.Append(" AND c.Telefono LIKE @Telefono");
                if (filtro.FechaCreacionDesde.HasValue)
                    query.Append(" AND c.FechaCreacion >= @FechaDesde");
                if (filtro.FechaCreacionHasta.HasValue)
                    query.Append(" AND c.FechaCreacion <= @FechaHasta");

                query.Append(" GROUP BY c.IdCliente, c.Nombre, c.Telefono, c.FechaCreacion HAVING 1=1");

                if (filtro.MontoTotalComprasMin.HasValue)
                {
                    query.Append(" AND ISNULL(SUM(v.Total), 0) >= @MontoMin");
                    parametros.Add(new SqlParameter("@MontoMin", filtro.MontoTotalComprasMin.Value));
                }

                if (filtro.MontoTotalComprasMax.HasValue)
                {
                    query.Append(" AND ISNULL(SUM(v.Total), 0) <= @MontoMax");
                    parametros.Add(new SqlParameter("@MontoMax", filtro.MontoTotalComprasMax.Value));
                }

                if (filtro.NumeroComprasMin.HasValue)
                {
                    query.Append(" AND COUNT(v.IdVenta) >= @NumMin");
                    parametros.Add(new SqlParameter("@NumMin", filtro.NumeroComprasMin.Value));
                }

                if (filtro.NumeroComprasMax.HasValue)
                {
                    query.Append(" AND COUNT(v.IdVenta) <= @NumMax");
                    parametros.Add(new SqlParameter("@NumMax", filtro.NumeroComprasMax.Value));
                }
            }

            // Ordenamiento
            if (!string.IsNullOrWhiteSpace(filtro.OrdenarPor))
            {
                string orden = filtro.Descendente ? "DESC" : "ASC";
                query.Append($" ORDER BY c.{filtro.OrdenarPor} {orden}");
            }
            else
            {
                query.Append(" ORDER BY c.Nombre ASC");
            }

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query.ToString(), con);
                cmd.Parameters.AddRange(parametros.ToArray());

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(MapearCliente(dr));
                    }
                }
            }

            return lista;
        }

        #endregion

        #region Métodos de Análisis

        /// <summary>
        /// Obtiene todos los clientes con estadísticas detalladas
        /// </summary>
        public List<ClienteDetalle> ObtenerClientesConDetalles()
        {
            List<ClienteDetalle> lista = new List<ClienteDetalle>();

            string query = @"
                SELECT 
                    c.IdCliente,
                    c.Nombre,
                    c.Telefono,
                    c.FechaCreacion,
                    COUNT(v.IdVenta) AS NumeroCompras,
                    ISNULL(SUM(v.Total), 0) AS TotalCompras,
                    CASE 
                        WHEN COUNT(v.IdVenta) > 0 THEN ISNULL(SUM(v.Total), 0) / COUNT(v.IdVenta)
                        ELSE 0
                    END AS PromedioCompra,
                    MAX(v.Fecha) AS UltimaCompra,
                    (SELECT TOP 1 p.Nombre 
                     FROM DetalleVenta dv
                     JOIN Productos p ON dv.IdProducto = p.IdProducto
                     JOIN Ventas v2 ON dv.IdVenta = v2.IdVenta
                     WHERE v2.IdCliente = c.IdCliente
                     GROUP BY p.Nombre
                     ORDER BY SUM(dv.Cantidad) DESC) AS ProductoMasComprado
                FROM Clientes c
                LEFT JOIN Ventas v ON c.IdCliente = v.IdCliente
                GROUP BY c.IdCliente, c.Nombre, c.Telefono, c.FechaCreacion
                ORDER BY TotalCompras DESC";

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ClienteDetalle
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Nombre = dr["Nombre"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"]),
                            NumeroCompras = Convert.ToInt32(dr["NumeroCompras"]),
                            TotalCompras = Convert.ToDecimal(dr["TotalCompras"]),
                            PromedioCompra = Convert.ToDecimal(dr["PromedioCompra"]),
                            UltimaCompra = dr["UltimaCompra"] == DBNull.Value ? null : Convert.ToDateTime(dr["UltimaCompra"]),
                            ProductoMasComprado = dr["ProductoMasComprado"]?.ToString() ?? "N/A"
                        });
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene el detalle de un cliente específico
        /// </summary>
        public ClienteDetalle ObtenerDetalleCliente(int idCliente)
        {
            var lista = ObtenerClientesConDetalles();
            return lista.Find(c => c.IdCliente == idCliente);
        }

        #endregion

        #region Métodos de Estadísticas

        /// <summary>
        /// Cuenta el total de clientes
        /// </summary>
        public int ContarTotal()
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Clientes", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Cuenta los clientes activos (con al menos una compra)
        /// </summary>
        public int ContarActivos()
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(DISTINCT IdCliente) 
                    FROM Ventas 
                    WHERE IdCliente IS NOT NULL", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        #endregion

        #region Métodos Auxiliares Privados

        /// <summary>
        /// Mapea un SqlDataReader a un objeto Cliente
        /// </summary>
        private Cliente MapearCliente(SqlDataReader dr)
        {
            return new Cliente
            {
                IdCliente = Convert.ToInt32(dr["IdCliente"]),
                Nombre = dr["Nombre"].ToString(),
                Telefono = dr["Telefono"].ToString(),
                FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
            };
        }

        #endregion
    }
}