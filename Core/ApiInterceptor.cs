using Facturacion.Web.Models.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Web;

namespace Facturacion.Web.Core
{
    /// <summary>
    /// Interceptor para manejo de headers, errores y logging de peticiones API
    /// Proporciona funcionalidad centralizada para interceptar y procesar respuestas
    /// </summary>
    public class ApiInterceptor
    {
        private readonly SessionManager _sessionManager;

        public ApiInterceptor()
        {
            _sessionManager = new SessionManager();
        }

        #region Métodos de Interceptación

        /// <summary>
        /// Intercepta y procesa una respuesta de la API antes de devolverla
        /// </summary>
        /// <typeparam name="T">Tipo de datos de la respuesta</typeparam>
        /// <param name="respuesta">Respuesta de la API</param>
        /// <param name="endpoint">Endpoint que se consultó</param>
        /// <param name="metodo">Método HTTP utilizado</param>
        /// <returns>Respuesta procesada</returns>
        public RespuestaDto<T> InterceptarRespuesta<T>(RespuestaDto<T> respuesta, string endpoint, string metodo = "GET")
        {
            try
            {
                // Registrar la petición
                RegistrarPeticion(endpoint, metodo, respuesta.Exito);

                // Procesar respuesta según el tipo
                if (!respuesta.Exito)
                {
                    return ProcesarRespuestaError(respuesta, endpoint, metodo);
                }
                else
                {
                    return ProcesarRespuestaExitosa(respuesta, endpoint, metodo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en interceptor: {ex.Message}");

                return new RespuestaDto<T>
                {
                    Exito = false,
                    Mensaje = "Error interno del interceptor",
                    Detalle = ex.Message
                };
            }
        }

        /// <summary>
        /// Intercepta errores globales de la API
        /// </summary>
        /// <param name="excepcion">Excepción ocurrida</param>
        /// <param name="endpoint">Endpoint donde ocurrió el error</param>
        /// <param name="metodo">Método HTTP</param>
        /// <returns>Respuesta de error procesada</returns>
        public RespuestaDto<T> InterceptarError<T>(Exception excepcion, string endpoint, string metodo = "GET")
        {
            try
            {
                // Registrar el error
                RegistrarError(excepcion, endpoint, metodo);

                // Determinar tipo de error y mensaje apropiado
                var (mensaje, detalle) = DeterminarMensajeError(excepcion);

                return new RespuestaDto<T>
                {
                    Exito = false,
                    Mensaje = mensaje,
                    Detalle = detalle
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error crítico en interceptor: {ex.Message}");

                return new RespuestaDto<T>
                {
                    Exito = false,
                    Mensaje = "Error crítico del sistema",
                    Detalle = "Se produjo un error inesperado. Contacte al administrador."
                };
            }
        }

        #endregion

        #region Métodos de Procesamiento

        /// <summary>
        /// Procesa respuestas exitosas de la API
        /// </summary>
        private RespuestaDto<T> ProcesarRespuestaExitosa<T>(RespuestaDto<T> respuesta, string endpoint, string metodo)
        {
            try
            {
                // Renovar sesión en peticiones exitosas si el usuario está autenticado
                if (_sessionManager.EstaAutenticado())
                {
                    _sessionManager.RenovarSesion();
                }

                // Aplicar transformaciones específicas según el endpoint
                respuesta = AplicarTransformacionesEspecificas(respuesta, endpoint);

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error procesando respuesta exitosa: {ex.Message}");
                return respuesta; // Devolver respuesta original si hay error en procesamiento
            }
        }

        /// <summary>
        /// Procesa respuestas de error de la API
        /// </summary>
        private RespuestaDto<T> ProcesarRespuestaError<T>(RespuestaDto<T> respuesta, string endpoint, string metodo)
        {
            try
            {
                // Manejar errores específicos
                respuesta = ManejarErroresEspecificos(respuesta, endpoint);

                // Manejar errores de autenticación
                if (EsErrorDeAutenticacion(respuesta))
                {
                    ManejarErrorAutenticacion();
                }

                // Aplicar mensajes amigables
                respuesta = AplicarMensajesAmigables(respuesta);

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error procesando respuesta de error: {ex.Message}");
                return respuesta; // Devolver respuesta original si hay error en procesamiento
            }
        }

        /// <summary>
        /// Aplica transformaciones específicas según el endpoint
        /// </summary>
        private RespuestaDto<T> AplicarTransformacionesEspecificas<T>(RespuestaDto<T> respuesta, string endpoint)
        {
            try
            {
                // Transformaciones específicas para endpoints de autenticación
                if (endpoint.Contains("/auth/login"))
                {
                    // Procesar respuesta de login exitoso
                    ProcesarLoginExitoso(respuesta);
                }
                else if (endpoint.Contains("/auth/logout"))
                {
                    // Procesar respuesta de logout exitoso
                    ProcesarLogoutExitoso();
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en transformaciones específicas: {ex.Message}");
                return respuesta;
            }
        }

        /// <summary>
        /// Maneja errores específicos según el tipo de respuesta
        /// </summary>
        private RespuestaDto<T> ManejarErroresEspecificos<T>(RespuestaDto<T> respuesta, string endpoint)
        {
            try
            {
                // Errores específicos de validación
                if (respuesta.Mensaje?.Contains("validación") == true ||
                    respuesta.Mensaje?.Contains("validation") == true)
                {
                    respuesta.Mensaje = "Datos inválidos";
                }

                // Errores específicos de duplicados
                if (respuesta.Detalle?.Contains("ya existe") == true ||
                    respuesta.Detalle?.Contains("duplicado") == true)
                {
                    respuesta.Mensaje = "El registro ya existe";
                }

                // Errores específicos de no encontrado
                if (respuesta.Mensaje?.Contains("no encontrado") == true ||
                    respuesta.Mensaje?.Contains("not found") == true)
                {
                    respuesta.Mensaje = "Información no encontrada";
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error manejando errores específicos: {ex.Message}");
                return respuesta;
            }
        }

        /// <summary>
        /// Aplica mensajes más amigables para el usuario
        /// </summary>
        private RespuestaDto<T> AplicarMensajesAmigables<T>(RespuestaDto<T> respuesta)
        {
            try
            {
                // Mapear mensajes técnicos a mensajes amigables
                var mensajesAmigables = new Dictionary<string, string>
                {
                    { "Bad Request", "Solicitud incorrecta" },
                    { "Internal Server Error", "Error interno del servidor" },
                    { "Service Unavailable", "Servicio no disponible" },
                    { "Timeout", "Tiempo de espera agotado" },
                    { "Network Error", "Error de conexión" },
                    { "Unauthorized", "Sesión expirada" },
                    { "Forbidden", "Acceso no autorizado" }
                };

                foreach (var mapeo in mensajesAmigables)
                {
                    if (respuesta.Mensaje?.Contains(mapeo.Key) == true)
                    {
                        respuesta.Mensaje = mapeo.Value;
                        break;
                    }
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aplicando mensajes amigables: {ex.Message}");
                return respuesta;
            }
        }

        #endregion

        #region Métodos de Manejo de Autenticación

        /// <summary>
        /// Verifica si la respuesta indica un error de autenticación
        /// </summary>
        private bool EsErrorDeAutenticacion<T>(RespuestaDto<T> respuesta)
        {
            return respuesta.Mensaje?.Contains("No autorizado") == true ||
                   respuesta.Mensaje?.Contains("Unauthorized") == true ||
                   respuesta.Mensaje?.Contains("Sesión") == true ||
                   respuesta.Mensaje?.Contains("Token") == true;
        }

        /// <summary>
        /// Maneja errores de autenticación cerrando sesión y redirigiendo
        /// </summary>
        private void ManejarErrorAutenticacion()
        {
            try
            {
                // Cerrar sesión local
                _sessionManager.CerrarSesion();

                // Redirigir a login si estamos en contexto web
                if (HttpContext.Current?.Response != null)
                {
                    var currentUrl = HttpContext.Current.Request.Url.ToString();
                    var loginUrl = $"~/Pages/Auth/Login.aspx?returnUrl={HttpUtility.UrlEncode(currentUrl)}";

                    HttpContext.Current.Response.Redirect(loginUrl);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error manejando error de autenticación: {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa login exitoso (aunque normalmente se hace en AuthService)
        /// </summary>
        private void ProcesarLoginExitoso<T>(RespuestaDto<T> respuesta)
        {
            try
            {
                // Aquí se podría agregar lógica adicional después de login exitoso
                // como registrar último acceso, etc.
                System.Diagnostics.Debug.WriteLine("Login exitoso procesado por interceptor");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error procesando login exitoso: {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa logout exitoso
        /// </summary>
        private void ProcesarLogoutExitoso()
        {
            try
            {
                // Asegurar que la sesión local esté cerrada
                _sessionManager.CerrarSesion();
                System.Diagnostics.Debug.WriteLine("Logout exitoso procesado por interceptor");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error procesando logout exitoso: {ex.Message}");
            }
        }

        #endregion

        #region Métodos de Logging y Registro

        /// <summary>
        /// Registra una petición realizada a la API
        /// </summary>
        private void RegistrarPeticion(string endpoint, string metodo, bool exitosa)
        {
            try
            {
                var mensaje = $"API {metodo} {endpoint} - {(exitosa ? "EXITOSA" : "ERROR")}";
                var usuario = _sessionManager.ObtenerNombreUsuario();

                if (!string.IsNullOrEmpty(usuario))
                {
                    mensaje += $" - Usuario: {usuario}";
                }

                System.Diagnostics.Debug.WriteLine($"[API-LOG] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {mensaje}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registrando petición: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra un error ocurrido durante la petición
        /// </summary>
        private void RegistrarError(Exception excepcion, string endpoint, string metodo)
        {
            try
            {
                var mensaje = $"ERROR API {metodo} {endpoint}: {excepcion.Message}";
                var usuario = _sessionManager.ObtenerNombreUsuario();

                if (!string.IsNullOrEmpty(usuario))
                {
                    mensaje += $" - Usuario: {usuario}";
                }

                System.Diagnostics.Debug.WriteLine($"[API-ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {mensaje}");

                // En producción, aquí se podría enviar a un sistema de logging externo
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registrando error: {ex.Message}");
            }
        }

        /// <summary>
        /// Determina el mensaje de error apropiado según la excepción
        /// </summary>
        private (string mensaje, string detalle) DeterminarMensajeError(Exception excepcion)
        {
            try
            {
                switch (excepcion)
                {
                    case TimeoutException _:
                        return ("Tiempo de espera agotado", "La operación tardó demasiado tiempo en completarse");

                    case System.Net.Http.HttpRequestException _:
                        return ("Error de conexión", "No se pudo conectar con el servidor");

                    case UnauthorizedAccessException _:
                        return ("Acceso no autorizado", "No tiene permisos para realizar esta operación");

                    case ArgumentException _:
                        return ("Datos inválidos", "Los datos proporcionados no son válidos");

                    case InvalidOperationException _:
                        return ("Operación no válida", "La operación solicitada no se puede realizar");

                    default:
                        return ("Error inesperado", excepcion.Message);
                }
            }
            catch
            {
                return ("Error crítico", "Se produjo un error inesperado en el sistema");
            }
        }

        #endregion

        #region Métodos Estáticos de Utilidad

        /// <summary>
        /// Crea una instancia del interceptor con validaciones
        /// </summary>
        /// <returns>Instancia del interceptor</returns>
        public static ApiInterceptor CrearInstancia()
        {
            try
            {
                return new ApiInterceptor();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando instancia de interceptor: {ex.Message}");
                throw new InvalidOperationException("No se pudo crear el interceptor de API", ex);
            }
        }

        #endregion
    }
}