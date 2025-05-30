using Facturacion.Web.Core;
using Facturacion.Web.Services;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facturacion.Web.App_Code.Base;

namespace Facturacion.Web.Pages.Master
{
    public partial class SiteMaster : MasterPage
    {
        #region Propiedades Privadas

        private AuthService _authService;
        private SessionManager _sessionManager;

        #endregion

        #region Eventos del Ciclo de Vida

        protected void Page_Init(object sender, EventArgs e)
        {
            // Inicializar servicios
            _authService = new AuthService();
            _sessionManager = new SessionManager();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ConfigurarMasterPage();
                CargarInfoUsuario();
                ConfigurarVisibilidadMenu();
                MostrarMensajesSistema();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Actualizar información del usuario si ha cambiado
            ActualizarInfoUsuario();
        }

        #endregion

        #region Métodos de Configuración

        /// <summary>
        /// Configura elementos básicos de la master page
        /// </summary>
        private void ConfigurarMasterPage()
        {
            // Configurar año en footer
            lblAño.Text = DateTime.Now.Year.ToString();

            // Configurar versión del sistema
            lblVersionSistema.Text = $"Versión {AppSettings.AppVersion}";

            // Configurar título base si no está establecido
            if (string.IsNullOrEmpty(Page.Title))
            {
                Page.Title = AppSettings.AppTitle;
            }
        }

        /// <summary>
        /// Carga la información del usuario actual
        /// </summary>
        private void CargarInfoUsuario()
        {
            if (_authService.EstaAutenticado())
            {
                var usuario = _authService.ObtenerUsuarioActual();
                if (usuario != null)
                {
                    // Mostrar nombre del usuario en el menú
                    lblUsuario.Text = usuario.NombreCompleto;

                    // Llenar información del modal
                    lblModalUsuario.Text = usuario.NombreUsuario;
                    lblModalNombre.Text = usuario.NombreCompleto;
                    lblModalRol.Text = usuario.Rol;
                    lblModalUltimoAcceso.Text = usuario.FechaUltimoAcceso?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
                }
            }
            else
            {
                lblUsuario.Text = "Invitado";
            }
        }

        /// <summary>
        /// Actualiza la información del usuario si ha cambiado
        /// </summary>
        private void ActualizarInfoUsuario()
        {
            if (_authService.EstaAutenticado())
            {
                var tiempoRestante = _authService.ObtenerTiempoRestanteMinutos();

                // Si la sesión está próxima a expirar (menos de 5 minutos)
                if (tiempoRestante <= 5 && tiempoRestante > 0)
                {
                    MostrarMensajeAdvertencia($"Su sesión expirará en {tiempoRestante} minuto(s)");
                }
            }
        }

        /// <summary>
        /// Configura la visibilidad del menú según los roles del usuario
        /// </summary>
        private void ConfigurarVisibilidadMenu()
        {
            if (!_authService.EstaAutenticado())
            {
                // Si no está autenticado, ocultar menús principales
                // Esto se maneja principalmente en las páginas individuales
                return;
            }

            var usuarioRol = _authService.ObtenerRol();

            // Configurar visibilidad según roles (ejemplo)
            // Los administradores ven todo, otros roles pueden tener restricciones
            switch (usuarioRol?.ToLower())
            {
                case "admin":
                    // Los administradores ven todo el menú
                    break;
                case "vendedor":
                    // Los vendedores pueden no ver ciertos reportes administrativos
                    break;
                case "supervisor":
                    // Los supervisores ven la mayoría de opciones
                    break;
                default:
                    // Rol básico o desconocido
                    break;
            }
        }

        #endregion

        #region Eventos de Controles

        /// <summary>
        /// Maneja el evento de logout
        /// </summary>
        protected async void lnkLogout_Click(object sender, EventArgs e)
        {
            try
            {
                // Ejecutar logout
                var resultado = await _authService.LogoutAsync();

                if (resultado.Exito)
                {
                    // Redirigir a la página de login
                    Response.Redirect("~/Pages/Auth/Login.aspx");
                }
                else
                {
                    // Si hay error en logout del servidor, forzar cierre local
                    _authService.ForzarCierreSesion();
                    Response.Redirect("~/Pages/Auth/Login.aspx");
                }
            }
            catch (Exception ex)
            {
                // En caso de error, forzar cierre de sesión local
                System.Diagnostics.Debug.WriteLine($"Error en logout: {ex.Message}");
                _authService.ForzarCierreSesion();
                Response.Redirect("~/Pages/Auth/Login.aspx");
            }
        }

        #endregion

        #region Métodos de Mensajes

        /// <summary>
        /// Muestra mensajes del sistema guardados en Session
        /// </summary>
        private void MostrarMensajesSistema()
        {
            // Verificar si hay mensajes en Session
            if (Session["MensajeExito"] != null)
            {
                MostrarMensajeExito(Session["MensajeExito"].ToString());
                Session.Remove("MensajeExito");
            }

            if (Session["MensajeError"] != null)
            {
                MostrarMensajeError(Session["MensajeError"].ToString());
                Session.Remove("MensajeError");
            }

            if (Session["MensajeAdvertencia"] != null)
            {
                MostrarMensajeAdvertencia(Session["MensajeAdvertencia"].ToString());
                Session.Remove("MensajeAdvertencia");
            }

            if (Session["MensajeInfo"] != null)
            {
                MostrarMensajeInfo(Session["MensajeInfo"].ToString());
                Session.Remove("MensajeInfo");
            }
        }

        /// <summary>
        /// Muestra un mensaje de éxito
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        public void MostrarMensajeExito(string mensaje)
        {
            pnlMensajeExito.Visible = true;
            lblMensajeExito.Text = mensaje;
        }

        /// <summary>
        /// Muestra un mensaje de error
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        public void MostrarMensajeError(string mensaje)
        {
            pnlMensajeError.Visible = true;
            lblMensajeError.Text = mensaje;
        }

        /// <summary>
        /// Muestra un mensaje de advertencia
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        public void MostrarMensajeAdvertencia(string mensaje)
        {
            pnlMensajeAdvertencia.Visible = true;
            lblMensajeAdvertencia.Text = mensaje;
        }

        /// <summary>
        /// Muestra un mensaje informativo
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        public void MostrarMensajeInfo(string mensaje)
        {
            pnlMensajeInfo.Visible = true;
            lblMensajeInfo.Text = mensaje;
        }

        /// <summary>
        /// Limpia todos los mensajes
        /// </summary>
        public void LimpiarMensajes()
        {
            pnlMensajeExito.Visible = false;
            pnlMensajeError.Visible = false;
            pnlMensajeAdvertencia.Visible = false;
            pnlMensajeInfo.Visible = false;
        }

        #endregion

        #region Métodos Públicos para las Páginas

        /// <summary>
        /// Permite a las páginas hijas establecer el título
        /// </summary>
        /// <param name="titulo">Título de la página</param>
        public void EstablecerTitulo(string titulo)
        {
            Page.Title = $"{titulo} - {AppSettings.AppTitle}";
        }

        /// <summary>
        /// Permite a las páginas hijas agregar elementos al breadcrumb
        /// </summary>
        /// <param name="texto">Texto del breadcrumb</param>
        /// <param name="url">URL opcional</param>
        /// <param name="esActivo">Si es el elemento activo (último)</param>
        public void AgregarBreadcrumb(string texto, string url = null, bool esActivo = false)
        {
            var li = new System.Web.UI.HtmlControls.HtmlGenericControl("li");
            li.Attributes.Add("class", esActivo ? "breadcrumb-item active" : "breadcrumb-item");

            if (!string.IsNullOrEmpty(url) && !esActivo)
            {
                var link = new System.Web.UI.HtmlControls.HtmlGenericControl("a");
                link.Attributes.Add("href", ResolveUrl(url));
                link.InnerText = texto;
                li.Controls.Add(link);
            }
            else
            {
                li.InnerText = texto;
                if (esActivo)
                {
                    li.Attributes.Add("aria-current", "page");
                }
            }

            // Buscar el ContentPlaceHolder del breadcrumb y agregar el elemento
            var breadcrumbContent = FindControl("BreadcrumbContent") as ContentPlaceHolder;
            breadcrumbContent?.Controls.Add(li);
        }

        /// <summary>
        /// Obtiene el servicio de autenticación
        /// </summary>
        /// <returns>Instancia de AuthService</returns>
        public AuthService ObtenerAuthService()
        {
            return _authService;
        }

        /// <summary>
        /// Obtiene el manager de sesión
        /// </summary>
        /// <returns>Instancia de SessionManager</returns>
        public SessionManager ObtenerSessionManager()
        {
            return _sessionManager;
        }

        #endregion

        #region Métodos de Utilidad

        /// <summary>
        /// Registra un script de JavaScript para ejecutar en el cliente
        /// </summary>
        /// <param name="key">Clave única del script</param>
        /// <param name="script">Código JavaScript</param>
        public void RegistrarScript(string key, string script)
        {
            if (!ClientScript.IsStartupScriptRegistered(key))
            {
                ClientScript.RegisterStartupScript(this.GetType(), key, script, true);
            }
        }

        /// <summary>
        /// Redirige a una página con mensaje
        /// </summary>
        /// <param name="url">URL de destino</param>
        /// <param name="mensaje">Mensaje</param>
        /// <param name="tipoMensaje">Tipo de mensaje (exito, error, advertencia, info)</param>
        public void RedirigirConMensaje(string url, string mensaje, string tipoMensaje = "exito")
        {
            switch (tipoMensaje.ToLower())
            {
                case "exito":
                    Session["MensajeExito"] = mensaje;
                    break;
                case "error":
                    Session["MensajeError"] = mensaje;
                    break;
                case "advertencia":
                    Session["MensajeAdvertencia"] = mensaje;
                    break;
                case "info":
                    Session["MensajeInfo"] = mensaje;
                    break;
            }
            Response.Redirect(url);
        }

        #endregion

        #region Limpieza de Recursos

        protected override void OnUnload(EventArgs e)
        {
            try
            {
                _authService?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error liberando recursos en Master: {ex.Message}");
            }

            base.OnUnload(e);
        }

        #endregion
    }
}