using Facturacion.Web.Models.DTOs.Auth;
using Newtonsoft.Json;
using System;
using System.Web;

namespace Facturacion.Web.Core
{
    /// <summary>
    /// Manejo centralizado de sesión y JWT token
    /// Gestiona la autenticación del usuario y almacenamiento de datos de sesión
    /// </summary>
    public class SessionManager
    {
        private readonly string _tokenKey;
        private readonly string _userKey;
        private readonly int _sessionTimeoutMinutes;

        public SessionManager()
        {
            _tokenKey = AppSettings.SessionTokenKey;
            _userKey = AppSettings.SessionUserKey;
            _sessionTimeoutMinutes = AppSettings.SessionTimeoutMinutes;
        }

        #region Métodos Públicos de Autenticación

        /// <summary>
        /// Establece la sesión del usuario después del login exitoso
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="usuario">Información del usuario</param>
        public void EstablecerSesion(string token, UsuarioPerfilDto usuario)
        {
            try
            {
                // Limpiar sesión anterior si existe
                CerrarSesion();

                // Guardar token en cookie segura
                GuardarTokenEnCookie(token);

                // Guardar datos del usuario en sesión
                GuardarUsuarioEnSesion(usuario);

                // Establecer tiempo de expiración de sesión
                EstablecerTiempoExpiracion();

                // Log de inicio de sesión
                System.Diagnostics.Debug.WriteLine($"Sesión establecida para usuario: {usuario.NombreUsuario}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al establecer sesión: {ex.Message}");
                throw new InvalidOperationException("Error al establecer la sesión del usuario", ex);
            }
        }

        /// <summary>
        /// Cierra la sesión actual del usuario
        /// </summary>
        public void CerrarSesion()
        {
            try
            {
                // Limpiar cookie del token
                LimpiarTokenDeCookie();

                // Limpiar datos de sesión
                LimpiarSesion();

                // Abandonar sesión web
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session.Abandon();
                }

                System.Diagnostics.Debug.WriteLine("Sesión cerrada correctamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cerrar sesión: {ex.Message}");
                // No lanzar excepción en logout para evitar problemas
            }
        }

        /// <summary>
        /// Verifica si el usuario está autenticado
        /// </summary>
        /// <returns>True si está autenticado, False en caso contrario</returns>
        public bool EstaAutenticado()
        {
            try
            {
                // Verificar si existe token
                var token = ObtenerToken();
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }

                // Verificar si el token no ha expirado
                if (TokenEstaExpirado())
                {
                    CerrarSesion();
                    return false;
                }

                // Verificar si existen datos del usuario
                var usuario = ObtenerUsuario();
                if (usuario == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar autenticación: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Renueva la sesión actual (actualiza tiempo de expiración)
        /// </summary>
        public void RenovarSesion()
        {
            try
            {
                if (EstaAutenticado())
                {
                    EstablecerTiempoExpiracion();
                    System.Diagnostics.Debug.WriteLine("Sesión renovada");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al renovar sesión: {ex.Message}");
            }
        }

        #endregion

        #region Métodos de Obtención de Datos

        /// <summary>
        /// Obtiene el token JWT de la sesión actual
        /// </summary>
        /// <returns>Token JWT o cadena vacía si no existe</returns>
        public string ObtenerToken()
        {
            try
            {
                // Intentar obtener de cookie primero
                var tokenCookie = HttpContext.Current?.Request.Cookies[_tokenKey];
                if (tokenCookie != null && !string.IsNullOrEmpty(tokenCookie.Value))
                {
                    return tokenCookie.Value;
                }

                // Si no existe en cookie, intentar obtener de sesión
                if (HttpContext.Current?.Session != null)
                {
                    return HttpContext.Current.Session[_tokenKey]?.ToString() ?? string.Empty;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener token: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Obtiene los datos del usuario de la sesión actual
        /// </summary>
        /// <returns>Datos del usuario o null si no existe</returns>
        public UsuarioPerfilDto ObtenerUsuario()
        {
            try
            {
                if (HttpContext.Current?.Session != null)
                {
                    var userData = HttpContext.Current.Session[_userKey]?.ToString();
                    if (!string.IsNullOrEmpty(userData))
                    {
                        return JsonConvert.DeserializeObject<UsuarioPerfilDto>(userData);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener usuario: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene el ID del usuario actual
        /// </summary>
        /// <returns>ID del usuario o cadena vacía</returns>
        public string ObtenerUsuarioId()
        {
            try
            {
                var usuario = ObtenerUsuario();
                return usuario?.Id.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener ID de usuario: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el nombre del usuario actual
        /// </summary>
        /// <returns>Nombre del usuario o cadena vacía</returns>
        public string ObtenerNombreUsuario()
        {
            try
            {
                var usuario = ObtenerUsuario();
                return usuario?.NombreUsuario ?? string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener nombre de usuario: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el nombre completo del usuario actual
        /// </summary>
        /// <returns>Nombre completo del usuario</returns>
        public string ObtenerNombreCompleto()
        {
            try
            {
                var usuario = ObtenerUsuario();
                return usuario?.NombreCompleto ?? string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener nombre completo: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el rol del usuario actual
        /// </summary>
        /// <returns>Rol del usuario</returns>
        public string ObtenerRol()
        {
            try
            {
                var usuario = ObtenerUsuario();
                return usuario?.Rol ?? string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener rol: {ex.Message}");
                return string.Empty;
            }
        }

        #endregion

        #region Métodos de Validación

        /// <summary>
        /// Verifica si el usuario tiene un rol específico
        /// </summary>
        /// <param name="rol">Rol a verificar</param>
        /// <returns>True si tiene el rol, False en caso contrario</returns>
        public bool TieneRol(string rol)
        {
            try
            {
                var usuarioRol = ObtenerRol();
                return string.Equals(usuarioRol, rol, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar rol: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si el token ha expirado
        /// </summary>
        /// <returns>True si ha expirado, False en caso contrario</returns>
        public bool TokenEstaExpirado()
        {
            try
            {
                if (HttpContext.Current?.Session != null)
                {
                    var expiracion = HttpContext.Current.Session["TokenExpiration"];
                    if (expiracion != null && DateTime.TryParse(expiracion.ToString(), out DateTime fechaExpiracion))
                    {
                        return DateTime.Now > fechaExpiracion;
                    }
                }

                // Si no hay información de expiración, considerar como expirado
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al verificar expiración: {ex.Message}");
                return true;
            }
        }

        /// <summary>
        /// Obtiene el tiempo restante de la sesión
        /// </summary>
        /// <returns>Tiempo restante en minutos</returns>
        public int ObtenerTiempoRestanteMinutos()
        {
            try
            {
                if (HttpContext.Current?.Session != null)
                {
                    var expiracion = HttpContext.Current.Session["TokenExpiration"];
                    if (expiracion != null && DateTime.TryParse(expiracion.ToString(), out DateTime fechaExpiracion))
                    {
                        var tiempoRestante = fechaExpiracion - DateTime.Now;
                        return tiempoRestante.TotalMinutes > 0 ? (int)tiempoRestante.TotalMinutes : 0;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener tiempo restante: {ex.Message}");
                return 0;
            }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Guarda el token en una cookie segura
        /// </summary>
        private void GuardarTokenEnCookie(string token)
        {
            try
            {
                if (HttpContext.Current?.Response != null)
                {
                    var cookie = new HttpCookie(_tokenKey, token)
                    {
                        HttpOnly = true,
                        Secure = HttpContext.Current.Request.IsSecureConnection,
                        Expires = DateTime.Now.AddMinutes(_sessionTimeoutMinutes),
                        SameSite = SameSiteMode.Lax
                    };

                    HttpContext.Current.Response.Cookies.Add(cookie);

                    // También guardar en sesión como respaldo
                    HttpContext.Current.Session[_tokenKey] = token;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar token en cookie: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Guarda los datos del usuario en la sesión
        /// </summary>
        private void GuardarUsuarioEnSesion(UsuarioPerfilDto usuario)
        {
            try
            {
                if (HttpContext.Current?.Session != null)
                {
                    var userData = JsonConvert.SerializeObject(usuario);
                    HttpContext.Current.Session[_userKey] = userData;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar usuario en sesión: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Establece el tiempo de expiración de la sesión
        /// </summary>
        private void EstablecerTiempoExpiracion()
        {
            try
            {
                if (HttpContext.Current?.Session != null)
                {
                    var expiracion = DateTime.Now.AddMinutes(_sessionTimeoutMinutes);
                    HttpContext.Current.Session["TokenExpiration"] = expiracion.ToString("yyyy-MM-dd HH:mm:ss");
                    HttpContext.Current.Session.Timeout = _sessionTimeoutMinutes;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al establecer tiempo de expiración: {ex.Message}");
            }
        }

        /// <summary>
        /// Limpia el token de la cookie
        /// </summary>
        private void LimpiarTokenDeCookie()
        {
            try
            {
                if (HttpContext.Current?.Response != null)
                {
                    var cookie = new HttpCookie(_tokenKey, string.Empty)
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        HttpOnly = true,
                        Secure = HttpContext.Current.Request.IsSecureConnection
                    };

                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al limpiar cookie: {ex.Message}");
            }
        }

        /// <summary>
        /// Limpia todos los datos de la sesión
        /// </summary>
        private void LimpiarSesion()
        {
            try
            {
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session.Remove(_tokenKey);
                    HttpContext.Current.Session.Remove(_userKey);
                    HttpContext.Current.Session.Remove("TokenExpiration");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al limpiar sesión: {ex.Message}");
            }
        }

        #endregion

        #region Métodos Estáticos de Utilidad

        /// <summary>
        /// Obtiene una instancia del SessionManager con validación de contexto
        /// </summary>
        /// <returns>Instancia de SessionManager</returns>
        public static SessionManager GetInstance()
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("SessionManager solo puede ser usado en el contexto de una aplicación web");
            }

            return new SessionManager();
        }

        /// <summary>
        /// Verifica rápidamente si hay una sesión activa
        /// </summary>
        /// <returns>True si hay sesión activa</returns>
        public static bool TieneSesionActiva()
        {
            try
            {
                var sessionManager = new SessionManager();
                return sessionManager.EstaAutenticado();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Redirige a la página de login si no hay sesión activa
        /// </summary>
        /// <param name="paginaActual">URL de la página actual para redirigir después del login</param>
        public static void RedirigrirSiNoAutenticado(string paginaActual = "")
        {
            try
            {
                if (!TieneSesionActiva() && HttpContext.Current?.Response != null)
                {
                    var loginUrl = "~/Pages/Auth/Login.aspx";

                    if (!string.IsNullOrEmpty(paginaActual))
                    {
                        loginUrl += $"?returnUrl={HttpUtility.UrlEncode(paginaActual)}";
                    }

                    HttpContext.Current.Response.Redirect(loginUrl);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al redirigir: {ex.Message}");
            }
        }

        #endregion
    }
}