using Facturacion.Web.Core;
using Facturacion.Web.Models.DTOs.Articulos;
using Facturacion.Web.Models.DTOs.Clientes;
using Facturacion.Web.Models.DTOs.Common;
using Facturacion.Web.Models.DTOs.Facturas;
using Facturacion.Web.Models.DTOs.Reportes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Facturacion.Web.Services
{
    /// <summary>
    /// Servicio para manejo completo de reportes y estadísticas
    /// Consume las APIs de reportes del backend para dashboard y análisis
    /// ⭐ ESENCIAL PARA DASHBOARD Y ANÁLISIS DE VENTAS
    /// </summary>
    public class ReporteService : IDisposable
    {
        private readonly ApiClient _apiClient;
        private readonly SessionManager _sessionManager;
        private readonly ApiInterceptor _interceptor;
        private bool _disposed = false;

        public ReporteService()
        {
            _apiClient = new ApiClient();
            _sessionManager = new SessionManager();
            _interceptor = ApiInterceptor.CrearInstancia();
        }

        #region Dashboard Principal

        /// <summary>
        /// Obtiene los datos completos del dashboard principal
        /// ⭐ MÉTODO PRINCIPAL PARA LA PÁGINA DE DASHBOARD
        /// </summary>
        /// <returns>Datos completos del dashboard con todas las métricas</returns>
        public async Task<RespuestaDto<DashboardDto>> ObtenerDashboardAsync()
        {
            try
            {
                VerificarAutenticacion();

                var respuesta = await _apiClient.GetAsync<DashboardDto>("reporte/dashboard");
                return _interceptor.InterceptarRespuesta(respuesta, "reporte/dashboard", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<DashboardDto>(ex, "reporte/dashboard", "GET");
            }
        }

        #endregion

        #region Reportes de Ventas

        /// <summary>
        /// Genera un reporte completo de ventas para un período específico
        /// ⭐ REPORTE PRINCIPAL DE VENTAS
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del período</param>
        /// <param name="fechaFin">Fecha de fin del período</param>
        /// <returns>Reporte completo de ventas</returns>
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

                // Validar que el rango no sea excesivamente grande
                var diasDiferencia = (fechaFin - fechaInicio).Days;
                if (diasDiferencia > 730) // Más de 2 años
                {
                    return RespuestaDto<ReporteVentasDto>.CrearError("Rango muy amplio",
                        "El rango de fechas no puede ser mayor a 2 años");
                }

                var endpoint = $"reporte/ventas?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";
                var respuesta = await _apiClient.GetAsync<ReporteVentasDto>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<ReporteVentasDto>(ex, "reporte/ventas", "GET");
            }
        }

        /// <summary>
        /// Obtiene las ventas del día actual
        /// </summary>
        /// <param name="fecha">Fecha específica (opcional, por defecto hoy)</param>
        /// <returns>Resumen de ventas del día</returns>
        public async Task<RespuestaDto<ResumenDiarioDto>> ObtenerVentasDelDiaAsync(DateTime? fecha = null)
        {
            try
            {
                VerificarAutenticacion();

                var fechaConsulta = fecha ?? DateTime.Now.Date;
                var endpoint = $"reporte/ventas-del-dia?fecha={fechaConsulta:yyyy-MM-dd}";

                var respuesta = await _apiClient.GetAsync<object>(endpoint);

                if (respuesta.Exito && respuesta.Resultado != null)
                {
                    // Convertir la respuesta al formato esperado
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(respuesta.Resultado.ToString());

                    var resumenDiario = new ResumenDiarioDto
                    {
                        Fecha = fechaConsulta,
                        TotalVentas = data?.TotalVentas ?? 0,
                        TotalVentasFormateado = data?.TotalVentasFormateado?.ToString() ?? "$0",
                        TotalFacturas = data?.TotalFacturas ?? 0
                    };

                    return RespuestaDto<ResumenDiarioDto>.CrearExitoso(
                        "Ventas del día obtenidas",
                        resumenDiario,
                        $"Ventas del día {fechaConsulta:dd/MM/yyyy}");
                }

                return RespuestaDto<ResumenDiarioDto>.CrearError("Error obteniendo ventas", "No se pudieron obtener las ventas del día");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<ResumenDiarioDto>(ex, "reporte/ventas-del-dia", "GET");
            }
        }

        /// <summary>
        /// Obtiene el promedio de ventas diarias en un período
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <returns>Promedio de ventas diarias</returns>
        public async Task<RespuestaDto<decimal>> ObtenerPromedioVentasDiariasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                VerificarAutenticacion();

                if (fechaFin < fechaInicio)
                {
                    return RespuestaDto<decimal>.CrearError("Rango inválido",
                        "La fecha final no puede ser anterior a la fecha inicial");
                }

                var endpoint = $"reporte/promedio-ventas-diarias?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";
                var respuesta = await _apiClient.GetAsync<object>(endpoint);

                if (respuesta.Exito && respuesta.Resultado != null)
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(respuesta.Resultado.ToString());
                    decimal promedio = data?.Promedio ?? 0;

                    return RespuestaDto<decimal>.CrearExitoso(
                        "Promedio calculado",
                        promedio,
                        $"Promedio de ventas diarias: {data?.PromedioFormateado}");
                }

                return RespuestaDto<decimal>.CrearError("Error calculando promedio", "No se pudo calcular el promedio de ventas");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<decimal>(ex, "reporte/promedio-ventas-diarias", "GET");
            }
        }

        #endregion

        #region Análisis de Productos

        /// <summary>
        /// Obtiene los artículos más vendidos en un período
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <param name="top">Número de artículos a obtener</param>
        /// <returns>Lista de artículos más vendidos</returns>
        public async Task<RespuestaDto<List<ArticuloVendidoDto>>> ObtenerArticulosMasVendidosAsync(
            DateTime fechaInicio,
            DateTime fechaFin,
            int top = 10)
        {
            try
            {
                VerificarAutenticacion();

                if (fechaFin < fechaInicio)
                {
                    return RespuestaDto<List<ArticuloVendidoDto>>.CrearError("Rango inválido",
                        "La fecha final no puede ser anterior a la fecha inicial");
                }

                if (top <= 0) top = 10;
                if (top > 50) top = 50; // Limitar para performance

                var endpoint = $"reporte/articulos-mas-vendidos?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}&top={top}";
                var respuesta = await _apiClient.GetAsync<List<ArticuloVendidoDto>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ArticuloVendidoDto>>(ex, "reporte/articulos-mas-vendidos", "GET");
            }
        }

        /// <summary>
        /// Obtiene los artículos con stock bajo
        /// </summary>
        /// <returns>Lista de artículos con stock bajo</returns>
        public async Task<RespuestaDto<List<ArticuloDto>>> ObtenerArticulosStockBajoAsync()
        {
            try
            {
                VerificarAutenticacion();

                var respuesta = await _apiClient.GetAsync<List<ArticuloDto>>("reporte/articulos-stock-bajo");
                return _interceptor.InterceptarRespuesta(respuesta, "reporte/articulos-stock-bajo", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ArticuloDto>>(ex, "reporte/articulos-stock-bajo", "GET");
            }
        }

        #endregion

        #region Análisis de Clientes

        /// <summary>
        /// Obtiene los clientes frecuentes (que más compran) en un período
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <param name="top">Número de clientes a obtener</param>
        /// <returns>Lista de clientes frecuentes</returns>
        public async Task<RespuestaDto<List<ClienteFrecuenteDto>>> ObtenerClientesFrecuentesAsync(
            DateTime fechaInicio,
            DateTime fechaFin,
            int top = 10)
        {
            try
            {
                VerificarAutenticacion();

                if (fechaFin < fechaInicio)
                {
                    return RespuestaDto<List<ClienteFrecuenteDto>>.CrearError("Rango inválido",
                        "La fecha final no puede ser anterior a la fecha inicial");
                }

                if (top <= 0) top = 10;
                if (top > 50) top = 50; // Limitar para performance

                var endpoint = $"reporte/clientes-frecuentes?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}&top={top}";
                var respuesta = await _apiClient.GetAsync<List<ClienteFrecuenteDto>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ClienteFrecuenteDto>>(ex, "reporte/clientes-frecuentes", "GET");
            }
        }

        #endregion

        #region Análisis por Categorías y Tiempo

        /// <summary>
        /// Obtiene las ventas por categoría de artículo en un período
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <returns>Ventas agrupadas por categoría</returns>
        public async Task<RespuestaDto<Dictionary<string, decimal>>> ObtenerVentasPorCategoriaAsync(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            try
            {
                VerificarAutenticacion();

                if (fechaFin < fechaInicio)
                {
                    return RespuestaDto<Dictionary<string, decimal>>.CrearError("Rango inválido",
                        "La fecha final no puede ser anterior a la fecha inicial");
                }

                var endpoint = $"reporte/ventas-por-categoria?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";
                var respuesta = await _apiClient.GetAsync<Dictionary<string, decimal>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<Dictionary<string, decimal>>(ex, "reporte/ventas-por-categoria", "GET");
            }
        }

        /// <summary>
        /// Obtiene las ventas por mes para un año específico
        /// </summary>
        /// <param name="año">Año a consultar</param>
        /// <returns>Ventas agrupadas por mes</returns>
        public async Task<RespuestaDto<Dictionary<string, decimal>>> ObtenerVentasPorMesAsync(int año)
        {
            try
            {
                VerificarAutenticacion();

                if (año < 2000 || año > DateTime.Now.Year + 1)
                {
                    return RespuestaDto<Dictionary<string, decimal>>.CrearError("Año inválido",
                        "El año debe estar entre 2000 y el año siguiente al actual");
                }

                var endpoint = $"reporte/ventas-por-mes/{año}";
                var respuesta = await _apiClient.GetAsync<Dictionary<string, decimal>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<Dictionary<string, decimal>>(ex, $"reporte/ventas-por-mes/{año}", "GET");
            }
        }

        /// <summary>
        /// Obtiene la cantidad de facturas por estado
        /// </summary>
        /// <returns>Facturas agrupadas por estado</returns>
        public async Task<RespuestaDto<Dictionary<string, int>>> ObtenerFacturasPorEstadoAsync()
        {
            try
            {
                VerificarAutenticacion();

                var respuesta = await _apiClient.GetAsync<Dictionary<string, int>>("reporte/facturas-por-estado");
                return _interceptor.InterceptarRespuesta(respuesta, "reporte/facturas-por-estado", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<Dictionary<string, int>>(ex, "reporte/facturas-por-estado", "GET");
            }
        }

        #endregion

        #region Métodos de Análisis Personalizado

        /// <summary>
        /// Genera un reporte personalizado con múltiples métricas
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <param name="incluirArticulos">Incluir análisis de artículos</param>
        /// <param name="incluirClientes">Incluir análisis de clientes</param>
        /// <param name="incluirCategorias">Incluir análisis por categorías</param>
        /// <returns>Reporte personalizado</returns>
        public async Task<RespuestaDto<object>> GenerarReportePersonalizadoAsync(
            DateTime fechaInicio,
            DateTime fechaFin,
            bool incluirArticulos = true,
            bool incluirClientes = true,
            bool incluirCategorias = true)
        {
            try
            {
                VerificarAutenticacion();

                if (fechaFin < fechaInicio)
                {
                    return RespuestaDto<object>.CrearError("Rango inválido",
                        "La fecha final no puede ser anterior a la fecha inicial");
                }

                var reportePersonalizado = new
                {
                    Periodo = new { FechaInicio = fechaInicio, FechaFin = fechaFin },
                    ReporteVentas = (object)null,
                    ArticulosMasVendidos = (object)null,
                    ClientesFrecuentes = (object)null,
                    VentasPorCategoria = (object)null,
                    FechaGeneracion = DateTime.Now
                };

                // Obtener reporte base de ventas
                var ventasTask = GenerarReporteVentasAsync(fechaInicio, fechaFin);

                // Crear tareas para los reportes opcionales
                var articulosTask = incluirArticulos ?
                    ObtenerArticulosMasVendidosAsync(fechaInicio, fechaFin, 10) :
                    Task.FromResult(RespuestaDto<List<ArticuloVendidoDto>>.CrearExitoso("No incluido", new List<ArticuloVendidoDto>()));

                var clientesTask = incluirClientes ?
                    ObtenerClientesFrecuentesAsync(fechaInicio, fechaFin, 10) :
                    Task.FromResult(RespuestaDto<List<ClienteFrecuenteDto>>.CrearExitoso("No incluido", new List<ClienteFrecuenteDto>()));

                var categoriasTask = incluirCategorias ?
                    ObtenerVentasPorCategoriaAsync(fechaInicio, fechaFin) :
                    Task.FromResult(RespuestaDto<Dictionary<string, decimal>>.CrearExitoso("No incluido", new Dictionary<string, decimal>()));

                // Esperar a que se completen todas las tareas
                await Task.WhenAll(ventasTask, articulosTask, clientesTask, categoriasTask);

                // Construir el reporte personalizado
                var resultado = new
                {
                    Periodo = new { FechaInicio = fechaInicio, FechaFin = fechaFin },
                    ReporteVentas = ventasTask.Result.Exito ? ventasTask.Result.Resultado : null,
                    ArticulosMasVendidos = incluirArticulos && articulosTask.Result.Exito ? articulosTask.Result.Resultado : null,
                    ClientesFrecuentes = incluirClientes && clientesTask.Result.Exito ? clientesTask.Result.Resultado : null,
                    VentasPorCategoria = incluirCategorias && categoriasTask.Result.Exito ? categoriasTask.Result.Resultado : null,
                    FechaGeneracion = DateTime.Now,
                    Errores = new List<string>()
                };

                // Agregar errores si los hay
                var errores = new List<string>();
                if (!ventasTask.Result.Exito) errores.Add($"Ventas: {ventasTask.Result.Mensaje}");
                if (incluirArticulos && !articulosTask.Result.Exito) errores.Add($"Artículos: {articulosTask.Result.Mensaje}");
                if (incluirClientes && !clientesTask.Result.Exito) errores.Add($"Clientes: {clientesTask.Result.Mensaje}");
                if (incluirCategorias && !categoriasTask.Result.Exito) errores.Add($"Categorías: {categoriasTask.Result.Mensaje}");

                if (errores.Count > 0)
                {
                    return RespuestaDto<object>.CrearError("Reporte con errores",
                        $"Se generó el reporte pero con algunos errores: {string.Join(", ", errores)}");
                }

                return RespuestaDto<object>.CrearExitoso(
                    "Reporte personalizado generado",
                    resultado,
                    $"Reporte completo para el período {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<object>(ex, "reporte/personalizado", "GET");
            }
        }

        /// <summary>
        /// Obtiene métricas rápidas para mostrar en widgets
        /// </summary>
        /// <returns>Métricas básicas del sistema</returns>
        public async Task<RespuestaDto<object>> ObtenerMetricasRapidasAsync()
        {
            try
            {
                VerificarAutenticacion();

                var hoy = DateTime.Now.Date;
                var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

                // Obtener métricas en paralelo para mejor performance
                var ventasHoyTask = ObtenerVentasDelDiaAsync(hoy);
                var articulosStockBajoTask = ObtenerArticulosStockBajoAsync();
                var facturasPorEstadoTask = ObtenerFacturasPorEstadoAsync();

                await Task.WhenAll(ventasHoyTask, articulosStockBajoTask, facturasPorEstadoTask);

                var metricas = new
                {
                    VentasHoy = ventasHoyTask.Result.Exito ? ventasHoyTask.Result.Resultado : new ResumenDiarioDto(),
                    ArticulosStockBajo = articulosStockBajoTask.Result.Exito ?
                        articulosStockBajoTask.Result.Resultado?.Count ?? 0 : 0,
                    EstadosFacturas = facturasPorEstadoTask.Result.Exito ?
                        facturasPorEstadoTask.Result.Resultado : new Dictionary<string, int>(),
                    FechaActualizacion = DateTime.Now
                };

                return RespuestaDto<object>.CrearExitoso(
                    "Métricas rápidas obtenidas",
                    metricas,
                    "Métricas actualizadas del sistema");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<object>(ex, "reporte/metricas-rapidas", "GET");
            }
        }

        #endregion

        #region Métodos de Utilidades

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

        /// <summary>
        /// Valida un rango de fechas
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <param name="maximoDias">Máximo de días permitidos</param>
        /// <returns>Resultado de validación</returns>
        private ValidoDto ValidarRangoFechas(DateTime fechaInicio, DateTime fechaFin, int maximoDias = 365)
        {
            if (fechaFin < fechaInicio)
            {
                return ValidoDto.Invalido("La fecha final no puede ser anterior a la fecha inicial");
            }

            var diasDiferencia = (fechaFin - fechaInicio).Days;
            if (diasDiferencia > maximoDias)
            {
                return ValidoDto.Invalido($"El rango de fechas no puede ser mayor a {maximoDias} días");
            }

            if (fechaInicio > DateTime.Now.Date)
            {
                return ValidoDto.Invalido("La fecha de inicio no puede ser futura");
            }

            return ValidoDto.Valido("Rango de fechas válido");
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
                    System.Diagnostics.Debug.WriteLine($"Error liberando recursos de ReporteService: {ex.Message}");
                }
            }
        }

        #endregion

        #region Métodos Estáticos de Utilidad

        /// <summary>
        /// Crea una instancia del servicio de reportes
        /// </summary>
        /// <returns>Nueva instancia de ReporteService</returns>
        public static ReporteService CrearInstancia()
        {
            return new ReporteService();
        }

        #endregion
    }
}