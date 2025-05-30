using Facturacion.Web.Core;
using Facturacion.Web.Models.DTOs.Articulos;
using Facturacion.Web.Models.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facturacion.Web.Services
{
    /// <summary>
    /// Servicio para manejo completo de categorías de artículos
    /// Consume las APIs de categorías del backend para CRUD y organización de artículos
    /// ⭐ COMPLEMENTA LA FUNCIONALIDAD DE ARTÍCULOS EN EL SISTEMA
    /// </summary>
    public class CategoriaService : IDisposable
    {
        private readonly ApiClient _apiClient;
        private readonly SessionManager _sessionManager;
        private readonly ApiInterceptor _interceptor;
        private bool _disposed = false;

        public CategoriaService()
        {
            _apiClient = new ApiClient();
            _sessionManager = new SessionManager();
            _interceptor = ApiInterceptor.CrearInstancia();
        }

        #region Métodos CRUD Básicos

        /// <summary>
        /// Obtiene todas las categorías activas del sistema
        /// ⭐ PARA LLENAR DROPDOWNS EN FORMULARIOS
        /// </summary>
        /// <returns>Lista completa de categorías</returns>
        public async Task<RespuestaDto<List<CategoriaArticuloDto>>> ObtenerTodasAsync()
        {
            try
            {
                VerificarAutenticacion();

                var respuesta = await _apiClient.GetAsync<List<CategoriaArticuloDto>>("categoriaarticulo");
                return _interceptor.InterceptarRespuesta(respuesta, "categoriaarticulo", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<CategoriaArticuloDto>>(ex, "categoriaarticulo", "GET");
            }
        }

        /// <summary>
        /// Obtiene una categoría específica por su ID
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Categoría encontrada</returns>
        public async Task<RespuestaDto<CategoriaArticuloDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<CategoriaArticuloDto>.CrearError("ID inválido",
                        "El ID de la categoría debe ser mayor a 0");
                }

                var respuesta = await _apiClient.GetAsync<CategoriaArticuloDto>($"categoriaarticulo/{id}");
                return _interceptor.InterceptarRespuesta(respuesta, $"categoriaarticulo/{id}", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<CategoriaArticuloDto>(ex, $"categoriaarticulo/{id}", "GET");
            }
        }

        /// <summary>
        /// Crea una nueva categoría en el sistema
        /// </summary>
        /// <param name="categoriaDto">Datos de la categoría a crear</param>
        /// <returns>Categoría creada</returns>
        public async Task<RespuestaDto<CategoriaArticuloDto>> CrearAsync(CategoriaArticuloDto categoriaDto)
        {
            try
            {
                VerificarAutenticacion();

                // Validaciones previas
                var validacionPrevia = ValidarCategoriaAntesDeSend(categoriaDto);
                if (!validacionPrevia.EsValido)
                {
                    return RespuestaDto<CategoriaArticuloDto>.CrearError("Validación fallida", validacionPrevia.Detalle);
                }

                // Limpiar y normalizar datos
                categoriaDto = NormalizarDatosCategoria(categoriaDto);

                var respuesta = await _apiClient.PostAsync<CategoriaArticuloDto>("categoriaarticulo", categoriaDto);
                respuesta = _interceptor.InterceptarRespuesta(respuesta, "categoriaarticulo", "POST");

                if (respuesta.Exito)
                {
                    System.Diagnostics.Debug.WriteLine($"Categoría creada exitosamente: {categoriaDto.Nombre}");
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando categoría: {ex.Message}");
                return _interceptor.InterceptarError<CategoriaArticuloDto>(ex, "categoriaarticulo", "POST");
            }
        }

        /// <summary>
        /// Actualiza una categoría existente
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <param name="categoriaDto">Datos actualizados de la categoría</param>
        /// <returns>Categoría actualizada</returns>
        public async Task<RespuestaDto<CategoriaArticuloDto>> ActualizarAsync(int id, CategoriaArticuloDto categoriaDto)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<CategoriaArticuloDto>.CrearError("ID inválido",
                        "El ID de la categoría debe ser mayor a 0");
                }

                // Validaciones previas
                var validacionPrevia = ValidarCategoriaAntesDeSend(categoriaDto);
                if (!validacionPrevia.EsValido)
                {
                    return RespuestaDto<CategoriaArticuloDto>.CrearError("Validación fallida", validacionPrevia.Detalle);
                }

                // Asegurar que el ID coincida
                categoriaDto.Id = id;

                // Limpiar y normalizar datos
                categoriaDto = NormalizarDatosCategoria(categoriaDto);

                var respuesta = await _apiClient.PutAsync<CategoriaArticuloDto>($"categoriaarticulo/{id}", categoriaDto);
                return _interceptor.InterceptarRespuesta(respuesta, $"categoriaarticulo/{id}", "PUT");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<CategoriaArticuloDto>(ex, $"categoriaarticulo/{id}", "PUT");
            }
        }

        /// <summary>
        /// Elimina (inactiva) una categoría del sistema
        /// </summary>
        /// <param name="id">ID de la categoría</param>
        /// <returns>Resultado de la operación</returns>
        public async Task<RespuestaDto<object>> EliminarAsync(int id)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<object>.CrearError("ID inválido",
                        "El ID de la categoría debe ser mayor a 0");
                }

                var respuesta = await _apiClient.DeleteAsync<object>($"categoriaarticulo/{id}");
                return _interceptor.InterceptarRespuesta(respuesta, $"categoriaarticulo/{id}", "DELETE");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<object>(ex, $"categoriaarticulo/{id}", "DELETE");
            }
        }

        #endregion

        #region Métodos de Consulta y Búsqueda

        /// <summary>
        /// Obtiene categorías de forma paginada con filtros
        /// ⭐ PARA GRILLAS Y LISTADOS DE CATEGORÍAS
        /// </summary>
        /// <param name="pagina">Número de página</param>
        /// <param name="elementosPorPagina">Elementos por página</param>
        /// <param name="busqueda">Texto de búsqueda</param>
        /// <returns>Lista paginada de categorías</returns>
        public async Task<RespuestaDto<PaginacionDto<CategoriaArticuloDto>>> ObtenerPaginadoAsync(
            int pagina = 1,
            int elementosPorPagina = 10,
            string busqueda = null)
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

                var queryString = string.Join("&", queryParams);
                var endpoint = $"categoriaarticulo/paginado?{queryString}";

                var respuesta = await _apiClient.GetAsync<PaginacionDto<CategoriaArticuloDto>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<PaginacionDto<CategoriaArticuloDto>>(ex, "categoriaarticulo/paginado", "GET");
            }
        }

        /// <summary>
        /// Busca categorías por nombre para autocompletar
        /// </summary>
        /// <param name="termino">Término de búsqueda</param>
        /// <param name="limite">Número máximo de resultados</param>
        /// <returns>Lista de categorías que coinciden</returns>
        public async Task<RespuestaDto<List<CategoriaArticuloDto>>> BuscarCategoriasAsync(string termino, int limite = 10)
        {
            try
            {
                VerificarAutenticacion();

                if (string.IsNullOrWhiteSpace(termino))
                {
                    return RespuestaDto<List<CategoriaArticuloDto>>.CrearExitoso("Búsqueda vacía", new List<CategoriaArticuloDto>());
                }

                if (termino.Trim().Length < 2)
                {
                    return RespuestaDto<List<CategoriaArticuloDto>>.CrearError("Término muy corto",
                        "Debe ingresar al menos 2 caracteres para buscar");
                }

                // Usar el endpoint paginado con búsqueda
                var respuestaPaginada = await ObtenerPaginadoAsync(1, limite, termino.Trim());

                if (respuestaPaginada.Exito && respuestaPaginada.Resultado != null)
                {
                    return RespuestaDto<List<CategoriaArticuloDto>>.CrearExitoso(
                        "Categorías encontradas",
                        respuestaPaginada.Resultado.Lista ?? new List<CategoriaArticuloDto>(),
                        $"Se encontraron {respuestaPaginada.Resultado.TotalRegistros} categorías");
                }

                return RespuestaDto<List<CategoriaArticuloDto>>.CrearError(respuestaPaginada.Mensaje, respuestaPaginada.Detalle);
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<CategoriaArticuloDto>>(ex, "categoriaarticulo/buscar", "GET");
            }
        }

        /// <summary>
        /// Obtiene los artículos que pertenecen a una categoría específica
        /// </summary>
        /// <param name="categoriaId">ID de la categoría</param>
        /// <returns>Lista de artículos de la categoría</returns>
        public async Task<RespuestaDto<List<ArticuloDto>>> ObtenerArticulosDeCategoriaAsync(int categoriaId)
        {
            try
            {
                VerificarAutenticacion();

                if (categoriaId <= 0)
                {
                    return RespuestaDto<List<ArticuloDto>>.CrearError("Categoría inválida",
                        "El ID de la categoría debe ser mayor a 0");
                }

                var respuesta = await _apiClient.GetAsync<List<ArticuloDto>>($"categoriaarticulo/{categoriaId}/articulos");
                return _interceptor.InterceptarRespuesta(respuesta, $"categoriaarticulo/{categoriaId}/articulos", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ArticuloDto>>(ex, $"categoriaarticulo/{categoriaId}/articulos", "GET");
            }
        }

        /// <summary>
        /// Verifica si existe una categoría con el nombre especificado
        /// </summary>
        /// <param name="nombre">Nombre de la categoría</param>
        /// <returns>True si existe</returns>
        public async Task<RespuestaDto<bool>> ExistePorNombreAsync(string nombre)
        {
            try
            {
                VerificarAutenticacion();

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return RespuestaDto<bool>.CrearError("Nombre requerido",
                        "El nombre de la categoría es requerido");
                }

                var respuesta = await _apiClient.GetAsync<object>($"categoriaarticulo/existe-nombre/{Uri.EscapeDataString(nombre.Trim())}");

                if (respuesta.Exito && respuesta.Resultado != null)
                {
                    // El backend devuelve un objeto con la propiedad "Existe"
                    var resultado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(respuesta.Resultado.ToString());
                    bool existe = resultado?.Existe ?? false;

                    return RespuestaDto<bool>.CrearExitoso(
                        existe ? "Categoría existe" : "Categoría no existe",
                        existe,
                        $"La categoría '{nombre}' {(existe ? "ya existe" : "no existe")} en el sistema");
                }

                return RespuestaDto<bool>.CrearError("Error verificando nombre", "No se pudo verificar la existencia de la categoría");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<bool>(ex, $"categoriaarticulo/existe-nombre/{nombre}", "GET");
            }
        }

        #endregion

        #region Métodos de Utilidades para Formularios

        /// <summary>
        /// Obtiene categorías activas para llenar dropdowns/combobox
        /// ⭐ MÉTODO ESPECÍFICO PARA FORMULARIOS
        /// </summary>
        /// <returns>Lista simplificada de categorías para dropdowns</returns>
        public async Task<RespuestaDto<List<KeyValuePair<int, string>>>> ObtenerParaDropdownAsync()
        {
            try
            {
                var respuestaCategorias = await ObtenerTodasAsync();

                if (!respuestaCategorias.Exito || respuestaCategorias.Resultado == null)
                {
                    return RespuestaDto<List<KeyValuePair<int, string>>>.CrearError(
                        respuestaCategorias.Mensaje,
                        respuestaCategorias.Detalle);
                }

                var dropdown = new List<KeyValuePair<int, string>>();

                // Agregar opción vacía
                dropdown.Add(new KeyValuePair<int, string>(0, "-- Seleccione una categoría --"));

                // Agregar categorías ordenadas alfabéticamente
                foreach (var categoria in respuestaCategorias.Resultado)
                {
                    dropdown.Add(new KeyValuePair<int, string>(categoria.Id, categoria.Nombre));
                }

                return RespuestaDto<List<KeyValuePair<int, string>>>.CrearExitoso(
                    "Categorías para dropdown",
                    dropdown,
                    $"Se obtuvieron {respuestaCategorias.Resultado.Count} categorías");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<KeyValuePair<int, string>>>(ex, "categoriaarticulo/dropdown", "GET");
            }
        }

        /// <summary>
        /// Obtiene estadísticas básicas de una categoría
        /// </summary>
        /// <param name="categoriaId">ID de la categoría</param>
        /// <returns>Estadísticas de la categoría</returns>
        public async Task<RespuestaDto<object>> ObtenerEstadisticasCategoriaAsync(int categoriaId)
        {
            try
            {
                VerificarAutenticacion();

                if (categoriaId <= 0)
                {
                    return RespuestaDto<object>.CrearError("Categoría inválida",
                        "El ID de la categoría debe ser mayor a 0");
                }

                // Obtener la categoría y sus artículos para generar estadísticas
                var respuestaCategoria = await ObtenerPorIdAsync(categoriaId);
                var respuestaArticulos = await ObtenerArticulosDeCategoriaAsync(categoriaId);

                if (!respuestaCategoria.Exito || !respuestaArticulos.Exito)
                {
                    return RespuestaDto<object>.CrearError("Error obteniendo datos",
                        "No se pudieron obtener los datos de la categoría");
                }

                var categoria = respuestaCategoria.Resultado;
                var articulos = respuestaArticulos.Resultado ?? new List<ArticuloDto>();

                var estadisticas = new
                {
                    CategoriaNombre = categoria?.Nombre,
                    TotalArticulos = articulos.Count,
                    ArticulosConStock = articulos.Count(a => a.Stock > 0),
                    ArticulosStockBajo = articulos.Count(a => a.StockBajo),
                    ValorTotalInventario = articulos.Sum(a => a.PrecioUnitario * a.Stock),
                    PromedioPrecios = articulos.Count > 0 ? articulos.Average(a => a.PrecioUnitario) : 0
                };

                return RespuestaDto<object>.CrearExitoso(
                    "Estadísticas obtenidas",
                    estadisticas,
                    $"Estadísticas generadas para la categoría {categoria?.Nombre}");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<object>(ex, $"categoriaarticulo/{categoriaId}/estadisticas", "GET");
            }
        }

        #endregion

        #region Métodos de Validación y Utilidades

        /// <summary>
        /// Valida los datos de una categoría antes de enviarlos al servidor
        /// </summary>
        /// <param name="categoriaDto">Datos de la categoría</param>
        /// <returns>Resultado de validación</returns>
        private ValidoDto ValidarCategoriaAntesDeSend(CategoriaArticuloDto categoriaDto)
        {
            try
            {
                if (categoriaDto == null)
                {
                    return ValidoDto.Invalido("Los datos de la categoría son requeridos");
                }

                if (string.IsNullOrWhiteSpace(categoriaDto.Nombre))
                {
                    return ValidoDto.Invalido("El nombre de la categoría es requerido");
                }

                // Validaciones de formato
                if (categoriaDto.Nombre.Trim().Length < 3)
                {
                    return ValidoDto.Invalido("El nombre debe tener al menos 3 caracteres");
                }

                if (categoriaDto.Nombre.Trim().Length > 100)
                {
                    return ValidoDto.Invalido("El nombre no puede exceder los 100 caracteres");
                }

                // Validar descripción si se proporciona
                if (!string.IsNullOrWhiteSpace(categoriaDto.Descripcion) && categoriaDto.Descripcion.Trim().Length > 250)
                {
                    return ValidoDto.Invalido("La descripción no puede exceder los 250 caracteres");
                }

                return ValidoDto.Valido("Categoría válida para procesar");
            }
            catch (Exception ex)
            {
                return ValidoDto.Invalido($"Error en validación: {ex.Message}");
            }
        }

        /// <summary>
        /// Normaliza y limpia los datos de la categoría
        /// </summary>
        /// <param name="categoriaDto">Categoría original</param>
        /// <returns>Categoría con datos normalizados</returns>
        private CategoriaArticuloDto NormalizarDatosCategoria(CategoriaArticuloDto categoriaDto)
        {
            if (categoriaDto == null) return categoriaDto;

            // Limpiar y normalizar campos de texto
            categoriaDto.Nombre = LimpiarTexto(categoriaDto.Nombre);

            if (!string.IsNullOrWhiteSpace(categoriaDto.Descripcion))
            {
                categoriaDto.Descripcion = LimpiarTexto(categoriaDto.Descripcion);
            }

            return categoriaDto;
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
                    System.Diagnostics.Debug.WriteLine($"Error liberando recursos de CategoriaService: {ex.Message}");
                }
            }
        }

        #endregion

        #region Métodos Estáticos de Utilidad

        /// <summary>
        /// Crea una instancia del servicio de categorías
        /// </summary>
        /// <returns>Nueva instancia de CategoriaService</returns>
        public static CategoriaService CrearInstancia()
        {
            return new CategoriaService();
        }

        #endregion
    }
}