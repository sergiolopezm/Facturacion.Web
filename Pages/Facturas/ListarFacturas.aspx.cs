using Facturacion.Web.Models.DTOs.Common;
using Facturacion.Web.Models.DTOs.Facturas;
using Facturacion.Web.Services;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Facturacion.Web.Pages.Facturas
{
    public partial class ListarFacturas : System.Web.UI.Page
    {
        private readonly FacturaService _facturaService;
        private const int _pageSize = 10;

        public ListarFacturas()
        {
            _facturaService = new FacturaService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Verificar si el usuario está autenticado
                if (!AuthService.TieneSesionActiva())
                {
                    Response.Redirect("~/Pages/Auth/Login.aspx?returnUrl=" + Server.UrlEncode(Request.RawUrl));
                    return;
                }

                if (!IsPostBack)
                {
                    // Cargar datos iniciales
                    ConfigurarFiltrosFecha();
                    CargarFacturas();

                    // Verificar si hay mensaje en la URL
                    string mensaje = Request.QueryString["mensaje"];
                    if (!string.IsNullOrEmpty(mensaje))
                    {
                        string tipo = Request.QueryString["tipo"] ?? "success";
                        MostrarMensaje(mensaje, tipo != "success");
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar la página: " + ex.Message, true);
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                CargarFacturas();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al buscar facturas: " + ex.Message, true);
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                // Limpiar filtros
                txtBusqueda.Text = string.Empty;
                ConfigurarFiltrosFecha();
                ddlEstado.SelectedIndex = 0;

                // Recargar facturas
                CargarFacturas();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al limpiar filtros: " + ex.Message, true);
            }
        }

        protected void btnNuevaFactura_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("CrearFactura.aspx");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al redirigir: " + ex.Message, true);
            }
        }

        protected void gvFacturas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "VerDetalle")
                {
                    int facturaId = Convert.ToInt32(e.CommandArgument);
                    Response.Redirect($"DetalleFactura.aspx?id={facturaId}");
                }
                else if (e.CommandName == "Anular")
                {
                    // Abrir modal de anulación
                    int facturaId = Convert.ToInt32(e.CommandArgument);
                    hdnFacturaId.Value = facturaId.ToString();
                    ScriptManager.RegisterStartupScript(this, GetType(), "AbrirModal",
                        $"abrirModalAnulacion({facturaId});", true);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al procesar la acción: " + ex.Message, true);
            }
        }

        protected void gvFacturas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvFacturas.PageIndex = e.NewPageIndex;
                CargarFacturas();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cambiar de página: " + ex.Message, true);
            }
        }

        protected void btnConfirmarAnulacion_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar motivo
                if (string.IsNullOrWhiteSpace(txtMotivoAnulacion.Text))
                {
                    MostrarMensaje("Debe ingresar un motivo de anulación.", true);
                    return;
                }

                // Obtener ID de la factura
                if (!int.TryParse(hdnFacturaId.Value, out int facturaId))
                {
                    MostrarMensaje("ID de factura inválido.", true);
                    return;
                }

                // Anular factura
                var respuesta = _facturaService.AnularAsync(facturaId, txtMotivoAnulacion.Text.Trim()).Result;
                if (respuesta.Exito)
                {
                    MostrarMensaje("Factura anulada correctamente.", false);
                    CargarFacturas();
                }
                else
                {
                    MostrarMensaje("Error al anular factura: " + respuesta.Detalle, true);
                }

                // Limpiar y cerrar modal
                txtMotivoAnulacion.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, GetType(), "CerrarModal",
                    "$('#modalAnular').modal('hide');", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al anular factura: " + ex.Message, true);
            }
        }

        #region Métodos Auxiliares

        private void ConfigurarFiltrosFecha()
        {
            // Establecer fechas por defecto (último mes)
            DateTime hoy = DateTime.Now.Date;
            DateTime inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            DateTime finMes = inicioMes.AddMonths(1).AddDays(-1);

            txtFechaInicio.Text = inicioMes.ToString("yyyy-MM-dd");
            txtFechaFin.Text = finMes.ToString("yyyy-MM-dd");
        }

        private void CargarFacturas()
        {
            try
            {
                // Obtener parámetros de filtro
                string busqueda = txtBusqueda.Text.Trim();
                DateTime? fechaInicio = null;
                DateTime? fechaFin = null;
                string estado = ddlEstado.SelectedValue;

                // Parsear fechas
                if (DateTime.TryParse(txtFechaInicio.Text, out DateTime fechaIni))
                {
                    fechaInicio = fechaIni;
                }

                if (DateTime.TryParse(txtFechaFin.Text, out DateTime fechaFi))
                {
                    fechaFin = fechaFi.AddDays(1).AddSeconds(-1); // Hasta el final del día
                }

                // Obtener página actual
                int pagina = gvFacturas.PageIndex + 1;

                // Llamar al servicio
                var respuesta = _facturaService.ObtenerPaginadoAsync(
                    pagina, _pageSize, busqueda, fechaInicio, fechaFin, estado).Result;

                if (respuesta.Exito && respuesta.Resultado != null)
                {
                    var resultado = respuesta.Resultado;

                    // Asignar datos a la grilla
                    gvFacturas.DataSource = resultado.Lista;
                    gvFacturas.DataBind();

                    // Actualizar información de paginación
                    ActualizarInfoPaginacion(resultado);
                }
                else
                {
                    // Mostrar mensaje de error
                    MostrarMensaje("Error al cargar facturas: " + respuesta.Detalle, true);

                    // Limpiar grilla
                    gvFacturas.DataSource = new List<FacturaResumenDto>();
                    gvFacturas.DataBind();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar facturas: " + ex.Message, true);

                // Limpiar grilla
                gvFacturas.DataSource = new List<FacturaResumenDto>();
                gvFacturas.DataBind();
            }
        }

        private void ActualizarInfoPaginacion(PaginacionDto<FacturaResumenDto> resultado)
        {
            // Mostrar información de paginación
            int inicio = (resultado.Pagina - 1) * resultado.ElementosPorPagina + 1;
            int fin = Math.Min(inicio + resultado.ElementosPorPagina - 1, resultado.TotalRegistros);

            if (resultado.TotalRegistros > 0)
            {
                lblPaginacion.Text = $"Mostrando {inicio} a {fin} de {resultado.TotalRegistros} facturas";
            }
            else
            {
                lblPaginacion.Text = "No se encontraron facturas";
            }
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            panelMensaje.Visible = true;
            panelMensaje.CssClass = esError ? "alert alert-danger alert-dismissible" : "alert alert-success alert-dismissible";
            ltlMensaje.Text = mensaje;

            // Script para ocultar el mensaje después de 5 segundos
            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarMensaje",
                "setTimeout(function() { $('#" + panelMensaje.ClientID + "').fadeOut('slow'); }, 5000);", true);
        }

        #endregion
    }
}