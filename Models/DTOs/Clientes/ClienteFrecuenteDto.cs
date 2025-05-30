using System;

namespace Facturacion.Web.Models.DTOs.Clientes
{
    /// <summary>
    /// DTO para clientes frecuentes
    /// </summary>
    public class ClienteFrecuenteDto
    {
        public int ClienteId { get; set; }
        public string NombreCompleto { get; set; }
        public string NumeroDocumento { get; set; }
        public int TotalFacturas { get; set; }
        public decimal MontoTotalCompras { get; set; }
        public string MontoTotalComprasFormateado { get; set; }
        public DateTime? UltimaCompra { get; set; }

        public ClienteFrecuenteDto()
        {
            NombreCompleto = string.Empty;
            NumeroDocumento = string.Empty;
            MontoTotalComprasFormateado = string.Empty;
        }
    }
}