using System;
using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class ProductoBL
    {
        private readonly ProductoDAO _dao = new ProductoDAO();

        public List<Producto> Listar()
        {
            return _dao.ObtenerTodos();
        }

        public Producto ObtenerPorId(int id)
        {
            return _dao.ObtenerPorId(id);
        }

        public bool Crear(Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Codigo))
                throw new Exception("El código es obligatorio.");

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                throw new Exception("El nombre es obligatorio.");

            if (producto.PrecioVenta <= 0)
                throw new Exception("El precio de venta debe ser mayor a cero.");

            return _dao.Insertar(producto);
        }

        public bool Actualizar(Producto producto)
        {
            if (producto.IdProducto <= 0)
                throw new Exception("IdProducto inválido.");

            return _dao.Actualizar(producto);
        }

        public bool Eliminar(int id)
        {
            if (id <= 0)
                throw new Exception("IdProducto inválido.");

            return _dao.Eliminar(id);
        }
    }
}
