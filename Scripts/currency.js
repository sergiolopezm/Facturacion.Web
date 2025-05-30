/**
 * currency.js
 * Funciones para manejo de moneda y formato en el lado cliente
 */

var Facturacion = Facturacion || {};

// Módulo de funciones de moneda
Facturacion.Currency = (function () {
    // Configuración
    var config = {
        symbol: '$',
        decimalSeparator: ',',
        thousandSeparator: '.',
        decimalPlaces: 2,
        currencyClass: 'currency-value'
    };

    // Inicializar configuración
    function initialize(options) {
        if (options) {
            config.symbol = options.symbol || '$';
            config.decimalSeparator = options.decimalSeparator || ',';
            config.thousandSeparator = options.thousandSeparator || '.';
            config.decimalPlaces = options.decimalPlaces || 2;
            config.currencyClass = options.currencyClass || 'currency-value';
        }
    }

    // Dar formato a un valor numérico como moneda
    function format(value, includeSymbol) {
        if (value === null || value === undefined) return '';

        // Asegurar que value sea un número
        value = parseFloat(value);
        if (isNaN(value)) return '';

        // Formatear el número
        var parts = value.toFixed(config.decimalPlaces).split('.');
        var integerPart = parts[0];
        var decimalPart = parts.length > 1 ? parts[1] : '';

        // Agregar separador de miles
        integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, config.thousandSeparator);

        // Construir el resultado
        var result = integerPart;
        if (decimalPart.length > 0) {
            result += config.decimalSeparator + decimalPart;
        }

        // Agregar símbolo si se solicita
        if (includeSymbol !== false) {
            result = config.symbol + result;
        }

        return result;
    }

    // Convertir un string con formato de moneda a número
    function parse(value) {
        if (!value) return 0;

        // Remover el símbolo
        value = value.toString().replace(config.symbol, '');

        // Remover separadores de miles y convertir separador decimal
        value = value.replace(new RegExp('\\' + config.thousandSeparator, 'g'), '')
            .replace(config.decimalSeparator, '.');

        return parseFloat(value) || 0;
    }

    // Validar si un valor tiene formato de moneda válido
    function isValid(value) {
        if (!value) return false;

        // Permitir símbolo al inicio
        var pattern = '^';
        if (config.symbol) {
            pattern += '\\' + config.symbol + '?';
        }

        // Patrón para números con separador de miles y decimal
        pattern += '\\d{1,3}(\\' + config.thousandSeparator + '\\d{3})*';

        // Permitir decimales
        if (config.decimalPlaces > 0) {
            pattern += '(\\' + config.decimalSeparator + '\\d{1,' + config.decimalPlaces + '})?';
        }

        pattern += '$';

        var regex = new RegExp(pattern);
        return regex.test(value);
    }

    // Aplicar formato a todos los elementos con clase currency-value
    function applyFormatToElements() {
        $('.' + config.currencyClass).each(function () {
            var $this = $(this);
            var value = $this.data('value') || $this.text();

            if (value) {
                value = parse(value);
                $this.text(format(value));
                $this.data('value', value);
            }
        });
    }

    // Configurar máscara para inputs de moneda
    function setupCurrencyInputs() {
        if ($.fn.inputmask) {
            $('.currency-input').inputmask('numeric', {
                radixPoint: config.decimalSeparator,
                groupSeparator: config.thousandSeparator,
                digits: config.decimalPlaces,
                autoGroup: true,
                prefix: config.symbol + ' ',
                rightAlign: false,
                allowMinus: false
            });
        }
    }

    // Calcular subtotal (precio * cantidad)
    function calculateSubtotal(price, quantity) {
        price = parse(price);
        quantity = parseInt(quantity) || 0;
        return price * quantity;
    }

    // Calcular IVA
    function calculateIVA(value, percentage) {
        value = parse(value);
        percentage = percentage || 19; // IVA Colombia por defecto
        return value * (percentage / 100);
    }

    // Calcular descuento
    function calculateDiscount(value, percentage, minimumValue) {
        value = parse(value);
        percentage = percentage || 5; // Descuento por defecto
        minimumValue = minimumValue || 500000; // Valor mínimo para aplicar descuento

        if (value >= minimumValue) {
            return value * (percentage / 100);
        }
        return 0;
    }

    // Calcular total
    function calculateTotal(subtotal, discount, iva) {
        subtotal = parse(subtotal);
        discount = parse(discount);
        iva = parse(iva);

        return subtotal - discount + iva;
    }

    // Inicializar cuando el documento está listo
    $(document).ready(function () {
        // Aplicar formato a elementos
        applyFormatToElements();

        // Configurar inputs
        setupCurrencyInputs();
    });

    // API pública
    return {
        initialize: initialize,
        format: format,
        parse: parse,
        isValid: isValid,
        calculateSubtotal: calculateSubtotal,
        calculateIVA: calculateIVA,
        calculateDiscount: calculateDiscount,
        calculateTotal: calculateTotal,
        applyFormatToElements: applyFormatToElements,
        setupCurrencyInputs: setupCurrencyInputs
    };
})();

// Inicializar con valores por defecto
Facturacion.Currency.initialize({
    symbol: '$',
    decimalSeparator: ',',
    thousandSeparator: '.',
    decimalPlaces: 2
});