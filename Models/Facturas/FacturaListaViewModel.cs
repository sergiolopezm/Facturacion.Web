using Facturacion.Web.Models.DTOs.Common;
using Facturacion.Web.Models.DTOs.Facturas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Web.Models.Facturas
{
    /// <summary>
    /// Modelo de vista para la pantalla de listado de facturas
    /// Contiene los datos necesarios para mostrar, filtrar y paginar las facturas
    /// </summary>
    public class FacturaListaViewModel
    {
        #region Propiedades de Paginación

        /// <summary>
        /// Número de página actual
        /// </summary>
        public int PaginaActual { get; set; }

        /// <summary>
        /// Número total de páginas
        /// </summary>
        public int TotalPaginas { get; set; }

        /// <summary>
        /// Número total de registros
        /// </summary>
        public int TotalRegistros { get; set; }

        /// <summary>
        /// Cantidad de elementos por página
        /// </summary>
        public int ElementosPorPagina { get; set; }

        /// <summary>
        /// Indica si hay página anterior
        /// </summary>
        public bool TienePaginaAnterior => PaginaActual > 1;

        /// <summary>
        /// Indica si hay página siguiente
        /// </summary>
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

        /// <summary>
        /// Número de página anterior
        /// </summary>
        public int PaginaAnterior => TienePaginaAnterior ? PaginaActual - 1 : 1;

        /// <summary>
        /// Número de página siguiente
        /// </summary>
        public int PaginaSiguiente => TienePaginaSiguiente ? PaginaActual + 1 : TotalPaginas;

        #endregion

        #region Propiedades de Filtros

        /// <summary>
        /// Texto de búsqueda para filtrar las facturas
        /// </summary>
        [Display(Name = "Búsqueda")]
        public string Busqueda { get; set; }

        /// <summary>
        /// Fecha de inicio para filtrar
        /// </summary>
        [Display(Name = "Fecha Inicial")]
        public DateTime? FechaInicio { get; set; }

        /// <summary>
        /// Fecha de fin para filtrar
        /// </summary>
        [Display(Name = "Fecha Final")]
        public DateTime? FechaFin { get; set; }

        /// <summary>
        /// Estado de la factura para filtrar
        /// </summary>
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        /// <summary>
        /// Lista de estados disponibles para el dropdown
        /// </summary>
        public List<KeyValuePair<string, string>> EstadosDisponibles { get; set; }

        #endregion

        #region Propiedades de Datos

        /// <summary>
        /// Lista de facturas para mostrar en la grilla
        /// </summary>
        public List<FacturaResumenDto> Facturas { get; set; }

        /// <summary>
        /// Mensaje para mostrar al usuario (éxito, error, etc.)
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// Tipo de mensaje (success, error, warning, info)
        /// </summary>
        public string TipoMensaje { get; set; }

        #endregion

        #region Propiedades de UI

        /// <summary>
        /// Indicador de carga para la interfaz
        /// </summary>
        public bool Cargando { get; set; }

        /// <summary>
        /// ID de la factura seleccionada (para ver detalles o anular)
        /// </summary>
        public int FacturaSeleccionadaId { get; set; }

        /// <summary>
        /// Número de la factura seleccionada
        /// </summary>
        public string FacturaSeleccionadaNumero { get; set; }

        /// <summary>
        /// Indica si el modal de confirmación está visible
        /// </summary>
        public bool MostrarModalConfirmacion { get; set; }

        /// <summary>
        /// Mensaje para el modal de confirmación
        /// </summary>
        public string MensajeConfirmacion { get; set; }

        /// <summary>
        /// Acción a realizar tras la confirmación
        /// </summary>
        public string AccionConfirmacion { get; set; }

        #endregion

        #region Propiedades de Resumen

        /// <summary>
        /// Cantidad total de facturas
        /// </summary>
        public int CantidadFacturas { get; set; }

        /// <summary>
        /// Monto total de facturas
        /// </summary>
        public decimal MontoTotal { get; set; }

        /// <summary>
        /// Monto total formateado como moneda
        /// </summary>
        public string MontoTotalFormateado { get; set; }

        /// <summary>
        /// Cantidad de facturas activas
        /// </summary>
        public int FacturasActivas { get; set; }

        /// <summary>
        /// Cantidad de facturas anuladas
        /// </summary>
        public int FacturasAnuladas { get; set; }

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public FacturaListaViewModel()
        {
            // Inicializar listas
            Facturas = new List<FacturaResumenDto>();
            EstadosDisponibles = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("", "Todos"),
                new KeyValuePair<string, string>("Activa", "Activa"),
                new KeyValuePair<string, string>("Anulada", "Anulada")
            };

            // Inicializar valores por defecto
            PaginaActual = 1;
            ElementosPorPagina = 10;
            TotalPaginas = 1;
            TotalRegistros = 0;

            // Inicializar filtros
            Busqueda = string.Empty;
            Estado = string.Empty;

            // Por defecto, filtrar el último mes
            FechaInicio = DateTime.Now.AddMonths(-1);
            FechaFin = DateTime.Now;

            // Inicializar mensajes
            Mensaje = string.Empty;
            TipoMensaje = string.Empty;
            MensajeConfirmacion = string.Empty;
            AccionConfirmacion = string.Empty;

            // Inicializar propiedades de UI
            Cargando = false;
            MostrarModalConfirmacion = false;
            FacturaSeleccionadaId = 0;
            FacturaSeleccionadaNumero = string.Empty;

            // Inicializar resumen
            CantidadFacturas = 0;
            MontoTotal = 0;
            MontoTotalFormateado = "$0";
            FacturasActivas = 0;
            FacturasAnuladas = 0;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Actualiza el modelo de vista con datos de paginación
        /// </summary>
        /// <param name="paginacion">Datos de paginación</param>
        public void ActualizarPaginacion(PaginacionDto<FacturaResumenDto> paginacion)
        {
            if (paginacion == null)
                return;

            // Actualizar propiedades de paginación
            PaginaActual = paginacion.Pagina;
            TotalPaginas = paginacion.TotalPaginas;
            TotalRegistros = paginacion.TotalRegistros;
            ElementosPorPagina = paginacion.ElementosPorPagina;

            // Actualizar lista de facturas
            Facturas = paginacion.Lista ?? new List<FacturaResumenDto>();

            // Actualizar resumen
            CalcularResumen();
        }

        /// <summary>
        /// Calcula el resumen de facturas (total, montos, etc.)
        /// </summary>
        public void CalcularResumen()
        {
            CantidadFacturas = TotalRegistros;

            // Calcular montos y cantidades por estado
            MontoTotal = 0;
            FacturasActivas = 0;
            FacturasAnuladas = 0;

            foreach (var factura in Facturas)
            {
                MontoTotal += factura.Total;

                if (factura.Estado == "Activa")
                    FacturasActivas++;
                else if (factura.Estado == "Anulada")
                    FacturasAnuladas++;
            }

            // Formatear monto
            MontoTotalFormateado = FormatearMoneda(MontoTotal);
        }

        /// <summary>
        /// Prepara el modelo para anular una factura
        /// </summary>
        /// <param name="facturaId">ID de la factura a anular</param>
        /// <param name="numeroFactura">Número de la factura</param>
        public void PrepararAnulacion(int facturaId, string numeroFactura)
        {
            FacturaSeleccionadaId = facturaId;
            FacturaSeleccionadaNumero = numeroFactura;
            MensajeConfirmacion = $"¿Está seguro que desea anular la factura {numeroFactura}?";
            AccionConfirmacion = "Anular";
            MostrarModalConfirmacion = true;
        }

        /// <summary>
        /// Limpia los filtros de búsqueda
        /// </summary>
        public void LimpiarFiltros()
        {
            Busqueda = string.Empty;
            Estado = string.Empty;
            FechaInicio = DateTime.Now.AddMonths(-1);
            FechaFin = DateTime.Now;
            PaginaActual = 1;
        }

        /// <summary>
        /// Formatea un valor decimal como moneda
        /// </summary>
        /// <param name="valor">Valor a formatear</param>
        /// <returns>Valor formateado</returns>
        private string FormatearMoneda(decimal valor)
        {
            return string.Format("${0:N0}", valor);
        }

        #endregion
    }
}