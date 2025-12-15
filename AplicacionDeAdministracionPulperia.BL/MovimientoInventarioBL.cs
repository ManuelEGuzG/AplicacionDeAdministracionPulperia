using System;
using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class MovimientoInventarioBL
    {
        private readonly MovimientoInventarioDAO _dao = new MovimientoInventarioDAO();

        /// <summary>
        /// Registra un nuevo movimiento de inventario
        /// </summary>
        /// <param name="movimiento">Movimiento a registrar</param>
        /// <returns>True si se registró correctamente</returns>
        public bool RegistrarMovimiento(MovimientoInventario movimiento)
        {
            if (movimiento == null)
                throw new ArgumentNullException(nameof(movimiento), "El movimiento no puede ser nulo.");

            if (movimiento.Cantidad == 0)
                throw new Exception("La cantidad del movimiento no puede ser cero.");

            if (string.IsNullOrWhiteSpace(movimiento.TipoMovimiento))
                throw new Exception("El tipo de movimiento es obligatorio.");

            // Establecer fecha si no está definida
            if (movimiento.Fecha == default(DateTime))
                movimiento.Fecha = DateTime.Now;

            return _dao.Insertar(movimiento);
        }

        /// <summary>
        /// Lista todos los movimientos de un producto específico
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <returns>Lista de movimientos ordenados por fecha descendente</returns>
        public List<MovimientoInventario> ListarPorProducto(int idProducto)
        {
            if (idProducto <= 0)
                throw new ArgumentException("El ID del producto es inválido.", nameof(idProducto));

            return _dao.ObtenerPorProducto(idProducto);
        }
    }
}