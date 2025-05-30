using System;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Web.Models.DTOs.Clientes
{
    /// <summary>
    /// DTO para cliente
    /// </summary>
    public class ClienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de documento es requerido")]
        [StringLength(15, ErrorMessage = "El número de documento no puede exceder los 15 caracteres")]
        [Display(Name = "Número de Documento")]
        public string NumeroDocumento { get; set; }

        [Required(ErrorMessage = "Los nombres son requeridos")]
        [StringLength(100, ErrorMessage = "Los nombres no pueden exceder los 100 caracteres")]
        [Display(Name = "Nombres")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder los 100 caracteres")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [StringLength(250, ErrorMessage = "La dirección no puede exceder los 250 caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Propiedades calculadas para UI
        public string NombreCompleto { get; set; }
        public string CreadoPor { get; set; }
        public string ModificadoPor { get; set; }
        public int TotalFacturas { get; set; }
        public decimal MontoTotalCompras { get; set; }
        public string MontoTotalComprasFormateado { get; set; }

        public ClienteDto()
        {
            NumeroDocumento = string.Empty;
            Nombres = string.Empty;
            Apellidos = string.Empty;
            Direccion = string.Empty;
            Telefono = string.Empty;
            Email = string.Empty;
            NombreCompleto = string.Empty;
            CreadoPor = string.Empty;
            ModificadoPor = string.Empty;
            MontoTotalComprasFormateado = string.Empty;
            Activo = true;
        }
    }
}