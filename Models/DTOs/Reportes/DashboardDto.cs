using Facturacion.Web.Models.DTOs.Articulos;
using Facturacion.Web.Models.DTOs.Clientes;
using System;
using System.Collections.Generic;

namespace Facturacion.Web.Models.DTOs.Reportes
{
    /// <summary>
    /// DTO para dashboard
    /// </summary>
    public class DashboardDto
    {
        public DateTime FechaGeneracion { get; set; }
        public ResumenDiarioDto ResumenHoy { get; set; }
        public ResumenMensualDto ResumenMes { get; set; }
        public Dictionary<string, decimal> VentasPorCategoria { get; set; }
        public List<ArticuloVendidoDto> ArticulosMasVendidos { get; set; }
        public List<ClienteFrecuenteDto> ClientesFrecuentes { get; set; }
        public int ArticulosStockBajo { get; set; }
        public Dictionary<string, int> FacturasPorEstado { get; set; }
        public Dictionary<string, decimal> VentasPorMes { get; set; }

        public DashboardDto()
        {
            ResumenHoy = new ResumenDiarioDto();
            ResumenMes = new ResumenMensualDto();
            VentasPorCategoria = new Dictionary<string, decimal>();
            ArticulosMasVendidos = new List<ArticuloVendidoDto>();
            ClientesFrecuentes = new List<ClienteFrecuenteDto>();
            FacturasPorEstado = new Dictionary<string, int>();
            VentasPorMes = new Dictionary<string, decimal>();
        }
    }
}
