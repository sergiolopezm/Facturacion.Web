namespace Facturacion.Web.Models.DTOs.Facturas
{
    /// <summary>
    /// DTO para totales de factura
    /// </summary>
    public class FacturaTotalesDto
    {
        public decimal Subtotal { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal ValorDescuento { get; set; }
        public decimal BaseImpuestos { get; set; }
        public decimal PorcentajeIVA { get; set; }
        public decimal ValorIVA { get; set; }
        public decimal Total { get; set; }
    }
}