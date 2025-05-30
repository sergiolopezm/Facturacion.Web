using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Web.Models.DTOs.Facturas
{
    /// <summary>
    /// DTO para factura
    /// </summary>
    public class FacturaDto
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El cliente es requerido")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        // Datos del cliente al momento de la factura
        [Required(ErrorMessage = "El número de documento del cliente es requerido")]
        [Display(Name = "Número de Documento")]
        public string ClienteNumeroDocumento { get; set; }

        [Required(ErrorMessage = "Los nombres del cliente son requeridos")]
        [Display(Name = "Nombres")]
        public string ClienteNombres { get; set; }

        [Required(ErrorMessage = "Los apellidos del cliente son requeridos")]
        [Display(Name = "Apellidos")]
        public string ClienteApellidos { get; set; }

        [Required(ErrorMessage = "La dirección del cliente es requerida")]
        [Display(Name = "Dirección")]
        public string ClienteDireccion { get; set; }

        [Required(ErrorMessage = "El teléfono del cliente es requerido")]
        [Display(Name = "Teléfono")]
        public string ClienteTelefono { get; set; }

        // Totales calculados
        public decimal SubTotal { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal ValorDescuento { get; set; }
        public decimal BaseImpuestos { get; set; }
        public decimal PorcentajeIVA { get; set; }
        public decimal ValorIVA { get; set; }
        public decimal Total { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres")]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        public string Estado { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Propiedades de navegación para UI
        public string ClienteNombreCompleto { get; set; }
        public string CreadoPor { get; set; }
        public string ModificadoPor { get; set; }
        public List<FacturaDetalleDto> Detalles { get; set; }
        public int TotalArticulos { get; set; }
        public int TotalCantidad { get; set; }

        // Propiedades formateadas para mostrar
        public string SubTotalFormateado { get; set; }
        public string ValorDescuentoFormateado { get; set; }
        public string ValorIVAFormateado { get; set; }
        public string TotalFormateado { get; set; }
        public string FechaFormateada { get; set; }

        public FacturaDto()
        {
            NumeroFactura = string.Empty;
            ClienteNumeroDocumento = string.Empty;
            ClienteNombres = string.Empty;
            ClienteApellidos = string.Empty;
            ClienteDireccion = string.Empty;
            ClienteTelefono = string.Empty;
            Observaciones = string.Empty;
            Estado = "Activa";
            ClienteNombreCompleto = string.Empty;
            CreadoPor = string.Empty;
            ModificadoPor = string.Empty;
            Detalles = new List<FacturaDetalleDto>();
            SubTotalFormateado = string.Empty;
            ValorDescuentoFormateado = string.Empty;
            ValorIVAFormateado = string.Empty;
            TotalFormateado = string.Empty;
            FechaFormateada = string.Empty;
            Activo = true;
        }
    }
}