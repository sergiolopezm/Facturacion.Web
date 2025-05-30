/**
 * api-client.js
 * Cliente JavaScript para consumir las APIs del backend
 * Complementa la funcionalidad de ApiClient.cs en el lado del cliente
 */

// Namespace principal para evitar conflictos
var Facturacion = Facturacion || {};

// Módulo ApiClient
Facturacion.ApiClient = (function () {
    // Configuración del cliente
    var config = {
        baseUrl: '',
        timeoutMs: 30000, // 30 segundos
        sitio: '',
        clave: '',
        defaultHeaders: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        }
    };

    // Inicializar la configuración del cliente
    function initialize(options) {
        if (options) {
            config.baseUrl = options.baseUrl || '';
            config.timeoutMs = options.timeoutMs || 30000;
            config.sitio = options.sitio || '';
            config.clave = options.clave || '';

            if (options.headers) {
                for (var header in options.headers) {
                    config.defaultHeaders[header] = options.headers[header];
                }
            }
        }

        // Cargar token de autenticación si existe
        loadAuthToken();
    }

    // Cargar token desde sessionStorage
    function loadAuthToken() {
        try {
            var token = sessionStorage.getItem('FacturacionToken');
            if (token) {
                config.defaultHeaders['Authorization'] = 'Bearer ' + token;
            }
        } catch (e) {
            console.error('Error cargando token de autenticación:', e);
        }
    }

    // Guardar token en sessionStorage
    function saveAuthToken(token) {
        try {
            if (token) {
                sessionStorage.setItem('FacturacionToken', token);
                config.defaultHeaders['Authorization'] = 'Bearer ' + token;
            } else {
                sessionStorage.removeItem('FacturacionToken');
                delete config.defaultHeaders['Authorization'];
            }
        } catch (e) {
            console.error('Error guardando token de autenticación:', e);
        }
    }

    // Método principal para realizar peticiones AJAX
    function ajax(method, endpoint, data) {
        return new Promise(function (resolve, reject) {
            // Construir URL completa
            var url = config.baseUrl + endpoint;

            // Configuración de la petición
            var ajaxConfig = {
                type: method,
                url: url,
                data: data ? JSON.stringify(data) : null,
                headers: Object.assign({}, config.defaultHeaders),
                timeout: config.timeoutMs,
                success: function (response) {
                    resolve(handleSuccessResponse(response));
                },
                error: function (xhr, status, error) {
                    reject(handleErrorResponse(xhr, status, error, url));
                }
            };

            // Agregar headers de acceso a la API
            if (config.sitio) {
                ajaxConfig.headers['Sitio'] = config.sitio;
            }
            if (config.clave) {
                ajaxConfig.headers['Clave'] = config.clave;
            }

            // Realizar la petición
            $.ajax(ajaxConfig);
        });
    }

    // Manejar respuesta exitosa
    function handleSuccessResponse(response) {
        try {
            // Si la respuesta ya es un objeto (jQuery la parsea automáticamente)
            if (typeof response === 'object') {
                return response;
            }

            // Si es un string, intentar parsearlo como JSON
            return JSON.parse(response);
        } catch (e) {
            console.warn('Error parseando respuesta JSON:', e);
            return {
                Exito: true,
                Mensaje: 'Operación exitosa',
                Detalle: 'La operación se completó pero no se pudo procesar la respuesta',
                Resultado: response
            };
        }
    }

    // Manejar respuesta de error
    function handleErrorResponse(xhr, status, error, url) {
        var response = {
            Exito: false,
            Mensaje: 'Error en la petición',
            Detalle: error || 'Se produjo un error al comunicarse con el servidor',
            Resultado: null
        };

        try {
            // Intentar obtener mensaje de error del servidor
            if (xhr.responseText) {
                var errorResponse = JSON.parse(xhr.responseText);
                if (errorResponse) {
                    response.Mensaje = errorResponse.Mensaje || response.Mensaje;
                    response.Detalle = errorResponse.Detalle || response.Detalle;
                }
            }

            // Manejo específico de códigos HTTP
            switch (xhr.status) {
                case 401: // No autorizado
                    response.Mensaje = 'Sesión expirada';
                    response.Detalle = 'Su sesión ha expirado o no tiene autorización para esta operación';
                    // Redirigir a login
                    handleSessionExpired();
                    break;
                case 403: // Prohibido
                    response.Mensaje = 'Acceso denegado';
                    response.Detalle = 'No tiene permisos para realizar esta operación';
                    break;
                case 404: // No encontrado
                    response.Mensaje = 'Recurso no encontrado';
                    response.Detalle = 'El recurso solicitado no existe';
                    break;
                case 400: // Bad Request
                    response.Mensaje = response.Mensaje || 'Datos inválidos';
                    break;
                case 500: // Error del servidor
                    response.Mensaje = 'Error interno del servidor';
                    break;
                case 0: // Error de conexión
                    if (status === 'timeout') {
                        response.Mensaje = 'Tiempo de espera agotado';
                        response.Detalle = 'El servidor tardó demasiado en responder';
                    } else {
                        response.Mensaje = 'Error de conexión';
                        response.Detalle = 'No se pudo establecer conexión con el servidor';
                    }
                    break;
            }

            // Registrar error
            console.error('Error API [' + xhr.status + ']', {
                url: url,
                status: status,
                error: error,
                response: response
            });

            return response;
        } catch (e) {
            console.error('Error procesando respuesta de error:', e);
            return response;
        }
    }

    // Manejar expiración de sesión
    function handleSessionExpired() {
        // Limpiar token
        saveAuthToken(null);

        // Almacenar URL actual para redirigir después del login
        try {
            sessionStorage.setItem('RedirectAfterLogin', window.location.href);
        } catch (e) {
            console.warn('No se pudo guardar URL de redirección:', e);
        }

        // Mostrar mensaje al usuario
        if (window.Facturacion && window.Facturacion.Common && window.Facturacion.Common.showMessage) {
            window.Facturacion.Common.showMessage('Sesión expirada', 'Su sesión ha expirado. Será redirigido a la página de login.', 'warning');
        } else {
            alert('Su sesión ha expirado. Será redirigido a la página de login.');
        }

        // Redirigir a página de login después de un breve retraso
        setTimeout(function () {
            window.location.href = '/Pages/Auth/Login.aspx';
        }, 1500);
    }

    // Métodos HTTP
    function get(endpoint) {
        return ajax('GET', endpoint);
    }

    function post(endpoint, data) {
        return ajax('POST', endpoint, data);
    }

    function put(endpoint, data) {
        return ajax('PUT', endpoint, data);
    }

    function del(endpoint) {
        return ajax('DELETE', endpoint);
    }

    function patch(endpoint, data) {
        return ajax('PATCH', endpoint, data);
    }

    // API pública
    return {
        initialize: initialize,
        get: get,
        post: post,
        put: put,
        delete: del,
        patch: patch,
        setAuthToken: saveAuthToken,
        clearAuthToken: function () { saveAuthToken(null); }
    };
})();

// Inicialización cuando el documento está listo
$(document).ready(function () {
    // Inicializar con valores por defecto
    // Estos valores deberían ser proporcionados por el servidor
    var apiSettings = {
        baseUrl: window.apiBaseUrl || '/api/',
        timeoutMs: window.apiTimeoutMs || 30000,
        sitio: window.apiSitio || 'FacturacionWeb',
        clave: window.apiClave || ''
    };

    Facturacion.ApiClient.initialize(apiSettings);

    console.log('API Client inicializado');
});