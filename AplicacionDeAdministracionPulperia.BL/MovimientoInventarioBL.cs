using System;
using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class MovimientoInventarioBL
    {
        private readonly MovimientoInventarioDAO _dao = new MovimientoInventarioDAO();

        public bool RegistrarMovimiento(MovimientoInventario mov)
        {
            if (mov.Cantidad == 0)
                throw new Exception("La cantidad del movimiento no puede ser cero.");

            return _dao.Insertar(mov);
        }

        public List<MovimientoInventario> ListarPorProducto(int idProducto)
        {
            return _dao.ObtenerPorProducto(idProducto);
        }
    }
}
