using Facturacion.Web.Models.DTOs.Common;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;

namespace Facturacion.Web.Core
{
    /// <summary>
    /// Cliente HTTP base para consumir las APIs del backend
    /// Maneja la configuración base, headers comunes y serialización JSON
    /// </summary>
    public class ApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private bool _disposed = false;

        public ApiClient()
        {
            _baseUrl = AppSettings.ApiBaseUrl;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(AppSettings.ApiTimeoutSeconds);

            // Headers por defecto
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Facturacion.Web/1.0");

            // Headers de acceso a la API (desde configuración)
            _httpClient.DefaultRequestHeaders.Add("Sitio", AppSettings.ApiSitio);
            _httpClient.DefaultRequestHeaders.Add("Clave", AppSettings.ApiClave);
        }

        /// <summary>
        /// Realiza una petición GET a la API
        /// </summary>
        public async Task<RespuestaDto<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                ConfigurarAutenticacion();

                var response = await _httpClient.GetAsync(endpoint);
                return await ProcesarRespuesta<T>(response);
            }
            catch (Exception ex)
            {
                return CrearRespuestaError<T>(ex);
            }
        }

        /// <summary>
        /// Realiza una petición POST a la API
        /// </summary>
        public async Task<RespuestaDto<T>> PostAsync<T>(string endpoint, object data = null)
        {
            try
            {
                ConfigurarAutenticacion();

                var json = data != null ? JsonConvert.SerializeObject(data) : string.Empty;
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                return await ProcesarRespuesta<T>(response);
            }
            catch (Exception ex)
            {
                return CrearRespuestaError<T>(ex);
            }
        }

        /// <summary>
        /// Realiza una petición PUT a la API
        /// </summary>
        public async Task<RespuestaDto<T>> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                ConfigurarAutenticacion();

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);
                return await ProcesarRespuesta<T>(response);
            }
            catch (Exception ex)
            {
                return CrearRespuestaError<T>(ex);
            }
        }

        /// <summary>
        /// Realiza una petición DELETE a la API
        /// </summary>
        public async Task<RespuestaDto<T>> DeleteAsync<T>(string endpoint)
        {
            try
            {
                ConfigurarAutenticacion();

                var response = await _httpClient.DeleteAsync(endpoint);
                return await ProcesarRespuesta<T>(response);
            }
            catch (Exception ex)
            {
                return CrearRespuestaError<T>(ex);
            }
        }

        /// <summary>
        /// Realiza una petición PATCH a la API
        /// </summary>
        public async Task<RespuestaDto<T>> PatchAsync<T>(string endpoint, object data)
        {
            try
            {
                ConfigurarAutenticacion();

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);
                return await ProcesarRespuesta<T>(response);
            }
            catch (Exception ex)
            {
                return CrearRespuestaError<T>(ex);
            }
        }

        /// <summary>
        /// Configura los headers de autenticación si el usuario está logueado
        /// </summary>
        private void ConfigurarAutenticacion()
        {
            var sessionManager = new SessionManager();

            if (sessionManager.EstaAutenticado())
            {
                var token = sessionManager.ObtenerToken();
                var usuarioId = sessionManager.ObtenerUsuarioId();

                // Remover headers de autorización existentes
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Remove("UsuarioId");

                // Agregar nuevos headers
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("UsuarioId", usuarioId);
            }
        }

        /// <summary>
        /// Procesa la respuesta HTTP y la convierte en RespuestaDto
        /// </summary>
        private async Task<RespuestaDto<T>> ProcesarRespuesta<T>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    // Intentar deserializar como RespuestaDto del backend
                    var respuestaBackend = JsonConvert.DeserializeObject<RespuestaBackendDto>(responseContent);

                    if (respuestaBackend != null && respuestaBackend.Exito)
                    {
                        T resultado = default(T);

                        if (respuestaBackend.Resultado != null)
                        {
                            if (typeof(T) == typeof(string))
                            {
                                resultado = (T)(object)respuestaBackend.Resultado.ToString();
                            }
                            else
                            {
                                resultado = JsonConvert.DeserializeObject<T>(respuestaBackend.Resultado.ToString());
                            }
                        }

                        return new RespuestaDto<T>
                        {
                            Exito = true,
                            Mensaje = respuestaBackend.Mensaje,
                            Detalle = respuestaBackend.Detalle,
                            Resultado = resultado
                        };
                    }
                    else
                    {
                        return new RespuestaDto<T>
                        {
                            Exito = false,
                            Mensaje = respuestaBackend?.Mensaje ?? "Error en la respuesta del servidor",
                            Detalle = respuestaBackend?.Detalle ?? responseContent
                        };
                    }
                }
                catch (JsonException)
                {
                    // Si no se puede deserializar como RespuestaDto, intentar deserializar directamente
                    try
                    {
                        var resultado = JsonConvert.DeserializeObject<T>(responseContent);
                        return new RespuestaDto<T>
                        {
                            Exito = true,
                            Mensaje = "Operación exitosa",
                            Resultado = resultado
                        };
                    }
                    catch
                    {
                        return new RespuestaDto<T>
                        {
                            Exito = false,
                            Mensaje = "Error al procesar la respuesta",
                            Detalle = "No se pudo deserializar la respuesta del servidor"
                        };
                    }
                }
            }
            else
            {
                // Manejar códigos de error HTTP
                string mensaje = ObtenerMensajeError(response.StatusCode);
                string detalle = responseContent;

                // Si el contenido es JSON, intentar extraer el mensaje de error
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<RespuestaBackendDto>(responseContent);
                    if (errorResponse != null)
                    {
                        mensaje = errorResponse.Mensaje ?? mensaje;
                        detalle = errorResponse.Detalle ?? detalle;
                    }
                }
                catch
                {
                    // Ignorar errores de deserialización para respuestas de error
                }

                // Manejar casos especiales
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Limpiar sesión si el token expiró
                    var sessionManager = new SessionManager();
                    sessionManager.CerrarSesion();

                    // Redirigir a login si estamos en una página web
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Response.Redirect("~/Pages/Auth/Login.aspx");
                    }
                }

                return new RespuestaDto<T>
                {
                    Exito = false,
                    Mensaje = mensaje,
                    Detalle = detalle
                };
            }
        }

        /// <summary>
        /// Obtiene un mensaje de error amigable basado en el código de estado HTTP
        /// </summary>
        private string ObtenerMensajeError(System.Net.HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case System.Net.HttpStatusCode.BadRequest:
                    return "Solicitud incorrecta";
                case System.Net.HttpStatusCode.Unauthorized:
                    return "No autorizado";
                case System.Net.HttpStatusCode.Forbidden:
                    return "Acceso prohibido";
                case System.Net.HttpStatusCode.NotFound:
                    return "Recurso no encontrado";
                case System.Net.HttpStatusCode.InternalServerError:
                    return "Error interno del servidor";
                case System.Net.HttpStatusCode.BadGateway:
                    return "Error de conexión con el servidor";
                case System.Net.HttpStatusCode.ServiceUnavailable:
                    return "Servicio no disponible";
                case System.Net.HttpStatusCode.RequestTimeout:
                    return "Tiempo de espera agotado";
                default:
                    return $"Error del servidor ({(int)statusCode})";
            }
        }

        /// <summary>
        /// Crea una respuesta de error para excepciones
        /// </summary>
        private RespuestaDto<T> CrearRespuestaError<T>(Exception ex)
        {
            string mensaje = "Error de conexión";
            string detalle = ex.Message;

            if (ex is HttpRequestException)
            {
                mensaje = "Error de conexión con el servidor";
            }
            else if (ex is TaskCanceledException)
            {
                mensaje = "Tiempo de espera agotado";
                detalle = "La operación tardó demasiado tiempo en completarse";
            }

            return new RespuestaDto<T>
            {
                Exito = false,
                Mensaje = mensaje,
                Detalle = detalle
            };
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// DTO para deserializar las respuestas del backend
    /// </summary>
    internal class RespuestaBackendDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public string Detalle { get; set; }
        public object Resultado { get; set; }
    }
}