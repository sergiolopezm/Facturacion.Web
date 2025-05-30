/**
 * common.js
 * Funciones comunes para toda la aplicación
 */

var Facturacion = Facturacion || {};

// Módulo de funciones comunes
Facturacion.Common = (function () {
    // Inicializar tooltips, popovers, etc.
    function initializeComponents() {
        // Inicializar tooltips de Bootstrap
        if ($.fn.tooltip) {
            $('[data-toggle="tooltip"]').tooltip();
        }

        // Inicializar popovers de Bootstrap
        if ($.fn.popover) {
            $('[data-toggle="popover"]').popover();
        }

        // Inicializar DatePickers
        initializeDatepickers();

        // Inicializar validaciones de formularios
        initializeFormValidations();
    }

    // Inicializar datepickers
    function initializeDatepickers() {
        if ($.fn.datepicker) {
            $('.datepicker').datepicker({
                format: 'dd/mm/yyyy',
                language: 'es',
                autoclose: true,
                todayHighlight: true
            });
        }
    }

    // Inicializar validaciones de formularios
    function initializeFormValidations() {
        if ($.fn.validate) {
            $('form.needs-validation').validate({
                errorElement: 'span',
                errorClass: 'invalid-feedback',
                errorPlacement: function (error, element) {
                    error.insertAfter(element);
                },
                highlight: function (element) {
                    $(element).addClass('is-invalid').removeClass('is-valid');
                },
                unhighlight: function (element) {
                    $(element).addClass('is-valid').removeClass('is-invalid');
                }
            });
        }
    }

    // Mostrar mensaje al usuario
    function showMessage(title, message, type) {
        type = type || 'info';

        // Si hay una función de SweetAlert disponible
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: title,
                text: message,
                icon: type,
                confirmButtonText: 'Aceptar'
            });
        }
        // Si hay un componente toast disponible
        else if ($('#toast-container').length > 0) {
            var toastHtml =
                '<div class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-delay="5000">' +
                '  <div class="toast-header bg-' + getBootstrapClass(type) + ' text-white">' +
                '    <strong class="mr-auto">' + title + '</strong>' +
                '    <button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">' +
                '      <span aria-hidden="true">&times;</span>' +
                '    </button>' +
                '  </div>' +
                '  <div class="toast-body">' + message + '</div>' +
                '</div>';

            $('#toast-container').append(toastHtml);
            $('.toast').toast('show');
        }
        // Fallback a alert
        else {
            alert(title + ': ' + message);
        }
    }

    // Obtener clase de Bootstrap según el tipo de mensaje
    function getBootstrapClass(type) {
        switch (type) {
            case 'success': return 'success';
            case 'error': return 'danger';
            case 'warning': return 'warning';
            case 'info':
            default:
                return 'info';
        }
    }

    // Confirmar acción
    function confirmAction(title, message, callback) {
        // Si hay una función de SweetAlert disponible
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: title,
                text: message,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Sí, continuar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed && typeof callback === 'function') {
                    callback();
                }
            });
        }
        // Fallback a confirm
        else {
            if (confirm(title + ': ' + message) && typeof callback === 'function') {
                callback();
            }
        }
    }

    // Formatear valores de moneda
    function formatCurrency(value) {
        if (value === null || value === undefined) return '';

        value = parseFloat(value);
        if (isNaN(value)) return '';

        return '$' + value.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }

    // Parsear valores de moneda
    function parseCurrency(value) {
        if (!value) return 0;

        // Remover símbolo de moneda y comas
        value = value.replace(/\$/g, '').replace(/,/g, '');
        return parseFloat(value) || 0;
    }

    // Formatear fecha
    function formatDate(date, format) {
        if (!date) return '';

        format = format || 'dd/mm/yyyy';

        // Si es string, convertir a Date
        if (typeof date === 'string') {
            date = new Date(date);
        }

        // Si no es válida, retornar vacío
        if (!(date instanceof Date) || isNaN(date)) {
            return '';
        }

        var day = date.getDate().toString().padStart(2, '0');
        var month = (date.getMonth() + 1).toString().padStart(2, '0');
        var year = date.getFullYear();

        return format
            .replace('dd', day)
            .replace('mm', month)
            .replace('yyyy', year);
    }

    // Obtener parámetro de URL
    function getUrlParameter(name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        var results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    }

    // Serializar formulario a objeto JSON
    function serializeFormToJson(form) {
        var formData = {};
        var formArray = $(form).serializeArray();

        $.each(formArray, function () {
            if (formData[this.name]) {
                if (!formData[this.name].push) {
                    formData[this.name] = [formData[this.name]];
                }
                formData[this.name].push(this.value || '');
            } else {
                formData[this.name] = this.value || '';
            }
        });

        return formData;
    }

    // Llenar formulario desde objeto
    function fillFormFromObject(form, data) {
        if (!form || !data) return;

        $.each(data, function (key, value) {
            var ctrl = $('[name="' + key + '"]', form);

            if (ctrl.is('select')) {
                ctrl.val(value);
            }
            else if (ctrl.is('textarea')) {
                ctrl.val(value);
            }
            else if (ctrl.is(':checkbox')) {
                ctrl.prop('checked', value === true || value === 'true' || value === 1);
            }
            else if (ctrl.is(':radio')) {
                ctrl.filter('[value="' + value + '"]').prop('checked', true);
            }
            else {
                ctrl.val(value);
            }
        });
    }

    // Mostrar/ocultar indicador de carga
    function toggleLoader(show) {
        if (show) {
            if ($('#global-loader').length === 0) {
                $('body').append('<div id="global-loader" class="loader-container"><div class="loader"></div></div>');
            }
            $('#global-loader').show();
        } else {
            $('#global-loader').hide();
        }
    }

    // Detectar si es dispositivo móvil
    function isMobile() {
        return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
    }

    // Validar que un elemento esté en el viewport
    function isElementInViewport(el) {
        if (typeof jQuery === "function" && el instanceof jQuery) {
            el = el[0];
        }

        var rect = el.getBoundingClientRect();

        return (
            rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
            rect.right <= (window.innerWidth || document.documentElement.clientWidth)
        );
    }

    // Scroll suave a elemento
    function scrollToElement(element, offset) {
        offset = offset || 0;

        if (element) {
            var targetPosition = $(element).offset().top - offset;
            $('html, body').animate({
                scrollTop: targetPosition
            }, 500);
        }
    }

    // Inicializar cuando el documento está listo
    $(document).ready(function () {
        initializeComponents();
    });

    // API pública
    return {
        showMessage: showMessage,
        confirmAction: confirmAction,
        formatCurrency: formatCurrency,
        parseCurrency: parseCurrency,
        formatDate: formatDate,
        getUrlParameter: getUrlParameter,
        serializeFormToJson: serializeFormToJson,
        fillFormFromObject: fillFormFromObject,
        toggleLoader: toggleLoader,
        isMobile: isMobile,
        isElementInViewport: isElementInViewport,
        scrollToElement: scrollToElement,
        initializeComponents: initializeComponents
    };
})();