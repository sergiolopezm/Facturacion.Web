// Funciones para manejo de cálculos y validaciones en el formulario de facturación

// Búsqueda de cliente por documento
function buscarCliente() {
    var documento = $('#txtDocumentoCliente').val();
    if (documento.trim() === '') {
        return;
    }

    // Mostrar indicador de carga
    $('#btnBuscarCliente').html('<i class="fa fa-spinner fa-spin"></i>');
    $('#btnBuscarCliente').prop('disabled', true);

    // Simular clic en el botón de búsqueda del servidor
    // Esto activará el evento btnBuscarCliente_Click en el servidor
    setTimeout(function () {
        __doPostBack('btnBuscarCliente', '');
    }, 100);
}

// Búsqueda de artículo por código
function buscarArticulo() {
    var codigo = $('#txtCodigoArticulo').val();
    if (codigo.trim() === '') {
        return;
    }

    // Mostrar indicador de carga
    $('#btnBuscarArticulo').html('<i class="fa fa-spinner fa-spin"></i>');
    $('#btnBuscarArticulo').prop('disabled', true);

    // Simular clic en el botón de búsqueda del servidor
    setTimeout(function () {
        __doPostBack('btnBuscarArticulo', '');
    }, 100);
}

// Calcular subtotal del artículo
function calcularSubtotal() {
    var cantidad = parseInt($('#txtCantidad').val()) || 0;
    var precio = parseFloat($('#txtPrecioUnitario').val().replace(',', '.')) || 0;

    if (cantidad <= 0 || precio <= 0) {
        $('#txtSubtotal').val('');
        return;
    }

    var subtotal = cantidad * precio;

    // Formatear como moneda
    var formatter = new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        minimumFractionDigits: 2
    });

    $('#txtSubtotal').val(formatter.format(subtotal));
}

// Validación de campos numéricos
function validarNumerico(event, esEntero) {
    // Permitir: backspace, delete, tab, escape, enter y .
    if ($.inArray(event.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
        // Permitir: Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
        (event.keyCode === 65 && event.ctrlKey === true) ||
        (event.keyCode === 67 && event.ctrlKey === true) ||
        (event.keyCode === 86 && event.ctrlKey === true) ||
        (event.keyCode === 88 && event.ctrlKey === true) ||
        // Permitir: home, end, left, right
        (event.keyCode >= 35 && event.keyCode <= 39)) {
        return;
    }

    // Si es entero, no permitir punto decimal
    if (esEntero && (event.key === '.' || event.key === ',')) {
        event.preventDefault();
        return;
    }

    // Asegurar que sea un número
    if ((event.shiftKey || (event.keyCode < 48 || event.keyCode > 57)) &&
        (event.keyCode < 96 || event.keyCode > 105)) {
        event.preventDefault();
    }
}

// Configurar eventos cuando el documento esté listo
$(document).ready(function () {
    // Validar que cantidad sea un entero
    $('#txtCantidad').on('keydown', function (event) {
        validarNumerico(event, true);
    });

    // Validar que precio sea un número decimal
    $('#txtPrecioUnitario').on('keydown', function (event) {
        validarNumerico(event, false);
    });

    // Calcular subtotal cuando cambie cantidad o precio
    $('#txtCantidad, #txtPrecioUnitario').on('change keyup', function () {
        calcularSubtotal();
    });

    // Buscar cliente cuando se presiona Enter en el campo de documento
    $('#txtDocumentoCliente').on('keypress', function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            buscarCliente();
        }
    });

    // Buscar artículo cuando se presiona Enter en el campo de código
    $('#txtCodigoArticulo').on('keypress', function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            buscarArticulo();
        }
    });
});