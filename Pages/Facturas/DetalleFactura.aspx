<%@ Page Title="Detalle de Factura" Language="C#" MasterPageFile="~/Pages/Master/SiteMaster" AutoEventWireup="true" CodeBehind="DetalleFactura.aspx.cs" Inherits="Facturacion.Web.Pages.Facturas.DetalleFactura" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container my-4">
        <div class="card">
            <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                <h3>Detalle de Factura <asp:Label ID="lblNumeroFactura" runat="server" CssClass="badge bg-light text-primary ms-2"></asp:Label></h3>
                <div>
                    <asp:Button ID="btnImprimir" runat="server" Text="Imprimir" CssClass="btn btn-light me-2" OnClientClick="window.print(); return false;" />
                    <asp:Button ID="btnVolver" runat="server" Text="Volver a la Lista" CssClass="btn btn-light" OnClick="btnVolver_Click" />
                </div>
            </div>
            <div class="card-body">
                <!-- Alertas para mensajes -->
                <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-dismissible fade show">
                    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </asp:Panel>

                <!-- Información de la factura -->
                <div class="row">
                    <div class="col-md-6">
                        <div class="card mb-3">
                            <div class="card-header bg-light">
                                <h5 class="mb-0">Información de la Factura</h5>
                            </div>
                            <div class="card-body">
                                <div class="row mb-2">
                                    <div class="col-md-4 fw-bold">Número:</div>
                                    <div class="col-md-8"><asp:Label ID="lblNumero" runat="server"></asp:Label></div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-md-4 fw-bold">Estado:</div>
                                    <div class="col-md-8"><asp:Label ID="lblEstado" runat="server"></asp:Label></div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-md-4 fw-bold">Fecha:</div>
                                    <div class="col-md-8"><asp:Label ID="lblFecha" runat="server"></asp:Label></div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-md-4 fw-bold">Usuario:</div>
                                    <div class="col-md-8"><asp:Label ID="lblUsuario" runat="server"></asp:Label></div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-md-4 fw-bold">Observaciones:</div>
                                    <div class="col-md-8"><asp:Label ID="lblObservaciones" runat="server"></asp:Label></div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="card mb-3">
                            <div class="card-header bg-light">
                                <h5 class="mb-0">Información del Cliente</h5>
                            </div>
                            <div class="card-body">
                                <div class="row mb-2">
                                    <div class="col-md-4 fw-bold">Nombre:</div>
                                    <div class="col-md-8"><asp:Label ID="lblCliente" runat="server"></asp:Label></div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-md-4 fw-bold">Documento:</div>
                                    <div class="col-md-8"><asp:Label ID="lblDocumento" runat="server"></asp:Label></div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-md-4 fw-bold">Dirección:</div>
                                    <div class="col-md-8"><asp:Label ID="lblDireccion" runat="server"></asp:Label></div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-md-4 fw-bold">Teléfono:</div>
                                    <div class="col-md-8"><asp:Label ID="lblTelefono" runat="server"></asp:Label></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Detalles de factura -->
                <div class="card mt-3">
                    <div class="card-header bg-light">
                        <h5 class="mb-0">Artículos</h5>
                    </div>
                    <div class="card-body">
                        <asp:GridView ID="gvDetalles" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover table-bordered"
                            EmptyDataText="No hay artículos en esta factura">
                            <Columns>
                                <asp:BoundField DataField="ArticuloCodigo" HeaderText="Código" />
                                <asp:BoundField DataField="ArticuloNombre" HeaderText="Artículo" />
                                <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="PrecioUnitarioFormateado" HeaderText="Precio Unitario" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="SubtotalFormateado" HeaderText="Subtotal" ItemStyle-HorizontalAlign="Right" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

                <!-- Totales -->
                <div class="row mt-3">
                    <div class="col-md-6 offset-md-6">
                        <div class="card">
                            <div class="card-header bg-light">
                                <h5 class="mb-0">Totales</h5>
                            </div>
                            <div class="card-body">
                                <div class="row mb-2">
                                    <div class="col-6 text-end fw-bold">Subtotal:</div>
                                    <div class="col-6 text-end"><asp:Label ID="lblSubtotal" runat="server"></asp:Label></div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-6 text-end fw-bold">Descuento (<asp:Label ID="lblPorcentajeDescuento" runat="server"></asp:Label>%):</div>
                                    <div class="col-6 text-end"><asp:Label ID="lblDescuento" runat="server"></asp:Label></div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-6 text-end fw-bold">Base Imponible:</div>
                                    <div class="col-6 text-end"><asp:Label ID="lblBaseImponible" runat="server"></asp:Label></div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-6 text-end fw-bold">IVA (<asp:Label ID="lblPorcentajeIva" runat="server"></asp:Label>%):</div>
                                    <div class="col-6 text-end"><asp:Label ID="lblIva" runat="server"></asp:Label></div>
                                </div>
                                <div class="row">
                                    <div class="col-6 text-end fw-bold">TOTAL:</div>
                                    <div class="col-6 text-end fw-bold"><asp:Label ID="lblTotal" runat="server" CssClass="text-primary fs-5"></asp:Label></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Botones de acción -->
                <div class="mt-4 text-center">
                    <asp:Button ID="btnAnular" runat="server" Text="Anular Factura" CssClass="btn btn-danger" OnClick="btnAnular_Click" OnClientClick="return confirm('¿Está seguro que desea anular esta factura?');" />
                </div>
            </div>
        </div>
    </div>

    <!-- Modal para motivo de anulación -->
    <div class="modal fade" id="modalAnular" tabindex="-1" aria-labelledby="modalAnularLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalAnularLabel">Anular Factura</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label for="txtMotivo" class="form-label">Motivo de Anulación:</label>
                        <asp:TextBox ID="txtMotivo" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvMotivo" runat="server" ControlToValidate="txtMotivo"
                            ErrorMessage="El motivo es requerido" CssClass="text-danger" Display="Dynamic" 
                            ValidationGroup="Anulacion"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnConfirmarAnulacion" runat="server" Text="Anular" CssClass="btn btn-danger"
                        OnClick="btnConfirmarAnulacion_Click" ValidationGroup="Anulacion" />
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function mostrarModalAnulacion() {
            var modal = new bootstrap.Modal(document.getElementById('modalAnular'));
            modal.show();
            return false;
        }
    </script>
</asp:Content>