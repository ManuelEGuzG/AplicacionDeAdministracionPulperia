using System;
using System.Collections.Generic;
using System.Linq;
using AplicacionDeAdministracionPulperia.DA;
using AplicacionDeAdministracionPulperia.Model;

namespace AplicacionDeAdministracionPulperia.BL
{
    public class VentaBL
    {
        private readonly VentaDAO _dao = new VentaDAO();
        private readonly DetalleVentaDAO _detalleDao = new DetalleVentaDAO();
        private readonly ProductoDAO _productoDao = new ProductoDAO();

        #region Métodos de Creación

        public int Crear(Venta venta, List<DetalleVenta> detalles)
        {
            // Validaciones de negocio
            ValidarVenta(venta, detalles);

            // Verificar disponibilidad de stock para todos los productos
            foreach (var detalle in detalles)
            {
                var producto = _productoDao.ObtenerPorId(detalle.IdProducto);
                if (producto == null)
                    throw new Exception($"El producto con ID {detalle.IdProducto} no existe.");

                if (producto.Existencias < detalle.Cantidad)
                    throw new Exception($"Stock insuficiente para '{producto.Nombre}'. " +
                                      $"Disponible: {producto.Existencias}, Solicitado: {detalle.Cantidad}");
            }

            // Calcular el total de la venta
            decimal totalCalculado = detalles.Sum(d => d.Subtotal);

            // Validar que el total coincida (tolerancia de 0.01 para redondeos)
            if (Math.Abs(venta.Total - totalCalculado) > 0.01m)
                throw new Exception($"El total de la venta no coincide. Esperado: {totalCalculado:C}, Recibido: {venta.Total:C}");

            // Generar número de recibo si no existe
            if (string.IsNullOrWhiteSpace(venta.NumeroRecibo))
            {
                venta.NumeroRecibo = GenerarNumeroRecibo();
            }

            // Verificar que el número de recibo no exista
            var ventaExistente = _dao.ObtenerPorNumeroRecibo(venta.NumeroRecibo);
            if (ventaExistente != null)
                throw new Exception($"Ya existe una venta con el número de recibo '{venta.NumeroRecibo}'.");

            // Insertar la venta
            int idVenta = _dao.Insertar(venta);

            // Insertar los detalles
            foreach (var detalle in detalles)
            {
                detalle.IdVenta = idVenta;
                _detalleDao.Insertar(detalle);
            }

            return idVenta;
        }

        #endregion

        #region Métodos de Consulta

        public Venta ObtenerPorId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la venta es inválido.", nameof(id));

            var venta = _dao.ObtenerPorId(id);
            if (venta == null)
                throw new Exception($"No se encontró la venta con ID {id}.");

            return venta;
        }

        public Venta ObtenerPorNumeroRecibo(string numeroRecibo)
        {
            if (string.IsNullOrWhiteSpace(numeroRecibo))
                throw new ArgumentException("El número de recibo no puede estar vacío.", nameof(numeroRecibo));

            return _dao.ObtenerPorNumeroRecibo(numeroRecibo);
        }

        public List<Venta> Listar()
        {
            return _dao.ObtenerTodas();
        }

        public List<VentaDetallada> ListarVentasDetalladas(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            return _dao.ObtenerVentasDetalladas(fechaInicio, fechaFin);
        }

        public VentaDetallada ObtenerVentaDetallada(int idVenta)
        {
            if (idVenta <= 0)
                throw new ArgumentException("El ID de la venta es inválido.", nameof(idVenta));

            var ventasDetalladas = _dao.ObtenerVentasDetalladas();
            var venta = ventasDetalladas.Find(v => v.IdVenta == idVenta);

            if (venta == null)
                throw new Exception($"No se encontró la venta con ID {idVenta}.");

            return venta;
        }

        #endregion

        #region Métodos de Análisis

        public ResumenVentasPeriodo ObtenerResumenPeriodo(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var ventas = _dao.ObtenerTodas();

            // Filtrar por fechas si se proporcionan
            if (fechaInicio.HasValue)
                ventas = ventas.Where(v => v.Fecha >= fechaInicio.Value).ToList();

            if (fechaFin.HasValue)
                ventas = ventas.Where(v => v.Fecha <= fechaFin.Value).ToList();

            return new ResumenVentasPeriodo
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                NumeroVentas = ventas.Count,
                TotalVentas = ventas.Sum(v => v.Total),
                VentaPromedio = ventas.Count > 0 ? ventas.Average(v => v.Total) : 0,
                VentaMinima = ventas.Count > 0 ? ventas.Min(v => v.Total) : 0,
                VentaMaxima = ventas.Count > 0 ? ventas.Max(v => v.Total) : 0
            };
        }

        public Dictionary<string, decimal> ObtenerVentasPorTipoPago(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var ventas = _dao.ObtenerTodas();

            if (fechaInicio.HasValue)
                ventas = ventas.Where(v => v.Fecha >= fechaInicio.Value).ToList();

            if (fechaFin.HasValue)
                ventas = ventas.Where(v => v.Fecha <= fechaFin.Value).ToList();

            return ventas
                .GroupBy(v => v.TipoPago)
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Total));
        }

        public Dictionary<string, int> ObtenerVentasPorCajero(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            var ventas = _dao.ObtenerTodas();

            if (fechaInicio.HasValue)
                ventas = ventas.Where(v => v.Fecha >= fechaInicio.Value).ToList();

            if (fechaFin.HasValue)
                ventas = ventas.Where(v => v.Fecha <= fechaFin.Value).ToList();

            return ventas
                .GroupBy(v => v.Cajero)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public ResumenVentasPeriodo ObtenerResumenHoy()
        {
            var hoy = DateTime.Today;
            var manana = hoy.AddDays(1);
            return ObtenerResumenPeriodo(hoy, manana);
        }

        public ResumenVentasPeriodo ObtenerResumenMesActual()
        {
            var hoy = DateTime.Today;
            var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
            return ObtenerResumenPeriodo(primerDiaMes, ultimoDiaMes);
        }

        #endregion

        #region Estadísticas

        public int ContarTotal()
        {
            return _dao.ContarTotal();
        }

        #endregion

        #region Validaciones Privadas

        private void ValidarVenta(Venta venta, List<DetalleVenta> detalles)
        {
            if (venta == null)
                throw new ArgumentNullException(nameof(venta), "La venta no puede ser nula.");

            if (detalles == null || detalles.Count == 0)
                throw new Exception("La venta debe contener al menos un producto.");

            if (venta.Total <= 0)
                throw new Exception("El total de la venta debe ser mayor a cero.");

            if (string.IsNullOrWhiteSpace(venta.TipoPago))
                throw new Exception("El tipo de pago es obligatorio.");

            if (string.IsNullOrWhiteSpace(venta.Cajero))
                throw new Exception("El nombre del cajero es obligatorio.");

            // Validar los tipos de pago permitidos
            var tiposPagoPermitidos = new[] { "Efectivo", "Tarjeta", "Transferencia", "Sinpe", "Mixto" };
            if (!tiposPagoPermitidos.Contains(venta.TipoPago))
                throw new Exception($"Tipo de pago no válido. Use: {string.Join(", ", tiposPagoPermitidos)}");

            // Validar cada detalle
            foreach (var detalle in detalles)
            {
                if (detalle.Cantidad <= 0)
                    throw new Exception("La cantidad de cada producto debe ser mayor a cero.");

                if (detalle.PrecioUnitario <= 0)
                    throw new Exception("El precio unitario debe ser mayor a cero.");

                if (detalle.Subtotal <= 0)
                    throw new Exception("El subtotal debe ser mayor a cero.");

                // Validar que el subtotal coincida con cantidad * precio
                decimal subtotalCalculado = detalle.Cantidad * detalle.PrecioUnitario;
                if (Math.Abs(detalle.Subtotal - subtotalCalculado) > 0.01m)
                    throw new Exception($"El subtotal no coincide para el producto ID {detalle.IdProducto}. " +
                                      $"Esperado: {subtotalCalculado:C}, Recibido: {detalle.Subtotal:C}");
            }
        }

        private string GenerarNumeroRecibo()
        {
            // Formato: YYYYMMDD-HHMMSS-RANDOM
            return $"{DateTime.Now:yyyyMMdd-HHmmss}-{new Random().Next(1000, 9999)}";
        }

        #endregion
    }
}