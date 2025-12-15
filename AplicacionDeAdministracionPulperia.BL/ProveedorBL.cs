using System;
using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class ProveedorBL
    {
        private readonly ProveedorDAO _dao = new ProveedorDAO();

        public List<Proveedor> Listar()
        {
            return _dao.ObtenerTodos();
        }

        public Proveedor ObtenerPorId(int id)
        {
            if (id <= 0)
                throw new Exception("IdProveedor inválido.");

            return _dao.ObtenerPorId(id);
        }

        public bool Crear(Proveedor proveedor)
        {
            if (proveedor == null)
                throw new Exception("Datos del proveedor no proporcionados.");

            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
                throw new Exception("El nombre es obligatorio.");

            return _dao.Insertar(proveedor);
        }

        public bool Actualizar(Proveedor proveedor)
        {
            if (proveedor.IdProveedor <= 0)
                throw new Exception("IdProveedor inválido.");

            return _dao.Actualizar(proveedor);
        }

        public bool Eliminar(int id)
        {
            if (id <= 0)
                throw new Exception("IdProveedor inválido.");

            return _dao.Eliminar(id);
        }
    }
}
