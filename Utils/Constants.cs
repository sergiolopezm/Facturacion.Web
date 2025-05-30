namespace Facturacion.Web.Utils
{
    /// <summary>
    /// Clase que contiene todas las constantes utilizadas en la aplicación
    /// </summary>
    public static class Constants
    {
        #region Configuraciones Generales

        /// <summary>
        /// Nombre de la aplicación
        /// </summary>
        public const string AppName = "Sistema de Facturación";

        /// <summary>
        /// Versión de la aplicación
        /// </summary>
        public const string AppVersion = "1.0.0";

        /// <summary>
        /// Fecha de lanzamiento de la versión actual
        /// </summary>
        public const string AppReleaseDate = "Mayo 2025";

        /// <summary>
        /// Nombre de la compañía
        /// </summary>
        public const string CompanyName = "Empresa de Facturación S.A.S";

        #endregion

        #region Parámetros de Negocio

        /// <summary>
        /// Porcentaje de IVA aplicado a los productos (19%)
        /// </summary>
        public const decimal IvaPercentage = 19m;

        /// <summary>
        /// Porcentaje de descuento para facturas que superan el monto mínimo (5%)
        /// </summary>
        public const decimal DiscountPercentage = 5m;

        /// <summary>
        /// Monto mínimo para aplicar descuento ($500,000)
        /// </summary>
        public const decimal MinAmountForDiscount = 500000m;

        /// <summary>
        /// Cantidad máxima de artículos por factura
        /// </summary>
        public const int MaxItemsPerInvoice = 50;

        /// <summary>
        /// Número máximo de facturas por cliente por día
        /// </summary>
        public const int MaxInvoicesPerClientPerDay = 10;

        /// <summary>
        /// Valor máximo permitido para un artículo
        /// </summary>
        public const decimal MaxArticlePrice = 100000000m;

        /// <summary>
        /// Número máximo de decimales para montos monetarios
        /// </summary>
        public const int DecimalPlaces = 2;

        #endregion

        #region Formatos

        /// <summary>
        /// Formato corto de fecha (DD/MM/YYYY)
        /// </summary>
        public const string ShortDateFormat = "dd/MM/yyyy";

        /// <summary>
        /// Formato largo de fecha (DD de Mes de YYYY)
        /// </summary>
        public const string LongDateFormat = "dd 'de' MMMM 'de' yyyy";

        /// <summary>
        /// Formato de fecha y hora (DD/MM/YYYY HH:MM)
        /// </summary>
        public const string DateTimeFormat = "dd/MM/yyyy HH:mm";

        /// <summary>
        /// Formato de fecha para base de datos (YYYY-MM-DD)
        /// </summary>
        public const string DatabaseDateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Formato de moneda ($#,##0.00)
        /// </summary>
        public const string CurrencyFormat = "$#,##0.00";

        /// <summary>
        /// Formato de porcentaje (0.00%)
        /// </summary>
        public const string PercentFormat = "0.00'%'";

        /// <summary>
        /// Formato de cantidad (#,##0)
        /// </summary>
        public const string QuantityFormat = "#,##0";

        #endregion

        #region Validaciones

        /// <summary>
        /// Longitud máxima para documento de identidad
        /// </summary>
        public const int MaxDocumentLength = 15;

        /// <summary>
        /// Longitud máxima para nombres
        /// </summary>
        public const int MaxNameLength = 100;

        /// <summary>
        /// Longitud máxima para apellidos
        /// </summary>
        public const int MaxLastNameLength = 100;

        /// <summary>
        /// Longitud máxima para dirección
        /// </summary>
        public const int MaxAddressLength = 250;

        /// <summary>
        /// Longitud máxima para teléfono
        /// </summary>
        public const int MaxPhoneLength = 20;

        /// <summary>
        /// Longitud máxima para código de artículo
        /// </summary>
        public const int MaxArticleCodeLength = 20;

        /// <summary>
        /// Longitud máxima para nombre de artículo
        /// </summary>
        public const int MaxArticleNameLength = 100;

        /// <summary>
        /// Longitud máxima para descripción de artículo
        /// </summary>
        public const int MaxArticleDescriptionLength = 250;

        /// <summary>
        /// Longitud máxima para observaciones de factura
        /// </summary>
        public const int MaxInvoiceObservationsLength = 500;

        /// <summary>
        /// Longitud máxima para nombre de usuario
        /// </summary>
        public const int MaxUsernameLength = 50;

        /// <summary>
        /// Longitud máxima para correo electrónico
        /// </summary>
        public const int MaxEmailLength = 100;

        /// <summary>
        /// Cantidad mínima de caracteres para búsqueda
        /// </summary>
        public const int MinSearchLength = 3;

        #endregion

        #region Mensajes

        /// <summary>
        /// Mensaje para factura guardada con éxito
        /// </summary>
        public const string InvoiceSavedMessage = "La factura ha sido guardada con éxito";

        /// <summary>
        /// Mensaje para factura anulada con éxito
        /// </summary>
        public const string InvoiceCancelledMessage = "La factura ha sido anulada con éxito";

        /// <summary>
        /// Mensaje para confirmación de anulación de factura
        /// </summary>
        public const string ConfirmCancelInvoiceMessage = "¿Está seguro de que desea anular esta factura? Esta acción no se puede deshacer.";

        /// <summary>
        /// Mensaje para confirmación de eliminación de artículo
        /// </summary>
        public const string ConfirmDeleteItemMessage = "¿Está seguro de que desea eliminar este artículo de la factura?";

        /// <summary>
        /// Mensaje para confirmación de limpieza de formulario
        /// </summary>
        public const string ConfirmClearFormMessage = "¿Está seguro de que desea limpiar el formulario? Todos los datos ingresados se perderán.";

        /// <summary>
        /// Mensaje para error de validación
        /// </summary>
        public const string ValidationErrorMessage = "Por favor corrija los errores en el formulario antes de continuar.";

        /// <summary>
        /// Mensaje para error de conexión con el servidor
        /// </summary>
        public const string ServerConnectionErrorMessage = "No se pudo conectar con el servidor. Por favor intente nuevamente más tarde.";

        /// <summary>
        /// Mensaje para error de autenticación
        /// </summary>
        public const string AuthenticationErrorMessage = "Las credenciales proporcionadas no son válidas. Por favor intente nuevamente.";

        /// <summary>
        /// Mensaje para sesión expirada
        /// </summary>
        public const string SessionExpiredMessage = "Su sesión ha expirado. Por favor inicie sesión nuevamente.";

        /// <summary>
        /// Mensaje para cliente no encontrado
        /// </summary>
        public const string ClientNotFoundMessage = "No se encontró un cliente con el número de documento ingresado.";

        /// <summary>
        /// Mensaje para artículo no encontrado
        /// </summary>
        public const string ArticleNotFoundMessage = "No se encontró un artículo con el código ingresado.";

        /// <summary>
        /// Mensaje para stock insuficiente
        /// </summary>
        public const string InsufficientStockMessage = "No hay suficiente stock disponible para el artículo seleccionado.";

        /// <summary>
        /// Mensaje para artículo agregado correctamente
        /// </summary>
        public const string ArticleAddedMessage = "El artículo ha sido agregado correctamente a la factura.";

        #endregion

        #region Claves de Sesión y Cookies

        /// <summary>
        /// Clave para token de autenticación
        /// </summary>
        public const string AuthTokenKey = "AuthToken";

        /// <summary>
        /// Clave para datos de usuario en sesión
        /// </summary>
        public const string UserDataKey = "UserData";

        /// <summary>
        /// Clave para carrito de compras en sesión
        /// </summary>
        public const string CartKey = "ShoppingCart";

        /// <summary>
        /// Clave para URL de retorno después de login
        /// </summary>
        public const string ReturnUrlKey = "ReturnUrl";

        /// <summary>
        /// Tiempo de vida de la cookie de autenticación en minutos
        /// </summary>
        public const int AuthCookieLifetimeMinutes = 60;

        #endregion

        #region URLs

        /// <summary>
        /// URL de la página de inicio
        /// </summary>
        public const string HomeUrl = "~/Default.aspx";

        /// <summary>
        /// URL de la página de login
        /// </summary>
        public const string LoginUrl = "~/Pages/Auth/Login.aspx";

        /// <summary>
        /// URL de la página de logout
        /// </summary>
        public const string LogoutUrl = "~/Pages/Auth/Logout.aspx";

        /// <summary>
        /// URL de la página de creación de factura
        /// </summary>
        public const string CreateInvoiceUrl = "~/Pages/Facturas/CrearFactura.aspx";

        /// <summary>
        /// URL de la página de listado de facturas
        /// </summary>
        public const string ListInvoicesUrl = "~/Pages/Facturas/ListarFacturas.aspx";

        /// <summary>
        /// URL de la página de detalle de factura
        /// </summary>
        public const string InvoiceDetailUrl = "~/Pages/Facturas/DetalleFactura.aspx";

        /// <summary>
        /// URL de la página de listado de clientes
        /// </summary>
        public const string ListClientsUrl = "~/Pages/Clientes/ListarClientes.aspx";

        /// <summary>
        /// URL de la página de gestión de cliente
        /// </summary>
        public const string ManageClientUrl = "~/Pages/Clientes/GestionarCliente.aspx";

        /// <summary>
        /// URL de la página de listado de artículos
        /// </summary>
        public const string ListArticlesUrl = "~/Pages/Articulos/ListarArticulos.aspx";

        /// <summary>
        /// URL de la página de gestión de artículo
        /// </summary>
        public const string ManageArticleUrl = "~/Pages/Articulos/GestionarArticulo.aspx";

        /// <summary>
        /// URL de la página de dashboard
        /// </summary>
        public const string DashboardUrl = "~/Pages/Reportes/Dashboard.aspx";

        #endregion

        #region IDs de Control

        /// <summary>
        /// ID del control de grilla de artículos en factura
        /// </summary>
        public const string InvoiceItemsGridId = "gvDetallesFactura";

        /// <summary>
        /// ID del panel de mensajes
        /// </summary>
        public const string MessagePanelId = "pnlMensaje";

        /// <summary>
        /// ID del label de mensaje
        /// </summary>
        public const string MessageLabelId = "lblMensaje";

        /// <summary>
        /// ID del panel de errores
        /// </summary>
        public const string ErrorPanelId = "pnlError";

        /// <summary>
        /// ID del label de error
        /// </summary>
        public const string ErrorLabelId = "lblError";

        #endregion

        #region Estados de Factura

        /// <summary>
        /// Estado de factura activa
        /// </summary>
        public const string InvoiceStatusActive = "Activa";

        /// <summary>
        /// Estado de factura anulada
        /// </summary>
        public const string InvoiceStatusCancelled = "Anulada";

        /// <summary>
        /// Estado de factura pendiente
        /// </summary>
        public const string InvoiceStatusPending = "Pendiente";

        #endregion

        #region Roles de Usuario

        /// <summary>
        /// Rol de administrador
        /// </summary>
        public const string RoleAdmin = "Administrador";

        /// <summary>
        /// Rol de vendedor
        /// </summary>
        public const string RoleSeller = "Vendedor";

        /// <summary>
        /// Rol de supervisor
        /// </summary>
        public const string RoleSupervisor = "Supervisor";

        /// <summary>
        /// Rol de cliente
        /// </summary>
        public const string RoleClient = "Cliente";

        #endregion

        #region Opciones de Paginación

        /// <summary>
        /// Tamaño de página por defecto
        /// </summary>
        public const int DefaultPageSize = 10;

        /// <summary>
        /// Opciones de tamaño de página
        /// </summary>
        public static readonly int[] PageSizeOptions = { 10, 20, 50, 100 };

        /// <summary>
        /// Número máximo de páginas a mostrar en la paginación
        /// </summary>
        public const int MaxPagesToShow = 5;

        #endregion
    }
}