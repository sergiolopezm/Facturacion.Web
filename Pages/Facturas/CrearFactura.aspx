<%@ Page Title="Crear Factura" Language="C#" MasterPageFile="~/Pages/Master/SiteMaster" AutoEventWireup="true" CodeBehind="CrearFactura.aspx.cs" Inherits="Facturacion.Web.Pages.Facturas.CrearFactura" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/forms.css" rel="stylesheet" />
    <script src="../../Scripts/factura-calculations.js" type="text/javascript"></script>
    <script src="../../Scripts/validation.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h2><i class="fa fa-file-invoice"></i> Crear Nueva Factura</h2>
    </div>

    <asp:Panel ID="panelMensaje" runat="server" Visible="false" CssClass="alert alert-dismissible">
        <button type="button" class="close" data-dismiss="alert">&times;</button>
        <asp:Literal ID="ltlMensaje" runat="server"></asp:Literal>
    </asp:Panel>

    <div class="form-container">
        <!-- Encabezado de la factura -->
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h3 class="panel-title">Encabezado de Factura</h3>
            </div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtFecha">Fecha:</label>
                            <asp:TextBox ID="txtFecha" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtNumeroFactura">Número de Factura:</label>
                            <asp:TextBox ID="txtNumeroFactura" runat="server" CssClass="form-control" ReadOnly="true" Text="[Autogenerado]"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <!-- Datos del Cliente -->
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtDocumentoCliente">Documento Cliente: <span class="text-danger">*</span></label>
                            <div class="input-group">
                                <asp:TextBox ID="txtDocumentoCliente" runat="server" CssClass="form-control" 
                                    placeholder="Ingrese documento" MaxLength="15" 
                                    onchange="buscarCliente()"></asp:TextBox>
                                <span class="input-group-btn">
                                    <asp:Button ID="btnBuscarCliente" runat="server" CssClass="btn btn-info" 
                                        Text="Buscar" OnClick="btnBuscarCliente_Click" />
                                </span>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvDocumentoCliente" runat="server" 
                                ControlToValidate="txtDocumentoCliente" CssClass="text-danger"
                                ErrorMessage="El documento del cliente es obligatorio" 
                                Display="Dynamic" ValidationGroup="GuardarFactura"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtIdCliente">ID Cliente:</label>
                            <asp:TextBox ID="txtIdCliente" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtNombresCliente">Nombres: <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtNombresCliente" runat="server" CssClass="form-control" 
                                placeholder="Nombres del cliente" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvNombresCliente" runat="server" 
                                ControlToValidate="txtNombresCliente" CssClass="text-danger"
                                ErrorMessage="Los nombres del cliente son obligatorios" 
                                Display="Dynamic" ValidationGroup="GuardarFactura"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtApellidosCliente">Apellidos: <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtApellidosCliente" runat="server" CssClass="form-control" 
                                placeholder="Apellidos del cliente" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvApellidosCliente" runat="server" 
                                ControlToValidate="txtApellidosCliente" CssClass="text-danger"
                                ErrorMessage="Los apellidos del cliente son obligatorios" 
                                Display="Dynamic" ValidationGroup="GuardarFactura"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtDireccionCliente">Dirección: <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtDireccionCliente" runat="server" CssClass="form-control" 
                                placeholder="Dirección del cliente" MaxLength="250"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDireccionCliente" runat="server" 
                                ControlToValidate="txtDireccionCliente" CssClass="text-danger"
                                ErrorMessage="La dirección del cliente es obligatoria" 
                                Display="Dynamic" ValidationGroup="GuardarFactura"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group">
                            <label for="txtTelefonoCliente">Teléfono: <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtTelefonoCliente" runat="server" CssClass="form-control" 
                                placeholder="Teléfono del cliente" MaxLength="20"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTelefonoCliente" runat="server" 
                                ControlToValidate="txtTelefonoCliente" CssClass="text-danger"
                                ErrorMessage="El teléfono del cliente es obligatorio" 
                                Display="Dynamic" ValidationGroup="GuardarFactura"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
                
                <div class="row">
                    <div class="col-md-12">
                        <div class="form-group">
                            <label for="txtObservaciones">Observaciones:</label>
                            <asp:TextBox ID="txtObservaciones" runat="server" CssClass="form-control" 
                                TextMode="MultiLine" Rows="2" MaxLength="500" placeholder="Observaciones (opcional)"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Detalle de la factura -->
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h3 class="panel-title">Detalle de Factura</h3>
            </div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="txtCodigoArticulo">Código Artículo: <span class="text-danger">*</span></label>
                            <div class="input-group">
                                <asp:TextBox ID="txtCodigoArticulo" runat="server" CssClass="form-control" 
                                    placeholder="Código" onchange="buscarArticulo()"></asp:TextBox>
                                <span class="input-group-btn">
                                    <asp:Button ID="btnBuscarArticulo" runat="server" CssClass="btn btn-info" 
                                        Text="Buscar" OnClick="btnBuscarArticulo_Click" CausesValidation="false" />
                                </span>
                            </div>
                            <asp:RequiredFieldValidator ID="rfvCodigoArticulo" runat="server" 
                                ControlToValidate="txtCodigoArticulo" CssClass="text-danger"
                                ErrorMessage="El código del artículo es obligatorio" 
                                Display="Dynamic" ValidationGroup="AgregarArticulo"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label for="txtNombreArticulo">Nombre Artículo:</label>
                            <asp:TextBox ID="txtNombreArticulo" runat="server" CssClass="form-control" 
                                ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="form-group">
                            <label for="txtCantidad">Cantidad: <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" 
                                placeholder="Cantidad" TextMode="Number" min="1" onchange="calcularSubtotal()"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCantidad" runat="server" 
                                ControlToValidate="txtCantidad" CssClass="text-danger"
                                ErrorMessage="La cantidad es obligatoria" 
                                Display="Dynamic" ValidationGroup="AgregarArticulo"></asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="rvCantidad" runat="server" 
                                ControlToValidate="txtCantidad" CssClass="text-danger"
                                ErrorMessage="La cantidad debe ser mayor a 0" 
                                Display="Dynamic" MinimumValue="1" MaximumValue="9999" Type="Integer"
                                ValidationGroup="AgregarArticulo"></asp:RangeValidator>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="form-group">
                            <label for="txtPrecioUnitario">Precio Unitario: <span class="text-danger">*</span></label>
                            <asp:TextBox ID="txtPrecioUnitario" runat="server" CssClass="form-control" 
                                placeholder="Precio" onchange="calcularSubtotal()"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvPrecioUnitario" runat="server" 
                                ControlToValidate="txtPrecioUnitario" CssClass="text-danger"
                                ErrorMessage="El precio unitario es obligatorio" 
                                Display="Dynamic" ValidationGroup="AgregarArticulo"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revPrecioUnitario" runat="server" 
                                ControlToValidate="txtPrecioUnitario" CssClass="text-danger"
                                ErrorMessage="Ingrese un precio válido" 
                                Display="Dynamic" ValidationExpression="^\d+(\.\d{1,2})?$"
                                ValidationGroup="AgregarArticulo"></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="form-group">
                            <label for="txtSubtotal">Subtotal:</label>
                            <asp:TextBox ID="txtSubtotal" runat="server" CssClass="form-control" 
                                ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12 text-right">
                        <asp:Button ID="btnAgregarArticulo" runat="server" CssClass="btn btn-success" 
                            Text="Agregar Artículo" OnClick="btnAgregarArticulo_Click" ValidationGroup="AgregarArticulo" />
                    </div>
                </div>

                <!-- Grilla de artículos -->
                <div class="row mt-3">
                    <div class="col-md-12">
                        <div class="table-responsive">
                            <asp:GridView ID="gvDetalles" runat="server" AutoGenerateColumns="False" 
                                CssClass="table table-striped table-bordered table-hover" 
                                OnRowCommand="gvDetalles_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="ArticuloId" HeaderText="ID" />
                                    <asp:BoundField DataField="ArticuloCodigo" HeaderText="Código" />
                                    <asp:BoundField DataField="ArticuloNombre" HeaderText="Artículo" />
                                    <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                                    <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unit." DataFormatString="{0:C}" />
                                    <asp:BoundField DataField="Subtotal" HeaderText="Subtotal" DataFormatString="{0:C}" />
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEliminar" runat="server" 
                                                CommandName="Eliminar" 
                                                CommandArgument='<%# Container.DataItemIndex %>'
                                                CssClass="btn btn-danger btn-sm" 
                                                OnClientClick="return confirm('¿Está seguro de eliminar este artículo?');">
                                                <i class="fa fa-trash"></i> Eliminar
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="alert alert-info">
                                        No hay artículos agregados a la factura.
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </div>

                <!-- Totales -->
                <div class="row">
                    <div class="col-md-6">
                        <div class="alert alert-warning">
                            <strong>Nota:</strong> Si el valor bruto es igual o mayor a $500.000, se aplicará un descuento del 5%.
                        </div>
                    </div>
                    <div class="col-md-6">
                        <table class="table table-bordered table-totals">
                            <tr>
                                <td><strong>Valor Bruto:</strong></td>
                                <td class="text-right">
                                    <asp:Label ID="lblValorBruto" runat="server" Text="$0"></asp:Label>
                                    <asp:HiddenField ID="hdnValorBruto" runat="server" Value="0" />
                                </td>
                            </tr>
                            <tr>
                                <td><strong>Descuento (5%):</strong></td>
                                <td class="text-right">
                                    <asp:Label ID="lblValorDescuento" runat="server" Text="$0"></asp:Label>
                                    <asp:HiddenField ID="hdnValorDescuento" runat="server" Value="0" />
                                </td>
                            </tr>
                            <tr>
                                <td><strong>IVA (19%):</strong></td>
                                <td class="text-right">
                                    <asp:Label ID="lblValorIVA" runat="server" Text="$0"></asp:Label>
                                    <asp:HiddenField ID="hdnValorIVA" runat="server" Value="0" />
                                </td>
                            </tr>
                            <tr class="info">
                                <td><strong>TOTAL:</strong></td>
                                <td class="text-right">
                                    <strong><asp:Label ID="lblValorTotal" runat="server" Text="$0"></asp:Label></strong>
                                    <asp:HiddenField ID="hdnValorTotal" runat="server" Value="0" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <!-- Botones de acción -->
        <div class="form-actions text-right">
            <asp:Button ID="btnCancelar" runat="server" CssClass="btn btn-default" 
                Text="Cancelar" OnClick="btnCancelar_Click" CausesValidation="false" />
            <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-primary" 
                Text="Guardar Factura" OnClick="btnGuardar_Click" ValidationGroup="GuardarFactura" />
        </div>
    </div>

    <script type="text/javascript">
        function buscarCliente() {
            // Esta función se implementará en el archivo factura-calculations.js
        }
        <a href="../Articulos/">../Articulos/</a>
        function buscarArticulo() {
            // Esta función se implementará en el archivo factura-calculations.js
        }

        function calcularSubtotal() {
            // Esta función se implementará en el archivo factura-calculations.js
        }
    </script>
</asp:Content>
