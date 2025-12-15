using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.DA
{
    /// <summary>
    /// DAO para la gestión de productos con todas las operaciones CRUD y consultas avanzadas
    /// </summary>
    public class ProductoDAO : Conexion
    {
        #region Métodos Básicos CRUD

        /// <summary>
        /// Obtiene todos los productos de la base de datos
        /// </summary>
        public List<Producto> ObtenerTodos()
        {
            List<Producto> lista = new List<Producto>();

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Productos ORDER BY Nombre", con);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(MapearProducto(dr));
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
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
                        p = MapearProducto(dr);
                    }
                }
            }

            return p;
        }

        /// <summary>
        /// Obtiene un producto por su código
        /// </summary>
        public Producto ObtenerPorCodigo(string codigo)
        {
            Producto p = null;

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE Codigo = @codigo", con);
                cmd.Parameters.AddWithValue("@codigo", codigo);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        p = MapearProducto(dr);
                    }
                }
            }

            return p;
        }

        /// <summary>
        /// Inserta un nuevo producto
        /// </summary>
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
                cmd.Parameters.AddWithValue("@Descripcion", p.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PrecioVenta", p.PrecioVenta);
                cmd.Parameters.AddWithValue("@PrecioCosto", p.PrecioCosto);
                cmd.Parameters.AddWithValue("@Existencias", p.Existencias);
                cmd.Parameters.AddWithValue("@PuntoReorden", p.PuntoReorden);
                cmd.Parameters.AddWithValue("@IdProveedor", (object)p.IdProveedor ?? DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
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
                cmd.Parameters.AddWithValue("@Descripcion", p.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PrecioVenta", p.PrecioVenta);
                cmd.Parameters.AddWithValue("@PrecioCosto", p.PrecioCosto);
                cmd.Parameters.AddWithValue("@Existencias", p.Existencias);
                cmd.Parameters.AddWithValue("@PuntoReorden", p.PuntoReorden);
                cmd.Parameters.AddWithValue("@IdProveedor", (object)p.IdProveedor ?? DBNull.Value);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Elimina un producto
        /// </summary>
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

        #endregion

        #region Métodos con Filtros

        /// <summary>
        /// Obtiene productos aplicando filtros dinámicos
        /// </summary>
        public List<Producto> ObtenerConFiltros(ProductoFiltro filtro)
        {
            List<Producto> lista = new List<Producto>();
            StringBuilder query = new StringBuilder("SELECT * FROM Productos WHERE 1=1");
            List<SqlParameter> parametros = new List<SqlParameter>();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(filtro.Codigo))
            {
                query.Append(" AND Codigo LIKE @Codigo");
                parametros.Add(new SqlParameter("@Codigo", $"%{filtro.Codigo}%"));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Nombre))
            {
                query.Append(" AND Nombre LIKE @Nombre");
                parametros.Add(new SqlParameter("@Nombre", $"%{filtro.Nombre}%"));
            }

            if (filtro.PrecioVentaMin.HasValue)
            {
                query.Append(" AND PrecioVenta >= @PrecioVentaMin");
                parametros.Add(new SqlParameter("@PrecioVentaMin", filtro.PrecioVentaMin.Value));
            }

            if (filtro.PrecioVentaMax.HasValue)
            {
                query.Append(" AND PrecioVenta <= @PrecioVentaMax");
                parametros.Add(new SqlParameter("@PrecioVentaMax", filtro.PrecioVentaMax.Value));
            }

            if (filtro.PrecioCostoMin.HasValue)
            {
                query.Append(" AND PrecioCosto >= @PrecioCostoMin");
                parametros.Add(new SqlParameter("@PrecioCostoMin", filtro.PrecioCostoMin.Value));
            }

            if (filtro.PrecioCostoMax.HasValue)
            {
                query.Append(" AND PrecioCosto <= @PrecioCostoMax");
                parametros.Add(new SqlParameter("@PrecioCostoMax", filtro.PrecioCostoMax.Value));
            }

            if (filtro.ExistenciasMin.HasValue)
            {
                query.Append(" AND Existencias >= @ExistenciasMin");
                parametros.Add(new SqlParameter("@ExistenciasMin", filtro.ExistenciasMin.Value));
            }

            if (filtro.ExistenciasMax.HasValue)
            {
                query.Append(" AND Existencias <= @ExistenciasMax");
                parametros.Add(new SqlParameter("@ExistenciasMax", filtro.ExistenciasMax.Value));
            }

            if (filtro.IdProveedor.HasValue)
            {
                query.Append(" AND IdProveedor = @IdProveedor");
                parametros.Add(new SqlParameter("@IdProveedor", filtro.IdProveedor.Value));
            }

            if (filtro.BajoStock.HasValue && filtro.BajoStock.Value)
            {
                query.Append(" AND Existencias <= PuntoReorden");
            }

            if (filtro.FechaCreacionDesde.HasValue)
            {
                query.Append(" AND FechaCreacion >= @FechaDesde");
                parametros.Add(new SqlParameter("@FechaDesde", filtro.FechaCreacionDesde.Value));
            }

            if (filtro.FechaCreacionHasta.HasValue)
            {
                query.Append(" AND FechaCreacion <= @FechaHasta");
                parametros.Add(new SqlParameter("@FechaHasta", filtro.FechaCreacionHasta.Value));
            }

            // Aplicar ordenamiento
            if (!string.IsNullOrWhiteSpace(filtro.OrdenarPor))
            {
                string orden = filtro.Descendente ? "DESC" : "ASC";
                query.Append($" ORDER BY {filtro.OrdenarPor} {orden}");
            }
            else
            {
                query.Append(" ORDER BY Nombre ASC");
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
                        lista.Add(MapearProducto(dr));
                    }
                }
            }

            return lista;
        }

        #endregion

        #region Métodos de Análisis

        /// <summary>
        /// Obtiene productos con información detallada incluyendo estadísticas de ventas
        /// </summary>
        public List<ProductoDetalle> ObtenerProductosConDetalles()
        {
            List<ProductoDetalle> lista = new List<ProductoDetalle>();

            string query = @"
                SELECT 
                    p.*,
                    ISNULL(prov.Nombre, 'Sin Proveedor') AS NombreProveedor,
                    ISNULL(SUM(dv.Cantidad), 0) AS TotalVendido,
                    ISNULL(SUM(dv.Subtotal), 0) AS MontoGenerado,
                    ((p.PrecioVenta - p.PrecioCosto) / p.PrecioVenta * 100) AS MargenGanancia,
                    (SELECT MAX(v.Fecha) FROM Ventas v 
                     JOIN DetalleVenta dv2 ON v.IdVenta = dv2.IdVenta 
                     WHERE dv2.IdProducto = p.IdProducto) AS UltimaVenta,
                    (SELECT MAX(mi.Fecha) FROM MovimientosInventario mi 
                     WHERE mi.IdProducto = p.IdProducto AND mi.TipoMovimiento = 'Entrada') AS UltimaEntrada
                FROM Productos p
                LEFT JOIN Proveedores prov ON p.IdProveedor = prov.IdProveedor
                LEFT JOIN DetalleVenta dv ON p.IdProducto = dv.IdProducto
                GROUP BY p.IdProducto, p.Codigo, p.Nombre, p.Descripcion, p.PrecioVenta, 
                         p.PrecioCosto, p.Existencias, p.PuntoReorden, p.IdProveedor, 
                         p.FechaCreacion, prov.Nombre";

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ProductoDetalle
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Codigo = dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            PrecioVenta = Convert.ToDecimal(dr["PrecioVenta"]),
                            PrecioCosto = Convert.ToDecimal(dr["PrecioCosto"]),
                            Existencias = Convert.ToInt32(dr["Existencias"]),
                            PuntoReorden = Convert.ToInt32(dr["PuntoReorden"]),
                            NombreProveedor = dr["NombreProveedor"].ToString(),
                            TotalVendido = Convert.ToInt32(dr["TotalVendido"]),
                            MontoGenerado = Convert.ToDecimal(dr["MontoGenerado"]),
                            MargenGanancia = Convert.ToDecimal(dr["MargenGanancia"]),
                            UltimaVenta = dr["UltimaVenta"] == DBNull.Value ? null : Convert.ToDateTime(dr["UltimaVenta"]),
                            UltimaEntrada = dr["UltimaEntrada"] == DBNull.Value ? null : Convert.ToDateTime(dr["UltimaEntrada"])
                        });
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene el inventario con alertas de stock
        /// </summary>
        public List<InventarioConAlerta> ObtenerInventarioConAlertas()
        {
            List<InventarioConAlerta> lista = new List<InventarioConAlerta>();

            string query = @"
                SELECT 
                    p.IdProducto,
                    p.Codigo,
                    p.Nombre,
                    p.Existencias,
                    p.PuntoReorden,
                    p.PrecioCosto,
                    CASE 
                        WHEN p.Existencias = 0 THEN 'Agotado'
                        WHEN p.Existencias <= (p.PuntoReorden * 0.5) THEN 'Crítico'
                        WHEN p.Existencias <= p.PuntoReorden THEN 'Bajo'
                        WHEN p.Existencias <= (p.PuntoReorden * 2) THEN 'Normal'
                        ELSE 'Alto'
                    END AS EstadoStock,
                    (p.Existencias * p.PrecioCosto) AS ValorInventario
                FROM Productos p
                ORDER BY 
                    CASE 
                        WHEN p.Existencias = 0 THEN 1
                        WHEN p.Existencias <= (p.PuntoReorden * 0.5) THEN 2
                        WHEN p.Existencias <= p.PuntoReorden THEN 3
                        ELSE 4
                    END";

            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new InventarioConAlerta
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Codigo = dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            Existencias = Convert.ToInt32(dr["Existencias"]),
                            PuntoReorden = Convert.ToInt32(dr["PuntoReorden"]),
                            EstadoStock = dr["EstadoStock"].ToString(),
                            ValorInventario = Convert.ToDecimal(dr["ValorInventario"]),
                            DiasInventarioEstimado = 0 // Se puede calcular con ventas promedio
                        });
                    }
                }
            }

            return lista;
        }

        #endregion

        #region Métodos de Estadísticas

        /// <summary>
        /// Cuenta el total de productos
        /// </summary>
        public int ContarTotal()
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Productos", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Cuenta los productos con stock bajo
        /// </summary>
        public int ContarBajoStock()
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Productos WHERE Existencias <= PuntoReorden", con);
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Calcula el valor total del inventario
        /// </summary>
        public decimal ObtenerValorTotalInventario()
        {
            using (SqlConnection con = ObtenerConexion())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(Existencias * PrecioCosto), 0) FROM Productos", con);
                return (decimal)cmd.ExecuteScalar();
            }
        }

        #endregion

        #region Métodos Auxiliares Privados

        /// <summary>
        /// Mapea un SqlDataReader a un objeto Producto
        /// </summary>
        private Producto MapearProducto(SqlDataReader dr)
        {
            return new Producto
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

        #endregion
    }
}