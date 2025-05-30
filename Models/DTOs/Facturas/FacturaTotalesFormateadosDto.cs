namespace Facturacion.Web.Models.DTOs.Facturas
{
    /// <summary>
    /// DTO para totales formateados de factura
    /// </summary>
    public class FacturaTotalesFormateadosDto
    {
        public string Subtotal { get; set; }
        public string PorcentajeDescuento { get; set; }
        public string ValorDescuento { get; set; }
        public string BaseImpuestos { get; set; }
        public string PorcentajeIVA { get; set; }
        public string ValorIVA { get; set; }
        public string Total { get; set; }

        public FacturaTotalesFormateadosDto()
        {
            Subtotal = string.Empty;
            PorcentajeDescuento = string.Empty;
            ValorDescuento = string.Empty;
            BaseImpuestos = string.Empty;
            PorcentajeIVA = string.Empty;
            ValorIVA = string.Empty;
            Total = string.Empty;
        }
    }
}