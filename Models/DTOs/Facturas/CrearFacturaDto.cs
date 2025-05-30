using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Web.Models.DTOs.Facturas
{
    /// <summary>
    /// DTO para crear factura
    /// </summary>
    public class CrearFacturaDto
    {
        [Required(ErrorMessage = "El cliente es requerido")]
        public int ClienteId { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres")]
        public string Observaciones { get; set; }

        [Required(ErrorMessage = "Debe incluir al menos un artículo")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un artículo")]
        public List<CrearFacturaDetalleDto> Detalles { get; set; }

        public CrearFacturaDto()
        {
            Observaciones = string.Empty;
            Detalles = new List<CrearFacturaDetalleDto>();
        }
    }
}