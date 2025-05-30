using Facturacion.Web.Core;
using Facturacion.Web.Models.DTOs.Auth;
using Facturacion.Web.Models.DTOs.Common;
using System;
using System.Threading.Tasks;

namespace Facturacion.Web.Services
{
    /// <summary>
    /// Servicio para manejo de autenticación y gestión de usuarios
    /// Consume las APIs de autenticación del backend
    /// </summary>
    public class AuthService : IDisposable
    {
        private readonly ApiClient _apiClient;
        private readonly SessionManager _sessionManager;
        private readonly ApiInterceptor _interceptor;
        private bool _disposed = false;

        public AuthService()
        {
            _apiClient = new ApiClient();
            _sessionManager = new SessionManager();
            _interceptor = ApiInterceptor.CrearInstancia();
        }

        #region Métodos de Autenticación

        /// <summary>
        /// Realiza el login del usuario en el sistema
        /// </summary>
        /// <param name="loginDto">Datos de login</param>
        /// <returns>Respuesta con token y datos del usuario</returns>
        public async Task<RespuestaDto<TokenDto>> LoginAsync(UsuarioLoginDto loginDto)
        {
            try
            {
                // Agregar IP del cliente
                loginDto.Ip = ObtenerIpCliente();

                // Llamar a la API de login
                var respuesta = await _apiClient.PostAsync<TokenDto>("auth/login", loginDto);

                // Interceptar la respuesta
                respuesta = _interceptor.InterceptarRespuesta(respuesta, "auth/login", "POST");

                // Si el login es exitoso, establecer la sesión
                if (respuesta.Exito && respuesta.Resultado != null)
                {
                    var tokenData = respuesta.Resultado;
                    _sessionManager.EstablecerSesion(tokenData.Token, tokenData.Usuario);

                    // Log de login exitoso
                    System.Diagnostics.Debug.WriteLine($"Login exitoso para usuario: {loginDto.NombreUsuario}");
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en login: {ex.Message}");
                return _interceptor.InterceptarError<TokenDto>(ex, "auth/login", "POST");
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario actual
        /// </summary>
        /// <returns>Respuesta de la operación</returns>
        public async Task<RespuestaDto<object>> LogoutAsync()
        {
            try
            {
                // Verificar si hay sesión activa
                if (!_sessionManager.EstaAutenticado())
                {
                    return RespuestaDto<object>.CrearError("No hay sesión activa", "No se encontró una sesión activa para cerrar");
                }

                var usuarioActual = _sessionManager.ObtenerNombreUsuario();

                // Llamar a la API de logout
                var respuesta = await _apiClient.PostAsync<object>("auth/logout");

                // Interceptar la respuesta
                respuesta = _interceptor.InterceptarRespuesta(respuesta, "auth/logout", "POST");

                // Cerrar sesión local independientemente del resultado de la API
                _sessionManager.CerrarSesion();

                // Log de logout
                System.Diagnostics.Debug.WriteLine($"Logout completado para usuario: {usuarioActual}");

                return respuesta.Exito ? respuesta :
                    RespuestaDto<object>.CrearExitoso("Sesión cerrada", null, "La sesión se cerró correctamente");
            }
            catch (Exception ex)
            {
                // Cerrar sesión local aunque haya error en la API
                _sessionManager.CerrarSesion();
                System.Diagnostics.Debug.WriteLine($"Error en logout: {ex.Message}");

                return RespuestaDto<object>.CrearExitoso("Sesión cerrada", null,
                    "La sesión local se cerró correctamente");
            }
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <param name="registroDto">Datos del nuevo usuario</param>
        /// <returns>Respuesta con los datos del usuario creado</returns>
        public async Task<RespuestaDto<UsuarioPerfilDto>> RegistrarAsync(UsuarioRegistroDto registroDto)
        {
            try
            {
                // Validar que el usuario esté autenticado y tenga permisos para registrar
                if (!_sessionManager.EstaAutenticado())
                {
                    return RespuestaDto<UsuarioPerfilDto>.CrearError("No autorizado",
                        "Debe estar autenticado para registrar nuevos usuarios");
                }

                // Llamar a la API de registro
                var respuesta = await _apiClient.PostAsync<UsuarioPerfilDto>("auth/registro", registroDto);

                // Interceptar la respuesta
                respuesta = _interceptor.InterceptarRespuesta(respuesta, "auth/registro", "POST");

                if (respuesta.Exito)
                {
                    System.Diagnostics.Debug.WriteLine($"Usuario registrado exitosamente: {registroDto.NombreUsuario}");
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en registro: {ex.Message}");
                return _interceptor.InterceptarError<UsuarioPerfilDto>(ex, "auth/registro", "POST");
            }
        }

        /// <summary>
        /// Obtiene el perfil del usuario actual
        /// </summary>
        /// <returns>Datos del perfil del usuario</returns>
        public async Task<RespuestaDto<UsuarioPerfilDto>> ObtenerPerfilAsync()
        {
            try
            {
                // Verificar autenticación
                if (!_sessionManager.EstaAutenticado())
                {
                    return RespuestaDto<UsuarioPerfilDto>.CrearError("No autorizado",
                        "Debe estar autenticado para obtener el perfil");
                }

                // Llamar a la API de perfil
                var respuesta = await _apiClient.GetAsync<UsuarioPerfilDto>("auth/perfil");

                // Interceptar la respuesta
                respuesta = _interceptor.InterceptarRespuesta(respuesta, "auth/perfil", "GET");

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo perfil: {ex.Message}");
                return _interceptor.InterceptarError<UsuarioPerfilDto>(ex, "auth/perfil", "GET");
            }
        }

        #endregion

        #region Métodos de Validación

        /// <summary>
        /// Verifica si el usuario actual está autenticado
        /// </summary>
        /// <returns>True si está autenticado</returns>
        public bool EstaAutenticado()
        {
            return _sessionManager.EstaAutenticado();
        }

        /// <summary>
        /// Verifica si el usuario tiene un rol específico
        /// </summary>
        /// <param name="rol">Rol a verificar</param>
        /// <returns>True si tiene el rol</returns>
        public bool TieneRol(string rol)
        {
            return _sessionManager.TieneRol(rol);
        }

        /// <summary>
        /// Obtiene los datos del usuario actual de la sesión local
        /// </summary>
        /// <returns>Datos del usuario o null si no está autenticado</returns>
        public UsuarioPerfilDto ObtenerUsuarioActual()
        {
            return _sessionManager.ObtenerUsuario();
        }

        /// <summary>
        /// Obtiene el nombre del usuario actual
        /// </summary>
        /// <returns>Nombre del usuario</returns>
        public string ObtenerNombreUsuario()
        {
            return _sessionManager.ObtenerNombreUsuario();
        }

        /// <summary>
        /// Obtiene el nombre completo del usuario actual
        /// </summary>
        /// <returns>Nombre completo</returns>
        public string ObtenerNombreCompleto()
        {
            return _sessionManager.ObtenerNombreCompleto();
        }

        /// <summary>
        /// Obtiene el rol del usuario actual
        /// </summary>
        /// <returns>Rol del usuario</returns>
        public string ObtenerRol()
        {
            return _sessionManager.ObtenerRol();
        }

        /// <summary>
        /// Obtiene el tiempo restante de la sesión en minutos
        /// </summary>
        /// <returns>Minutos restantes</returns>
        public int ObtenerTiempoRestanteMinutos()
        {
            return _sessionManager.ObtenerTiempoRestanteMinutos();
        }

        /// <summary>
        /// Verifica si el token está próximo a expirar
        /// </summary>
        /// <param name="minutosAntes">Minutos antes de la expiración para considerar como próximo</param>
        /// <returns>True si está próximo a expirar</returns>
        public bool TokenProximoAExpirar(int minutosAntes = 5)
        {
            var tiempoRestante = ObtenerTiempoRestanteMinutos();
            return tiempoRestante <= minutosAntes && tiempoRestante > 0;
        }

        #endregion

        #region Métodos de Sesión

        /// <summary>
        /// Renueva la sesión actual
        /// </summary>
        public void RenovarSesion()
        {
            _sessionManager.RenovarSesion();
        }

        /// <summary>
        /// Verifica y renueva automáticamente la sesión si es necesario
        /// </summary>
        /// <returns>True si la sesión sigue siendo válida</returns>
        public bool VerificarYRenovarSesion()
        {
            try
            {
                if (_sessionManager.EstaAutenticado())
                {
                    // Si la sesión está próxima a expirar, renovarla
                    if (TokenProximoAExpirar(10))
                    {
                        _sessionManager.RenovarSesion();
                        System.Diagnostics.Debug.WriteLine("Sesión renovada automáticamente");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verificando sesión: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Fuerza el cierre de sesión local
        /// </summary>
        public void ForzarCierreSesion()
        {
            _sessionManager.CerrarSesion();
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Obtiene la IP del cliente actual
        /// </summary>
        /// <returns>Dirección IP</returns>
        private string ObtenerIpCliente()
        {
            try
            {
                if (System.Web.HttpContext.Current?.Request != null)
                {
                    var request = System.Web.HttpContext.Current.Request;

                    // Intentar obtener IP real detrás de proxy/load balancer
                    string ip = request.Headers["X-Forwarded-For"]
                             ?? request.Headers["X-Real-IP"]
                             ?? request.UserHostAddress;

                    // Si viene con múltiples IPs separadas por coma, tomar la primera
                    if (!string.IsNullOrEmpty(ip) && ip.Contains(","))
                    {
                        ip = ip.Split(',')[0].Trim();
                    }

                    return ip ?? "Unknown";
                }
                return "Unknown";
            }
            catch
            {
                return "Unknown";
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
                    System.Diagnostics.Debug.WriteLine($"Error liberando recursos de AuthService: {ex.Message}");
                }
            }
        }

        #endregion

        #region Métodos Estáticos de Utilidad

        /// <summary>
        /// Crea una instancia del servicio de autenticación
        /// </summary>
        /// <returns>Nueva instancia de AuthService</returns>
        public static AuthService CrearInstancia()
        {
            return new AuthService();
        }

        /// <summary>
        /// Verifica rápidamente si hay una sesión activa sin crear instancia
        /// </summary>
        /// <returns>True si hay sesión activa</returns>
        public static bool TieneSesionActiva()
        {
            return SessionManager.TieneSesionActiva();
        }

        #endregion
    }
}