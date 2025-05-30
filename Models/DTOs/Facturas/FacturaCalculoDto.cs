using System.Collections.Generic;

namespace Facturacion.Web.Models.DTOs.Facturas
{
    /// <summary>
    /// DTO para cálculos de factura
    /// </summary>
    public class FacturaCalculoDto
    {
        public List<CrearFacturaDetalleDto> Detalles { get; set; }
        public FacturaTotalesDto Totales { get; set; }
        public FacturaTotalesFormateadosDto TotalesFormateados { get; set; }

        public FacturaCalculoDto()
        {
            Detalles = new List<CrearFacturaDetalleDto>();
            Totales = new FacturaTotalesDto();
            TotalesFormateados = new FacturaTotalesFormateadosDto();
        }
    }
}