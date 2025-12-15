using System;
using System.Collections.Generic;
using System.Linq;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class ProductoBL
    {
        private readonly ProductoDAO _dao = new ProductoDAO();
        private readonly MovimientoInventarioDAO _movimientoDao = new MovimientoInventarioDAO();

        #region Métodos de Consulta

        public List<Producto> Listar()
        {
            return _dao.ObtenerTodos();
        }

        public List<Producto> ListarConFiltros(ProductoFiltro filtro)
        {
            if (filtro == null)
                throw new ArgumentNullException(nameof(filtro), "El filtro no puede ser nulo.");

            return _dao.ObtenerConFiltros(filtro);
        }

        public List<ProductoDetalle> ListarConDetalles()
        {
            return _dao.ObtenerProductosConDetalles();
        }

        public List<InventarioConAlerta> ObtenerInventarioConAlertas()
        {
            return _dao.ObtenerInventarioConAlertas();
        }

        public List<Producto> ObtenerProductosBajoStock()
        {
            var filtro = new ProductoFiltro { BajoStock = true };
            return _dao.ObtenerConFiltros(filtro);
        }

        public Producto ObtenerPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del producto debe ser mayor a cero.", nameof(id));

            var producto = _dao.ObtenerPorId(id);
            if (producto == null)
                throw new Exception($"No se encontró el producto con ID {id}.");

            return producto;
        }

        public Producto ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código del producto no puede estar vacío.", nameof(codigo));

            return _dao.ObtenerPorCodigo(codigo);
        }

        #endregion

        #region Métodos de Modificación

        public bool Crear(Producto producto)
        {
            ValidarProducto(producto);

            // Verificar que el código no exista
            var existente = _dao.ObtenerPorCodigo(producto.Codigo);
            if (existente != null)
                throw new Exception($"Ya existe un producto con el código '{producto.Codigo}'.");

            return _dao.Insertar(producto);
        }

        public bool Actualizar(Producto producto)
        {
            if (producto.IdProducto <= 0)
                throw new ArgumentException("El ID del producto es inválido.", nameof(producto.IdProducto));

            ValidarProducto(producto);

            // Verificar que el código no esté duplicado (excepto para el mismo producto)
            var existente = _dao.ObtenerPorCodigo(producto.Codigo);
            if (existente != null && existente.IdProducto != producto.IdProducto)
                throw new Exception($"Ya existe otro producto con el código '{producto.Codigo}'.");

            return _dao.Actualizar(producto);
        }

        public bool Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del producto es inválido.", nameof(id));

            // Verificar que el producto exista
            var producto = _dao.ObtenerPorId(id);
            if (producto == null)
                throw new Exception($"No se encontró el producto con ID {id}.");

            return _dao.Eliminar(id);
        }

        #endregion

        #region Gestión de Inventario

        public bool AjustarInventario(int idProducto, int cantidad, string tipoMovimiento, string notas)
        {
            if (idProducto <= 0)
                throw new ArgumentException("El ID del producto es inválido.", nameof(idProducto));

            if (cantidad == 0)
                throw new ArgumentException("La cantidad no puede ser cero.", nameof(cantidad));

            if (string.IsNullOrWhiteSpace(tipoMovimiento))
                throw new ArgumentException("El tipo de movimiento es obligatorio.", nameof(tipoMovimiento));

            // Validar tipos de movimiento permitidos
            var tiposPermitidos = new[] { "Entrada", "Salida", "Ajuste", "Devolución" };
            if (!tiposPermitidos.Contains(tipoMovimiento))
                throw new ArgumentException($"Tipo de movimiento no válido. Use: {string.Join(", ", tiposPermitidos)}");

            // Verificar que el producto exista
            var producto = _dao.ObtenerPorId(idProducto);
            if (producto == null)
                throw new Exception($"No se encontró el producto con ID {idProducto}.");

            // Para salidas, verificar que haya suficiente stock
            if (tipoMovimiento == "Salida" && producto.Existencias + cantidad < 0)
                throw new Exception($"No hay suficiente stock. Existencias actuales: {producto.Existencias}");

            var movimiento = new MovimientoInventario
            {
                IdProducto = idProducto,
                Fecha = DateTime.Now,
                Cantidad = cantidad,
                TipoMovimiento = tipoMovimiento,
                Notas = notas ?? ""
            };

            return _movimientoDao.Insertar(movimiento);
        }

        public bool ActualizarPrecios(int idProducto, decimal? nuevoPrecioVenta, decimal? nuevoPrecioCosto)
        {
            if (idProducto <= 0)
                throw new ArgumentException("El ID del producto es inválido.", nameof(idProducto));

            if (!nuevoPrecioVenta.HasValue && !nuevoPrecioCosto.HasValue)
                throw new ArgumentException("Debe proporcionar al menos un precio a actualizar.");

            var producto = _dao.ObtenerPorId(idProducto);
            if (producto == null)
                throw new Exception($"No se encontró el producto con ID {idProducto}.");

            if (nuevoPrecioVenta.HasValue)
            {
                if (nuevoPrecioVenta.Value <= 0)
                    throw new ArgumentException("El precio de venta debe ser mayor a cero.");
                producto.PrecioVenta = nuevoPrecioVenta.Value;
            }

            if (nuevoPrecioCosto.HasValue)
            {
                if (nuevoPrecioCosto.Value <= 0)
                    throw new ArgumentException("El precio de costo debe ser mayor a cero.");
                producto.PrecioCosto = nuevoPrecioCosto.Value;
            }

            // Validar que el precio de venta sea mayor al precio de costo
            if (producto.PrecioVenta <= producto.PrecioCosto)
                throw new Exception("El precio de venta debe ser mayor al precio de costo para tener ganancia.");

            return _dao.Actualizar(producto);
        }

        #endregion

        #region Estadísticas

        public EstadisticasProducto ObtenerEstadisticas()
        {
            return new EstadisticasProducto
            {
                TotalProductos = _dao.ContarTotal(),
                ProductosBajoStock = _dao.ContarBajoStock(),
                ValorTotalInventario = _dao.ObtenerValorTotalInventario()
            };
        }

        #endregion

        #region Validaciones Privadas

        private void ValidarProducto(Producto producto)
        {
            if (producto == null)
                throw new ArgumentNullException(nameof(producto), "El producto no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(producto.Codigo))
                throw new Exception("El código del producto es obligatorio.");

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                throw new Exception("El nombre del producto es obligatorio.");

            if (producto.PrecioVenta <= 0)
                throw new Exception("El precio de venta debe ser mayor a cero.");

            if (producto.PrecioCosto <= 0)
                throw new Exception("El precio de costo debe ser mayor a cero.");

            if (producto.PrecioVenta <= producto.PrecioCosto)
                throw new Exception("El precio de venta debe ser mayor al precio de costo para tener ganancia.");

            if (producto.Existencias < 0)
                throw new Exception("Las existencias no pueden ser negativas.");

            if (producto.PuntoReorden < 0)
                throw new Exception("El punto de reorden no puede ser negativo.");
        }

        #endregion
    }
}