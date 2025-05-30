using Facturacion.Web.Core;
using Facturacion.Web.Models.DTOs.Clientes;
using Facturacion.Web.Models.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Facturacion.Web.Services
{
    /// <summary>
    /// Servicio para manejo completo de clientes
    /// Consume las APIs de cliente del backend para CRUD y consultas específicas
    /// ⭐ CLAVE PARA EL FORMULARIO DE FACTURAS - SELECCIÓN DE CLIENTES
    /// </summary>
    public class ClienteService : IDisposable
    {
        private readonly ApiClient _apiClient;
        private readonly SessionManager _sessionManager;
        private readonly ApiInterceptor _interceptor;
        private bool _disposed = false;

        public ClienteService()
        {
            _apiClient = new ApiClient();
            _sessionManager = new SessionManager();
            _interceptor = ApiInterceptor.CrearInstancia();
        }

        #region Métodos CRUD Básicos

        /// <summary>
        /// Obtiene todos los clientes activos del sistema
        /// </summary>
        /// <returns>Lista completa de clientes</returns>
        public async Task<RespuestaDto<List<ClienteDto>>> ObtenerTodosAsync()
        {
            try
            {
                VerificarAutenticacion();

                var respuesta = await _apiClient.GetAsync<List<ClienteDto>>("cliente");
                return _interceptor.InterceptarRespuesta(respuesta, "cliente", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ClienteDto>>(ex, "cliente", "GET");
            }
        }

        /// <summary>
        /// Obtiene un cliente específico por su ID
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <returns>Cliente encontrado</returns>
        public async Task<RespuestaDto<ClienteDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<ClienteDto>.CrearError("ID inválido",
                        "El ID del cliente debe ser mayor a 0");
                }

                var respuesta = await _apiClient.GetAsync<ClienteDto>($"cliente/{id}");
                return _interceptor.InterceptarRespuesta(respuesta, $"cliente/{id}", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<ClienteDto>(ex, $"cliente/{id}", "GET");
            }
        }

        /// <summary>
        /// Busca un cliente por su número de documento
        /// ⭐ MÉTODO CLAVE PARA EL FORMULARIO DE FACTURAS
        /// </summary>
        /// <param name="numeroDocumento">Número de documento del cliente</param>
        /// <returns>Cliente encontrado</returns>
        public async Task<RespuestaDto<ClienteDto>> ObtenerPorDocumentoAsync(string numeroDocumento)
        {
            try
            {
                VerificarAutenticacion();

                if (string.IsNullOrWhiteSpace(numeroDocumento))
                {
                    return RespuestaDto<ClienteDto>.CrearError("Documento requerido",
                        "El número de documento es requerido");
                }

                // Limpiar el número de documento (remover espacios, caracteres especiales)
                numeroDocumento = LimpiarNumeroDocumento(numeroDocumento);

                var respuesta = await _apiClient.GetAsync<ClienteDto>($"cliente/documento/{numeroDocumento}");
                return _interceptor.InterceptarRespuesta(respuesta, $"cliente/documento/{numeroDocumento}", "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<ClienteDto>(ex, $"cliente/documento/{numeroDocumento}", "GET");
            }
        }

        /// <summary>
        /// Crea un nuevo cliente en el sistema
        /// </summary>
        /// <param name="clienteDto">Datos del cliente a crear</param>
        /// <returns>Cliente creado</returns>
        public async Task<RespuestaDto<ClienteDto>> CrearAsync(ClienteDto clienteDto)
        {
            try
            {
                VerificarAutenticacion();

                // Validaciones previas
                var validacionPrevia = ValidarClienteAntesDeSend(clienteDto);
                if (!validacionPrevia.EsValido)
                {
                    return RespuestaDto<ClienteDto>.CrearError("Validación fallida", validacionPrevia.Detalle);
                }

                // Limpiar y normalizar datos
                clienteDto = NormalizarDatosCliente(clienteDto);

                var respuesta = await _apiClient.PostAsync<ClienteDto>("cliente", clienteDto);
                respuesta = _interceptor.InterceptarRespuesta(respuesta, "cliente", "POST");

                if (respuesta.Exito)
                {
                    System.Diagnostics.Debug.WriteLine($"Cliente creado exitosamente: {clienteDto.NombreCompleto}");
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando cliente: {ex.Message}");
                return _interceptor.InterceptarError<ClienteDto>(ex, "cliente", "POST");
            }
        }

        /// <summary>
        /// Actualiza un cliente existente
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <param name="clienteDto">Datos actualizados del cliente</param>
        /// <returns>Cliente actualizado</returns>
        public async Task<RespuestaDto<ClienteDto>> ActualizarAsync(int id, ClienteDto clienteDto)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<ClienteDto>.CrearError("ID inválido",
                        "El ID del cliente debe ser mayor a 0");
                }

                // Validaciones previas
                var validacionPrevia = ValidarClienteAntesDeSend(clienteDto);
                if (!validacionPrevia.EsValido)
                {
                    return RespuestaDto<ClienteDto>.CrearError("Validación fallida", validacionPrevia.Detalle);
                }

                // Asegurar que el ID coincida
                clienteDto.Id = id;

                // Limpiar y normalizar datos
                clienteDto = NormalizarDatosCliente(clienteDto);

                var respuesta = await _apiClient.PutAsync<ClienteDto>($"cliente/{id}", clienteDto);
                return _interceptor.InterceptarRespuesta(respuesta, $"cliente/{id}", "PUT");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<ClienteDto>(ex, $"cliente/{id}", "PUT");
            }
        }

        /// <summary>
        /// Elimina (inactiva) un cliente del sistema
        /// </summary>
        /// <param name="id">ID del cliente</param>
        /// <returns>Resultado de la operación</returns>
        public async Task<RespuestaDto<object>> EliminarAsync(int id)
        {
            try
            {
                VerificarAutenticacion();

                if (id <= 0)
                {
                    return RespuestaDto<object>.CrearError("ID inválido",
                        "El ID del cliente debe ser mayor a 0");
                }

                var respuesta = await _apiClient.DeleteAsync<object>($"cliente/{id}");
                return _interceptor.InterceptarRespuesta(respuesta, $"cliente/{id}", "DELETE");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<object>(ex, $"cliente/{id}", "DELETE");
            }
        }

        #endregion

        #region Métodos de Consulta y Búsqueda

        /// <summary>
        /// Obtiene clientes de forma paginada con filtros
        /// ⭐ PARA GRILLAS Y LISTADOS DE CLIENTES
        /// </summary>
        /// <param name="pagina">Número de página</param>
        /// <param name="elementosPorPagina">Elementos por página</param>
        /// <param name="busqueda">Texto de búsqueda</param>
        /// <returns>Lista paginada de clientes</returns>
        public async Task<RespuestaDto<PaginacionDto<ClienteDto>>> ObtenerPaginadoAsync(
            int pagina = 1,
            int elementosPorPagina = 10,
            string busqueda = null)
        {
            try
            {
                VerificarAutenticacion();

                if (pagina <= 0) pagina = 1;
                if (elementosPorPagina <= 0) elementosPorPagina = 10;

                // Construir query string
                var queryParams = new List<string>
                {
                    $"pagina={pagina}",
                    $"elementosPorPagina={elementosPorPagina}"
                };

                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    queryParams.Add($"busqueda={Uri.EscapeDataString(busqueda.Trim())}");
                }

                var queryString = string.Join("&", queryParams);
                var endpoint = $"cliente/paginado?{queryString}";

                var respuesta = await _apiClient.GetAsync<PaginacionDto<ClienteDto>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<PaginacionDto<ClienteDto>>(ex, "cliente/paginado", "GET");
            }
        }

        /// <summary>
        /// Busca clientes por nombre o documento para autocompletar
        /// ⭐ MÉTODO PARA AUTOCOMPLETAR EN EL FORMULARIO DE FACTURAS
        /// </summary>
        /// <param name="termino">Término de búsqueda</param>
        /// <param name="limite">Número máximo de resultados</param>
        /// <returns>Lista de clientes que coinciden</returns>
        public async Task<RespuestaDto<List<ClienteDto>>> BuscarClientesAsync(string termino, int limite = 10)
        {
            try
            {
                VerificarAutenticacion();

                if (string.IsNullOrWhiteSpace(termino))
                {
                    return RespuestaDto<List<ClienteDto>>.CrearExitoso("Búsqueda vacía", new List<ClienteDto>());
                }

                if (termino.Trim().Length < 2)
                {
                    return RespuestaDto<List<ClienteDto>>.CrearError("Término muy corto",
                        "Debe ingresar al menos 2 caracteres para buscar");
                }

                // Usar el endpoint paginado con búsqueda
                var respuestaPaginada = await ObtenerPaginadoAsync(1, limite, termino.Trim());

                if (respuestaPaginada.Exito && respuestaPaginada.Resultado != null)
                {
                    return RespuestaDto<List<ClienteDto>>.CrearExitoso(
                        "Clientes encontrados",
                        respuestaPaginada.Resultado.Lista ?? new List<ClienteDto>(),
                        $"Se encontraron {respuestaPaginada.Resultado.TotalRegistros} clientes");
                }

                return RespuestaDto<List<ClienteDto>>.CrearError(respuestaPaginada.Mensaje, respuestaPaginada.Detalle);
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ClienteDto>>(ex, "cliente/buscar", "GET");
            }
        }

        /// <summary>
        /// Obtiene los clientes más frecuentes (que más compran)
        /// </summary>
        /// <param name="top">Número de clientes a obtener</param>
        /// <returns>Lista de clientes frecuentes</returns>
        public async Task<RespuestaDto<List<ClienteFrecuenteDto>>> ObtenerClientesFrecuentesAsync(int top = 10)
        {
            try
            {
                VerificarAutenticacion();

                if (top <= 0) top = 10;

                var endpoint = $"cliente/frecuentes?top={top}";
                var respuesta = await _apiClient.GetAsync<List<ClienteFrecuenteDto>>(endpoint);
                return _interceptor.InterceptarRespuesta(respuesta, endpoint, "GET");
            }
            catch (Exception ex)
            {
                return _interceptor.InterceptarError<List<ClienteFrecuenteDto>>(ex, "cliente/frecuentes", "GET");
            }
        }

        #endregion

        #region Métodos de Validación y Utilidades

        /// <summary>
        /// Valida los datos de un cliente antes de enviarlos al servidor
        /// </summary>
        /// <param name="clienteDto">Datos del cliente</param>
        /// <returns>Resultado de validación</returns>
        private ValidoDto ValidarClienteAntesDeSend(ClienteDto clienteDto)
        {
            try
            {
                if (clienteDto == null)
                {
                    return ValidoDto.Invalido("Los datos del cliente son requeridos");
                }

                if (string.IsNullOrWhiteSpace(clienteDto.NumeroDocumento))
                {
                    return ValidoDto.Invalido("El número de documento es requerido");
                }

                if (string.IsNullOrWhiteSpace(clienteDto.Nombres))
                {
                    return ValidoDto.Invalido("Los nombres son requeridos");
                }

                if (string.IsNullOrWhiteSpace(clienteDto.Apellidos))
                {
                    return ValidoDto.Invalido("Los apellidos son requeridos");
                }

                if (string.IsNullOrWhiteSpace(clienteDto.Direccion))
                {
                    return ValidoDto.Invalido("La dirección es requerida");
                }

                if (string.IsNullOrWhiteSpace(clienteDto.Telefono))
                {
                    return ValidoDto.Invalido("El teléfono es requerido");
                }

                // Validaciones de formato
                if (clienteDto.NumeroDocumento.Trim().Length < 6)
                {
                    return ValidoDto.Invalido("El número de documento debe tener al menos 6 caracteres");
                }

                if (clienteDto.Nombres.Trim().Length < 2)
                {
                    return ValidoDto.Invalido("Los nombres deben tener al menos 2 caracteres");
                }

                if (clienteDto.Apellidos.Trim().Length < 2)
                {
                    return ValidoDto.Invalido("Los apellidos deben tener al menos 2 caracteres");
                }

                // Validar email si se proporciona
                if (!string.IsNullOrWhiteSpace(clienteDto.Email) && !EsEmailValido(clienteDto.Email))
                {
                    return ValidoDto.Invalido("El formato del email no es válido");
                }

                // Validar teléfono
                if (!EsTelefonoValido(clienteDto.Telefono))
                {
                    return ValidoDto.Invalido("El formato del teléfono no es válido");
                }

                return ValidoDto.Valido("Cliente válido para procesar");
            }
            catch (Exception ex)
            {
                return ValidoDto.Invalido($"Error en validación: {ex.Message}");
            }
        }

        /// <summary>
        /// Normaliza y limpia los datos del cliente
        /// </summary>
        /// <param name="clienteDto">Cliente original</param>
        /// <returns>Cliente con datos normalizados</returns>
        private ClienteDto NormalizarDatosCliente(ClienteDto clienteDto)
        {
            if (clienteDto == null) return clienteDto;

            // Limpiar y normalizar campos de texto
            clienteDto.NumeroDocumento = LimpiarNumeroDocumento(clienteDto.NumeroDocumento);
            clienteDto.Nombres = LimpiarTexto(clienteDto.Nombres);
            clienteDto.Apellidos = LimpiarTexto(clienteDto.Apellidos);
            clienteDto.Direccion = LimpiarTexto(clienteDto.Direccion);
            clienteDto.Telefono = LimpiarTelefono(clienteDto.Telefono);

            if (!string.IsNullOrWhiteSpace(clienteDto.Email))
            {
                clienteDto.Email = clienteDto.Email.Trim().ToLowerInvariant();
            }

            // Generar nombre completo
            clienteDto.NombreCompleto = $"{clienteDto.Nombres} {clienteDto.Apellidos}";

            return clienteDto;
        }

        /// <summary>
        /// Limpia un número de documento removiendo caracteres especiales
        /// </summary>
        /// <param name="numeroDocumento">Número original</param>
        /// <returns>Número limpio</returns>
        private string LimpiarNumeroDocumento(string numeroDocumento)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento))
                return string.Empty;

            // Remover espacios, puntos, comas y otros caracteres especiales
            return System.Text.RegularExpressions.Regex.Replace(numeroDocumento.Trim(), @"[^\w]", "");
        }

        /// <summary>
        /// Limpia un campo de texto
        /// </summary>
        /// <param name="texto">Texto original</param>
        /// <returns>Texto limpio</returns>
        private string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return texto.Trim();
        }

        /// <summary>
        /// Limpia un número de teléfono
        /// </summary>
        /// <param name="telefono">Teléfono original</param>
        /// <returns>Teléfono limpio</returns>
        private string LimpiarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return string.Empty;

            // Permitir números, espacios, paréntesis, guiones y el signo +
            return System.Text.RegularExpressions.Regex.Replace(telefono.Trim(), @"[^\d\s\(\)\-\+]", "");
        }

        /// <summary>
        /// Valida si un email tiene formato correcto
        /// </summary>
        /// <param name="email">Email a validar</param>
        /// <returns>True si es válido</returns>
        private bool EsEmailValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida si un teléfono tiene formato básico correcto
        /// </summary>
        /// <param name="telefono">Teléfono a validar</param>
        /// <returns>True si es válido</returns>
        private bool EsTelefonoValido(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return false;

            // Extraer solo números
            var soloNumeros = System.Text.RegularExpressions.Regex.Replace(telefono, @"[^\d]", "");

            // Debe tener entre 7 y 15 dígitos
            return soloNumeros.Length >= 7 && soloNumeros.Length <= 15;
        }

        /// <summary>
        /// Verifica que el usuario esté autenticado
        /// </summary>
        private void VerificarAutenticacion()
        {
            if (!_sessionManager.EstaAutenticado())
            {
                throw new UnauthorizedAccessException("Debe estar autenticado para acceder a este servicio");
            }
        }

        #endregion

        #region Implementación IDisposable

        /// <summary>
        /// Libera los recursos utilizados
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos de manera segura
        /// </summary>
        /// <param name="disposing">True si se está liberando explícitamente</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                try
                {
                    _apiClient?.Dispose();
                    _disposed = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error liberando recursos de ClienteService: {ex.Message}");
                }
            }
        }

        #endregion

        #region Métodos Estáticos de Utilidad

        /// <summary>
        /// Crea una instancia del servicio de clientes
        /// </summary>
        /// <returns>Nueva instancia de ClienteService</returns>
        public static ClienteService CrearInstancia()
        {
            return new ClienteService();
        }

        #endregion
    }
}