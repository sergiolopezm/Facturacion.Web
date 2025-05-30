using Facturacion.Web.Models.DTOs.Articulos;
using Facturacion.Web.Models.DTOs.Clientes;
using System;
using System.Collections.Generic;

namespace Facturacion.Web.Models.DTOs.Facturas
{
    /// <summary>
    /// DTO para reporte de ventas
    /// </summary>
    public class ReporteVentasDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalFacturas { get; set; }
        public decimal TotalVentas { get; set; }
        public string TotalVentasFormateado { get; set; }
        public decimal TotalIVA { get; set; }
        public string TotalIVAFormateado { get; set; }
        public decimal TotalDescuentos { get; set; }
        public string TotalDescuentosFormateado { get; set; }
        public List<ArticuloVendidoDto> ArticulosMasVendidos { get; set; }
        public List<ClienteFrecuenteDto> ClientesFrecuentes { get; set; }

        public ReporteVentasDto()
        {
            TotalVentasFormateado = string.Empty;
            TotalIVAFormateado = string.Empty;
            TotalDescuentosFormateado = string.Empty;
            ArticulosMasVendidos = new List<ArticuloVendidoDto>();
            ClientesFrecuentes = new List<ClienteFrecuenteDto>();
        }
    }
}