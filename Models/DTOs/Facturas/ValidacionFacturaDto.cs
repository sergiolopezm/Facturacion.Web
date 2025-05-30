using System.Collections.Generic;

namespace Facturacion.Web.Models.DTOs.Facturas
{
    /// <summary>
    /// DTO para validación de factura
    /// </summary>
    public class ValidacionFacturaDto
    {
        public bool EsValida { get; set; }
        public List<string> Errores { get; set; }
        public List<string> Advertencias { get; set; }
        public FacturaTotalesDto TotalesCalculados { get; set; }

        public ValidacionFacturaDto()
        {
            Errores = new List<string>();
            Advertencias = new List<string>();
            TotalesCalculados = new FacturaTotalesDto();
        }
    }
}