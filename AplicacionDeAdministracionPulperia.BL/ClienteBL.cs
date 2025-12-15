using System;
using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class ClienteBL
    {
        private readonly ClienteDAO _dao = new ClienteDAO();

        public List<Cliente> Listar()
        {
            return _dao.ObtenerTodos();
        }

        public Cliente ObtenerPorId(int id)
        {
            return _dao.ObtenerPorId(id);
        }

        public bool Crear(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new Exception("El nombre del cliente es obligatorio.");

            // Teléfono es opcional según la base de datos, pero puedes validar si lo necesitas
            return _dao.Insertar(cliente);
        }

        public bool Actualizar(Cliente cliente)
        {
            if (cliente.IdCliente <= 0)
                throw new Exception("IdCliente inválido.");

            return _dao.Actualizar(cliente);
        }

        public bool Eliminar(int id)
        {
            if (id <= 0)
                throw new Exception("IdCliente inválido.");

            return _dao.Eliminar(id);
        }
    }
}
