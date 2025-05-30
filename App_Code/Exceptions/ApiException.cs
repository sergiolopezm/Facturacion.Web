using Facturacion.Web.Models.DTOs.Common;
using System;
using System.Net;

namespace Facturacion.Web.Core.App_Code.Exceptions
{
    /// <summary>
    /// Excepción personalizada para manejar errores en llamadas a la API
    /// Proporciona información detallada sobre el error y facilita un manejo consistente
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Código de estado HTTP asociado al error
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Detalle adicional sobre el error
        /// </summary>
        public string DetailedMessage { get; private set; }

        /// <summary>
        /// Endpoint de la API donde ocurrió el error
        /// </summary>
        public string ApiEndpoint { get; private set; }

        /// <summary>
        /// Método HTTP usado en la petición (GET, POST, etc.)
        /// </summary>
        public string HttpMethod { get; private set; }

        /// <summary>
        /// Datos de respuesta originales (si están disponibles)
        /// </summary>
        public RespuestaDto<object> ResponseData { get; private set; }

        /// <summary>
        /// Constructor básico
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        public ApiException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.InternalServerError;
            DetailedMessage = message;
        }

        /// <summary>
        /// Constructor con código de estado
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        public ApiException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
            DetailedMessage = message;
        }

        /// <summary>
        /// Constructor completo con detalles de la petición
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        /// <param name="detailedMessage">Mensaje detallado</param>
        /// <param name="apiEndpoint">Endpoint de la API</param>
        /// <param name="httpMethod">Método HTTP</param>
        public ApiException(string message, HttpStatusCode statusCode, string detailedMessage,
            string apiEndpoint, string httpMethod) : base(message)
        {
            StatusCode = statusCode;
            DetailedMessage = detailedMessage;
            ApiEndpoint = apiEndpoint;
            HttpMethod = httpMethod;
        }

        /// <summary>
        /// Constructor para manejar respuestas de la API
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="statusCode">Código de estado HTTP</param>
        /// <param name="responseData">Datos de respuesta</param>
        /// <param name="apiEndpoint">Endpoint de la API</param>
        /// <param name="httpMethod">Método HTTP</param>
        public ApiException(string message, HttpStatusCode statusCode, RespuestaDto<object> responseData,
            string apiEndpoint, string httpMethod) : base(message)
        {
            StatusCode = statusCode;
            DetailedMessage = responseData?.Detalle ?? message;
            ResponseData = responseData;
            ApiEndpoint = apiEndpoint;
            HttpMethod = httpMethod;
        }

        /// <summary>
        /// Constructor para encapsular otra excepción
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="innerException">Excepción interna</param>
        /// <param name="apiEndpoint">Endpoint de la API</param>
        /// <param name="httpMethod">Método HTTP</param>
        public ApiException(string message, Exception innerException,
            string apiEndpoint, string httpMethod) : base(message, innerException)
        {
            StatusCode = HttpStatusCode.InternalServerError;
            DetailedMessage = innerException?.Message ?? message;
            ApiEndpoint = apiEndpoint;
            HttpMethod = httpMethod;
        }

        /// <summary>
        /// Determina si la excepción está relacionada con problemas de autenticación
        /// </summary>
        public bool IsAuthenticationError()
        {
            return StatusCode == HttpStatusCode.Unauthorized ||
                   StatusCode == HttpStatusCode.Forbidden;
        }

        /// <summary>
        /// Determina si la excepción está relacionada con problemas de conectividad
        /// </summary>
        public bool IsConnectivityError()
        {
            return StatusCode == HttpStatusCode.GatewayTimeout ||
                   StatusCode == HttpStatusCode.ServiceUnavailable ||
                   StatusCode == HttpStatusCode.RequestTimeout;
        }

        /// <summary>
        /// Obtiene un mensaje amigable para el usuario según el tipo de error
        /// </summary>
        public string GetUserFriendlyMessage()
        {
            if (IsAuthenticationError())
                return "Su sesión ha expirado o no tiene permisos para realizar esta operación. Por favor, inicie sesión nuevamente.";

            if (IsConnectivityError())
                return "No se pudo establecer conexión con el servidor. Por favor, verifique su conexión a internet e intente nuevamente.";

            if (StatusCode == HttpStatusCode.BadRequest)
                return "Los datos enviados no son válidos. Por favor, revise la información ingresada e intente nuevamente.";

            if (StatusCode == HttpStatusCode.NotFound)
                return "El recurso solicitado no fue encontrado.";

            return "Ha ocurrido un error en la aplicación. Por favor, intente nuevamente más tarde.";
        }

        /// <summary>
        /// Registra la excepción en el sistema de logs
        /// </summary>
        public void LogException()
        {
            // Aquí se podría implementar la lógica para registrar la excepción
            // en el sistema de logs de la aplicación (log4net, NLog, etc.)
            System.Diagnostics.Debug.WriteLine($"API ERROR [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]: {HttpMethod} {ApiEndpoint} - {StatusCode} - {Message}");
            System.Diagnostics.Debug.WriteLine($"DETAIL: {DetailedMessage}");
        }
    }
}