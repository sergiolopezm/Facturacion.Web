using Facturacion.Web.Models.DTOs.Clientes;
using Facturacion.Web.Models.DTOs.Facturas;
using Facturacion.Web.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Facturacion.Web.Pages.Facturas
{
    public partial class CrearFactura : System.Web.UI.Page
    {
        // Servicios
        private readonly FacturaService _facturaService;
        private readonly ClienteService _clienteService;
        private readonly ArticuloService _articuloService;

        // Lista para almacenar los detalles de la factura
        private List<CrearFacturaDetalleDto> _detalles;

        public CrearFactura()
        {
            _facturaService = new FacturaService();
            _clienteService = new ClienteService();
            _articuloService = new ArticuloService();
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
                    // Inicializar formulario
                    InicializarFormulario();
                }

                // Recuperar la lista de detalles de ViewState
                _detalles = ViewState["Detalles"] as List<CrearFacturaDetalleDto> ?? new List<CrearFacturaDetalleDto>();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar la página: " + ex.Message, true);
            }
        }

        private void InicializarFormulario()
        {
            try
            {
                // Establecer fecha actual
                txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                // Inicializar la lista de detalles
                _detalles = new List<CrearFacturaDetalleDto>();
                ViewState["Detalles"] = _detalles;

                // Vincular la grilla de detalles
                ActualizarGrillaDetalles();

                // Calcular totales iniciales
                CalcularTotales();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al inicializar el formulario: " + ex.Message, true);
            }
        }

        protected void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                string documento = txtDocumentoCliente.Text.Trim();
                if (string.IsNullOrEmpty(documento))
                {
                    MostrarMensaje("Debe ingresar un número de documento para buscar el cliente.", true);
                    return;
                }

                // Buscar cliente por documento
                var respuesta = _clienteService.ObtenerPorDocumentoAsync(documento).Result;
                if (respuesta.Exito && respuesta.Resultado != null)
                {
                    // Llenar los campos del cliente
                    var cliente = respuesta.Resultado;
                    txtIdCliente.Text = cliente.Id.ToString();
                    txtNombresCliente.Text = cliente.Nombres;
                    txtApellidosCliente.Text = cliente.Apellidos;
                    txtDireccionCliente.Text = cliente.Direccion;
                    txtTelefonoCliente.Text = cliente.Telefono;

                    MostrarMensaje($"Cliente encontrado: {cliente.NombreCompleto}", false);
                }
                else
                {
                    MostrarMensaje("Cliente no encontrado. Por favor ingrese los datos manualmente.", true);
                    txtIdCliente.Text = string.Empty;
                    txtNombresCliente.Text = string.Empty;
                    txtApellidosCliente.Text = string.Empty;
                    txtDireccionCliente.Text = string.Empty;
                    txtTelefonoCliente.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al buscar cliente: " + ex.Message, true);
            }
        }

        protected void btnBuscarArticulo_Click(object sender, EventArgs e)
        {
            try
            {
                string codigo = txtCodigoArticulo.Text.Trim();
                if (string.IsNullOrEmpty(codigo))
                {
                    MostrarMensaje("Debe ingresar un código de artículo para buscar.", true);
                    return;
                }

                // Buscar artículo por código
                var respuesta = _articuloService.ObtenerPorCodigoAsync(codigo).Result;
                if (respuesta.Exito && respuesta.Resultado != null)
                {
                    // Llenar los campos del artículo
                    var articulo = respuesta.Resultado;
                    txtNombreArticulo.Text = articulo.Nombre;
                    txtPrecioUnitario.Text = articulo.PrecioUnitario.ToString("F2", CultureInfo.InvariantCulture);
                    txtCantidad.Text = "1"; // Por defecto 1 unidad

                    // Calcular subtotal inicial
                    decimal precio = articulo.PrecioUnitario;
                    int cantidad = 1;
                    txtSubtotal.Text = (precio * cantidad).ToString("C", CultureInfo.CreateSpecificCulture("es-CO"));

                    // Enfocar el campo de cantidad
                    ScriptManager.RegisterStartupScript(this, GetType(), "FocusCantidad",
                        "setTimeout(function() { document.getElementById('" + txtCantidad.ClientID + "').focus(); }, 100);", true);
                }
                else
                {
                    MostrarMensaje("Artículo no encontrado. Verifique el código.", true);
                    txtNombreArticulo.Text = string.Empty;
                    txtPrecioUnitario.Text = string.Empty;
                    txtCantidad.Text = string.Empty;
                    txtSubtotal.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al buscar artículo: " + ex.Message, true);
            }
        }

        protected void btnAgregarArticulo_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos requeridos
                if (!string.IsNullOrEmpty(txtCodigoArticulo.Text) &&
                    !string.IsNullOrEmpty(txtCantidad.Text) &&
                    !string.IsNullOrEmpty(txtPrecioUnitario.Text))
                {
                    // Validar que sean números válidos
                    if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
                    {
                        MostrarMensaje("La cantidad debe ser un número entero mayor a cero.", true);
                        return;
                    }

                    if (!decimal.TryParse(txtPrecioUnitario.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precio) || precio <= 0)
                    {
                        MostrarMensaje("El precio debe ser un número válido mayor a cero.", true);
                        return;
                    }

                    // Validar stock disponible
                    var respuestaArticulo = _articuloService.ObtenerPorCodigoAsync(txtCodigoArticulo.Text.Trim()).Result;
                    if (!respuestaArticulo.Exito || respuestaArticulo.Resultado == null)
                    {
                        MostrarMensaje("No se pudo validar el artículo. Verifique el código.", true);
                        return;
                    }

                    var articulo = respuestaArticulo.Resultado;
                    if (articulo.Stock < cantidad)
                    {
                        MostrarMensaje($"No hay suficiente stock disponible. Stock actual: {articulo.Stock}", true);
                        return;
                    }

                    // Verificar si ya existe el artículo en la lista
                    int articuloIndex = _detalles.FindIndex(d => d.ArticuloId == articulo.Id);
                    if (articuloIndex >= 0)
                    {
                        MostrarMensaje("El artículo ya está en la lista. Si desea modificarlo, primero elimínelo.", true);
                        return;
                    }

                    // Crear el detalle
                    var detalle = new CrearFacturaDetalleDto
                    {
                        ArticuloId = articulo.Id,
                        ArticuloCodigo = articulo.Codigo,
                        ArticuloNombre = articulo.Nombre,
                        Cantidad = cantidad,
                        PrecioUnitario = precio,
                        StockDisponible = articulo.Stock
                    };

                    // Agregar a la lista
                    _detalles.Add(detalle);
                    ViewState["Detalles"] = _detalles;

                    // Actualizar grilla y totales
                    ActualizarGrillaDetalles();
                    CalcularTotales();

                    // Limpiar campos del detalle
                    LimpiarCamposDetalle();

                    MostrarMensaje($"Artículo '{articulo.Nombre}' agregado a la factura.", false);
                }
                else
                {
                    MostrarMensaje("Debe completar todos los campos del artículo.", true);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al agregar artículo: " + ex.Message, true);
            }
        }

        protected void gvDetalles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Eliminar")
                {
                    // Obtener índice de la fila
                    int index = Convert.ToInt32(e.CommandArgument);

                    // Eliminar el detalle
                    if (index >= 0 && index < _detalles.Count)
                    {
                        string nombreArticulo = _detalles[index].ArticuloNombre;
                        _detalles.RemoveAt(index);
                        ViewState["Detalles"] = _detalles;

                        // Actualizar grilla y totales
                        ActualizarGrillaDetalles();
                        CalcularTotales();

                        MostrarMensaje($"Artículo '{nombreArticulo}' eliminado de la factura.", false);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al procesar la acción: " + ex.Message, true);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que haya al menos un artículo
                if (_detalles == null || _detalles.Count == 0)
                {
                    MostrarMensaje("Debe agregar al menos un artículo a la factura.", true);
                    return;
                }

                // Validar campos del cliente
                if (string.IsNullOrEmpty(txtDocumentoCliente.Text) ||
                    string.IsNullOrEmpty(txtNombresCliente.Text) ||
                    string.IsNullOrEmpty(txtApellidosCliente.Text) ||
                    string.IsNullOrEmpty(txtDireccionCliente.Text) ||
                    string.IsNullOrEmpty(txtTelefonoCliente.Text))
                {
                    MostrarMensaje("Debe completar todos los datos del cliente.", true);
                    return;
                }

                // Crear objeto factura
                var factura = new CrearFacturaDto
                {
                    ClienteId = string.IsNullOrEmpty(txtIdCliente.Text) ? 0 : Convert.ToInt32(txtIdCliente.Text),
                    Observaciones = txtObservaciones.Text,
                    Detalles = _detalles
                };

                // Si el cliente no existe en la base de datos (ID = 0), debemos crear uno nuevo
                if (factura.ClienteId == 0)
                {
                    var nuevoCliente = new ClienteDto
                    {
                        NumeroDocumento = txtDocumentoCliente.Text.Trim(),
                        Nombres = txtNombresCliente.Text.Trim(),
                        Apellidos = txtApellidosCliente.Text.Trim(),
                        Direccion = txtDireccionCliente.Text.Trim(),
                        Telefono = txtTelefonoCliente.Text.Trim()
                    };

                    var respuestaCliente = _clienteService.CrearAsync(nuevoCliente).Result;
                    if (respuestaCliente.Exito && respuestaCliente.Resultado != null)
                    {
                        factura.ClienteId = respuestaCliente.Resultado.Id;
                    }
                    else
                    {
                        MostrarMensaje("Error al crear el cliente: " + respuestaCliente.Detalle, true);
                        return;
                    }
                }

                // Guardar la factura
                var respuesta = _facturaService.CrearAsync(factura).Result;
                if (respuesta.Exito && respuesta.Resultado != null)
                {
                    var facturaCreada = respuesta.Resultado;

                    // Mostrar mensaje de éxito
                    MostrarMensaje($"Factura #{facturaCreada.NumeroFactura} creada exitosamente por un valor de {facturaCreada.TotalFormateado}", false);

                    // Deshabilitar controles
                    DeshabilitarControles();

                    // Mostrar botón para crear nueva factura
                    btnGuardar.Text = "Nueva Factura";
                    btnGuardar.CommandName = "Nueva";
                }
                else
                {
                    MostrarMensaje("Error al crear la factura: " + respuesta.Detalle, true);
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar la factura: " + ex.Message, true);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                // Redirigir a la lista de facturas
                Response.Redirect("ListarFacturas.aspx");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cancelar: " + ex.Message, true);
            }
        }

        #region Métodos Auxiliares

        private void ActualizarGrillaDetalles()
        {
            gvDetalles.DataSource = _detalles;
            gvDetalles.DataBind();
        }

        private void CalcularTotales()
        {
            try
            {
                // Valor bruto (subtotal)
                decimal valorBruto = _detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
                hdnValorBruto.Value = valorBruto.ToString(CultureInfo.InvariantCulture);
                lblValorBruto.Text = valorBruto.ToString("C", CultureInfo.CreateSpecificCulture("es-CO"));

                // Descuento (5% si valor bruto >= $500,000)
                decimal valorDescuento = 0;
                if (valorBruto >= 500000)
                {
                    valorDescuento = Math.Round(valorBruto * 0.05m, 2);
                }
                hdnValorDescuento.Value = valorDescuento.ToString(CultureInfo.InvariantCulture);
                lblValorDescuento.Text = valorDescuento.ToString("C", CultureInfo.CreateSpecificCulture("es-CO"));

                // Base impuestos
                decimal baseImpuestos = valorBruto - valorDescuento;

                // IVA (19%)
                decimal valorIVA = Math.Round(baseImpuestos * 0.19m, 2);
                hdnValorIVA.Value = valorIVA.ToString(CultureInfo.InvariantCulture);
                lblValorIVA.Text = valorIVA.ToString("C", CultureInfo.CreateSpecificCulture("es-CO"));

                // Total
                decimal valorTotal = baseImpuestos + valorIVA;
                hdnValorTotal.Value = valorTotal.ToString(CultureInfo.InvariantCulture);
                lblValorTotal.Text = valorTotal.ToString("C", CultureInfo.CreateSpecificCulture("es-CO"));
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al calcular totales: " + ex.Message, true);
            }
        }

        private void LimpiarCamposDetalle()
        {
            txtCodigoArticulo.Text = string.Empty;
            txtNombreArticulo.Text = string.Empty;
            txtCantidad.Text = string.Empty;
            txtPrecioUnitario.Text = string.Empty;
            txtSubtotal.Text = string.Empty;
        }

        private void DeshabilitarControles()
        {
            // Deshabilitar todos los controles de entrada
            txtDocumentoCliente.Enabled = false;
            txtNombresCliente.Enabled = false;
            txtApellidosCliente.Enabled = false;
            txtDireccionCliente.Enabled = false;
            txtTelefonoCliente.Enabled = false;
            txtObservaciones.Enabled = false;
            txtCodigoArticulo.Enabled = false;
            txtCantidad.Enabled = false;
            txtPrecioUnitario.Enabled = false;
            btnBuscarCliente.Enabled = false;
            btnBuscarArticulo.Enabled = false;
            btnAgregarArticulo.Enabled = false;
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