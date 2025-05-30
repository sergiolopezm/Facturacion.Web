namespace Facturacion.Web.Models.DTOs.Articulos
{
    /// <summary>
    /// DTO para artículos vendidos
    /// </summary>
    public class ArticuloVendidoDto
    {
        public int ArticuloId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int CantidadVendida { get; set; }
        public decimal MontoVendido { get; set; }
        public string MontoVendidoFormateado { get; set; }
        public int VecesVendido { get; set; }

        public ArticuloVendidoDto()
        {
            Codigo = string.Empty;
            Nombre = string.Empty;
            MontoVendidoFormateado = string.Empty;
        }
    }
}