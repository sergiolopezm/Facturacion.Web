<%@ Page Title="Listar Facturas" Language="C#" MasterPageFile="~/Pages/Master/SiteMaster" AutoEventWireup="true" CodeBehind="ListarFacturas.aspx.cs" Inherits="Facturacion.Web.Pages.Facturas.ListarFacturas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Styles/grids.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="row">
            <div class="col-md-6">
                <h2><i class="fa fa-list-alt"></i> Listado de Facturas</h2>
            </div>
            <div class="col-md-6 text-right">
                <asp:Button ID="btnNuevaFactura" runat="server" CssClass="btn btn-primary" 
                    Text="Nueva Factura" OnClick="btnNuevaFactura_Click" />
            </div>
        </div>
    </div>

    <asp:Panel ID="panelMensaje" runat="server" Visible="false" CssClass="alert alert-dismissible">
        <button type="button" class="close" data-dismiss="alert">&times;</button>
        <asp:Literal ID="ltlMensaje" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Filtros de búsqueda -->
    <div class="panel panel-default">
        <div class="panel-heading">
            <h3 class="panel-title">Filtros de Búsqueda</h3>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-3">
                    <div class="form-group">
                        <label for="txtBusqueda">Búsqueda:</label>
                        <asp:TextBox ID="txtBusqueda" runat="server" CssClass="form-control" 
                            placeholder="Número, cliente o documento"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-2">
                    <div class="form-group">
                        <label for="txtFechaInicio">Fecha Inicio:</label>
                        <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control" 
                            TextMode="Date"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-2">
                    <div class="form-group">
                        <label for="txtFechaFin">Fecha Fin:</label>
                        <asp:TextBox ID="txtFechaFin" runat="server" CssClass="form-control" 
                            TextMode="Date"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-2">
                    <div class="form-group">
                        <label for="ddlEstado">Estado:</label>
                        <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Todos" Value=""></asp:ListItem>
                            <asp:ListItem Text="Activa" Value="Activa"></asp:ListItem>
                            <asp:ListItem Text="Anulada" Value="Anulada"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="form-group">
                        <label>&nbsp;</label>
                        <div>
                            <asp:Button ID="btnBuscar" runat="server" CssClass="btn btn-primary" 
                                Text="Buscar" OnClick="btnBuscar_Click" />
                            <asp:Button ID="btnLimpiar" runat="server" CssClass="btn btn-default" 
                                Text="Limpiar" OnClick="btnLimpiar_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Grilla de facturas -->
    <div class="panel panel-default">
        <div class="panel-heading">
            <h3 class="panel-title">Facturas Registradas</h3>
        </div>
        <div class="panel-body">
            <div class="table-responsive">
                <asp:GridView ID="gvFacturas" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-striped table-bordered table-hover" 
                    OnRowCommand="gvFacturas_RowCommand" AllowPaging="True" 
                    OnPageIndexChanging="gvFacturas_PageIndexChanging"
                    PageSize="10" PagerSettings-Mode="NumericFirstLast" 
                    PagerSettings-FirstPageText="&laquo;" PagerSettings-LastPageText="&raquo;"
                    PagerSettings-PageButtonCount="5" PagerStyle-CssClass="pagination-container">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="NumeroFactura" HeaderText="Número Factura" />
                        <asp:BoundField DataField="FechaFormateada" HeaderText="Fecha" />
                        <asp:BoundField DataField="ClienteNombreCompleto" HeaderText="Cliente" />
                        <asp:BoundField DataField="ClienteNumeroDocumento" HeaderText="Documento" />
                        <asp:BoundField DataField="TotalFormateado" HeaderText="Total" />
                        <asp:BoundField DataField="Estado" HeaderText="Estado" />
                        <asp:BoundField DataField="TotalArticulos" HeaderText="Artículos" />
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkVerDetalle" runat="server" 
                                    CommandName="VerDetalle" 
                                    CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn btn-info btn-sm">
                                    <i class="fa fa-eye"></i> Ver
                                </asp:LinkButton>
                                <asp:LinkButton ID="lnkAnular" runat="server" 
                                    CommandName="Anular" 
                                    CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn btn-danger btn-sm" 
                                    Visible='<%# Eval("Estado").ToString() == "Activa" %>'
                                    OnClientClick="return confirm('¿Está seguro de anular esta factura?');">
                                    <i class="fa fa-ban"></i> Anular
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="alert alert-info">
                            No se encontraron facturas con los criterios de búsqueda especificados.
                        </div>
                    </EmptyDataTemplate>
                    <PagerStyle CssClass="pagination-container" />
                </asp:GridView>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <asp:Label ID="lblPaginacion" runat="server" CssClass="pagination-info"></asp:Label>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal para Anular Factura -->
    <div class="modal fade" id="modalAnular" tabindex="-1" role="dialog" aria-labelledby="modalAnularLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="modalAnularLabel">Anular Factura</h4>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label for="txtMotivoAnulacion">Motivo de Anulación:</label>
                        <asp:TextBox ID="txtMotivoAnulacion" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="3" MaxLength="250"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvMotivoAnulacion" runat="server" 
                            ControlToValidate="txtMotivoAnulacion" CssClass="text-danger"
                            ErrorMessage="El motivo de anulación es obligatorio" 
                            Display="Dynamic" ValidationGroup="Anulacion"></asp:RequiredFieldValidator>
                    </div>
                    <asp:HiddenField ID="hdnFacturaId" runat="server" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnConfirmarAnulacion" runat="server" CssClass="btn btn-danger" 
                        Text="Anular Factura" ValidationGroup="Anulacion" OnClick="btnConfirmarAnulacion_Click" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function abrirModalAnulacion(facturaId) {
            $('#<%= hdnFacturaId.ClientID %>').val(facturaId);
            $('#modalAnular').modal('show');
            return false;
        }
    </script>
</asp:Content>