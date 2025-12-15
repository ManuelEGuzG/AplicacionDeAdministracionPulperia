using System;
using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class ProveedorBL
    {
        private readonly ProveedorDAO _dao = new ProveedorDAO();

        #region Métodos de Consulta

        public List<Proveedor> Listar()
        {
            return _dao.ObtenerTodos();
        }

        public List<ProveedorDetalle> ListarConDetalles()
        {
            return _dao.ObtenerProveedoresConDetalles();
        }

        public Proveedor ObtenerPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del proveedor debe ser mayor a cero.", nameof(id));

            var proveedor = _dao.ObtenerPorId(id);
            if (proveedor == null)
                throw new Exception($"No se encontró el proveedor con ID {id}.");

            return proveedor;
        }

        public ProveedorDetalle ObtenerDetalleProveedor(int idProveedor)
        {
            if (idProveedor <= 0)
                throw new ArgumentException("El ID del proveedor es inválido.", nameof(idProveedor));

            var proveedores = _dao.ObtenerProveedoresConDetalles();
            var detalle = proveedores.Find(p => p.IdProveedor == idProveedor);

            if (detalle == null)
                throw new Exception($"No se encontró el proveedor con ID {idProveedor}.");

            return detalle;
        }

        #endregion

        #region Métodos de Modificación

        public bool Crear(Proveedor proveedor)
        {
            if (proveedor == null)
                throw new ArgumentNullException(nameof(proveedor), "El proveedor no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
                throw new Exception("El nombre del proveedor es obligatorio.");

            // Correo, teléfono y dirección son opcionales
            return _dao.Insertar(proveedor);
        }

        public bool Actualizar(Proveedor proveedor)
        {
            if (proveedor == null)
                throw new ArgumentNullException(nameof(proveedor), "El proveedor no puede ser nulo.");

            if (proveedor.IdProveedor <= 0)
                throw new ArgumentException("El ID del proveedor es inválido.", nameof(proveedor.IdProveedor));

            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
                throw new Exception("El nombre del proveedor es obligatorio.");

            return _dao.Actualizar(proveedor);
        }

        public bool Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del proveedor es inválido.", nameof(id));

            // Verificar que el proveedor exista
            var proveedor = _dao.ObtenerPorId(id);
            if (proveedor == null)
                throw new Exception($"No se encontró el proveedor con ID {id}.");

            // Aquí podrías agregar validación para verificar si tiene productos asociados
            // y decidir si permitir la eliminación o no

            return _dao.Eliminar(id);
        }

        #endregion

        #region Estadísticas

        public int ContarTotal()
        {
            return _dao.ContarTotal();
        }

        public List<ProveedorDetalle> ObtenerProveedoresPrincipales(int top = 10)
        {
            if (top <= 0 || top > 100)
                throw new ArgumentException("El top debe estar entre 1 y 100.", nameof(top));

            var proveedores = _dao.ObtenerProveedoresConDetalles();

            // Ordenar por valor de inventario descendente
            proveedores.Sort((p1, p2) => p2.ValorInventario.CompareTo(p1.ValorInventario));

            // Tomar solo los top N
            if (proveedores.Count > top)
                proveedores.RemoveRange(top, proveedores.Count - top);

            return proveedores;
        }

        public List<ProveedorDetalle> ObtenerProveedoresSinProductos()
        {
            var proveedores = _dao.ObtenerProveedoresConDetalles();
            return proveedores.FindAll(p => p.NumeroProductos == 0);
        }

        #endregion
    }
}