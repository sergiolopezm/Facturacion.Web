using Facturacion.Web.Models.DTOs.Facturas;
using Facturacion.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Facturacion.Web.Business
{
    /// <summary>
    /// Clase para manejo de la lógica de negocio de facturas en el lado cliente
    /// Incluye validaciones, cálculos y transformaciones de datos
    /// ⭐ CLAVE PARA LOS CÁLCULOS EN EL FORMULARIO DE FACTURAS
    /// </summary>
    public class FacturaBusiness
    {
        // Porcentaje de IVA por defecto
        private readonly decimal _ivaPercentage;

        // Porcentaje de descuento por defecto
        private readonly decimal _discountPercentage;

        // Monto mínimo para aplicar descuento
        private readonly decimal _minAmountForDiscount;

        /// <summary>
        /// Constructor por defecto que inicializa con valores desde la configuración
        /// </summary>
        public FacturaBusiness()
        {
            // Obtener configuración desde AppSettings
            _ivaPercentage = Core.AppSettings.DefaultIvaPercentage;
            _discountPercentage = Core.AppSettings.DefaultDiscountPercentage;
            _minAmountForDiscount = Core.AppSettings.MinAmountForDiscount;

            // Verificar que los valores sean válidos
            if (_ivaPercentage <= 0) _ivaPercentage = 19m; // Valor por defecto
            if (_discountPercentage <= 0) _discountPercentage = 5m; // Valor por defecto
            if (_minAmountForDiscount <= 0) _minAmountForDiscount = 500000m; // Valor por defecto
        }

        /// <summary>
        /// Constructor que permite especificar los porcentajes de impuestos y descuentos
        /// </summary>
        /// <param name="ivaPercentage">Porcentaje de IVA</param>
        /// <param name="discountPercentage">Porcentaje de descuento</param>
        /// <param name="minAmountForDiscount">Monto mínimo para aplicar descuento</param>
        public FacturaBusiness(decimal ivaPercentage, decimal discountPercentage, decimal minAmountForDiscount)
        {
            _ivaPercentage = ivaPercentage > 0 ? ivaPercentage : 19m;
            _discountPercentage = discountPercentage > 0 ? discountPercentage : 5m;
            _minAmountForDiscount = minAmountForDiscount > 0 ? minAmountForDiscount : 500000m;
        }

        #region Cálculos de Facturas

        /// <summary>
        /// Calcula los totales para una factura basándose en sus detalles
        /// ⭐ MÉTODO PRINCIPAL PARA CÁLCULOS
        /// </summary>
        /// <param name="detalles">Lista de detalles de la factura</param>
        /// <returns>Objeto con los totales calculados</returns>
        public FacturaTotalesDto CalcularTotales(List<CrearFacturaDetalleDto> detalles)
        {
            if (detalles == null || !detalles.Any())
            {
                return new FacturaTotalesDto
                {
                    Subtotal = 0,
                    PorcentajeDescuento = 0,
                    ValorDescuento = 0,
                    BaseImpuestos = 0,
                    PorcentajeIVA = _ivaPercentage,
                    ValorIVA = 0,
                    Total = 0
                };
            }

            // 1. Calcular subtotal
            decimal subtotal = CalcularSubtotal(detalles);

            // 2. Calcular descuento
            decimal descuento = CalcularDescuento(subtotal);
            decimal porcentajeDescuentoAplicado = descuento > 0 ? _discountPercentage : 0;

            // 3. Calcular base imponible
            decimal baseImponible = subtotal - descuento;

            // 4. Calcular IVA
            decimal iva = CalcularIVA(baseImponible);

            // 5. Calcular total
            decimal total = baseImponible + iva;

            // 6. Crear objeto de totales
            return new FacturaTotalesDto
            {
                Subtotal = subtotal,
                PorcentajeDescuento = porcentajeDescuentoAplicado,
                ValorDescuento = descuento,
                BaseImpuestos = baseImponible,
                PorcentajeIVA = _ivaPercentage,
                ValorIVA = iva,
                Total = total
            };
        }

        /// <summary>
        /// Calcula el subtotal sumando los valores de todos los detalles
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
        /// Calcula el descuento basado en el subtotal y las reglas de negocio
        /// </summary>
        /// <param name="subtotal">Subtotal de la factura</param>
        /// <returns>Valor del descuento</returns>
        public decimal CalcularDescuento(decimal subtotal)
        {
            // Regla de negocio: 5% de descuento si el subtotal es >= $500,000
            if (subtotal >= _minAmountForDiscount)
                return Math.Round(subtotal * (_discountPercentage / 100m), 2);

            return 0;
        }

        /// <summary>
        /// Calcula el IVA basado en la base imponible
        /// </summary>
        /// <param name="baseImponible">Base imponible (subtotal - descuento)</param>
        /// <returns>Valor del IVA</returns>
        public decimal CalcularIVA(decimal baseImponible)
        {
            return Math.Round(baseImponible * (_ivaPercentage / 100m), 2);
        }

        /// <summary>
        /// Formatea un objeto de totales para mostrar valores con formato de moneda
        /// </summary>
        /// <param name="totales">Objeto con los totales calculados</param>
        /// <returns>Objeto con los totales formateados</returns>
        public FacturaTotalesFormateadosDto FormatearTotales(FacturaTotalesDto totales)
        {
            if (totales == null)
                return new FacturaTotalesFormateadosDto();

            return new FacturaTotalesFormateadosDto
            {
                Subtotal = CurrencyHelper.FormatCurrency(totales.Subtotal),
                PorcentajeDescuento = totales.PorcentajeDescuento.ToString("0.##") + "%",
                ValorDescuento = CurrencyHelper.FormatCurrency(totales.ValorDescuento),
                BaseImpuestos = CurrencyHelper.FormatCurrency(totales.BaseImpuestos),
                PorcentajeIVA = totales.PorcentajeIVA.ToString("0.##") + "%",
                ValorIVA = CurrencyHelper.FormatCurrency(totales.ValorIVA),
                Total = CurrencyHelper.FormatCurrency(totales.Total)
            };
        }

        #endregion

        #region Validaciones de Facturas

        /// <summary>
        /// Valida una factura completa antes de enviarla al servidor
        /// </summary>
        /// <param name="factura">Objeto con los datos de la factura</param>
        /// <returns>Resultado de la validación</returns>
        public ValidacionFacturaDto ValidarFactura(CrearFacturaDto factura)
        {
            var resultado = new ValidacionFacturaDto
            {
                EsValida = true
            };

            try
            {
                // Validar que la factura no sea nula
                if (factura == null)
                {
                    resultado.EsValida = false;
                    resultado.Errores.Add("No se proporcionaron datos para la factura");
                    return resultado;
                }

                // Validar cliente
                if (factura.ClienteId <= 0)
                {
                    resultado.EsValida = false;
                    resultado.Errores.Add("Debe seleccionar un cliente válido");
                }

                // Validar que la factura tenga al menos un detalle
                if (factura.Detalles == null || !factura.Detalles.Any())
                {
                    resultado.EsValida = false;
                    resultado.Errores.Add("Debe incluir al menos un artículo en la factura");
                    return resultado;
                }

                // Validar detalles
                for (int i = 0; i < factura.Detalles.Count; i++)
                {
                    var detalle = factura.Detalles[i];
                    var erroresDetalle = ValidarDetalleFactura(detalle, i + 1);

                    if (erroresDetalle.Any())
                    {
                        resultado.EsValida = false;
                        resultado.Errores.AddRange(erroresDetalle);
                    }
                }

                // Validar que no haya artículos duplicados
                var articulosUnicos = factura.Detalles
                    .Select(d => d.ArticuloId)
                    .Distinct()
                    .Count();

                if (articulosUnicos != factura.Detalles.Count)
                {
                    resultado.EsValida = false;
                    resultado.Errores.Add("No se pueden incluir artículos duplicados en la misma factura");
                }

                // Si la factura es válida, calcular totales
                if (resultado.EsValida)
                {
                    resultado.TotalesCalculados = CalcularTotales(factura.Detalles);
                }
            }
            catch (Exception ex)
            {
                resultado.EsValida = false;
                resultado.Errores.Add($"Error en la validación: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Error en validación de factura: {ex.Message}");
            }

            return resultado;
        }

        /// <summary>
        /// Valida un detalle de factura individual
        /// </summary>
        /// <param name="detalle">Detalle a validar</param>
        /// <param name="indice">Índice del detalle en la lista (para mensajes)</param>
        /// <returns>Lista de errores encontrados</returns>
        public List<string> ValidarDetalleFactura(CrearFacturaDetalleDto detalle, int indice)
        {
            var errores = new List<string>();

            if (detalle == null)
            {
                errores.Add($"El detalle #{indice} no contiene datos");
                return errores;
            }

            // Validar artículo
            if (detalle.ArticuloId <= 0)
            {
                errores.Add($"Debe seleccionar un artículo válido en la línea {indice}");
            }

            // Validar cantidad
            if (detalle.Cantidad <= 0)
            {
                errores.Add($"La cantidad en la línea {indice} debe ser mayor a cero");
            }

            // Validar precio
            if (detalle.PrecioUnitario <= 0)
            {
                errores.Add($"El precio unitario en la línea {indice} debe ser mayor a cero");
            }

            return errores;
        }

        /// <summary>
        /// Verifica si hay suficiente stock para un detalle específico
        /// </summary>
        /// <param name="detalle">Detalle a verificar</param>
        /// <param name="stockDisponible">Stock disponible del artículo</param>
        /// <returns>True si hay suficiente stock</returns>
        public bool VerificarStockSuficiente(CrearFacturaDetalleDto detalle, int stockDisponible)
        {
            if (detalle == null)
                return false;

            return stockDisponible >= detalle.Cantidad;
        }

        #endregion

        #region Conversiones y Transformaciones

        /// <summary>
        /// Convierte un FacturaViewModel a CrearFacturaDto para enviar al servidor
        /// </summary>
        /// <param name="viewModel">ViewModel del formulario</param>
        /// <returns>DTO para enviar al servidor</returns>
        public CrearFacturaDto ConvertirViewModelADto(Models.Facturas.FacturaViewModel viewModel)
        {
            if (viewModel == null)
                return null;

            var facturaDto = new CrearFacturaDto
            {
                ClienteId = viewModel.ClienteId,
                Observaciones = viewModel.Observaciones,
                Detalles = new List<CrearFacturaDetalleDto>()
            };

            // Convertir detalles
            if (viewModel.Detalles != null && viewModel.Detalles.Any())
            {
                foreach (var detalle in viewModel.Detalles)
                {
                    facturaDto.Detalles.Add(new CrearFacturaDetalleDto
                    {
                        ArticuloId = detalle.ArticuloId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        ArticuloCodigo = detalle.ArticuloCodigo,
                        ArticuloNombre = detalle.ArticuloNombre
                    });
                }
            }

            return facturaDto;
        }

        /// <summary>
        /// Convierte un DTO de factura a ViewModel para mostrar en el formulario
        /// </summary>
        /// <param name="facturaDto">DTO recibido del servidor</param>
        /// <returns>ViewModel para el formulario</returns>
        public Models.Facturas.FacturaViewModel ConvertirDtoAViewModel(FacturaDto facturaDto)
        {
            if (facturaDto == null)
                return null;

            var viewModel = new Models.Facturas.FacturaViewModel
            {
                id = facturaDto.Id,
                NumeroFactura = facturaDto.NumeroFactura,
                Fecha = facturaDto.Fecha,
                ClienteId = facturaDto.ClienteId,
                ClienteNumeroDocumento = facturaDto.ClienteNumeroDocumento,
                ClienteNombres = facturaDto.ClienteNombres,
                ClienteApellidos = facturaDto.ClienteApellidos,
                ClienteDireccion = facturaDto.ClienteDireccion,
                ClienteTelefono = facturaDto.ClienteTelefono,
                Observaciones = facturaDto.Observaciones,
                Estado = facturaDto.Estado,
                // Totales
                Subtotal = facturaDto.SubTotal,
                PorcentajeDescuento = facturaDto.PorcentajeDescuento,
                ValorDescuento = facturaDto.ValorDescuento,
                BaseImpuestos = facturaDto.BaseImpuestos,
                PorcentajeIVA = facturaDto.PorcentajeIVA,
                ValorIVA = facturaDto.ValorIVA,
                Total = facturaDto.Total,
                // Valores formateados
                SubtotalFormateado = facturaDto.SubTotalFormateado,
                ValorDescuentoFormateado = facturaDto.ValorDescuentoFormateado,
                ValorIVAFormateado = facturaDto.ValorIVAFormateado,
                TotalFormateado = facturaDto.TotalFormateado,
                // Cliente
                ClienteNombreCompleto = facturaDto.ClienteNombreCompleto,
                Detalles = new List<Models.Facturas.FacturaDetalleViewModel>()
            };

            // Convertir detalles
            if (facturaDto.Detalles != null && facturaDto.Detalles.Any())
            {
                foreach (var detalle in facturaDto.Detalles)
                {
                    viewModel.Detalles.Add(new Models.Facturas.FacturaDetalleViewModel
                    {
                        Id = detalle.Id,
                        FacturaId = detalle.FacturaId,
                        ArticuloId = detalle.ArticuloId,
                        ArticuloCodigo = detalle.ArticuloCodigo,
                        ArticuloNombre = detalle.ArticuloNombre,
                        ArticuloDescripcion = detalle.ArticuloDescripcion,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = detalle.PrecioUnitario,
                        Subtotal = detalle.Subtotal,
                        PrecioUnitarioFormateado = detalle.PrecioUnitarioFormateado,
                        SubtotalFormateado = detalle.SubtotalFormateado
                    });
                }
            }

            return viewModel;
        }

        /// <summary>
        /// Prepara los datos para una nueva factura
        /// </summary>
        /// <returns>ViewModel inicializado para una nueva factura</returns>
        public Models.Facturas.FacturaViewModel PrepararNuevaFactura()
        {
            return new Models.Facturas.FacturaViewModel
            {
                Fecha = DateTime.Now,
                Estado = "Activa",
                PorcentajeIVA = _ivaPercentage,
                PorcentajeDescuento = 0, // Se calcula automáticamente según el monto
                Detalles = new List<Models.Facturas.FacturaDetalleViewModel>()
            };
        }

        #endregion
    }
}