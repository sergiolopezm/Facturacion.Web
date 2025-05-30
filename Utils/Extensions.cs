using Facturacion.Web.Models.DTOs.Articulos;
using Facturacion.Web.Models.DTOs.Clientes;
using Facturacion.Web.Models.DTOs.Facturas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Facturacion.Web.Utils
{
    /// <summary>
    /// Métodos de extensión para tipos comunes en la aplicación
    /// Proporciona funcionalidad extendida a strings, decimales, listas, etc.
    /// </summary>
    public static class Extensions
    {
        #region String Extensions

        /// <summary>
        /// Verifica si un string está vacío o es nulo
        /// </summary>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Verifica si un string está vacío, es nulo o solo contiene espacios
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Limpia un string eliminando espacios y caracteres especiales
        /// </summary>
        public static string CleanString(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Remover espacios y caracteres especiales
            return Regex.Replace(value.Trim(), @"[^\w\s]", "");
        }

        /// <summary>
        /// Limpia un número de documento eliminando espacios y caracteres no alfanuméricos
        /// </summary>
        public static string CleanDocumentNumber(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Remover espacios y caracteres especiales excepto números y letras
            return Regex.Replace(value.Trim(), @"[^\w]", "");
        }

        /// <summary>
        /// Limpia un número de teléfono manteniendo solo números, +, -, ( y )
        /// </summary>
        public static string CleanPhoneNumber(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Remover todo excepto números, +, -, (, ) y espacios
            return Regex.Replace(value.Trim(), @"[^\d\+\-\(\)\s]", "");
        }

        /// <summary>
        /// Trunca un string a una longitud máxima y agrega elipsis si es necesario
        /// </summary>
        public static string Truncate(this string value, int maxLength, bool addEllipsis = true)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;

            return addEllipsis
                ? value.Substring(0, maxLength - 3) + "..."
                : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Convierte un string a título (primera letra de cada palabra en mayúscula)
        /// </summary>
        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        /// <summary>
        /// Verifica si un string es un email válido
        /// </summary>
        public static bool IsValidEmail(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(value);
                return addr.Address == value;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Decimal Extensions

        /// <summary>
        /// Formatea un decimal como moneda colombiana
        /// </summary>
        public static string ToCurrencyString(this decimal value)
        {
            return CurrencyHelper.FormatCurrency(value);
        }

        /// <summary>
        /// Formatea un decimal como porcentaje
        /// </summary>
        public static string ToPercentageString(this decimal value)
        {
            return value.ToString("0.##") + "%";
        }

        /// <summary>
        /// Redondea un decimal a 2 decimales
        /// </summary>
        public static decimal RoundCurrency(this decimal value)
        {
            return Math.Round(value, 2);
        }

        /// <summary>
        /// Verifica si un decimal es mayor que cero
        /// </summary>
        public static bool IsPositive(this decimal value)
        {
            return value > 0;
        }

        /// <summary>
        /// Verifica si un decimal es igual a cero
        /// </summary>
        public static bool IsZero(this decimal value)
        {
            return value == 0;
        }

        #endregion

        #region DateTime Extensions

        /// <summary>
        /// Formatea una fecha como string en formato corto (dd/MM/yyyy)
        /// </summary>
        public static string ToShortDateString(this DateTime value)
        {
            return value.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Formatea una fecha y hora como string (dd/MM/yyyy HH:mm)
        /// </summary>
        public static string ToDateTimeString(this DateTime value)
        {
            return value.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Verifica si una fecha es hoy
        /// </summary>
        public static bool IsToday(this DateTime value)
        {
            return value.Date == DateTime.Today;
        }

        /// <summary>
        /// Verifica si una fecha es futura
        /// </summary>
        public static bool IsFutureDate(this DateTime value)
        {
            return value.Date > DateTime.Today;
        }

        /// <summary>
        /// Verifica si una fecha es pasada
        /// </summary>
        public static bool IsPastDate(this DateTime value)
        {
            return value.Date < DateTime.Today;
        }

        /// <summary>
        /// Devuelve el primer día del mes de la fecha dada
        /// </summary>
        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        /// <summary>
        /// Devuelve el último día del mes de la fecha dada
        /// </summary>
        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));
        }

        #endregion

        #region Collection Extensions

        /// <summary>
        /// Convierte una lista de objetos a un DataTable
        /// </summary>
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            var properties = typeof(T).GetProperties();

            // Agregar columnas
            foreach (var prop in properties)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataTable.Columns.Add(prop.Name, type);
            }

            // Agregar filas
            foreach (var item in items)
            {
                var values = new object[properties.Length];
                for (var i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item, null) ?? DBNull.Value;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        /// <summary>
        /// Verifica si una colección está vacía
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        /// <summary>
        /// Agrega un rango de elementos a una lista
        /// </summary>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null)
                return;

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Filtra una lista de detalles de factura por código o nombre de artículo
        /// </summary>
        public static IEnumerable<FacturaDetalleDto> FilterByArticulo(this IEnumerable<FacturaDetalleDto> detalles, string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                return detalles;

            filtro = filtro.ToLower();
            return detalles.Where(d =>
                (d.ArticuloCodigo != null && d.ArticuloCodigo.ToLower().Contains(filtro)) ||
                (d.ArticuloNombre != null && d.ArticuloNombre.ToLower().Contains(filtro)));
        }

        /// <summary>
        /// Suma el total de una lista de detalles de factura
        /// </summary>
        public static decimal SumSubtotal(this IEnumerable<CrearFacturaDetalleDto> detalles)
        {
            return detalles?.Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;
        }

        #endregion

        #region WebControls Extensions

        /// <summary>
        /// Establece los elementos seleccionados en un DropDownList según el valor
        /// </summary>
        public static void SetSelectedValue(this DropDownList dropDownList, object value)
        {
            if (value == null)
                return;

            var stringValue = value.ToString();
            if (string.IsNullOrEmpty(stringValue))
                return;

            var item = dropDownList.Items.FindByValue(stringValue);
            if (item != null)
                item.Selected = true;
        }

        /// <summary>
        /// Limpia todos los controles de un formulario
        /// </summary>
        public static void ClearControls(this Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                    ((TextBox)c).Text = string.Empty;
                else if (c is DropDownList)
                    ((DropDownList)c).ClearSelection();
                else if (c is CheckBox)
                    ((CheckBox)c).Checked = false;
                else if (c is RadioButton)
                    ((RadioButton)c).Checked = false;
                else if (c is HiddenField)
                    ((HiddenField)c).Value = string.Empty;
                else if (c.HasControls())
                    c.ClearControls();
            }
        }

        /// <summary>
        /// Carga una lista de artículos en un DropDownList
        /// </summary>
        public static void LoadArticulos(this DropDownList dropDownList, IEnumerable<ArticuloDto> articulos, bool includeEmpty = true)
        {
            dropDownList.Items.Clear();

            if (includeEmpty)
                dropDownList.Items.Add(new ListItem("-- Seleccione un artículo --", ""));

            if (articulos != null)
            {
                foreach (var articulo in articulos)
                {
                    var item = new ListItem(
                        $"{articulo.Codigo} - {articulo.Nombre} (${articulo.PrecioUnitarioFormateado})",
                        articulo.Id.ToString());
                    dropDownList.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Carga una lista de clientes en un DropDownList
        /// </summary>
        public static void LoadClientes(this DropDownList dropDownList, IEnumerable<ClienteDto> clientes, bool includeEmpty = true)
        {
            dropDownList.Items.Clear();

            if (includeEmpty)
                dropDownList.Items.Add(new ListItem("-- Seleccione un cliente --", ""));

            if (clientes != null)
            {
                foreach (var cliente in clientes)
                {
                    var item = new ListItem(
                        $"{cliente.NumeroDocumento} - {cliente.NombreCompleto}",
                        cliente.Id.ToString());
                    dropDownList.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Carga una lista de categorías en un DropDownList
        /// </summary>
        public static void LoadCategorias(this DropDownList dropDownList, IEnumerable<CategoriaArticuloDto> categorias, bool includeEmpty = true)
        {
            dropDownList.Items.Clear();

            if (includeEmpty)
                dropDownList.Items.Add(new ListItem("-- Seleccione una categoría --", ""));

            if (categorias != null)
            {
                foreach (var categoria in categorias)
                {
                    dropDownList.Items.Add(new ListItem(categoria.Nombre, categoria.Id.ToString()));
                }
            }
        }

        #endregion

        #region DTO Extensions

        /// <summary>
        /// Convierte un ClienteDto a un objeto JSON formateado
        /// </summary>
        public static string ToJsonString(this ClienteDto cliente)
        {
            if (cliente == null)
                return "null";

            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"Id\":{cliente.Id},");
            sb.Append($"\"NumeroDocumento\":\"{cliente.NumeroDocumento}\",");
            sb.Append($"\"Nombres\":\"{cliente.Nombres}\",");
            sb.Append($"\"Apellidos\":\"{cliente.Apellidos}\",");
            sb.Append($"\"Direccion\":\"{cliente.Direccion}\",");
            sb.Append($"\"Telefono\":\"{cliente.Telefono}\",");
            sb.Append($"\"Email\":\"{(cliente.Email ?? "")}\",");
            sb.Append($"\"NombreCompleto\":\"{cliente.NombreCompleto}\"");
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Convierte un ArticuloDto a un objeto JSON formateado
        /// </summary>
        public static string ToJsonString(this ArticuloDto articulo)
        {
            if (articulo == null)
                return "null";

            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"Id\":{articulo.Id},");
            sb.Append($"\"Codigo\":\"{articulo.Codigo}\",");
            sb.Append($"\"Nombre\":\"{articulo.Nombre}\",");
            sb.Append($"\"PrecioUnitario\":{articulo.PrecioUnitario},");
            sb.Append($"\"Stock\":{articulo.Stock},");
            sb.Append($"\"PrecioUnitarioFormateado\":\"{articulo.PrecioUnitarioFormateado}\"");
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Verifica si un detalle de factura tiene un artículo válido
        /// </summary>
        public static bool HasValidArticulo(this CrearFacturaDetalleDto detalle)
        {
            return detalle != null &&
                   detalle.ArticuloId > 0 &&
                   detalle.Cantidad > 0 &&
                   detalle.PrecioUnitario > 0;
        }

        /// <summary>
        /// Verifica si una factura tiene detalles válidos
        /// </summary>
        public static bool HasValidDetalles(this CrearFacturaDto factura)
        {
            return factura != null &&
                   factura.Detalles != null &&
                   factura.Detalles.Count > 0 &&
                   factura.Detalles.All(d => d.HasValidArticulo());
        }

        #endregion
    }
}