using System;
using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class ClienteBL
    {
        private readonly ClienteDAO _dao = new ClienteDAO();

        #region Métodos de Consulta

        public List<Cliente> Listar()
        {
            return _dao.ObtenerTodos();
        }

        public List<Cliente> ListarConFiltros(ClienteFiltro filtro)
        {
            if (filtro == null)
                throw new ArgumentNullException(nameof(filtro), "El filtro no puede ser nulo.");

            return _dao.ObtenerConFiltros(filtro);
        }

        public List<ClienteDetalle> ListarConDetalles()
        {
            return _dao.ObtenerClientesConDetalles();
        }

        public ClienteDetalle ObtenerDetalleCliente(int idCliente)
        {
            if (idCliente <= 0)
                throw new ArgumentException("El ID del cliente es inválido.", nameof(idCliente));

            var detalle = _dao.ObtenerDetalleCliente(idCliente);
            if (detalle == null)
                throw new Exception($"No se encontró el cliente con ID {idCliente}.");

            return detalle;
        }

        public Cliente ObtenerPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del cliente debe ser mayor a cero.", nameof(id));

            var cliente = _dao.ObtenerPorId(id);
            if (cliente == null)
                throw new Exception($"No se encontró el cliente con ID {id}.");

            return cliente;
        }

        #endregion

        #region Métodos de Modificación

        public bool Crear(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente), "El cliente no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new Exception("El nombre del cliente es obligatorio.");

            // El teléfono es opcional según la base de datos
            return _dao.Insertar(cliente);
        }

        public bool Actualizar(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente), "El cliente no puede ser nulo.");

            if (cliente.IdCliente <= 0)
                throw new ArgumentException("El ID del cliente es inválido.", nameof(cliente.IdCliente));

            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new Exception("El nombre del cliente es obligatorio.");

            return _dao.Actualizar(cliente);
        }

        public bool Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del cliente es inválido.", nameof(id));

            // Verificar que el cliente exista
            var cliente = _dao.ObtenerPorId(id);
            if (cliente == null)
                throw new Exception($"No se encontró el cliente con ID {id}.");

            return _dao.Eliminar(id);
        }

        #endregion

        #region Estadísticas

        public int ContarTotal()
        {
            return _dao.ContarTotal();
        }

        public int ContarActivos()
        {
            return _dao.ContarActivos();
        }

        public List<Cliente> ObtenerClientesVIP(decimal montoMinimo = 10000)
        {
            var filtro = new ClienteFiltro
            {
                MontoTotalComprasMin = montoMinimo,
                OrdenarPor = "TotalCompras",
                Descendente = true
            };

            return _dao.ObtenerConFiltros(filtro);
        }

        public List<Cliente> ObtenerClientesInactivos(int diasSinComprar = 90)
        {
            var fechaLimite = DateTime.Now.AddDays(-diasSinComprar);
            var todosLosClientes = _dao.ObtenerClientesConDetalles();

            var clientesInactivos = new List<Cliente>();
            foreach (var clienteDetalle in todosLosClientes)
            {
                if (!clienteDetalle.UltimaCompra.HasValue || clienteDetalle.UltimaCompra.Value < fechaLimite)
                {
                    clientesInactivos.Add(new Cliente
                    {
                        IdCliente = clienteDetalle.IdCliente,
                        Nombre = clienteDetalle.Nombre,
                        Telefono = clienteDetalle.Telefono,
                        FechaCreacion = clienteDetalle.FechaCreacion
                    });
                }
            }

            return clientesInactivos;
        }

        #endregion
    }
}