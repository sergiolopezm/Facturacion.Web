// Funciones de validación para formularios

// Validación de campos requeridos
function validarCamposRequeridos(formId) {
    var formValido = true;
    $('#' + formId + ' .required').each(function () {
        var campo = $(this);
        var valor = campo.val().trim();

        if (valor === '') {
            marcarCampoInvalido(campo);
            formValido = false;
        } else {
            marcarCampoValido(campo);
        }
    });

    return formValido;
}

// Marcar un campo como inválido
function marcarCampoInvalido(campo) {
    campo.addClass('is-invalid');
    campo.removeClass('is-valid');

    // Mostrar mensaje de error si existe
    var errorMsg = campo.data('error-msg') || 'Este campo es requerido';
    var errorElement = campo.siblings('.invalid-feedback');

    if (errorElement.length === 0) {
        campo.after('<div class="invalid-feedback">' + errorMsg + '</div>');
    } else {
        errorElement.text(errorMsg);
    }
}

// Marcar un campo como válido
function marcarCampoValido(campo) {
    campo.removeClass('is-invalid');
    campo.addClass('is-valid');
    campo.siblings('.invalid-feedback').remove();
}

// Validar formato de email
function validarEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

// Validar que un valor sea numérico
function validarNumerico(valor) {
    return !isNaN(parseFloat(valor)) && isFinite(valor);
}

// Validar que un valor sea un entero
function validarEntero(valor) {
    return Number.isInteger(Number(valor));
}

// Configurar eventos de validación cuando el documento esté listo
$(document).ready(function () {
    // Validar al perder el foco
    $('.required').on('blur', function () {
        var campo = $(this);
        var valor = campo.val().trim();

        if (valor === '') {
            marcarCampoInvalido(campo);
        } else {
            marcarCampoValido(campo);
        }
    });

    // Validar campos de email
    $('.email').on('blur', function () {
        var campo = $(this);
        var valor = campo.val().trim();

        if (valor !== '' && !validarEmail(valor)) {
            campo.data('error-msg', 'Email inválido');
            marcarCampoInvalido(campo);
        } else {
            marcarCampoValido(campo);
        }
    });

    // Validar campos numéricos
    $('.numeric').on('blur', function () {
        var campo = $(this);
        var valor = campo.val().trim();

        if (valor !== '' && !validarNumerico(valor)) {
            campo.data('error-msg', 'Valor numérico inválido');
            marcarCampoInvalido(campo);
        } else {
            marcarCampoValido(campo);
        }
    });

    // Validar enteros
    $('.integer').on('blur', function () {
        var campo = $(this);
        var valor = campo.val().trim();

        if (valor !== '' && !validarEntero(valor)) {
            campo.data('error-msg', 'Debe ser un número entero');
            marcarCampoInvalido(campo);
        } else {
            marcarCampoValido(campo);
        }
    });
});