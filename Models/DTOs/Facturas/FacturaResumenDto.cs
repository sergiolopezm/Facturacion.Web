using System;

namespace Facturacion.Web.Models.DTOs.Facturas
{
    /// <summary>
    /// DTO para resumen de factura en listas
    /// </summary>
    public class FacturaResumenDto
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime Fecha { get; set; }
        public string ClienteNombreCompleto { get; set; }
        public string ClienteNumeroDocumento { get; set; }
        public decimal Total { get; set; }
        public string TotalFormateado { get; set; }
        public string Estado { get; set; }
        public int TotalArticulos { get; set; }
        public string CreadoPor { get; set; }
        public string FechaFormateada { get; set; }

        public FacturaResumenDto()
        {
            NumeroFactura = string.Empty;
            ClienteNombreCompleto = string.Empty;
            ClienteNumeroDocumento = string.Empty;
            TotalFormateado = string.Empty;
            Estado = string.Empty;
            CreadoPor = string.Empty;
            FechaFormateada = string.Empty;
        }
    }
}