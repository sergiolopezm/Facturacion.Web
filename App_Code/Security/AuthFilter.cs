using System;
using System.Web;

namespace Facturacion.Web.Core.App_Code.Security
{
    /// <summary>
    /// Filtro de autenticación para páginas web
    /// Verifica que el usuario esté autenticado antes de mostrar la página
    /// </summary>
    public class AuthFilter : IHttpModule
    {
        private const string LOGIN_PAGE = "~/Pages/Auth/Login.aspx";
        private const string DEFAULT_PAGE = "~/Default.aspx";
        private static readonly string[] PUBLIC_PATHS = new string[]
        {
            "/pages/auth/login.aspx",
            "/pages/auth/logout.aspx",
            "/styles/",
            "/scripts/",
            "/content/",
            "/images/",
            "/favicon.ico"
        };

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnAuthenticateRequest;
            context.EndRequest += OnEndRequest;
        }

        public void Dispose()
        {
            // Nada que liberar
        }

        /// <summary>
        /// Evento que se ejecuta al autenticar la solicitud
        /// </summary>
        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;
            var request = context.Request;
            var response = context.Response;

            // No verificar autenticación para recursos públicos
            if (IsPublicResource(request.Path))
                return;

            try
            {
                // Verificar si hay sesión activa
                var sessionManager = new SessionManager();
                if (!sessionManager.EstaAutenticado())
                {
                    // Si es una solicitud AJAX, retornar 401
                    if (IsAjaxRequest(request))
                    {
                        response.StatusCode = 401;
                        response.StatusDescription = "Unauthorized";
                        response.Write("{\"Exito\":false,\"Mensaje\":\"Sesión expirada\",\"Detalle\":\"Debe iniciar sesión nuevamente\"}");
                        response.End();
                    }
                    else
                    {
                        // Guardar URL actual para redirigir después del login
                        try
                        {
                            if (!string.IsNullOrEmpty(request.RawUrl) &&
                                !request.RawUrl.Contains("login.aspx") &&
                                !request.RawUrl.Contains("logout.aspx"))
                            {
                                context.Session["ReturnUrl"] = request.RawUrl;
                            }
                        }
                        catch
                        {
                            // Ignorar errores al guardar URL
                        }

                        // Redirigir a login
                        response.Redirect(LOGIN_PAGE, true);
                    }
                }
                else
                {
                    // Verificar si el token está próximo a expirar
                    if (TokenProximoAExpirar(sessionManager))
                    {
                        // Renovar sesión
                        sessionManager.RenovarSesion();
                    }
                }
            }
            catch (Exception ex)
            {
                // Registrar el error
                System.Diagnostics.Debug.WriteLine($"Error en AuthFilter: {ex.Message}");

                // En caso de error, asegurar que la solicitud continúe
                return;
            }
        }

        /// <summary>
        /// Evento que se ejecuta al finalizar la solicitud
        /// </summary>
        private void OnEndRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var response = application.Context.Response;
            var request = application.Context.Request;

            // Agregar encabezados de seguridad
            if (!IsStaticResource(request.Path))
            {
                response.Headers.Add("X-Content-Type-Options", "nosniff");
                response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                response.Headers.Add("X-XSS-Protection", "1; mode=block");
            }
        }

        /// <summary>
        /// Verifica si el token está próximo a expirar (menos de 5 minutos)
        /// </summary>
        private bool TokenProximoAExpirar(SessionManager sessionManager)
        {
            var tiempoRestante = sessionManager.ObtenerTiempoRestanteMinutos();
            return tiempoRestante > 0 && tiempoRestante <= 5;
        }

        /// <summary>
        /// Verifica si la solicitud es para un recurso público
        /// </summary>
        private bool IsPublicResource(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            path = path.ToLowerInvariant();

            foreach (var publicPath in PUBLIC_PATHS)
            {
                if (path.StartsWith(publicPath, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Verifica si es un recurso estático
        /// </summary>
        private bool IsStaticResource(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            string extension = System.IO.Path.GetExtension(path);
            return !string.IsNullOrEmpty(extension) &&
                   (extension.Equals(".css", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".js", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".ico", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Verifica si la solicitud es AJAX
        /// </summary>
        private bool IsAjaxRequest(HttpRequest request)
        {
            if (request == null)
                return false;

            return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   request.Headers["Accept"] != null &&
                   request.Headers["Accept"].Contains("application/json");
        }
    }
}