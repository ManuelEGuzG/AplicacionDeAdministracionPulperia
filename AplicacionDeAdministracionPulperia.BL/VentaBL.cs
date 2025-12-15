using System;
using System.Collections.Generic;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class VentaBL
    {
        private readonly VentaDAO _dao = new VentaDAO();
        private readonly DetalleVentaDAO _detalleDao = new DetalleVentaDAO();

        public int Crear(Venta venta, List<DetalleVenta> detalles)
        {
            if (venta.Total <= 0)
                throw new Exception("El total de la venta debe ser mayor a cero.");

            if (detalles == null || detalles.Count == 0)
                throw new Exception("La venta debe contener al menos un producto.");

            int idVenta = _dao.Insertar(venta);

            foreach (var det in detalles)
            {
                det.IdVenta = idVenta;
                _detalleDao.Insertar(det);
            }

            return idVenta;
        }

        public Venta ObtenerPorId(int id)
        {
            return _dao.ObtenerPorId(id);
        }

        public List<Venta> Listar()
        {
            return _dao.ObtenerTodas();
        }
    }
}
