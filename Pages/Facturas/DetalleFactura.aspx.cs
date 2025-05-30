using Facturacion.Web.App_Code.Base;
using Facturacion.Web.Models.DTOs.Facturas;
using Facturacion.Web.Services;
using System;
using System.Web.UI;

namespace Facturacion.Web.Pages.Facturas
{
    public partial class DetalleFactura : BasePage
    {
        private int _facturaId;
        private FacturaDto _factura;
        private FacturaService _facturaService;

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidarAutenticacion();

            if (!int.TryParse(Request.QueryString["id"], out _facturaId))
            {
                MostrarMensaje("ID de factura inválido", "danger");
                btnAnular.Enabled = false;
                return;
            }

            _facturaService = new FacturaService();

            if (!IsPostBack)
            {
                CargarFactura();
            }
        }

        private async void CargarFactura()
        {
            try
            {
                var respuesta = await _facturaService.ObtenerPorIdAsync(_facturaId);

                if (!respuesta.Exito || respuesta.Resultado == null)
                {
                    MostrarMensaje(respuesta.Detalle ?? "No se pudo cargar la factura", "danger");
                    btnAnular.Enabled = false;
                    return;
                }

                _factura = respuesta.Resultado;

                // Llenar información de factura
                lblNumeroFactura.Text = _factura.NumeroFactura;
                lblNumero.Text = _factura.NumeroFactura;
                lblEstado.Text = _factura.Estado;
                lblFecha.Text = _factura.FechaFormateada ?? _factura.Fecha.ToString("dd/MM/yyyy HH:mm");
                lblUsuario.Text = _factura.CreadoPor ?? "No disponible";
                lblObservaciones.Text = string.IsNullOrEmpty(_factura.Observaciones)
                    ? "Sin observaciones"
                    : _factura.Observaciones;

                // Llenar información de cliente
                lblCliente.Text = _factura.ClienteNombreCompleto ?? $"{_factura.ClienteNombres} {_factura.ClienteApellidos}";
                lblDocumento.Text = _factura.ClienteNumeroDocumento;
                lblDireccion.Text = _factura.ClienteDireccion;
                lblTelefono.Text = _factura.ClienteTelefono;

                // Llenar grilla de detalles
                if (_factura.Detalles != null && _factura.Detalles.Count > 0)
                {
                    gvDetalles.DataSource = _factura.Detalles;
                    gvDetalles.DataBind();
                }

                // Llenar totales
                lblSubtotal.Text = _factura.SubTotalFormateado ?? FormatearMoneda(_factura.SubTotal);
                lblPorcentajeDescuento.Text = _factura.PorcentajeDescuento.ToString("0.##");
                lblDescuento.Text = _factura.ValorDescuentoFormateado ?? FormatearMoneda(_factura.ValorDescuento);
                lblBaseImponible.Text = FormatearMoneda(_factura.BaseImpuestos);
                lblPorcentajeIva.Text = _factura.PorcentajeIVA.ToString("0.##");
                lblIva.Text = _factura.ValorIVAFormateado ?? FormatearMoneda(_factura.ValorIVA);
                lblTotal.Text = _factura.TotalFormateado ?? FormatearMoneda(_factura.Total);

                // Deshabilitar anulación si ya está anulada
                btnAnular.Visible = _factura.Estado.ToUpper() != "ANULADA";
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error cargando la factura: {ex.Message}", "danger");
                btnAnular.Enabled = false;
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Facturas/ListarFacturas.aspx");
        }

        protected void btnAnular_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarModalAnulacion", "mostrarModalAnulacion();", true);
        }

        protected async void btnConfirmarAnulacion_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string motivo = txtMotivo.Text.Trim();

            try
            {
                var respuesta = await _facturaService.AnularAsync(_facturaId, motivo);

                if (respuesta.Exito)
                {
                    MostrarMensaje("Factura anulada correctamente", "success");
                    CargarFactura(); // Recargar para mostrar estado actualizado
                }
                else
                {
                    MostrarMensaje(respuesta.Detalle ?? "No se pudo anular la factura", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al anular la factura: {ex.Message}", "danger");
            }
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = $"alert alert-{tipo} alert-dismissible fade show";
            lblMensaje.Text = mensaje;
        }

        private string FormatearMoneda(decimal valor)
        {
            return string.Format("${0:N0}", valor);
        }
    }
}