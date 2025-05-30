using System;

namespace Facturacion.Web.Models.DTOs.Reportes
{
    /// <summary>
    /// DTO para resumen mensual
    /// </summary>
    public class ResumenMensualDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal TotalVentas { get; set; }
        public string TotalVentasFormateado { get; set; }
        public int TotalFacturas { get; set; }
        public decimal TotalIVA { get; set; }
        public string TotalIVAFormateado { get; set; }
        public decimal TotalDescuentos { get; set; }
        public string TotalDescuentosFormateado { get; set; }

        public ResumenMensualDto()
        {
            TotalVentasFormateado = string.Empty;
            TotalIVAFormateado = string.Empty;
            TotalDescuentosFormateado = string.Empty;
        }
    }
}