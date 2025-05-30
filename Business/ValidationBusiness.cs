using Facturacion.Web.Models.DTOs.Common;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Facturacion.Web.Business
{
    /// <summary>
    /// Clase para manejo centralizado de validaciones de negocio
    /// Proporciona métodos para validar diferentes tipos de datos y reglas de negocio
    /// ⭐ COMPLEMENTA LAS VALIDACIONES DE CLIENTE Y SERVIDOR
    /// </summary>
    public class ValidationBusiness
    {
        #region Validaciones de Texto

        /// <summary>
        /// Valida si un campo de texto requerido tiene valor
        /// </summary>
        /// <param name="valor">Texto a validar</param>
        /// <param name="nombreCampo">Nombre del campo para el mensaje de error</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarCampoRequerido(string valor, string nombreCampo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return ValidoDto.Invalido($"El campo {nombreCampo} es requerido");
            }

            return ValidoDto.Valido();
        }

        /// <summary>
        /// Valida que un texto cumpla con un rango de longitud especificado
        /// </summary>
        /// <param name="valor">Texto a validar</param>
        /// <param name="nombreCampo">Nombre del campo para el mensaje de error</param>
        /// <param name="longitudMinima">Longitud mínima permitida</param>
        /// <param name="longitudMaxima">Longitud máxima permitida</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarLongitud(string valor, string nombreCampo, int longitudMinima, int longitudMaxima)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return ValidoDto.Valido(); // No validamos longitud si es vacío
            }

            string textoLimpio = valor.Trim();

            if (textoLimpio.Length < longitudMinima)
            {
                return ValidoDto.Invalido($"El campo {nombreCampo} debe tener al menos {longitudMinima} caracteres");
            }

            if (textoLimpio.Length > longitudMaxima)
            {
                return ValidoDto.Invalido($"El campo {nombreCampo} no puede exceder los {longitudMaxima} caracteres");
            }

            return ValidoDto.Valido();
        }

        /// <summary>
        /// Valida que un texto cumpla con un patrón de expresión regular
        /// </summary>
        /// <param name="valor">Texto a validar</param>
        /// <param name="patron">Patrón de expresión regular</param>
        /// <param name="nombreCampo">Nombre del campo para el mensaje de error</param>
        /// <param name="mensajeError">Mensaje de error personalizado (opcional)</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarPatron(string valor, string patron, string nombreCampo, string mensajeError = null)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return ValidoDto.Valido(); // No validamos patrón si es vacío
            }

            if (!Regex.IsMatch(valor, patron))
            {
                string mensaje = mensajeError ?? $"El campo {nombreCampo} no tiene un formato válido";
                return ValidoDto.Invalido(mensaje);
            }

            return ValidoDto.Valido();
        }

        #endregion

        #region Validaciones Numéricas

        /// <summary>
        /// Valida que un valor sea un número entero válido
        /// </summary>
        /// <param name="valor">Valor a validar</param>
        /// <param name="nombreCampo">Nombre del campo para el mensaje de error</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarEntero(string valor, string nombreCampo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return ValidoDto.Valido(); // No validamos si es vacío
            }

            if (!int.TryParse(valor, out _))
            {
                return ValidoDto.Invalido($"El campo {nombreCampo} debe ser un número entero válido");
            }

            return ValidoDto.Valido();
        }

        /// <summary>
        /// Valida que un valor sea un número decimal válido
        /// </summary>
        /// <param name="valor">Valor a validar</param>
        /// <param name="nombreCampo">Nombre del campo para el mensaje de error</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarDecimal(string valor, string nombreCampo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return ValidoDto.Valido(); // No validamos si es vacío
            }

            // Limpiar el valor para manejar formato de moneda
            string valorLimpio = valor.Replace("$", "").Replace(".", "").Replace(",", ".");

            if (!decimal.TryParse(valorLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            {
                return ValidoDto.Invalido($"El campo {nombreCampo} debe ser un número decimal válido");
            }

            return ValidoDto.Valido();
        }

        /// <summary>
        /// Valida que un valor numérico esté dentro de un rango específico
        /// </summary>
        /// <param name="valor">Valor a validar</param>
        /// <param name="nombreCampo">Nombre del campo para el mensaje de error</param>
        /// <param name="minimo">Valor mínimo permitido</param>
        /// <param name="maximo">Valor máximo permitido</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarRango(decimal valor, string nombreCampo, decimal minimo, decimal maximo)
        {
            if (valor < minimo)
            {
                return ValidoDto.Invalido($"El campo {nombreCampo} debe ser mayor o igual a {minimo}");
            }

            if (valor > maximo)
            {
                return ValidoDto.Invalido($"El campo {nombreCampo} debe ser menor o igual a {maximo}");
            }

            return ValidoDto.Valido();
        }

        /// <summary>
        /// Valida que un valor numérico sea mayor que cero
        /// </summary>
        /// <param name="valor">Valor a validar</param>
        /// <param name="nombreCampo">Nombre del campo para el mensaje de error</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarMayorQueCero(decimal valor, string nombreCampo)
        {
            if (valor <= 0)
            {
                return ValidoDto.Invalido($"El campo {nombreCampo} debe ser mayor a cero");
            }

            return ValidoDto.Valido();
        }

        #endregion

        #region Validaciones de Documentos y Contacto

        /// <summary>
        /// Valida un número de documento (cédula, NIT, etc.)
        /// </summary>
        /// <param name="numeroDocumento">Número de documento a validar</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarNumeroDocumento(string numeroDocumento)
        {
            if (string.IsNullOrWhiteSpace(numeroDocumento))
            {
                return ValidoDto.Invalido("El número de documento es requerido");
            }

            // Limpiar el número de documento
            string documentoLimpio = Regex.Replace(numeroDocumento.Trim(), @"[^\w]", "");

            // Validar longitud mínima (6 caracteres para cualquier tipo de documento en Colombia)
            if (documentoLimpio.Length < 6)
            {
                return ValidoDto.Invalido("El número de documento debe tener al menos 6 caracteres");
            }

            // Validar que solo contenga números y posiblemente un guión al final
            if (!Regex.IsMatch(documentoLimpio, @"^\d+(-\d)?$"))
            {
                return ValidoDto.Invalido("El número de documento debe contener solo números (y posiblemente un dígito de verificación)");
            }

            return ValidoDto.Valido();
        }

        /// <summary>
        /// Valida un número de teléfono
        /// </summary>
        /// <param name="telefono">Teléfono a validar</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
            {
                return ValidoDto.Invalido("El teléfono es requerido");
            }

            // Limpiar el teléfono (solo permitir números, espacios, paréntesis, guiones y el signo +)
            string telefonoLimpio = Regex.Replace(telefono.Trim(), @"[^\d\s\(\)\-\+]", "");

            // Extraer solo números para verificar longitud
            string soloNumeros = Regex.Replace(telefonoLimpio, @"[^\d]", "");

            // Validar longitud (entre 7 y 15 dígitos)
            if (soloNumeros.Length < 7 || soloNumeros.Length > 15)
            {
                return ValidoDto.Invalido("El teléfono debe tener entre 7 y 15 dígitos");
            }

            return ValidoDto.Valido();
        }

        /// <summary>
        /// Valida una dirección de correo electrónico
        /// </summary>
        /// <param name="email">Email a validar</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ValidoDto.Valido(); // No validamos si es vacío
            }

            try
            {
                // Usar la clase System.Net.Mail.MailAddress para validar el formato
                var addr = new System.Net.Mail.MailAddress(email);

                // Verificar que el dominio tenga al menos un punto
                if (!addr.Host.Contains("."))
                {
                    return ValidoDto.Invalido("El email debe tener un dominio válido");
                }

                return ValidoDto.Valido();
            }
            catch
            {
                return ValidoDto.Invalido("El formato del email no es válido");
            }
        }

        #endregion

        #region Validaciones de Facturas

        /// <summary>
        /// Valida los campos del encabezado de una factura
        /// </summary>
        /// <param name="clienteId">ID del cliente</param>
        /// <param name="numeroDocumento">Número de documento del cliente</param>
        /// <param name="nombres">Nombres del cliente</param>
        /// <param name="apellidos">Apellidos del cliente</param>
        /// <param name="direccion">Dirección del cliente</param>
        /// <param name="telefono">Teléfono del cliente</param>
        /// <returns>Lista de errores encontrados</returns>
        public List<string> ValidarEncabezadoFactura(int clienteId, string numeroDocumento, string nombres,
                                                  string apellidos, string direccion, string telefono)
        {
            var errores = new List<string>();

            // Validar cliente seleccionado
            if (clienteId <= 0)
            {
                errores.Add("Debe seleccionar un cliente válido");
            }

            // Validar campos requeridos
            if (string.IsNullOrWhiteSpace(numeroDocumento))
            {
                errores.Add("El número de documento del cliente es requerido");
            }

            if (string.IsNullOrWhiteSpace(nombres))
            {
                errores.Add("Los nombres del cliente son requeridos");
            }

            if (string.IsNullOrWhiteSpace(apellidos))
            {
                errores.Add("Los apellidos del cliente son requeridos");
            }

            if (string.IsNullOrWhiteSpace(direccion))
            {
                errores.Add("La dirección del cliente es requerida");
            }

            if (string.IsNullOrWhiteSpace(telefono))
            {
                errores.Add("El teléfono del cliente es requerido");
            }

            // Validar formato de documento si se proporcionó
            if (!string.IsNullOrWhiteSpace(numeroDocumento))
            {
                var validacionDocumento = ValidarNumeroDocumento(numeroDocumento);
                if (!validacionDocumento.EsValido)
                {
                    errores.Add(validacionDocumento.Detalle);
                }
            }

            // Validar formato de teléfono si se proporcionó
            if (!string.IsNullOrWhiteSpace(telefono))
            {
                var validacionTelefono = ValidarTelefono(telefono);
                if (!validacionTelefono.EsValido)
                {
                    errores.Add(validacionTelefono.Detalle);
                }
            }

            return errores;
        }

        /// <summary>
        /// Valida los campos de un detalle de factura
        /// </summary>
        /// <param name="articuloId">ID del artículo</param>
        /// <param name="cantidad">Cantidad</param>
        /// <param name="precioUnitario">Precio unitario</param>
        /// <param name="indice">Índice del detalle para mensajes</param>
        /// <returns>Lista de errores encontrados</returns>
        public List<string> ValidarDetalleFactura(int articuloId, int cantidad, decimal precioUnitario, int indice)
        {
            var errores = new List<string>();

            // Validar artículo
            if (articuloId <= 0)
            {
                errores.Add($"Debe seleccionar un artículo válido en la línea {indice}");
            }

            // Validar cantidad
            if (cantidad <= 0)
            {
                errores.Add($"La cantidad en la línea {indice} debe ser mayor a cero");
            }

            // Validar precio unitario
            if (precioUnitario <= 0)
            {
                errores.Add($"El precio unitario en la línea {indice} debe ser mayor a cero");
            }

            return errores;
        }

        /// <summary>
        /// Verifica si un conjunto de detalles de factura contiene artículos duplicados
        /// </summary>
        /// <param name="articulosIds">Lista de IDs de artículos</param>
        /// <returns>True si hay duplicados</returns>
        public bool TieneArticulosDuplicados(List<int> articulosIds)
        {
            if (articulosIds == null || !articulosIds.Any())
                return false;

            return articulosIds.Count != articulosIds.Distinct().Count();
        }

        #endregion

        #region Validaciones de UI

        /// <summary>
        /// Valida que un control TextBox contenga un número entero válido
        /// </summary>
        /// <param name="valor">Valor del control</param>
        /// <param name="nombreCampo">Nombre del campo para el mensaje de error</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarTextBoxEntero(string valor, string nombreCampo)
        {
            // Primero validar si es requerido
            var validacionRequerido = ValidarCampoRequerido(valor, nombreCampo);
            if (!validacionRequerido.EsValido)
            {
                return validacionRequerido;
            }

            // Luego validar si es un entero válido
            return ValidarEntero(valor, nombreCampo);
        }

        /// <summary>
        /// Valida que un control TextBox contenga un número decimal válido
        /// </summary>
        /// <param name="valor">Valor del control</param>
        /// <param name="nombreCampo">Nombre del campo para el mensaje de error</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarTextBoxDecimal(string valor, string nombreCampo)
        {
            // Primero validar si es requerido
            var validacionRequerido = ValidarCampoRequerido(valor, nombreCampo);
            if (!validacionRequerido.EsValido)
            {
                return validacionRequerido;
            }

            // Luego validar si es un decimal válido
            return ValidarDecimal(valor, nombreCampo);
        }

        /// <summary>
        /// Valida un control DropDownList para asegurar que se haya seleccionado un valor
        /// </summary>
        /// <param name="valorSeleccionado">Valor seleccionado en el control</param>
        /// <param name="nombreCampo">Nombre del campo para el mensaje de error</param>
        /// <returns>Resultado de la validación</returns>
        public ValidoDto ValidarDropDownList(string valorSeleccionado, string nombreCampo)
        {
            if (string.IsNullOrWhiteSpace(valorSeleccionado) || valorSeleccionado == "0" || valorSeleccionado == "-1")
            {
                return ValidoDto.Invalido($"Debe seleccionar un {nombreCampo}");
            }

            return ValidoDto.Valido();
        }

        #endregion

        #region Utilidades de Validación

        /// <summary>
        /// Convierte una lista de errores en un mensaje único separado por saltos de línea
        /// </summary>
        /// <param name="errores">Lista de errores</param>
        /// <returns>Mensaje combinado</returns>
        public string CombinarErrores(List<string> errores)
        {
            if (errores == null || !errores.Any())
                return string.Empty;

            return string.Join("\n", errores);
        }

        /// <summary>
        /// Combina múltiples resultados de validación en uno solo
        /// </summary>
        /// <param name="resultados">Lista de resultados de validación</param>
        /// <returns>Resultado combinado</returns>
        public ValidoDto CombinarResultados(params ValidoDto[] resultados)
        {
            if (resultados == null || !resultados.Any())
                return ValidoDto.Valido();

            var invalidos = resultados.Where(r => !r.EsValido).ToList();
            if (!invalidos.Any())
                return ValidoDto.Valido();

            var mensajes = invalidos.Select(r => r.Detalle).Where(d => !string.IsNullOrEmpty(d));
            return ValidoDto.Invalido(string.Join("\n", mensajes));
        }

        /// <summary>
        /// Limpia un valor de entrada para validaciones
        /// </summary>
        /// <param name="valor">Valor a limpiar</param>
        /// <returns>Valor limpio</returns>
        public string LimpiarValor(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;

            return valor.Trim();
        }

        #endregion
    }
}