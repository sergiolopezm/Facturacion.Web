using Facturacion.Web.Core;
using Facturacion.Web.Models.DTOs.Articulos;
using Facturacion.Web.Models.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Facturacion.Web.Services
{
    /// <summary>
    /// Servicio para manejo completo de artículos
    /// Consume las APIs de artículos del backend para CRUD, búsquedas y stock
    /// ⭐ ESENCIAL PARA EL FORMULARIO DE FACTURAS - SELECCIÓN DE ARTÍCULOS
    /// </summary>
    public class ArticuloService : IDisposable
    {
        private readonly ApiClient _apiClient;
        private readonly SessionManager _sessionManager;
        private readonly ApiInterceptor _interceptor;
        private bool _disposed = false;

        public ArticuloService()
        {
            _apiClient = new ApiClient();
            _sessionManager = new SessionManager();
            _interceptor = ApiInterceptor.CrearInstancia();
        }

        #region Métodos CRUD Básicos

        /// <summary>
        /// Obtiene todos los artículos activos del sistema
        /// </summary>
        /// <returns>Lista completa de artículos</returns>
        public async Task<RespuestaDto<List<ArticuloDto>>> ObtenerTodosAsync()
        {
            try
            {
                VerificarAutenticacion();

                var respuesta = await _apiClient.GetAsync<List<ArticuloDto>>("articulo");
                return _interceptor.InterceptarRespuesta(respuesta, "articulo", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ArticuloDto>>(ex, "articulo", "GET");
            }
        }

        /// <summary>
        /// Obtiene un artículo específico por su ID
        /// </summary>
        /// <param name="id">ID del artículo</param>
        /// <returns>Artículo encontrado</returns>
        public async Task<RespuestaDto<ArticuloDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<ArticuloDto>.CrearError("ID inválido",
                        "El ID del artículo debe ser mayor a 0");
                }

                var respuesta = await _apiClient.GetAsync<ArticuloDto>($"articulo/{id}");
                return _interceptor.InterceptarRespuesta(respuesta, $"articulo/{id}", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<ArticuloDto>(ex, $"articulo/{id}", "GET");
            }
        }

        /// <summary>
        /// Busca un artículo por su código
        /// ⭐ MÉTODO FUNDAMENTAL PARA EL FORMULARIO DE FACTURAS
        /// </summary>
        /// <param name="codigo">Código del artículo</param>
        /// <returns>Artículo encontrado con stock y precio actual</returns>
        public async Task<RespuestaDto<ArticuloDto>> ObtenerPorCodigoAsync(string codigo)
        {
            try
            {
                VerificarAutenticacion();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    return RespuestaDto<ArticuloDto>.CrearError("Código requerido",
                        "El código del artículo es requerido");
                }

                // Limpiar el código (remover espacios, convertir a mayúsculas)
                codigo = LimpiarCodigoArticulo(codigo);

                var respuesta = await _apiClient.GetAsync<ArticuloDto>($"articulo/codigo/{codigo}");
                return _interceptor.InterceptarRespuesta(respuesta, $"articulo/codigo/{codigo}", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<ArticuloDto>(ex, $"articulo/codigo/{codigo}", "GET");
            }
        }

        /// <summary>
        /// Crea un nuevo artículo en el sistema
        /// </summary>
        /// <param name="articuloDto">Datos del artículo a crear</param>
        /// <returns>Artículo creado</returns>
        public async Task<RespuestaDto<ArticuloDto>> CrearAsync(ArticuloDto articuloDto)
        {
            try
            {
                VerificarAutenticacion();

                // Validaciones previas
                var validacionPrevia = ValidarArticuloAntesDeSend(articuloDto);
                if (!validacionPrevia.EsValido)
                {
                    return RespuestaDto<ArticuloDto>.CrearError("Validación fallida", validacionPrevia.Detalle);
                }

                // Limpiar y normalizar datos
                articuloDto = NormalizarDatosArticulo(articuloDto);

                var respuesta = await _apiClient.PostAsync<ArticuloDto>("articulo", articuloDto);
                respuesta = _interceptor.InterceptarRespuesta(respuesta, "articulo", "POST");

                if (respuesta.Exito)
                {
                    System.Diagnostics.Debug.WriteLine($"Artículo creado exitosamente: {articuloDto.Codigo} - {articuloDto.Nombre}");
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando artículo: {ex.Message}");
                return _interceptor.InterceptarError<ArticuloDto>(ex, "articulo", "POST");
            }
        }

        /// <summary>
        /// Actualiza un artículo existente
        /// </summary>
        /// <param name="id">ID del artículo</param>
        /// <param name="articuloDto">Datos actualizados del artículo</param>
        /// <returns>Artículo actualizado</returns>
        public async Task<RespuestaDto<ArticuloDto>> ActualizarAsync(int id, ArticuloDto articuloDto)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<ArticuloDto>.CrearError("ID inválido",
                        "El ID del artículo debe ser mayor a 0");
                }

                // Validaciones previas
                var validacionPrevia = ValidarArticuloAntesDeSend(articuloDto);
                if (!validacionPrevia.EsValido)
                {
                    return RespuestaDto<ArticuloDto>.CrearError("Validación fallida", validacionPrevia.Detalle);
                }

                // Asegurar que el ID coincida
                articuloDto.Id = id;

                // Limpiar y normalizar datos
                articuloDto = NormalizarDatosArticulo(articuloDto);

                var respuesta = await _apiClient.PutAsync<ArticuloDto>($"articulo/{id}", articuloDto);
                return _interceptor.InterceptarRespuesta(respuesta, $"articulo/{id}", "PUT");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<ArticuloDto>(ex, $"articulo/{id}", "PUT");
            }
        }

        /// <summary>
        /// Actualiza el stock de un artículo específico
        /// </summary>
        /// <param name="id">ID del artículo</param>
        /// <param name="nuevoStock">Nuevo valor de stock</param>
        /// <returns>Resultado de la operación</returns>
        public async Task<RespuestaDto<object>> ActualizarStockAsync(int id, int nuevoStock)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<object>.CrearError("ID inválido",
                        "El ID del artículo debe ser mayor a 0");
                }

                if (nuevoStock < 0)
                {
                    return RespuestaDto<object>.CrearError("Stock inválido",
                        "El stock no puede ser negativo");
                }

                var respuesta = await _apiClient.PatchAsync<object>($"articulo/{id}/stock", nuevoStock);
                return _interceptor.InterceptarRespuesta(respuesta, $"articulo/{id}/stock", "PATCH");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<object>(ex, $"articulo/{id}/stock", "PATCH");
            }
        }

        /// <summary>
        /// Elimina (inactiva) un artículo del sistema
        /// </summary>
        /// <param name="id">ID del artículo</param>
        /// <returns>Resultado de la operación</returns>
        public async Task<RespuestaDto<object>> EliminarAsync(int id)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<object>.CrearError("ID inválido",
                        "El ID del artículo debe ser mayor a 0");
                }

                var respuesta = await _apiClient.DeleteAsync<object>($"articulo/{id}");
                return _interceptor.InterceptarRespuesta(respuesta, $"articulo/{id}", "DELETE");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<object>(ex, $"articulo/{id}", "DELETE");
            }
        }

        #endregion

        #region Métodos de Consulta y Búsqueda

        /// <summary>
        /// Obtiene artículos de forma paginada con filtros
        /// ⭐ PARA GRILLAS Y LISTADOS DE ARTÍCULOS
        /// </summary>
        /// <param name="pagina">Número de página</param>
        /// <param name="elementosPorPagina">Elementos por página</param>
        /// <param name="busqueda">Texto de búsqueda</param>
        /// <param name="categoriaId">ID de categoría para filtrar</param>
        /// <returns>Lista paginada de artículos</returns>
        public async Task<RespuestaDto<PaginacionDto<ArticuloDto>>> ObtenerPaginadoAsync(
            int pagina = 1,
            int elementosPorPagina = 10,
            string busqueda = null,
            int? categoriaId = null)
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
                {
                    queryParams.Add($"busqueda={Uri.EscapeDataString(busqueda.Trim())}");
                }

                if (categoriaId.HasValue && categoriaId.Value > 0)
                {
                    queryParams.Add($"categoriaId={categoriaId.Value}");
                }

                var queryString = string.Join("&", queryParams);
                var endpoint = $"articulo/paginado?{queryString}";

                var respuesta = await _apiClient.GetAsync<PaginacionDto<ArticuloDto>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<PaginacionDto<ArticuloDto>>(ex, "articulo/paginado", "GET");
            }
        }

        /// <summary>
        /// Busca artículos por código o nombre para autocompletar
        /// ⭐ MÉTODO PARA AUTOCOMPLETAR EN EL FORMULARIO DE FACTURAS
        /// </summary>
        /// <param name="termino">Término de búsqueda (código o nombre)</param>
        /// <param name="limite">Número máximo de resultados</param>
        /// <returns>Lista de artículos que coinciden</returns>
        public async Task<RespuestaDto<List<ArticuloDto>>> BuscarArticulosAsync(string termino, int limite = 10)
        {
            try
            {
                VerificarAutenticacion();

                if (string.IsNullOrWhiteSpace(termino))
                {
                    return RespuestaDto<List<ArticuloDto>>.CrearExitoso("Búsqueda vacía", new List<ArticuloDto>());
                }

                if (termino.Trim().Length < 2)
                {
                    return RespuestaDto<List<ArticuloDto>>.CrearError("Término muy corto",
                        "Debe ingresar al menos 2 caracteres para buscar");
                }

                // Usar el endpoint paginado con búsqueda
                var respuestaPaginada = await ObtenerPaginadoAsync(1, limite, termino.Trim());

                if (respuestaPaginada.Exito && respuestaPaginada.Resultado != null)
                {
                    return RespuestaDto<List<ArticuloDto>>.CrearExitoso(
                        "Artículos encontrados",
                        respuestaPaginada.Resultado.Lista ?? new List<ArticuloDto>(),
                        $"Se encontraron {respuestaPaginada.Resultado.TotalRegistros} artículos");
                }

                return RespuestaDto<List<ArticuloDto>>.CrearError(respuestaPaginada.Mensaje, respuestaPaginada.Detalle);
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ArticuloDto>>(ex, "articulo/buscar", "GET");
            }
        }

        /// <summary>
        /// Obtiene artículos con stock bajo
        /// </summary>
        /// <returns>Lista de artículos con stock bajo</returns>
        public async Task<RespuestaDto<List<ArticuloDto>>> ObtenerArticulosConStockBajoAsync()
        {
            try
            {
                VerificarAutenticacion();

                var respuesta = await _apiClient.GetAsync<List<ArticuloDto>>("articulo/stock-bajo");
                return _interceptor.InterceptarRespuesta(respuesta, "articulo/stock-bajo", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ArticuloDto>>(ex, "articulo/stock-bajo", "GET");
            }
        }

        /// <summary>
        /// Obtiene los artículos más vendidos
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del período</param>
        /// <param name="fechaFin">Fecha de fin del período</param>
        /// <param name="top">Número de artículos a obtener</param>
        /// <returns>Lista de artículos más vendidos</returns>
        public async Task<RespuestaDto<List<ArticuloVendidoDto>>> ObtenerArticulosMasVendidosAsync(
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            int top = 10)
        {
            try
            {
                VerificarAutenticacion();

                if (top <= 0) top = 10;

                var queryParams = new List<string> { $"top={top}" };

                if (fechaInicio.HasValue)
                    queryParams.Add($"fechaInicio={fechaInicio.Value:yyyy-MM-dd}");

                if (fechaFin.HasValue)
                    queryParams.Add($"fechaFin={fechaFin.Value:yyyy-MM-dd}");

                var queryString = string.Join("&", queryParams);
                var endpoint = $"articulo/mas-vendidos?{queryString}";

                var respuesta = await _apiClient.GetAsync<List<ArticuloVendidoDto>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ArticuloVendidoDto>>(ex, "articulo/mas-vendidos", "GET");
            }
        }

        /// <summary>
        /// Obtiene artículos por categoría
        /// </summary>
        /// <param name="categoriaId">ID de la categoría</param>
        /// <returns>Lista de artículos de la categoría</returns>
        public async Task<RespuestaDto<List<ArticuloDto>>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            try
            {
                VerificarAutenticacion();

                if (categoriaId <= 0)
                {
                    return RespuestaDto<List<ArticuloDto>>.CrearError("Categoría inválida",
                        "El ID de la categoría debe ser mayor a 0");
                }

                var respuesta = await _apiClient.GetAsync<List<ArticuloDto>>($"articulo/categoria/{categoriaId}");
                return _interceptor.InterceptarRespuesta(respuesta, $"articulo/categoria/{categoriaId}", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ArticuloDto>>(ex, $"articulo/categoria/{categoriaId}", "GET");
            }
        }

        #endregion

        #region Métodos de Validación y Utilidades

        /// <summary>
        /// Verifica si un artículo tiene stock suficiente para una cantidad específica
        /// ⭐ MÉTODO CLAVE PARA VALIDACIONES EN EL FORMULARIO DE FACTURAS
        /// </summary>
        /// <param name="articuloId">ID del artículo</param>
        /// <param name="cantidadRequerida">Cantidad requerida</param>
        /// <returns>True si hay stock suficiente</returns>
        public async Task<RespuestaDto<bool>> VerificarStockSuficienteAsync(int articuloId, int cantidadRequerida)
        {
            try
            {
                VerificarAutenticacion();

                if (articuloId <= 0)
                {
                    return RespuestaDto<bool>.CrearError("Artículo inválido",
                        "El ID del artículo debe ser mayor a 0");
                }

                if (cantidadRequerida <= 0)
                {
                    return RespuestaDto<bool>.CrearError("Cantidad inválida",
                        "La cantidad debe ser mayor a 0");
                }

                // Obtener el artículo para verificar su stock
                var respuestaArticulo = await ObtenerPorIdAsync(articuloId);

                if (!respuestaArticulo.Exito || respuestaArticulo.Resultado == null)
                {
                    return RespuestaDto<bool>.CrearError("Artículo no encontrado",
                        "No se pudo verificar el stock del artículo");
                }

                var articulo = respuestaArticulo.Resultado;
                bool tieneStock = articulo.Stock >= cantidadRequerida;

                return RespuestaDto<bool>.CrearExitoso(
                    tieneStock ? "Stock suficiente" : "Stock insuficiente",
                    tieneStock,
                    $"Stock disponible: {articulo.Stock}, Cantidad requerida: {cantidadRequerida}");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<bool>(ex, "articulo/verificar-stock", "GET");
            }
        }

        /// <summary>
        /// Valida los datos de un artículo antes de enviarlos al servidor
        /// </summary>
        /// <param name="articuloDto">Datos del artículo</param>
        /// <returns>Resultado de validación</returns>
        private ValidoDto ValidarArticuloAntesDeSend(ArticuloDto articuloDto)
        {
            try
            {
                if (articuloDto == null)
                {
                    return ValidoDto.Invalido("Los datos del artículo son requeridos");
                }

                if (string.IsNullOrWhiteSpace(articuloDto.Codigo))
                {
                    return ValidoDto.Invalido("El código del artículo es requerido");
                }

                if (string.IsNullOrWhiteSpace(articuloDto.Nombre))
                {
                    return ValidoDto.Invalido("El nombre del artículo es requerido");
                }

                if (articuloDto.PrecioUnitario <= 0)
                {
                    return ValidoDto.Invalido("El precio unitario debe ser mayor a 0");
                }

                if (articuloDto.Stock < 0)
                {
                    return ValidoDto.Invalido("El stock no puede ser negativo");
                }

                if (articuloDto.StockMinimo < 0)
                {
                    return ValidoDto.Invalido("El stock mínimo no puede ser negativo");
                }

                // Validaciones de formato
                if (articuloDto.Codigo.Trim().Length < 2)
                {
                    return ValidoDto.Invalido("El código debe tener al menos 2 caracteres");
                }

                if (articuloDto.Nombre.Trim().Length < 3)
                {
                    return ValidoDto.Invalido("El nombre debe tener al menos 3 caracteres");
                }

                return ValidoDto.Valido("Artículo válido para procesar");
            }
            catch (Exception ex)
            {
                return ValidoDto.Invalido($"Error en validación: {ex.Message}");
            }
        }

        /// <summary>
        /// Normaliza y limpia los datos del artículo
        /// </summary>
        /// <param name="articuloDto">Artículo original</param>
        /// <returns>Artículo con datos normalizados</returns>
        private ArticuloDto NormalizarDatosArticulo(ArticuloDto articuloDto)
        {
            if (articuloDto == null) return articuloDto;

            // Limpiar y normalizar campos de texto
            articuloDto.Codigo = LimpiarCodigoArticulo(articuloDto.Codigo);
            articuloDto.Nombre = LimpiarTexto(articuloDto.Nombre);

            if (!string.IsNullOrWhiteSpace(articuloDto.Descripcion))
            {
                articuloDto.Descripcion = LimpiarTexto(articuloDto.Descripcion);
            }

            return articuloDto;
        }

        /// <summary>
        /// Limpia un código de artículo
        /// </summary>
        /// <param name="codigo">Código original</param>
        /// <returns>Código limpio y normalizado</returns>
        private string LimpiarCodigoArticulo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return string.Empty;

            // Convertir a mayúsculas y remover espacios extra
            return codigo.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// Limpia un campo de texto
        /// </summary>
        /// <param name="texto">Texto original</param>
        /// <returns>Texto limpio</returns>
        private string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return texto.Trim();
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
                    System.Diagnostics.Debug.WriteLine($"Error liberando recursos de ArticuloService: {ex.Message}");
                }
            }
        }

        #endregion

        #region Métodos Estáticos de Utilidad

        /// <summary>
        /// Crea una instancia del servicio de artículos
        /// </summary>
        /// <returns>Nueva instancia de ArticuloService</returns>
        public static ArticuloService CrearInstancia()
        {
            return new ArticuloService();
        }

        #endregion
    }
}