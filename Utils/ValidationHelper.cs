using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace Facturacion.Web.Utils
{
    /// <summary>
    /// Clase utilitaria para validaciones comunes en la aplicación
    /// Proporciona métodos para validar diferentes tipos de datos y generar mensajes de error
    /// </summary>
    public static class ValidationHelper
    {
        #region Validaciones Básicas

        /// <summary>
        /// Verifica si un valor es nulo o está vacío
        /// </summary>
        /// <param name="value">Valor a verificar</param>
        /// <returns>True si es nulo o vacío</returns>
        public static bool IsNullOrEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Verifica si un control tiene valor
        /// </summary>
        /// <param name="control">Control a verificar</param>
        /// <returns>True si tiene valor</returns>
        public static bool HasValue(TextBox control)
        {
            return control != null && !string.IsNullOrWhiteSpace(control.Text);
        }

        /// <summary>
        /// Verifica si un dropdown tiene selección válida
        /// </summary>
        /// <param name="control">Dropdown a verificar</param>
        /// <returns>True si tiene selección válida</returns>
        public static bool HasSelection(DropDownList control)
        {
            return control != null && control.SelectedIndex > 0;
        }

        #endregion

        #region Validaciones Numéricas

        /// <summary>
        /// Verifica si un valor es un entero válido
        /// </summary>
        /// <param name="value">Valor a verificar</param>
        /// <returns>True si es un entero válido</returns>
        public static bool IsValidInteger(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && int.TryParse(value, out _);
        }

        /// <summary>
        /// Verifica si un valor es un entero positivo
        /// </summary>
        /// <param name="value">Valor a verificar</param>
        /// <returns>True si es un entero positivo</returns>
        public static bool IsPositiveInteger(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// Verifica si un valor es un decimal válido
        /// </summary>
        /// <param name="value">Valor a verificar</param>
        /// <returns>True si es un decimal válido</returns>
        public static bool IsValidDecimal(string value)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                decimal.TryParse(value, NumberStyles.Currency | NumberStyles.AllowDecimalPoint,
                    CultureInfo.CurrentCulture, out _);
        }

        /// <summary>
        /// Verifica si un valor es un decimal positivo
        /// </summary>
        /// <param name="value">Valor a verificar</param>
        /// <returns>True si es un decimal positivo</returns>
        public static bool IsPositiveDecimal(string value)
        {
            if (decimal.TryParse(value, NumberStyles.Currency | NumberStyles.AllowDecimalPoint,
                CultureInfo.CurrentCulture, out decimal result))
            {
                return result > 0;
            }
            return false;
        }

        /// <summary>
        /// Verifica si un valor es una moneda válida
        /// </summary>
        /// <param name="value">Valor a verificar</param>
        /// <returns>True si es una moneda válida</returns>
        public static bool IsValidMoney(string value)
        {
            // Quitar símbolos de moneda para verificación
            string cleanValue = value;
            if (!string.IsNullOrEmpty(cleanValue))
            {
                cleanValue = cleanValue.Replace("$", "").Replace(".", "").Trim();
                // Reemplazar coma por punto para decimales
                cleanValue = cleanValue.Replace(",", ".");
            }

            return !string.IsNullOrWhiteSpace(cleanValue) &&
                decimal.TryParse(cleanValue, NumberStyles.Number | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out decimal result) && result >= 0;
        }

        #endregion

        #region Validaciones Específicas

        /// <summary>
        /// Verifica si un valor es un correo electrónico válido
        /// </summary>
        /// <param name="email">Correo a verificar</param>
        /// <returns>True si es un correo válido</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Patrón de validación de email más completo
                string pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si un valor es un número de teléfono válido
        /// </summary>
        /// <param name="phone">Teléfono a verificar</param>
        /// <returns>True si es un teléfono válido</returns>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Extraer solo números
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            // Un teléfono válido debe tener entre 7 y 15 dígitos
            return digitsOnly.Length >= 7 && digitsOnly.Length <= 15;
        }

        /// <summary>
        /// Verifica si un valor es un documento de identidad válido
        /// </summary>
        /// <param name="document">Documento a verificar</param>
        /// <returns>True si es un documento válido</returns>
        public static bool IsValidDocument(string document)
        {
            if (string.IsNullOrWhiteSpace(document))
                return false;

            // Extraer solo números y letras (para documentos alfanuméricos)
            string cleanDoc = new string(document.Where(c => char.IsLetterOrDigit(c)).ToArray());

            // Un documento válido debe tener entre 5 y 15 caracteres
            return cleanDoc.Length >= 5 && cleanDoc.Length <= 15;
        }

        /// <summary>
        /// Verifica si un valor es una fecha válida
        /// </summary>
        /// <param name="date">Fecha a verificar</param>
        /// <returns>True si es una fecha válida</returns>
        public static bool IsValidDate(string date)
        {
            return !string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out _);
        }

        #endregion

        #region Obtención de Mensajes de Error

        /// <summary>
        /// Obtiene mensaje de error para campo requerido
        /// </summary>
        /// <param name="fieldName">Nombre del campo</param>
        /// <returns>Mensaje de error</returns>
        public static string GetRequiredFieldMessage(string fieldName)
        {
            return $"El campo {fieldName} es obligatorio.";
        }

        /// <summary>
        /// Obtiene mensaje de error para valor numérico inválido
        /// </summary>
        /// <param name="fieldName">Nombre del campo</param>
        /// <returns>Mensaje de error</returns>
        public static string GetInvalidNumberMessage(string fieldName)
        {
            return $"El campo {fieldName} debe ser un número válido.";
        }

        /// <summary>
        /// Obtiene mensaje de error para valor de moneda inválido
        /// </summary>
        /// <param name="fieldName">Nombre del campo</param>
        /// <returns>Mensaje de error</returns>
        public static string GetInvalidMoneyMessage(string fieldName)
        {
            return $"El campo {fieldName} debe ser un valor monetario válido.";
        }

        /// <summary>
        /// Obtiene mensaje de error para valor que debe ser positivo
        /// </summary>
        /// <param name="fieldName">Nombre del campo</param>
        /// <returns>Mensaje de error</returns>
        public static string GetMustBePositiveMessage(string fieldName)
        {
            return $"El campo {fieldName} debe ser mayor a cero.";
        }

        /// <summary>
        /// Obtiene mensaje de error para email inválido
        /// </summary>
        /// <returns>Mensaje de error</returns>
        public static string GetInvalidEmailMessage()
        {
            return "El correo electrónico ingresado no es válido.";
        }

        /// <summary>
        /// Obtiene mensaje de error para teléfono inválido
        /// </summary>
        /// <returns>Mensaje de error</returns>
        public static string GetInvalidPhoneMessage()
        {
            return "El número de teléfono ingresado no es válido.";
        }

        /// <summary>
        /// Obtiene mensaje de error para documento inválido
        /// </summary>
        /// <returns>Mensaje de error</returns>
        public static string GetInvalidDocumentMessage()
        {
            return "El número de documento ingresado no es válido.";
        }

        #endregion

        #region Validaciones para JavaScript

        /// <summary>
        /// Genera código JavaScript para validar campos requeridos
        /// </summary>
        /// <param name="controlId">ID del control</param>
        /// <param name="fieldName">Nombre del campo para el mensaje</param>
        /// <returns>Código JavaScript</returns>
        public static string GenerateRequiredFieldScript(string controlId, string fieldName)
        {
            return $@"
                if (!document.getElementById('{controlId}').value.trim()) {{
                    alert('El campo {fieldName} es obligatorio.');
                    document.getElementById('{controlId}').focus();
                    return false;
                }}";
        }

        /// <summary>
        /// Genera código JavaScript para validar enteros
        /// </summary>
        /// <param name="controlId">ID del control</param>
        /// <param name="fieldName">Nombre del campo para el mensaje</param>
        /// <returns>Código JavaScript</returns>
        public static string GenerateIntegerValidationScript(string controlId, string fieldName)
        {
            return $@"
                var value = document.getElementById('{controlId}').value.trim();
                if (value && !/^[0-9]+$/.test(value)) {{
                    alert('El campo {fieldName} debe ser un número entero válido.');
                    document.getElementById('{controlId}').focus();
                    return false;
                }}";
        }

        /// <summary>
        /// Genera código JavaScript para validar enteros positivos
        /// </summary>
        /// <param name="controlId">ID del control</param>
        /// <param name="fieldName">Nombre del campo para el mensaje</param>
        /// <returns>Código JavaScript</returns>
        public static string GeneratePositiveIntegerValidationScript(string controlId, string fieldName)
        {
            return $@"
                var value = document.getElementById('{controlId}').value.trim();
                if (value) {{
                    var numValue = parseInt(value);
                    if (isNaN(numValue) || numValue <= 0) {{
                        alert('El campo {fieldName} debe ser un número entero mayor a cero.');
                        document.getElementById('{controlId}').focus();
                        return false;
                    }}
                }}";
        }

        /// <summary>
        /// Genera código JavaScript para validar moneda
        /// </summary>
        /// <param name="controlId">ID del control</param>
        /// <param name="fieldName">Nombre del campo para el mensaje</param>
        /// <returns>Código JavaScript</returns>
        public static string GenerateMoneyValidationScript(string controlId, string fieldName)
        {
            return $@"
                var value = document.getElementById('{controlId}').value.trim();
                if (value) {{
                    // Remover símbolos de moneda y separadores de miles
                    var cleanValue = value.replace('$', '').replace(/\./g, '').replace(',', '.');
                    var numValue = parseFloat(cleanValue);
                    if (isNaN(numValue) || numValue <= 0) {{
                        alert('El campo {fieldName} debe ser un valor monetario válido mayor a cero.');
                        document.getElementById('{controlId}').focus();
                        return false;
                    }}
                }}";
        }

        /// <summary>
        /// Genera código JavaScript para validar email
        /// </summary>
        /// <param name="controlId">ID del control</param>
        /// <returns>Código JavaScript</returns>
        public static string GenerateEmailValidationScript(string controlId)
        {
            return $@"
                var value = document.getElementById('{controlId}').value.trim();
                if (value && !/^[a-zA-Z0-9.!#$%&'*+/=?^_`{{|}}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{{0,61}}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{{0,61}}[a-zA-Z0-9])?)*$/.test(value)) {{
                    alert('El correo electrónico ingresado no es válido.');
                    document.getElementById('{controlId}').focus();
                    return false;
                }}";
        }

        /// <summary>
        /// Genera código JavaScript para validar que haya al menos un artículo en la factura
        /// </summary>
        /// <param name="gridId">ID de la grilla de artículos</param>
        /// <returns>Código JavaScript</returns>
        public static string GenerateAtLeastOneItemValidationScript(string gridId)
        {
            return $@"
                var grid = document.getElementById('{gridId}');
                if (grid && grid.rows.length <= 1) {{ // Solo la fila de encabezado
                    alert('Debe agregar al menos un artículo a la factura.');
                    return false;
                }}";
        }

        #endregion

        #region Utilidades para Formularios

        /// <summary>
        /// Limpia un control de texto
        /// </summary>
        /// <param name="control">Control a limpiar</param>
        public static void ClearTextBox(TextBox control)
        {
            if (control != null)
            {
                control.Text = string.Empty;
            }
        }

        /// <summary>
        /// Establece foco en un control
        /// </summary>
        /// <param name="control">Control a enfocar</param>
        public static void SetFocus(TextBox control)
        {
            if (control != null)
            {
                control.Focus();
            }
        }

        /// <summary>
        /// Registra script para establecer foco en un control
        /// </summary>
        /// <param name="page">Página actual</param>
        /// <param name="controlId">ID del control</param>
        public static void RegisterFocusScript(System.Web.UI.Page page, string controlId)
        {
            if (page != null && !string.IsNullOrEmpty(controlId))
            {
                string script = $"document.getElementById('{controlId}').focus();";
                page.ClientScript.RegisterStartupScript(page.GetType(), $"Focus_{controlId}", script, true);
            }
        }

        #endregion
    }
}