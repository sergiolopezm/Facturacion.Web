using System;

namespace Facturacion.Web.Models.DTOs.Reportes
{
    /// <summary>
    /// DTO para resumen diario
    /// </summary>
    public class ResumenDiarioDto
    {
        public DateTime Fecha { get; set; }
        public decimal TotalVentas { get; set; }
        public string TotalVentasFormateado { get; set; }
        public int TotalFacturas { get; set; }

        public ResumenDiarioDto()
        {
            TotalVentasFormateado = string.Empty;
        }
    }
}