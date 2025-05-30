using Facturacion.Web.Core;
using Facturacion.Web.Models.DTOs.Facturas;
using Facturacion.Web.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Facturacion.Web.Business
{
    /// <summary>
    /// Clase que maneja la lógica de negocio relacionada con moneda y cálculos financieros
    /// Implementa las reglas de negocio específicas para facturación, descuentos e impuestos
    /// </summary>
    public class CurrencyBusiness
    {
        // Constantes de negocio - Valores por defecto
        private readonly decimal _porcentajeIVA;
        private readonly decimal _porcentajeDescuento;
        private readonly decimal _montoMinimoDescuento;

        /// <summary>
        /// Constructor que inicializa la clase con valores por defecto o desde configuración
        /// </summary>
        public CurrencyBusiness()
        {
            // Obtener valores desde configuración o usar valores por defecto
            _porcentajeIVA = AppSettings.DefaultIvaPercentage; // Por defecto 19%
            _porcentajeDescuento = AppSettings.DefaultDiscountPercentage; // Por defecto 5%
            _montoMinimoDescuento = AppSettings.MinAmountForDiscount; // Por defecto 500,000
        }

        /// <summary>
        /// Calcula el subtotal de una factura basado en sus detalles
        /// </summary>
        /// <param name="detalles">Lista de detalles de la factura</param>
        /// <returns>Subtotal calculado</returns>
        public decimal CalcularSubtotal(List<CrearFacturaDetalleDto> detalles)
        {
            if (detalles == null || !detalles.Any())
                return 0;

            return detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
        }

        /// <summary>
        /// Calcula el descuento aplicable según las reglas de negocio
        /// (5% si el subtotal es >= 500,000, de lo contrario 0)
        /// </summary>
        /// <param name="subtotal">Subtotal de la factura</param>
        /// <returns>Valor del descuento</returns>
        public decimal CalcularDescuento(decimal subtotal)
        {
            // Aplicar descuento solo si el subtotal es mayor o igual al monto mínimo
            if (subtotal >= _montoMinimoDescuento)
            {
                return Math.Round(subtotal * (_porcentajeDescuento / 100m), 2);
            }
            return 0m;
        }

        /// <summary>
        /// Calcula el porcentaje de descuento aplicable según las reglas de negocio
        /// </summary>
        /// <param name="subtotal">Subtotal de la factura</param>
        /// <returns>Porcentaje de descuento aplicable</returns>
        public decimal ObtenerPorcentajeDescuento(decimal subtotal)
        {
            return subtotal >= _montoMinimoDescuento ? _porcentajeDescuento : 0;
        }

        /// <summary>
        /// Calcula el IVA (19%) sobre la base imponible
        /// </summary>
        /// <param name="baseImponible">Base para cálculo del IVA (subtotal - descuento)</param>
        /// <returns>Valor del IVA</returns>
        public decimal CalcularIVA(decimal baseImponible)
        {
            return Math.Round(baseImponible * (_porcentajeIVA / 100m), 2);
        }

        /// <summary>
        /// Calcula el total de la factura (subtotal - descuento + IVA)
        /// </summary>
        /// <param name="subtotal">Subtotal de la factura</param>
        /// <param name="descuento">Descuento aplicado</param>
        /// <param name="iva">IVA calculado</param>
        /// <returns>Total de la factura</returns>
        public decimal CalcularTotal(decimal subtotal, decimal descuento, decimal iva)
        {
            return subtotal - descuento + iva;
        }

        /// <summary>
        /// Calcula todos los totales de una factura en una sola operación
        /// </summary>
        /// <param name="detalles">Lista de detalles de la factura</param>
        /// <returns>Objeto con todos los totales calculados</returns>
        public FacturaTotalesDto CalcularTotalesFactura(List<CrearFacturaDetalleDto> detalles)
        {
            // Calcular subtotal
            decimal subtotal = CalcularSubtotal(detalles);

            // Determinar si aplica descuento
            decimal porcentajeDescuento = ObtenerPorcentajeDescuento(subtotal);
            decimal valorDescuento = CalcularDescuento(subtotal);

            // Calcular base imponible
            decimal baseImponible = subtotal - valorDescuento;

            // Calcular IVA
            decimal valorIVA = CalcularIVA(baseImponible);

            // Calcular total
            decimal total = baseImponible + valorIVA;

            // Crear objeto de totales
            return new FacturaTotalesDto
            {
                Subtotal = subtotal,
                PorcentajeDescuento = porcentajeDescuento,
                ValorDescuento = valorDescuento,
                BaseImpuestos = baseImponible,
                PorcentajeIVA = _porcentajeIVA,
                ValorIVA = valorIVA,
                Total = total
            };
        }

        /// <summary>
        /// Calcula totales y genera versiones formateadas para mostrar en UI
        /// </summary>
        /// <param name="detalles">Lista de detalles de la factura</param>
        /// <returns>Objeto con todos los cálculos y formatos</returns>
        public FacturaCalculoDto CalcularYFormatearTotales(List<CrearFacturaDetalleDto> detalles)
        {
            var totales = CalcularTotalesFactura(detalles);

            // Formatear valores para mostrar
            var totalesFormateados = new FacturaTotalesFormateadosDto
            {
                Subtotal = CurrencyHelper.FormatCurrency(totales.Subtotal),
                PorcentajeDescuento = totales.PorcentajeDescuento.ToString("0.##") + "%",
                ValorDescuento = CurrencyHelper.FormatCurrency(totales.ValorDescuento),
                BaseImpuestos = CurrencyHelper.FormatCurrency(totales.BaseImpuestos),
                PorcentajeIVA = totales.PorcentajeIVA.ToString("0.##") + "%",
                ValorIVA = CurrencyHelper.FormatCurrency(totales.ValorIVA),
                Total = CurrencyHelper.FormatCurrency(totales.Total)
            };

            return new FacturaCalculoDto
            {
                Detalles = detalles,
                Totales = totales,
                TotalesFormateados = totalesFormateados
            };
        }

        /// <summary>
        /// Valida un valor monetario y retorna mensaje de error si no es válido
        /// </summary>
        /// <param name="valor">Valor a validar</param>
        /// <param name="campoNombre">Nombre del campo para el mensaje de error</param>
        /// <returns>Mensaje de error o null si es válido</returns>
        public string ValidarValorMonetario(string valor, string campoNombre)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return $"El {campoNombre} es requerido";

            if (!decimal.TryParse(valor,
                NumberStyles.Currency | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
                CultureInfo.CurrentCulture,
                out decimal resultado))
                return $"El {campoNombre} debe ser un valor monetario válido";

            if (resultado <= 0)
                return $"El {campoNombre} debe ser mayor que cero";

            return null; // Sin error
        }

        /// <summary>
        /// Convierte una cadena en formato monetario a decimal
        /// </summary>
        /// <param name="valor">Valor en formato de texto</param>
        /// <returns>Valor decimal o 0 si es inválido</returns>
        public decimal ConvertirADecimal(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            // Intentar convertir eliminando símbolos de moneda y separadores
            if (decimal.TryParse(valor.Replace("$", "").Replace(".", "").Replace(",", "."),
                NumberStyles.Currency | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal resultado))
                return resultado;

            // Segundo intento con la cultura actual
            if (decimal.TryParse(valor,
                NumberStyles.Currency | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
                CultureInfo.CurrentCulture,
                out resultado))
                return resultado;

            return 0;
        }

        /// <summary>
        /// Obtiene el valor total formateado de un conjunto de detalles
        /// </summary>
        /// <param name="detalles">Detalles de la factura</param>
        /// <returns>Valor total formateado</returns>
        public string ObtenerTotalFormateado(List<CrearFacturaDetalleDto> detalles)
        {
            var totales = CalcularTotalesFactura(detalles);
            return CurrencyHelper.FormatCurrency(totales.Total);
        }

        /// <summary>
        /// Formatea un valor decimal según configuración de moneda
        /// </summary>
        /// <param name="valor">Valor a formatear</param>
        /// <returns>Valor formateado como moneda</returns>
        public string FormatearMoneda(decimal valor)
        {
            return CurrencyHelper.FormatCurrency(valor);
        }
    }
}