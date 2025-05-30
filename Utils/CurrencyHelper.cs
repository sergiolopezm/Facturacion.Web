using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Facturacion.Web.Utils
{
    /// <summary>
    /// Clase utilitaria para manejo de formato de moneda
    /// Proporciona métodos para formatear, parsear y validar valores monetarios
    /// con configuración específica para el formato colombiano
    /// </summary>
    public static class CurrencyHelper
    {
        // Cultura colombiana para formato de moneda
        private static readonly CultureInfo ColombiaCulture = new CultureInfo("es-CO");

        // Símbolo de moneda por defecto
        private static readonly string DefaultCurrencySymbol = "$";

        // Cantidad de decimales por defecto
        private static readonly int DefaultDecimalPlaces = 2;

        /// <summary>
        /// Formatea un valor decimal como moneda con el formato colombiano
        /// </summary>
        /// <param name="value">Valor decimal a formatear</param>
        /// <param name="includeSymbol">Indica si se debe incluir el símbolo de moneda</param>
        /// <returns>Cadena formateada como moneda</returns>
        public static string FormatCurrency(decimal value, bool includeSymbol = true)
        {
            try
            {
                // Obtener configuración desde AppSettings
                string symbol = Core.AppSettings.CurrencySymbol;
                int decimals = Core.AppSettings.CurrencyDecimalPlaces;

                // Si no hay configuración, usar valores por defecto
                if (string.IsNullOrEmpty(symbol))
                    symbol = DefaultCurrencySymbol;

                if (decimals <= 0)
                    decimals = DefaultDecimalPlaces;

                // Configurar formato
                NumberFormatInfo nfi = (NumberFormatInfo)ColombiaCulture.NumberFormat.Clone();
                nfi.CurrencySymbol = symbol;
                nfi.CurrencyDecimalDigits = decimals;

                // Aplicar formato dependiendo si se incluye símbolo o no
                if (includeSymbol)
                    return string.Format(nfi, "{0:C}", value);
                else
                    return string.Format(nfi, "{0:N" + decimals + "}", value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formateando moneda: {ex.Message}");
                return value.ToString("0.00"); // Formato básico en caso de error
            }
        }

        /// <summary>
        /// Formatea un valor decimal como moneda utilizando una configuración personalizada
        /// </summary>
        /// <param name="value">Valor decimal a formatear</param>
        /// <param name="symbol">Símbolo de moneda a utilizar</param>
        /// <param name="decimalPlaces">Cantidad de decimales</param>
        /// <returns>Cadena formateada como moneda</returns>
        public static string FormatCurrencyCustom(decimal value, string symbol, int decimalPlaces)
        {
            try
            {
                NumberFormatInfo nfi = (NumberFormatInfo)ColombiaCulture.NumberFormat.Clone();
                nfi.CurrencySymbol = !string.IsNullOrEmpty(symbol) ? symbol : DefaultCurrencySymbol;
                nfi.CurrencyDecimalDigits = decimalPlaces > 0 ? decimalPlaces : DefaultDecimalPlaces;

                return string.Format(nfi, "{0:C}", value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error formateando moneda personalizada: {ex.Message}");
                return value.ToString("0.00"); // Formato básico en caso de error
            }
        }

        /// <summary>
        /// Convierte una cadena de texto con formato de moneda a un valor decimal
        /// </summary>
        /// <param name="currencyString">Cadena con formato de moneda</param>
        /// <returns>Valor decimal o null si no se puede convertir</returns>
        public static decimal? ParseCurrency(string currencyString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(currencyString))
                    return null;

                // Obtener símbolo de moneda
                string symbol = Core.AppSettings.CurrencySymbol;
                if (string.IsNullOrEmpty(symbol))
                    symbol = DefaultCurrencySymbol;

                // Limpiar la cadena
                string cleanValue = currencyString.Trim();

                // Eliminar el símbolo de moneda
                cleanValue = cleanValue.Replace(symbol, "");

                // Eliminar puntos o espacios de miles
                cleanValue = cleanValue.Replace(".", "").Replace(" ", "");

                // Reemplazar coma decimal por punto (para parsing)
                cleanValue = cleanValue.Replace(",", ".");

                // Convertir a decimal usando formato invariante
                if (decimal.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error parseando moneda: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Verifica si una cadena representa un formato de moneda válido
        /// </summary>
        /// <param name="currencyString">Cadena a validar</param>
        /// <returns>True si la cadena tiene un formato de moneda válido</returns>
        public static bool IsValidCurrencyFormat(string currencyString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(currencyString))
                    return false;

                // Obtener símbolo de moneda
                string symbol = Core.AppSettings.CurrencySymbol;
                if (string.IsNullOrEmpty(symbol))
                    symbol = DefaultCurrencySymbol;

                // Patrón de expresión regular para moneda colombiana
                // Acepta formato con o sin símbolo, con puntos de miles y coma decimal
                string escapedSymbol = Regex.Escape(symbol);
                string pattern = @"^(" + escapedSymbol + @")?[\s]?[\d]{1,3}(\.[\d]{3})*(,[\d]{1,2})?$";

                return Regex.IsMatch(currencyString.Trim(), pattern);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validando formato de moneda: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Convierte un valor numérico a texto en formato de moneda para mostrar en controles
        /// </summary>
        /// <param name="value">Valor a convertir</param>
        /// <returns>Texto con formato de moneda</returns>
        public static string ToMoneyString(object value)
        {
            try
            {
                if (value == null)
                    return FormatCurrency(0);

                if (value is decimal decValue)
                    return FormatCurrency(decValue);

                if (value is double dblValue)
                    return FormatCurrency((decimal)dblValue);

                if (value is int intValue)
                    return FormatCurrency(intValue);

                if (value is string strValue && decimal.TryParse(strValue, out decimal parsedValue))
                    return FormatCurrency(parsedValue);

                return FormatCurrency(0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error convirtiendo a string de moneda: {ex.Message}");
                return "0,00";
            }
        }

        /// <summary>
        /// Calcula el valor del IVA para un monto específico
        /// </summary>
        /// <param name="amount">Monto base</param>
        /// <param name="ivaPercentage">Porcentaje de IVA (por defecto 19%)</param>
        /// <returns>Valor del IVA</returns>
        public static decimal CalculateIVA(decimal amount, decimal ivaPercentage = 19m)
        {
            return Math.Round(amount * (ivaPercentage / 100m), 2);
        }

        /// <summary>
        /// Calcula el descuento para un monto específico
        /// </summary>
        /// <param name="amount">Monto base</param>
        /// <param name="discountPercentage">Porcentaje de descuento</param>
        /// <returns>Valor del descuento</returns>
        public static decimal CalculateDiscount(decimal amount, decimal discountPercentage)
        {
            return Math.Round(amount * (discountPercentage / 100m), 2);
        }

        /// <summary>
        /// Aplica descuento según las reglas de negocio (5% si es >= $500.000, 0% en caso contrario)
        /// </summary>
        /// <param name="amount">Monto base</param>
        /// <returns>Valor del descuento aplicado</returns>
        public static decimal ApplyBusinessDiscount(decimal amount)
        {
            decimal minimumForDiscount = Core.AppSettings.MinAmountForDiscount;
            decimal discountPercentage = Core.AppSettings.DefaultDiscountPercentage;

            if (minimumForDiscount <= 0)
                minimumForDiscount = 500000m; // Valor por defecto

            if (discountPercentage <= 0)
                discountPercentage = 5m; // Valor por defecto

            return amount >= minimumForDiscount ?
                CalculateDiscount(amount, discountPercentage) : 0m;
        }
    }
}