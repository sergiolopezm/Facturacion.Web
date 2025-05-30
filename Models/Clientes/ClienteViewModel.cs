using Facturacion.Web.Models.DTOs.Clientes;
using Facturacion.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.UI.WebControls;

namespace Facturacion.Web.Models.Clientes
{
    /// <summary>
    /// ViewModel para la gestión de clientes en la interfaz de usuario
    /// Contiene propiedades extendidas con validaciones y conversiones específicas para el frontend
    /// </summary>
    public class ClienteViewModel
    {
        #region Propiedades básicas

        public int Id { get; set; }

        [Required(ErrorMessage = "El número de documento es requerido")]
        [StringLength(15, ErrorMessage = "El número de documento no puede exceder los 15 caracteres")]
        [Display(Name = "Número de Documento*")]
        public string NumeroDocumento { get; set; }

        [Required(ErrorMessage = "Los nombres son requeridos")]
        [StringLength(100, ErrorMessage = "Los nombres no pueden exceder los 100 caracteres")]
        [Display(Name = "Nombres*")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder los 100 caracteres")]
        [Display(Name = "Apellidos*")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(250, ErrorMessage = "La dirección no puede exceder los 250 caracteres")]
        [Display(Name = "Dirección*")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        [RegularExpression(@"^[0-9\(\)\+\-\s]+$", ErrorMessage = "El teléfono solo debe contener números, espacios y los caracteres ()+-")]
        [Display(Name = "Teléfono*")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        #endregion

        #region Propiedades calculadas y de UI

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto => $"{Nombres} {Apellidos}";

        [Display(Name = "Creado Por")]
        public string CreadoPor { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Modificado Por")]
        public string ModificadoPor { get; set; }

        [Display(Name = "Fecha Modificación")]
        public DateTime? FechaModificacion { get; set; }

        [Display(Name = "Total Facturas")]
        public int TotalFacturas { get; set; }

        [Display(Name = "Monto Total Compras")]
        public decimal MontoTotalCompras { get; set; }

        [Display(Name = "Monto Total Compras")]
        public string MontoTotalComprasFormateado => CurrencyHelper.FormatCurrency(MontoTotalCompras);

        #endregion

        #region Estados para UI

        public bool EsNuevo => Id == 0;
        public bool TieneFacturas => TotalFacturas > 0;
        public bool TieneCompras => MontoTotalCompras > 0;
        public string EstadoActivo => Activo ? "Activo" : "Inactivo";
        public string EstadoClase => Activo ? "estado-activo" : "estado-inactivo";

        #endregion

        #region Constructores

        public ClienteViewModel()
        {
            // Inicializar con valores por defecto
            Id = 0;
            NumeroDocumento = string.Empty;
            Nombres = string.Empty;
            Apellidos = string.Empty;
            Direccion = string.Empty;
            Telefono = string.Empty;
            Email = string.Empty;
            CreadoPor = string.Empty;
            ModificadoPor = string.Empty;
            FechaCreacion = DateTime.Now;
            Activo = true;
        }

        /// <summary>
        /// Constructor que crea el ViewModel a partir de un DTO
        /// </summary>
        public ClienteViewModel(ClienteDto dto)
        {
            if (dto != null)
            {
                Id = dto.Id;
                NumeroDocumento = dto.NumeroDocumento;
                Nombres = dto.Nombres;
                Apellidos = dto.Apellidos;
                Direccion = dto.Direccion;
                Telefono = dto.Telefono;
                Email = dto.Email;
                Activo = dto.Activo;
                FechaCreacion = dto.FechaCreacion;
                FechaModificacion = dto.FechaModificacion;
                CreadoPor = dto.CreadoPor;
                ModificadoPor = dto.ModificadoPor;
                TotalFacturas = dto.TotalFacturas;
                MontoTotalCompras = dto.MontoTotalCompras;
            }
        }

        #endregion

        #region Métodos de conversión

        /// <summary>
        /// Convierte el ViewModel a un DTO para enviar al backend
        /// </summary>
        public ClienteDto ToDto()
        {
            return new ClienteDto
            {
                Id = this.Id,
                NumeroDocumento = this.NumeroDocumento?.Trim(),
                Nombres = this.Nombres?.Trim(),
                Apellidos = this.Apellidos?.Trim(),
                Direccion = this.Direccion?.Trim(),
                Telefono = this.Telefono?.Trim(),
                Email = string.IsNullOrWhiteSpace(this.Email) ? null : this.Email.Trim(),
                Activo = this.Activo
            };
        }

        /// <summary>
        /// Crea una lista de ViewModels a partir de una lista de DTOs
        /// </summary>
        public static List<ClienteViewModel> FromDtoList(List<ClienteDto> dtos)
        {
            if (dtos == null) return new List<ClienteViewModel>();

            var result = new List<ClienteViewModel>();
            foreach (var dto in dtos)
            {
                result.Add(new ClienteViewModel(dto));
            }
            return result;
        }

        #endregion

        #region Métodos de validación para WebForms

        /// <summary>
        /// Valida que el teléfono tenga un formato correcto
        /// </summary>
        public static bool ValidarTelefono(string telefono, out string mensaje)
        {
            mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(telefono))
            {
                mensaje = "El teléfono es requerido";
                return false;
            }

            // Verificar que solo contenga números, espacios y caracteres ()+-
            if (!System.Text.RegularExpressions.Regex.IsMatch(telefono, @"^[0-9\(\)\+\-\s]+$"))
            {
                mensaje = "El teléfono solo debe contener números, espacios y los caracteres ()+-";
                return false;
            }

            // Extraer solo los números para validar longitud
            var soloNumeros = System.Text.RegularExpressions.Regex.Replace(telefono, @"[^\d]", "");
            if (soloNumeros.Length < 7)
            {
                mensaje = "El teléfono debe tener al menos 7 dígitos";
                return false;
            }

            if (soloNumeros.Length > 15)
            {
                mensaje = "El teléfono no puede tener más de 15 dígitos";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida que el email tenga un formato correcto
        /// </summary>
        public static bool ValidarEmail(string email, out string mensaje)
        {
            mensaje = string.Empty;

            // Si el email está vacío, es válido (no es obligatorio)
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    mensaje = "El formato del email no es válido";
                    return false;
                }
                return true;
            }
            catch
            {
                mensaje = "El formato del email no es válido";
                return false;
            }
        }

        /// <summary>
        /// Valida el modelo completo para formularios WebForms
        /// </summary>
        public bool Validar(out List<string> errores)
        {
            errores = new List<string>();

            // Validaciones de campos requeridos
            if (string.IsNullOrWhiteSpace(NumeroDocumento))
                errores.Add("El número de documento es requerido");

            if (string.IsNullOrWhiteSpace(Nombres))
                errores.Add("Los nombres son requeridos");

            if (string.IsNullOrWhiteSpace(Apellidos))
                errores.Add("Los apellidos son requeridos");

            if (string.IsNullOrWhiteSpace(Direccion))
                errores.Add("La dirección es requerida");

            if (string.IsNullOrWhiteSpace(Telefono))
                errores.Add("El teléfono es requerido");

            // Validaciones de formato
            if (!string.IsNullOrWhiteSpace(Telefono))
            {
                string mensajeTelefono;
                if (!ValidarTelefono(Telefono, out mensajeTelefono))
                    errores.Add(mensajeTelefono);
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                string mensajeEmail;
                if (!ValidarEmail(Email, out mensajeEmail))
                    errores.Add(mensajeEmail);
            }

            if (NumeroDocumento?.Length > 15)
                errores.Add("El número de documento no puede exceder los 15 caracteres");

            if (Nombres?.Length > 100)
                errores.Add("Los nombres no pueden exceder los 100 caracteres");

            if (Apellidos?.Length > 100)
                errores.Add("Los apellidos no pueden exceder los 100 caracteres");

            if (Direccion?.Length > 250)
                errores.Add("La dirección no puede exceder los 250 caracteres");

            if (Email?.Length > 100)
                errores.Add("El email no puede exceder los 100 caracteres");

            return errores.Count == 0;
        }

        #endregion

        #region Métodos para la UI WebForms

        /// <summary>
        /// Rellena un DropDownList con la lista de clientes
        /// </summary>
        public static void RellenarDropDownList(DropDownList dropDownList, List<ClienteViewModel> clientes, bool incluirSeleccione = true)
        {
            dropDownList.Items.Clear();

            if (incluirSeleccione)
            {
                dropDownList.Items.Add(new ListItem("-- Seleccione un cliente --", "0"));
            }

            if (clientes != null && clientes.Count > 0)
            {
                foreach (var cliente in clientes.OrderBy(c => c.NombreCompleto))
                {
                    string texto = $"{cliente.NombreCompleto} ({cliente.NumeroDocumento})";
                    dropDownList.Items.Add(new ListItem(texto, cliente.Id.ToString()));
                }
            }
        }

        /// <summary>
        /// Busca un cliente por su número de documento
        /// </summary>
        public static ClienteViewModel BuscarPorDocumento(List<ClienteViewModel> clientes, string numeroDocumento)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento) || clientes == null || clientes.Count == 0)
                return null;

            return clientes.FirstOrDefault(c => c.NumeroDocumento.Equals(numeroDocumento.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}