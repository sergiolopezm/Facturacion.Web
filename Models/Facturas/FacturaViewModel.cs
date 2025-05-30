using Facturacion.Web.Models.DTOs.Articulos;
using Facturacion.Web.Models.DTOs.Clientes;
using Facturacion.Web.Models.DTOs.Facturas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Facturacion.Web.Models.Facturas
{
    /// <summary>
    /// Modelo de vista para el formulario de creación de facturas
    /// Contiene todos los datos necesarios para mostrar y gestionar el formulario
    /// </summary>
    public class FacturaViewModel
    {
        #region Propiedades de Cabecera

        /// <summary>
        /// Fecha de la factura (solo lectura, fecha actual)
        /// </summary>
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Número de factura (solo lectura, se genera al grabar)
        /// </summary>
        [Display(Name = "Número de Factura")]
        public string NumeroFactura { get; set; }

        /// <summary>
        /// ID del cliente seleccionado
        /// </summary>
        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        /// <summary>
        /// Número de documento del cliente
        /// </summary>
        [Required(ErrorMessage = "El número de documento del cliente es requerido")]
        [StringLength(15, ErrorMessage = "El número de documento no puede exceder los 15 caracteres")]
        [Display(Name = "Documento")]
        public string ClienteNumeroDocumento { get; set; }

        /// <summary>
        /// Nombres del cliente
        /// </summary>
        [Required(ErrorMessage = "Los nombres del cliente son requeridos")]
        [StringLength(100, ErrorMessage = "Los nombres no pueden exceder los 100 caracteres")]
        [Display(Name = "Nombres")]
        public string ClienteNombres { get; set; }

        /// <summary>
        /// Apellidos del cliente
        /// </summary>
        [Required(ErrorMessage = "Los apellidos del cliente son requeridos")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden exceder los 100 caracteres")]
        [Display(Name = "Apellidos")]
        public string ClienteApellidos { get; set; }

        /// <summary>
        /// Dirección del cliente
        /// </summary>
        [Required(ErrorMessage = "La dirección del cliente es requerida")]
        [StringLength(250, ErrorMessage = "La dirección no puede exceder los 250 caracteres")]
        [Display(Name = "Dirección")]
        public string ClienteDireccion { get; set; }

        /// <summary>
        /// Teléfono del cliente
        /// </summary>
        [Required(ErrorMessage = "El teléfono del cliente es requerido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        [Display(Name = "Teléfono")]
        public string ClienteTelefono { get; set; }

        /// <summary>
        /// Observaciones de la factura (opcional)
        /// </summary>
        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder los 500 caracteres")]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        #endregion

        #region Propiedades de Detalles

        /// <summary>
        /// Lista de detalles de la factura
        /// </summary>
        [Required(ErrorMessage = "Debe incluir al menos un artículo en la factura")]
        public List<FacturaDetalleViewModel> Detalles { get; set; }

        /// <summary>
        /// Detalle temporal para agregar nuevos artículos (no se persiste)
        /// </summary>
        public FacturaDetalleViewModel NuevoDetalle { get; set; }

        #endregion

        #region Propiedades de Totales (Solo Lectura)

        /// <summary>
        /// Subtotal de la factura (sin IVA)
        /// </summary>
        [Display(Name = "Subtotal")]
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Porcentaje de descuento aplicado (5% si subtotal >= $500,000, 0% en caso contrario)
        /// </summary>
        [Display(Name = "Porcentaje Descuento")]
        public decimal PorcentajeDescuento { get; set; }

        /// <summary>
        /// Valor del descuento calculado
        /// </summary>
        [Display(Name = "Descuento")]
        public decimal ValorDescuento { get; set; }

        /// <summary>
        /// Base para cálculo de impuestos (Subtotal - Descuento)
        /// </summary>
        [Display(Name = "Base Impuestos")]
        public decimal BaseImpuestos { get; set; }

        /// <summary>
        /// Porcentaje de IVA (19%)
        /// </summary>
        [Display(Name = "Porcentaje IVA")]
        public decimal PorcentajeIVA { get; set; }

        /// <summary>
        /// Valor del IVA calculado
        /// </summary>
        [Display(Name = "IVA")]
        public decimal ValorIVA { get; set; }

        /// <summary>
        /// Total de la factura (Base + IVA)
        /// </summary>
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Subtotal formateado como moneda
        /// </summary>
        public string SubTotalFormateado { get; set; }

        /// <summary>
        /// Valor del descuento formateado como moneda
        /// </summary>
        public string ValorDescuentoFormateado { get; set; }

        /// <summary>
        /// Valor del IVA formateado como moneda
        /// </summary>
        public string ValorIVAFormateado { get; set; }

        /// <summary>
        /// Total formateado como moneda
        /// </summary>
        public string TotalFormateado { get; set; }

        #endregion

        #region Propiedades para la UI

        /// <summary>
        /// Lista de clientes para el dropdown
        /// </summary>
        public List<ClienteDto> ClientesDisponibles { get; set; }

        /// <summary>
        /// Lista de artículos para el dropdown
        /// </summary>
        public List<ArticuloDto> ArticulosDisponibles { get; set; }

        /// <summary>
        /// Mensaje de error o éxito para mostrar al usuario
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// Tipo de mensaje (success, error, warning, info)
        /// </summary>
        public string TipoMensaje { get; set; }

        /// <summary>
        /// Indica si hubo un error en la validación
        /// </summary>
        public bool TieneErrores { get; set; }

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public FacturaViewModel()
        {
            // Inicializar fecha con la fecha actual
            Fecha = DateTime.Now;

            // Inicializar listas
            Detalles = new List<FacturaDetalleViewModel>();
            ClientesDisponibles = new List<ClienteDto>();
            ArticulosDisponibles = new List<ArticuloDto>();

            // Inicializar nuevo detalle para el formulario
            NuevoDetalle = new FacturaDetalleViewModel();

            // Inicializar valores por defecto
            PorcentajeIVA = 19; // 19% por defecto según requerimiento
            NumeroFactura = "Por asignar"; // Se asignará al grabar

            // Inicializar strings vacíos
            ClienteNumeroDocumento = string.Empty;
            ClienteNombres = string.Empty;
            ClienteApellidos = string.Empty;
            ClienteDireccion = string.Empty;
            ClienteTelefono = string.Empty;
            Observaciones = string.Empty;
            Mensaje = string.Empty;
            TipoMensaje = string.Empty;

            // Inicializar campos formateados
            SubTotalFormateado = "$0";
            ValorDescuentoFormateado = "$0";
            ValorIVAFormateado = "$0";
            TotalFormateado = "$0";
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Agrega un detalle a la factura
        /// </summary>
        /// <param name="detalle">Detalle a agregar</param>
        public void AgregarDetalle(FacturaDetalleViewModel detalle)
        {
            if (detalle == null)
                return;

            // Verificar si ya existe un detalle con el mismo artículo
            var detalleExistente = Detalles.FirstOrDefault(d => d.ArticuloId == detalle.ArticuloId);

            if (detalleExistente != null)
            {
                // Si ya existe, actualizar la cantidad
                detalleExistente.Cantidad += detalle.Cantidad;
                // Recalcular subtotal
                detalleExistente.Subtotal = detalleExistente.Cantidad * detalleExistente.PrecioUnitario;
                detalleExistente.SubtotalFormateado = FormatearMoneda(detalleExistente.Subtotal);
            }
            else
            {
                // Si no existe, agregar a la lista
                // Calcular subtotal
                detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                detalle.SubtotalFormateado = FormatearMoneda(detalle.Subtotal);

                Detalles.Add(detalle);
            }

            // Recalcular totales
            CalcularTotales();
        }

        /// <summary>
        /// Elimina un detalle de la factura
        /// </summary>
        /// <param name="index">Índice del detalle a eliminar</param>
        public void EliminarDetalle(int index)
        {
            if (index >= 0 && index < Detalles.Count)
            {
                Detalles.RemoveAt(index);
                // Recalcular totales
                CalcularTotales();
            }
        }

        /// <summary>
        /// Calcula los totales de la factura basándose en los detalles actuales
        /// </summary>
        public void CalcularTotales()
        {
            // Calcular subtotal
            SubTotal = Detalles.Sum(d => d.Subtotal);

            // Calcular descuento (5% si subtotal >= $500,000)
            decimal montoMinimoDescuento = 500000;
            PorcentajeDescuento = SubTotal >= montoMinimoDescuento ? 5 : 0;
            ValorDescuento = Math.Round(SubTotal * (PorcentajeDescuento / 100), 2);

            // Calcular base imponible
            BaseImpuestos = SubTotal - ValorDescuento;

            // Calcular IVA (19%)
            ValorIVA = Math.Round(BaseImpuestos * (PorcentajeIVA / 100), 2);

            // Calcular total
            Total = BaseImpuestos + ValorIVA;

            // Formatear valores para mostrar
            SubTotalFormateado = FormatearMoneda(SubTotal);
            ValorDescuentoFormateado = FormatearMoneda(ValorDescuento);
            ValorIVAFormateado = FormatearMoneda(ValorIVA);
            TotalFormateado = FormatearMoneda(Total);
        }

        /// <summary>
        /// Convierte el ViewModel a un DTO para enviar al API
        /// </summary>
        public CrearFacturaDto ConvertirACrearFacturaDto()
        {
            var facturaDto = new CrearFacturaDto
            {
                ClienteId = ClienteId,
                Observaciones = Observaciones,
                Detalles = Detalles.Select(d => new CrearFacturaDetalleDto
                {
                    ArticuloId = d.ArticuloId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    ArticuloCodigo = d.ArticuloCodigo,
                    ArticuloNombre = d.ArticuloNombre
                }).ToList()
            };

            return facturaDto;
        }

        /// <summary>
        /// Llena los datos del cliente basándose en el ID seleccionado
        /// </summary>
        /// <param name="cliente">Cliente seleccionado</param>
        public void CompletarDatosCliente(ClienteDto cliente)
        {
            if (cliente == null)
                return;

            ClienteId = cliente.Id;
            ClienteNumeroDocumento = cliente.NumeroDocumento;
            ClienteNombres = cliente.Nombres;
            ClienteApellidos = cliente.Apellidos;
            ClienteDireccion = cliente.Direccion;
            ClienteTelefono = cliente.Telefono;
        }

        /// <summary>
        /// Valida si la factura es válida para grabar
        /// </summary>
        /// <returns>True si es válida, False en caso contrario</returns>
        public bool EsValida()
        {
            TieneErrores = false;
            Mensaje = string.Empty;

            // Validar que haya al menos un detalle
            if (Detalles == null || Detalles.Count == 0)
            {
                Mensaje = "Debe incluir al menos un artículo en la factura";
                TipoMensaje = "error";
                TieneErrores = true;
                return false;
            }

            // Validar cliente
            if (string.IsNullOrWhiteSpace(ClienteNumeroDocumento))
            {
                Mensaje = "El número de documento del cliente es requerido";
                TipoMensaje = "error";
                TieneErrores = true;
                return false;
            }

            // Validar campos obligatorios del cliente
            if (string.IsNullOrWhiteSpace(ClienteNombres) ||
                string.IsNullOrWhiteSpace(ClienteApellidos) ||
                string.IsNullOrWhiteSpace(ClienteDireccion) ||
                string.IsNullOrWhiteSpace(ClienteTelefono))
            {
                Mensaje = "Todos los campos del cliente son requeridos";
                TipoMensaje = "error";
                TieneErrores = true;
                return false;
            }

            // Validar detalles
            foreach (var detalle in Detalles)
            {
                if (detalle.ArticuloId <= 0)
                {
                    Mensaje = "Todos los artículos deben ser válidos";
                    TipoMensaje = "error";
                    TieneErrores = true;
                    return false;
                }

                if (detalle.Cantidad <= 0)
                {
                    Mensaje = "La cantidad debe ser mayor a 0";
                    TipoMensaje = "error";
                    TieneErrores = true;
                    return false;
                }

                if (detalle.PrecioUnitario <= 0)
                {
                    Mensaje = "El precio unitario debe ser mayor a 0";
                    TipoMensaje = "error";
                    TieneErrores = true;
                    return false;
                }
            }

            return true;
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
    /// Modelo de vista para los detalles de la factura
    /// </summary>
    public class FacturaDetalleViewModel
    {
        /// <summary>
        /// ID del artículo
        /// </summary>
        [Required(ErrorMessage = "El artículo es requerido")]
        [Display(Name = "Artículo")]
        public int ArticuloId { get; set; }

        /// <summary>
        /// Código del artículo
        /// </summary>
        [Display(Name = "Código")]
        public string ArticuloCodigo { get; set; }

        /// <summary>
        /// Nombre del artículo
        /// </summary>
        [Display(Name = "Artículo")]
        public string ArticuloNombre { get; set; }

        /// <summary>
        /// Cantidad del artículo
        /// </summary>
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        /// <summary>
        /// Precio unitario del artículo
        /// </summary>
        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

        /// <summary>
        /// Subtotal calculado (Cantidad * Precio Unitario)
        /// </summary>
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Precio unitario formateado como moneda
        /// </summary>
        public string PrecioUnitarioFormateado { get; set; }

        /// <summary>
        /// Subtotal formateado como moneda
        /// </summary>
        public string SubtotalFormateado { get; set; }

        /// <summary>
        /// Stock disponible del artículo (para validación)
        /// </summary>
        public int StockDisponible { get; set; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public FacturaDetalleViewModel()
        {
            ArticuloCodigo = string.Empty;
            ArticuloNombre = string.Empty;
            Cantidad = 1; // Valor por defecto
            PrecioUnitarioFormateado = "$0";
            SubtotalFormateado = "$0";
        }
    }
}