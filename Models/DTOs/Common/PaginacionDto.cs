using System.Collections.Generic;

namespace Facturacion.Web.Models.DTOs.Common
{
    /// <summary>
    /// DTO para manejo de paginación
    /// </summary>
    /// <typeparam name="T">Tipo de datos en la lista</typeparam>
    public class PaginacionDto<T>
    {
        public int Pagina { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int ElementosPorPagina { get; set; }
        public List<T> Lista { get; set; }

        public PaginacionDto()
        {
            Lista = new List<T>();
        }

        public bool TienePaginaAnterior => Pagina > 1;
        public bool TienePaginaSiguiente => Pagina < TotalPaginas;
        public int PaginaAnterior => TienePaginaAnterior ? Pagina - 1 : 1;
        public int PaginaSiguiente => TienePaginaSiguiente ? Pagina + 1 : TotalPaginas;
    }
}