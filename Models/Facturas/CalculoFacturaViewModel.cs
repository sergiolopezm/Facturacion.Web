using Facturacion.Web.Models.DTOs.Facturas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Facturacion.Web.Models.Facturas
{
    /// <summary>
    /// Modelo de vista para manejar los cálculos de la factura
    /// Implementa las reglas de negocio específicas para los cálculos (descuentos, IVA, etc.)
    /// </summary>
    public class CalculoFacturaViewModel
    {
        #region Constantes de Negocio

        /// <summary>
        /// Porcentaje de IVA aplicado (19% según requerimientos)
        /// </summary>
        public const decimal PORCENTAJE_IVA = 19m;

        /// <summary>
        /// Porcentaje de descuento aplicado si se cumple el monto mínimo (5% según requerimientos)
        /// </summary>
        public const decimal PORCENTAJE_DESCUENTO = 5m;

        /// <summary>
        /// Monto mínimo para aplicar descuento ($500,000 según requerimientos)
        /// </summary>
        public const decimal MONTO_MINIMO_DESCUENTO = 500000m;

        #endregion

        #region Propiedades de Entrada

        /// <summary>
        /// Lista de detalles para realizar los cálculos
        /// </summary>
        public List<DetalleCalculoViewModel> Detalles { get; set; }

        #endregion

        #region Propiedades de Totales Calculados

        /// <summary>
        /// Subtotal de la factura (suma de subtotales de los detalles)
        /// </summary>
        public decimal Subtotal { get; private set; }

        /// <summary>
        /// Porcentaje de descuento aplicado (5% si subtotal >= $500,000, 0% en caso contrario)
        /// </summary>
        public decimal PorcentajeDescuento { get; private set; }

        /// <summary>
        /// Valor del descuento calculado
        /// </summary>
        public decimal ValorDescuento { get; private set; }

        /// <summary>
        /// Base para cálculo de impuestos (Subtotal - Descuento)
        /// </summary>
        public decimal BaseImpuestos { get; private set; }

        /// <summary>
        /// Porcentaje de IVA (19% según requerimientos)
        /// </summary>
        public decimal PorcentajeIVA { get; private set; }

        /// <summary>
        /// Valor del IVA calculado
        /// </summary>
        public decimal ValorIVA { get; private set; }

        /// <summary>
        /// Total de la factura (Base + IVA)
        /// </summary>
        public decimal Total { get; private set; }

        #endregion

        #region Propiedades Formateadas

        /// <summary>
        /// Subtotal formateado como moneda
        /// </summary>
        public string SubtotalFormateado { get; private set; }

        /// <summary>
        /// Porcentaje de descuento formateado
        /// </summary>
        public string PorcentajeDescuentoFormateado { get; private set; }

        /// <summary>
        /// Valor del descuento formateado como moneda
        /// </summary>
        public string ValorDescuentoFormateado { get; private set; }

        /// <summary>
        /// Base imponible formateada como moneda
        /// </summary>
        public string BaseImpuestosFormateada { get; private set; }

        /// <summary>
        /// Porcentaje de IVA formateado
        /// </summary>
        public string PorcentajeIVAFormateado { get; private set; }

        /// <summary>
        /// Valor del IVA formateado como moneda
        /// </summary>
        public string ValorIVAFormateado { get; private set; }

        /// <summary>
        /// Total formateado como moneda
        /// </summary>
        public string TotalFormateado { get; private set; }

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public CalculoFacturaViewModel()
        {
            Detalles = new List<DetalleCalculoViewModel>();
            PorcentajeIVA = PORCENTAJE_IVA;
            InicializarValores();
        }

        /// <summary>
        /// Constructor con detalles
        /// </summary>
        /// <param name="detalles">Lista de detalles para el cálculo</param>
        public CalculoFacturaViewModel(List<DetalleCalculoViewModel> detalles)
        {
            Detalles = detalles ?? new List<DetalleCalculoViewModel>();
            PorcentajeIVA = PORCENTAJE_IVA;
            Calcular();
        }

        /// <summary>
        /// Constructor a partir de detalles de ViewModel
        /// </summary>
        /// <param name="detalles">Lista de detalles del FacturaViewModel</param>
        public CalculoFacturaViewModel(List<FacturaDetalleViewModel> detalles)
        {
            Detalles = detalles?.Select(d => new DetalleCalculoViewModel
            {
                ArticuloId = d.ArticuloId,
                ArticuloCodigo = d.ArticuloCodigo,
                ArticuloNombre = d.ArticuloNombre,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList() ?? new List<DetalleCalculoViewModel>();

            PorcentajeIVA = PORCENTAJE_IVA;
            Calcular();
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Calcula todos los valores basados en los detalles
        /// </summary>
        public void Calcular()
        {
            // Calcular subtotal sumando los subtotales de cada detalle
            Subtotal = Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

            // Aplicar regla de descuento (5% si subtotal >= $500,000)
            PorcentajeDescuento = Subtotal >= MONTO_MINIMO_DESCUENTO ? PORCENTAJE_DESCUENTO : 0;
            ValorDescuento = Math.Round(Subtotal * (PorcentajeDescuento / 100m), 2);

            // Calcular base imponible
            BaseImpuestos = Subtotal - ValorDescuento;

            // Calcular IVA (19%)
            ValorIVA = Math.Round(BaseImpuestos * (PorcentajeIVA / 100m), 2);

            // Calcular total
            Total = BaseImpuestos + ValorIVA;

            // Actualizar valores formateados
            ActualizarValoresFormateados();
        }

        /// <summary>
        /// Actualiza los cálculos al agregar un detalle
        /// </summary>
        /// <param name="detalle">Detalle a agregar</param>
        public void AgregarDetalle(DetalleCalculoViewModel detalle)
        {
            if (detalle == null)
                return;

            // Buscar si ya existe un detalle con el mismo artículo
            var detalleExistente = Detalles.FirstOrDefault(d => d.ArticuloId == detalle.ArticuloId);

            if (detalleExistente != null)
            {
                // Actualizar cantidad
                detalleExistente.Cantidad += detalle.Cantidad;
            }
            else
            {
                // Agregar nuevo detalle
                Detalles.Add(detalle);
            }

            // Recalcular
            Calcular();
        }

        /// <summary>
        /// Actualiza los cálculos al eliminar un detalle
        /// </summary>
        /// <param name="indice">Índice del detalle a eliminar</param>
        public void EliminarDetalle(int indice)
        {
            if (indice >= 0 && indice < Detalles.Count)
            {
                Detalles.RemoveAt(indice);
                Calcular();
            }
        }

        /// <summary>
        /// Actualiza los cálculos al modificar un detalle
        /// </summary>
        /// <param name="indice">Índice del detalle a modificar</param>
        /// <param name="detalle">Nuevo detalle</param>
        public void ModificarDetalle(int indice, DetalleCalculoViewModel detalle)
        {
            if (indice >= 0 && indice < Detalles.Count && detalle != null)
            {
                Detalles[indice] = detalle;
                Calcular();
            }
        }

        /// <summary>
        /// Convierte el modelo de vista a un DTO para enviar al API
        /// </summary>
        /// <returns>DTO de cálculo de factura</returns>
        public FacturaCalculoDto ConvertirAFacturaCalculoDto()
        {
            var calculoDto = new FacturaCalculoDto
            {
                Detalles = Detalles.Select(d => new CrearFacturaDetalleDto
                {
                    ArticuloId = d.ArticuloId,
                    ArticuloCodigo = d.ArticuloCodigo,
                    ArticuloNombre = d.ArticuloNombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList(),
                Totales = new FacturaTotalesDto
                {
                    Subtotal = Subtotal,
                    PorcentajeDescuento = PorcentajeDescuento,
                    ValorDescuento = ValorDescuento,
                    BaseImpuestos = BaseImpuestos,
                    PorcentajeIVA = PorcentajeIVA,
                    ValorIVA = ValorIVA,
                    Total = Total
                }
            };

            return calculoDto;
        }

        /// <summary>
        /// Actualiza el modelo de vista desde un DTO recibido del API
        /// </summary>
        /// <param name="calculoDto">DTO de cálculo de factura</param>
        public void ActualizarDesdeDto(FacturaCalculoDto calculoDto)
        {
            if (calculoDto == null || calculoDto.Totales == null)
                return;

            // Actualizar detalles
            if (calculoDto.Detalles != null)
            {
                Detalles = calculoDto.Detalles.Select(d => new DetalleCalculoViewModel
                {
                    ArticuloId = d.ArticuloId,
                    ArticuloCodigo = d.ArticuloCodigo,
                    ArticuloNombre = d.ArticuloNombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList();
            }

            // Actualizar totales
            Subtotal = calculoDto.Totales.Subtotal;
            PorcentajeDescuento = calculoDto.Totales.PorcentajeDescuento;
            ValorDescuento = calculoDto.Totales.ValorDescuento;
            BaseImpuestos = calculoDto.Totales.BaseImpuestos;
            PorcentajeIVA = calculoDto.Totales.PorcentajeIVA;
            ValorIVA = calculoDto.Totales.ValorIVA;
            Total = calculoDto.Totales.Total;

            // Actualizar valores formateados
            ActualizarValoresFormateados();
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Inicializa los valores a cero
        /// </summary>
        private void InicializarValores()
        {
            Subtotal = 0;
            PorcentajeDescuento = 0;
            ValorDescuento = 0;
            BaseImpuestos = 0;
            ValorIVA = 0;
            Total = 0;

            ActualizarValoresFormateados();
        }

        /// <summary>
        /// Actualiza los valores formateados
        /// </summary>
        private void ActualizarValoresFormateados()
        {
            SubtotalFormateado = FormatearMoneda(Subtotal);
            PorcentajeDescuentoFormateado = $"{PorcentajeDescuento}%";
            ValorDescuentoFormateado = FormatearMoneda(ValorDescuento);
            BaseImpuestosFormateada = FormatearMoneda(BaseImpuestos);
            PorcentajeIVAFormateado = $"{PorcentajeIVA}%";
            ValorIVAFormateado = FormatearMoneda(ValorIVA);
            TotalFormateado = FormatearMoneda(Total);
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

    /// <summary>
    /// Modelo de vista para los detalles utilizados en los cálculos
    /// </summary>
    public class DetalleCalculoViewModel
    {
        /// <summary>
        /// ID del artículo
        /// </summary>
        public int ArticuloId { get; set; }

        /// <summary>
        /// Código del artículo
        /// </summary>
        public string ArticuloCodigo { get; set; }

        /// <summary>
        /// Nombre del artículo
        /// </summary>
        public string ArticuloNombre { get; set; }

        /// <summary>
        /// Cantidad del artículo
        /// </summary>
        public int Cantidad { get; set; }

        /// <summary>
        /// Precio unitario del artículo
        /// </summary>
        public decimal PrecioUnitario { get; set; }

        /// <summary>
        /// Subtotal calculado (Cantidad * Precio Unitario)
        /// </summary>
        public decimal Subtotal => Cantidad * PrecioUnitario;

        /// <summary>
        /// Subtotal formateado como moneda
        /// </summary>
        public string SubtotalFormateado => string.Format("${0:N0}", Subtotal);

        /// <summary>
        /// Precio unitario formateado como moneda
        /// </summary>
        public string PrecioUnitarioFormateado => string.Format("${0:N0}", PrecioUnitario);

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public DetalleCalculoViewModel()
        {
            ArticuloCodigo = string.Empty;
            ArticuloNombre = string.Empty;
            Cantidad = 1; // Valor por defecto
        }
    }
}