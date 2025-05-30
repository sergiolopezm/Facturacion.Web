using Facturacion.Web.Core;
using Facturacion.Web.Models.DTOs.Auth;
using Facturacion.Web.Services;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Facturacion.Web.Core.AppCode.Base
{
    /// <summary>
    /// Clase base para todas las páginas del sistema
    /// Proporciona funcionalidad común de autenticación, manejo de errores y servicios
    /// ⭐ FUNDAMENTAL PARA LA ARQUITECTURA - HEREDA DE System.Web.UI.Page
    /// </summary>
    public class BasePage : Page
    {
        #region Propiedades Protegidas

        /// <summary>
        /// Servicio de autenticación
        /// </summary>
        protected AuthService AuthService { get; private set; }

        /// <summary>
        /// Servicio de facturas
        /// </summary>
        protected FacturaService FacturaService { get; private set; }

        /// <summary>
        /// Servicio de clientes
        /// </summary>
        protected ClienteService ClienteService { get; private set; }

        /// <summary>
        /// Servicio de artículos
        /// </summary>
        protected ArticuloService ArticuloService { get; private set; }

        /// <summary>
        /// Servicio de categorías
        /// </summary>
        protected CategoriaService CategoriaService { get; private set; }

        /// <summary>
        /// Servicio de reportes
        /// </summary>
        protected ReporteService ReporteService { get; private set; }

        /// <summary>
        /// Manager de sesión
        /// </summary>
        protected SessionManager SessionManager { get; private set; }

        /// <summary>
        /// Usuario actual autenticado
        /// </summary>
        protected UsuarioPerfilDto UsuarioActual { get; private set; }

        /// <summary>
        /// Indica si la página requiere autenticación (por defecto true)
        /// </summary>
        protected virtual bool RequiereAutenticacion => true;

        /// <summary>
        /// Roles requeridos para acceder a la página (opcional)
        /// </summary>
        protected virtual string[] RolesRequeridos => null;

        /// <summary>
        /// Título de la página
        /// </summary>
        protected virtual string TituloPagina => "Sistema de Facturación";

        #endregion

        #region Eventos del Ciclo de Vida

        /// <summary>
        /// Se ejecuta al inicializar la página
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            try
            {
                // Inicializar servicios
                InicializarServicios();

                // Verificar autenticación si es requerida
                if (RequiereAutenticacion)
                {
                    VerificarAutenticacion();
                }

                // Configurar página
                ConfigurarPagina();

                base.OnInit(e);
            }
            catch (Exception ex)
            {
                ManejarErrorGlobal(ex);
            }
        }

        /// <summary>
        /// Se ejecuta al cargar la página
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    // Primera carga de la página
                    ConfigurarPrimeraCarga();
                }

                base.OnLoad(e);
            }
            catch (Exception ex)
            {
                ManejarErrorGlobal(ex);
            }
        }

        /// <summary>
        /// Se ejecuta antes de renderizar la página
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                // Renovar sesión automáticamente
                RenovarSesionSiEsNecesario();

                // Agregar scripts comunes
                AgregarScriptsComunes();

                base.OnPreRender(e);
            }
            catch (Exception ex)
            {
                ManejarErrorGlobal(ex);
            }
        }

        /// <summary>
        /// Se ejecuta al descargar la página
        /// </summary>
        protected override void OnUnload(EventArgs e)
        {
            try
            {
                // Liberar recursos
                LiberarRecursos();

                base.OnUnload(e);
            }
            catch (Exception ex)
            {
                // Log del error pero no mostrar al usuario
                System.Diagnostics.Debug.WriteLine($"Error en OnUnload: {ex.Message}");
            }
        }

        #endregion

        #region Métodos de Inicialización

        /// <summary>
        /// Inicializa todos los servicios necesarios
        /// </summary>
        private void InicializarServicios()
        {
            SessionManager = new SessionManager();
            AuthService = new AuthService();
            FacturaService = new FacturaService();
            ClienteService = new ClienteService();
            ArticuloService = new ArticuloService();
            CategoriaService = new CategoriaService();
            ReporteService = new ReporteService();
        }

        /// <summary>
        /// Configura las propiedades básicas de la página
        /// </summary>
        private void ConfigurarPagina()
        {
            // Configurar título
            if (Header != null)
            {
                Title = TituloPagina;
            }

            // Configurar meta tags
            ConfigurarMetaTags();

            // Configurar ViewState
            EnableViewState = true;
            ViewStateMode = ViewStateMode.Enabled;
        }

        /// <summary>
        /// Configura meta tags básicos
        /// </summary>
        private void ConfigurarMetaTags()
        {
            // Charset
            var metaCharset = new HtmlMeta();
            metaCharset.Attributes.Add("charset", "utf-8");
            Header?.Controls.AddAt(0, metaCharset);

            // Viewport para responsive
            var metaViewport = new HtmlMeta();
            metaViewport.Name = "viewport";
            metaViewport.Content = "width=device-width, initial-scale=1.0";
            Header?.Controls.Add(metaViewport);

            // Descripción
            var metaDescription = new HtmlMeta();
            metaDescription.Name = "description";
            metaDescription.Content = "Sistema de Facturación - Gestión integral de facturas, clientes y artículos";
            Header?.Controls.Add(metaDescription);
        }

        #endregion

        #region Métodos de Autenticación

        /// <summary>
        /// Verifica que el usuario esté autenticado
        /// </summary>
        private void VerificarAutenticacion()
        {
            if (!AuthService.EstaAutenticado())
            {
                // Usuario no autenticado, redirigir a login
                var returnUrl = HttpUtility.UrlEncode(Request.Url.ToString());
                Response.Redirect($"~/Pages/Auth/Login.aspx?returnUrl={returnUrl}");
                return;
            }

            // Obtener datos del usuario actual
            UsuarioActual = AuthService.ObtenerUsuarioActual();

            // Verificar roles si son requeridos
            if (RolesRequeridos != null && RolesRequeridos.Length > 0)
            {
                VerificarRoles();
            }
        }

        /// <summary>
        /// Verifica que el usuario tenga los roles requeridos
        /// </summary>
        private void VerificarRoles()
        {
            bool tieneRolRequerido = false;

            foreach (var rol in RolesRequeridos)
            {
                if (AuthService.TieneRol(rol))
                {
                    tieneRolRequerido = true;
                    break;
                }
            }

            if (!tieneRolRequerido)
            {
                // Usuario no tiene permisos, redirigir a página de acceso denegado
                Response.Redirect("~/Pages/Auth/AccessDenied.aspx");
            }
        }

        /// <summary>
        /// Renueva la sesión automáticamente si está próxima a expirar
        /// </summary>
        private void RenovarSesionSiEsNecesario()
        {
            if (RequiereAutenticacion && AuthService.EstaAutenticado())
            {
                AuthService.VerificarYRenovarSesion();
            }
        }

        #endregion

        #region Métodos de Configuración

        /// <summary>
        /// Configuración específica para la primera carga de la página
        /// Método virtual que pueden sobrescribir las páginas hijas
        /// </summary>
        protected virtual void ConfigurarPrimeraCarga()
        {
            // Implementación base vacía
            // Las páginas hijas pueden sobrescribir este método
        }

        /// <summary>
        /// Agrega scripts comunes a todas las páginas
        /// </summary>
        private void AgregarScriptsComunes()
        {
            // Script para manejo de sesión
            var scriptSesion = @"
                <script type='text/javascript'>
                    var sessionTimeoutMinutes = " + AppSettings.SessionTimeoutMinutes + @";
                    var warningMinutes = 5;
                    
                    // Función para mostrar advertencia de sesión próxima a expirar
                    function mostrarAdvertenciaSesion() {
                        if (confirm('Su sesión expirará en ' + warningMinutes + ' minutos. ¿Desea renovarla?')) {
                            // Hacer una petición AJAX simple para renovar la sesión
                            window.location.reload();
                        }
                    }
                    
                    // Configurar timer para advertencia de sesión
                    if (sessionTimeoutMinutes > 0) {
                        var warningTimeout = (sessionTimeoutMinutes - warningMinutes) * 60 * 1000;
                        setTimeout(mostrarAdvertenciaSesion, warningTimeout);
                    }
                </script>";

            ClientScript.RegisterStartupScript(this.GetType(), "SessionScript", scriptSesion, false);

            // Script para validaciones comunes
            RegisterCommonValidationScript();
        }

        /// <summary>
        /// Registra scripts de validación comunes
        /// </summary>
        private void RegisterCommonValidationScript()
        {
            var scriptValidacion = @"
                <script type='text/javascript'>
                    // Función para validar números
                    function validarNumero(valor) {
                        return !isNaN(valor) && valor !== '' && valor !== null;
                    }
                    
                    // Función para validar decimales (moneda)
                    function validarDecimal(valor) {
                        var regex = /^\d+(\.\d{1,2})?$/;
                        return regex.test(valor);
                    }
                    
                    // Función para formatear moneda
                    function formatearMoneda(valor) {
                        if (isNaN(valor)) return '$0';
                        return '$' + parseFloat(valor).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                    }
                    
                    // Función para mostrar loading
                    function mostrarLoading() {
                        if (document.getElementById('loadingOverlay')) {
                            document.getElementById('loadingOverlay').style.display = 'flex';
                        }
                    }
                    
                    // Función para ocultar loading
                    function ocultarLoading() {
                        if (document.getElementById('loadingOverlay')) {
                            document.getElementById('loadingOverlay').style.display = 'none';
                        }
                    }
                </script>";

            ClientScript.RegisterClientScriptBlock(this.GetType(), "ValidationScript", scriptValidacion, false);
        }

        #endregion

        #region Métodos de Utilidad Protegidos

        /// <summary>
        /// Muestra un mensaje de éxito al usuario
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        protected void MostrarMensajeExito(string mensaje)
        {
            var script = $"alert('Éxito: {EscaparJavaScript(mensaje)}');";
            ClientScript.RegisterStartupScript(this.GetType(), "MensajeExito", script, true);
        }

        /// <summary>
        /// Muestra un mensaje de error al usuario
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        protected void MostrarMensajeError(string mensaje)
        {
            var script = $"alert('Error: {EscaparJavaScript(mensaje)}');";
            ClientScript.RegisterStartupScript(this.GetType(), "MensajeError", script, true);
        }

        /// <summary>
        /// Muestra un mensaje de advertencia al usuario
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        protected void MostrarMensajeAdvertencia(string mensaje)
        {
            var script = $"alert('Advertencia: {EscaparJavaScript(mensaje)}');";
            ClientScript.RegisterStartupScript(this.GetType(), "MensajeAdvertencia", script, true);
        }

        /// <summary>
        /// Redirige a una página con mensaje
        /// </summary>
        /// <param name="url">URL de destino</param>
        /// <param name="mensaje">Mensaje opcional</param>
        protected void RedirigirConMensaje(string url, string mensaje = null)
        {
            if (!string.IsNullOrEmpty(mensaje))
            {
                Session["MensajeRedireccion"] = mensaje;
            }
            Response.Redirect(url);
        }

        /// <summary>
        /// Muestra mensaje de redirección si existe
        /// </summary>
        protected void MostrarMensajeRedireccion()
        {
            if (Session["MensajeRedireccion"] != null)
            {
                var mensaje = Session["MensajeRedireccion"].ToString();
                Session.Remove("MensajeRedireccion");
                MostrarMensajeExito(mensaje);
            }
        }

        /// <summary>
        /// Valida que el usuario tenga un rol específico
        /// </summary>
        /// <param name="rol">Rol requerido</param>
        /// <returns>True si tiene el rol</returns>
        protected bool ValidarRol(string rol)
        {
            return AuthService.TieneRol(rol);
        }

        /// <summary>
        /// Obtiene el nombre completo del usuario actual
        /// </summary>
        /// <returns>Nombre completo</returns>
        protected string ObtenerNombreUsuario()
        {
            return UsuarioActual?.NombreCompleto ?? "Usuario";
        }

        #endregion

        #region Manejo de Errores

        /// <summary>
        /// Maneja errores globales de la página
        /// </summary>
        /// <param name="ex">Excepción ocurrida</param>
        private void ManejarErrorGlobal(Exception ex)
        {
            // Log del error
            System.Diagnostics.Debug.WriteLine($"Error en {Request.Url}: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

            // Determinar mensaje para el usuario
            string mensajeUsuario = DeterminarMensajeError(ex);

            // Mostrar error al usuario
            MostrarMensajeError(mensajeUsuario);

            // Si es un error crítico, redirigir a página de error
            if (EsErrorCritico(ex))
            {
                Response.Redirect("~/Pages/Error.aspx");
            }
        }

        /// <summary>
        /// Determina el mensaje de error apropiado para mostrar al usuario
        /// </summary>
        /// <param name="ex">Excepción</param>
        /// <returns>Mensaje amigable</returns>
        private string DeterminarMensajeError(Exception ex)
        {
            switch (ex)
            {
                case UnauthorizedAccessException:
                    return "No tiene permisos para realizar esta operación";
                case TimeoutException:
                    return "La operación tardó demasiado tiempo. Intente nuevamente.";
                case ArgumentException:
                    return "Los datos proporcionados no son válidos";
                case InvalidOperationException:
                    return "No se puede realizar esta operación en este momento";
                default:
                    return "Ha ocurrido un error inesperado. Contacte al administrador si el problema persiste.";
            }
        }

        /// <summary>
        /// Determina si un error es crítico
        /// </summary>
        /// <param name="ex">Excepción</param>
        /// <returns>True si es crítico</returns>
        private bool EsErrorCritico(Exception ex)
        {
            return ex is OutOfMemoryException ||
                   ex is StackOverflowException ||
                   ex is AccessViolationException;
        }

        #endregion

        #region Métodos Privados de Utilidad

        /// <summary>
        /// Escapa texto para usar en JavaScript
        /// </summary>
        /// <param name="texto">Texto a escapar</param>
        /// <returns>Texto escapado</returns>
        private string EscaparJavaScript(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            return texto.Replace("'", "\\'")
                       .Replace("\"", "\\\"")
                       .Replace("\r\n", "\\n")
                       .Replace("\r", "\\n")
                       .Replace("\n", "\\n");
        }

        /// <summary>
        /// Libera recursos de los servicios
        /// </summary>
        private void LiberarRecursos()
        {
            try
            {
                AuthService?.Dispose();
                FacturaService?.Dispose();
                ClienteService?.Dispose();
                ArticuloService?.Dispose();
                CategoriaService?.Dispose();
                ReporteService?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error liberando recursos: {ex.Message}");
            }
        }

        #endregion
    }
}