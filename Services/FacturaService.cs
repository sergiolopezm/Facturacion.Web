using Facturacion.Web.Core;
using Facturacion.Web.Models.DTOs.Common;
using Facturacion.Web.Models.DTOs.Facturas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facturacion.Web.Services
{
    /// <summary>
    /// Servicio para manejo completo de facturas
    /// Consume las APIs de facturación del backend para CRUD, cálculos y reportes
    /// </summary>
    public class FacturaService : IDisposable
    {
        private readonly ApiClient _apiClient;
        private readonly SessionManager _sessionManager;
        private readonly ApiInterceptor _interceptor;
        private bool _disposed = false;

        public FacturaService()
        {
            _apiClient = new ApiClient();
            _sessionManager = new SessionManager();
            _interceptor = ApiInterceptor.CrearInstancia();
        }

        #region Métodos CRUD Básicos

        /// <summary>
        /// Obtiene todas las facturas del sistema
        /// </summary>
        /// <returns>Lista de facturas</returns>
        public async Task<RespuestaDto<List<FacturaDto>>> ObtenerTodasAsync()
        {
            try
            {
                VerificarAutenticacion();

                var respuesta = await _apiClient.GetAsync<List<FacturaDto>>("factura");
                return _interceptor.InterceptarRespuesta(respuesta, "factura", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<FacturaDto>>(ex, "factura", "GET");
            }
        }

        /// <summary>
        /// Obtiene una factura específica por su ID
        /// </summary>
        /// <param name="id">ID de la factura</param>
        /// <returns>Factura encontrada</returns>
        public async Task<RespuestaDto<FacturaDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<FacturaDto>.CrearError("ID inválido",
                        "El ID de la factura debe ser mayor a 0");
                }

                var respuesta = await _apiClient.GetAsync<FacturaDto>($"factura/{id}");
                return _interceptor.InterceptarRespuesta(respuesta, $"factura/{id}", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<FacturaDto>(ex, $"factura/{id}", "GET");
            }
        }

        /// <summary>
        /// Obtiene una factura por su número
        /// </summary>
        /// <param name="numeroFactura">Número de la factura</param>
        /// <returns>Factura encontrada</returns>
        public async Task<RespuestaDto<FacturaDto>> ObtenerPorNumeroAsync(string numeroFactura)
        {
            try
            {
                VerificarAutenticacion();

                if (string.IsNullOrWhiteSpace(numeroFactura))
                {
                    return RespuestaDto<FacturaDto>.CrearError("Número inválido",
                        "El número de factura es requerido");
                }

                var respuesta = await _apiClient.GetAsync<FacturaDto>($"factura/numero/{numeroFactura}");
                return _interceptor.InterceptarRespuesta(respuesta, $"factura/numero/{numeroFactura}", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<FacturaDto>(ex, $"factura/numero/{numeroFactura}", "GET");
            }
        }

        /// <summary>
        /// Crea una nueva factura en el sistema
        /// ⭐ MÉTODO PRINCIPAL PARA LA PRUEBA TÉCNICA
        /// </summary>
        /// <param name="crearFacturaDto">Datos de la factura a crear</param>
        /// <returns>Factura creada con su número generado</returns>
        public async Task<RespuestaDto<FacturaDto>> CrearAsync(CrearFacturaDto crearFacturaDto)
        {
            try
            {
                VerificarAutenticacion();

                // Validaciones previas
                var validacionPrevia = ValidarFacturaAntesDeSend(crearFacturaDto);
                if (!validacionPrevia.EsValido)
                {
                    return RespuestaDto<FacturaDto>.CrearError("Validación fallida", validacionPrevia.Detalle);
                }

                // Llamar a la API para crear la factura
                var respuesta = await _apiClient.PostAsync<FacturaDto>("factura", crearFacturaDto);
                respuesta = _interceptor.InterceptarRespuesta(respuesta, "factura", "POST");

                if (respuesta.Exito)
                {
                    System.Diagnostics.Debug.WriteLine($"Factura creada exitosamente. ID: {respuesta.Resultado?.Id}, Número: {respuesta.Resultado?.NumeroFactura}");
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando factura: {ex.Message}");
                return _interceptor.InterceptarError<FacturaDto>(ex, "factura", "POST");
            }
        }

        /// <summary>
        /// Anula una factura existente
        /// </summary>
        /// <param name="id">ID de la factura</param>
        /// <param name="motivo">Motivo de la anulación</param>
        /// <returns>Resultado de la operación</returns>
        public async Task<RespuestaDto<object>> AnularAsync(int id, string motivo)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<object>.CrearError("ID inválido",
                        "El ID de la factura debe ser mayor a 0");
                }

                if (string.IsNullOrWhiteSpace(motivo))
                {
                    return RespuestaDto<object>.CrearError("Motivo requerido",
                        "Debe proporcionar un motivo para anular la factura");
                }

                var respuesta = await _apiClient.PutAsync<object>($"factura/{id}/anular", motivo);
                return _interceptor.InterceptarRespuesta(respuesta, $"factura/{id}/anular", "PUT");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<object>(ex, $"factura/{id}/anular", "PUT");
            }
        }

        #endregion

        #region Métodos de Cálculo

        /// <summary>
        /// Calcula los totales de una factura basándose en sus detalles
        /// ⭐ MÉTODO CLAVE PARA CÁLCULOS AUTOMÁTICOS EN EL FORMULARIO
        /// </summary>
        /// <param name="detalles">Lista de detalles de la factura</param>
        /// <returns>Cálculo completo con totales formateados</returns>
        public async Task<RespuestaDto<FacturaCalculoDto>> CalcularTotalesAsync(List<CrearFacturaDetalleDto> detalles)
        {
            try
            {
                VerificarAutenticacion();

                if (detalles == null || detalles.Count == 0)
                {
                    return RespuestaDto<FacturaCalculoDto>.CrearError("Detalles requeridos",
                        "Debe proporcionar al menos un detalle para calcular los totales");
                }

                // Validar detalles antes de enviar
                foreach (var detalle in detalles)
                {
                    if (detalle.ArticuloId <= 0)
                    {
                        return RespuestaDto<FacturaCalculoDto>.CrearError("Artículo inválido",
                            "Todos los detalles deben tener un artículo válido");
                    }

                    if (detalle.Cantidad <= 0)
                    {
                        return RespuestaDto<FacturaCalculoDto>.CrearError("Cantidad inválida",
                            "La cantidad debe ser mayor a 0");
                    }

                    if (detalle.PrecioUnitario <= 0)
                    {
                        return RespuestaDto<FacturaCalculoDto>.CrearError("Precio inválido",
                            "El precio unitario debe ser mayor a 0");
                    }
                }

                var respuesta = await _apiClient.PostAsync<FacturaCalculoDto>("factura/calcular-totales", detalles);
                return _interceptor.InterceptarRespuesta(respuesta, "factura/calcular-totales", "POST");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<FacturaCalculoDto>(ex, "factura/calcular-totales", "POST");
            }
        }

        #endregion

        #region Métodos de Consulta y Filtrado

        /// <summary>
        /// Obtiene facturas de forma paginada con filtros opcionales
        /// ⭐ MÉTODO PARA LA GRILLA DE FACTURAS REQUERIDA
        /// </summary>
        /// <param name="pagina">Número de página</param>
        /// <param name="elementosPorPagina">Elementos por página</param>
        /// <param name="busqueda">Texto de búsqueda</param>
        /// <param name="fechaInicio">Fecha de inicio del filtro</param>
        /// <param name="fechaFin">Fecha de fin del filtro</param>
        /// <param name="estado">Estado de la factura</param>
        /// <param name="clienteId">ID del cliente</param>
        /// <returns>Lista paginada de facturas</returns>
        public async Task<RespuestaDto<PaginacionDto<FacturaResumenDto>>> ObtenerPaginadoAsync(
            int pagina = 1,
            int elementosPorPagina = 10,
            string busqueda = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string estado = null,
            int? clienteId = null)
        {
            try
            {
                VerificarAutenticacion();

                if (pagina <= 0) pagina = 1;
                if (elementosPorPagina <= 0) elementosPorPagina = 10;

                // Construir query string
                var queryParams = new List<string>
                {
                    $"pagina={pagina}",
                    $"elementosPorPagina={elementosPorPagina}"
                };

                if (!string.IsNullOrWhiteSpace(busqueda))
                    queryParams.Add($"busqueda={Uri.EscapeDataString(busqueda)}");

                if (fechaInicio.HasValue)
                    queryParams.Add($"fechaInicio={fechaInicio.Value:yyyy-MM-dd}");

                if (fechaFin.HasValue)
                    queryParams.Add($"fechaFin={fechaFin.Value:yyyy-MM-dd}");

                if (!string.IsNullOrWhiteSpace(estado))
                    queryParams.Add($"estado={Uri.EscapeDataString(estado)}");

                if (clienteId.HasValue)
                    queryParams.Add($"clienteId={clienteId.Value}");

                var queryString = string.Join("&", queryParams);
                var endpoint = $"factura/paginado?{queryString}";

                var respuesta = await _apiClient.GetAsync<PaginacionDto<FacturaResumenDto>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<PaginacionDto<FacturaResumenDto>>(ex, "factura/paginado", "GET");
            }
        }

        /// <summary>
        /// Obtiene las facturas de un cliente específico
        /// </summary>
        /// <param name="clienteId">ID del cliente</param>
        /// <returns>Lista de facturas del cliente</returns>
        public async Task<RespuestaDto<List<FacturaDto>>> ObtenerPorClienteAsync(int clienteId)
        {
            try
            {
                VerificarAutenticacion();

                if (clienteId <= 0)
                {
                    return RespuestaDto<List<FacturaDto>>.CrearError("Cliente inválido",
                        "El ID del cliente debe ser mayor a 0");
                }

                var respuesta = await _apiClient.GetAsync<List<FacturaDto>>($"factura/cliente/{clienteId}");
                return _interceptor.InterceptarRespuesta(respuesta, $"factura/cliente/{clienteId}", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<FacturaDto>>(ex, $"factura/cliente/{clienteId}", "GET");
            }
        }

        /// <summary>
        /// Obtiene facturas por rango de fechas
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <returns>Lista de facturas en el rango</returns>
        public async Task<RespuestaDto<List<FacturaDto>>> ObtenerPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                VerificarAutenticacion();

                if (fechaFin < fechaInicio)
                {
                    return RespuestaDto<List<FacturaDto>>.CrearError("Rango inválido",
                        "La fecha final no puede ser anterior a la fecha inicial");
                }

                var endpoint = $"factura/fecha?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";
                var respuesta = await _apiClient.GetAsync<List<FacturaDto>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<FacturaDto>>(ex, "factura/fecha", "GET");
            }
        }

        /// <summary>
        /// Obtiene los detalles de una factura específica
        /// </summary>
        /// <param name="facturaId">ID de la factura</param>
        /// <returns>Lista de detalles de la factura</returns>
        public async Task<RespuestaDto<List<FacturaDetalleDto>>> ObtenerDetallesAsync(int facturaId)
        {
            try
            {
                VerificarAutenticacion();

                if (facturaId <= 0)
                {
                    return RespuestaDto<List<FacturaDetalleDto>>.CrearError("Factura inválida",
                        "El ID de la factura debe ser mayor a 0");
                }

                var respuesta = await _apiClient.GetAsync<List<FacturaDetalleDto>>($"factura/{facturaId}/detalles");
                return _interceptor.InterceptarRespuesta(respuesta, $"factura/{facturaId}/detalles", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<FacturaDetalleDto>>(ex, $"factura/{facturaId}/detalles", "GET");
            }
        }

        #endregion

        #region Métodos de Reportes

        /// <summary>
        /// Genera un reporte de ventas para un período específico
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del reporte</param>
        /// <param name="fechaFin">Fecha de fin del reporte</param>
        /// <returns>Reporte de ventas</returns>
        public async Task<RespuestaDto<ReporteVentasDto>> GenerarReporteVentasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                VerificarAutenticacion();

                if (fechaFin < fechaInicio)
                {
                    return RespuestaDto<ReporteVentasDto>.CrearError("Rango inválido",
                        "La fecha final no puede ser anterior a la fecha inicial");
                }

                var endpoint = $"factura/reporte-ventas?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";
                var respuesta = await _apiClient.GetAsync<ReporteVentasDto>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<ReporteVentasDto>(ex, "factura/reporte-ventas", "GET");
            }
        }

        #endregion

        #region Métodos de Validación

        /// <summary>
        /// Valida una factura antes de enviarla al servidor
        /// </summary>
        /// <param name="crearFacturaDto">Datos de la factura</param>
        /// <returns>Resultado de validación</returns>
        private ValidoDto ValidarFacturaAntesDeSend(CrearFacturaDto crearFacturaDto)
        {
            try
            {
                if (crearFacturaDto == null)
                {
                    return ValidoDto.Invalido("Los datos de la factura son requeridos");
                }

                if (crearFacturaDto.ClienteId <= 0)
                {
                    return ValidoDto.Invalido("Debe seleccionar un cliente válido");
                }

                if (crearFacturaDto.Detalles == null || crearFacturaDto.Detalles.Count == 0)
                {
                    return ValidoDto.Invalido("Debe incluir al menos un artículo en la factura");
                }

                // Validar cada detalle
                for (int i = 0; i < crearFacturaDto.Detalles.Count; i++)
                {
                    var detalle = crearFacturaDto.Detalles[i];

                    if (detalle.ArticuloId <= 0)
                    {
                        return ValidoDto.Invalido($"El artículo en la línea {i + 1} no es válido");
                    }

                    if (detalle.Cantidad <= 0)
                    {
                        return ValidoDto.Invalido($"La cantidad en la línea {i + 1} debe ser mayor a 0");
                    }

                    if (detalle.PrecioUnitario <= 0)
                    {
                        return ValidoDto.Invalido($"El precio en la línea {i + 1} debe ser mayor a 0");
                    }
                }

                // Validar que no haya artículos duplicados
                var articulosUnicos = crearFacturaDto.Detalles.Select(d => d.ArticuloId).Distinct().Count();
                if (articulosUnicos != crearFacturaDto.Detalles.Count)
                {
                    return ValidoDto.Invalido("No se pueden incluir artículos duplicados en la misma factura");
                }

                return ValidoDto.Valido("Factura válida para procesar");
            }
            catch (Exception ex)
            {
                return ValidoDto.Invalido($"Error en validación: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica que el usuario esté autenticado
        /// </summary>
        private void VerificarAutenticacion()
        {
            if (!_sessionManager.EstaAutenticado())
            {
                throw new UnauthorizedAccessException("Debe estar autenticado para acceder a este servicio");
            }
        }

        #endregion

        #region Implementación IDisposable

        /// <summary>
        /// Libera los recursos utilizados
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos de manera segura
        /// </summary>
        /// <param name="disposing">True si se está liberando explícitamente</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                try
                {
                    _apiClient?.Dispose();
                    _disposed = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error liberando recursos de FacturaService: {ex.Message}");
                }
            }
        }

        #endregion

        #region Métodos Estáticos de Utilidad

        /// <summary>
        /// Crea una instancia del servicio de facturas
        /// </summary>
        /// <returns>Nueva instancia de FacturaService</returns>
        public static FacturaService CrearInstancia()
        {
            return new FacturaService();
        }

        #endregion
    }
}