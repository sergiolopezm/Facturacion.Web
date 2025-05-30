using System;
using System.Globalization;

namespace Facturacion.Web.Utils
{
    /// <summary>
    /// Clase utilitaria para manejo de fechas en la aplicación
    /// </summary>
    public static class DateHelper
    {
        #region Formateo de Fechas

        /// <summary>
        /// Formatea una fecha en formato corto (dd/MM/yyyy)
        /// </summary>
        public static string FormatShortDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Formatea una fecha en formato largo (dd de MMMM de yyyy)
        /// </summary>
        public static string FormatLongDate(DateTime date)
        {
            return date.ToString("dd 'de' MMMM 'de' yyyy", new CultureInfo("es-CO"));
        }

        /// <summary>
        /// Formatea una fecha con hora (dd/MM/yyyy HH:mm)
        /// </summary>
        public static string FormatDateWithTime(DateTime date)
        {
            return date.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Formatea una fecha con hora completa (dd/MM/yyyy HH:mm:ss)
        /// </summary>
        public static string FormatDateWithFullTime(DateTime date)
        {
            return date.ToString("dd/MM/yyyy HH:mm:ss");
        }

        /// <summary>
        /// Formatea una fecha en formato para bases de datos (yyyy-MM-dd)
        /// </summary>
        public static string FormatForDatabase(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Formatea una fecha con hora para bases de datos (yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public static string FormatDateTimeForDatabase(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        #endregion

        #region Conversión de Fechas

        /// <summary>
        /// Convierte una cadena a fecha utilizando formato específico
        /// </summary>
        public static DateTime? ParseExact(string dateString, string format)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime result))
                return result;

            return null;
        }

        /// <summary>
        /// Convierte una cadena a fecha utilizando formatos comunes
        /// </summary>
        public static DateTime? Parse(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            if (DateTime.TryParse(dateString, new CultureInfo("es-CO"),
                DateTimeStyles.None, out DateTime result))
                return result;

            return null;
        }

        /// <summary>
        /// Intenta convertir una cadena a fecha
        /// </summary>
        public static bool TryParse(string dateString, out DateTime result)
        {
            return DateTime.TryParse(dateString, new CultureInfo("es-CO"),
                DateTimeStyles.None, out result);
        }

        /// <summary>
        /// Convierte una fecha a una representación JavaScript
        /// </summary>
        public static string ToJavaScriptDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        #endregion

        #region Cálculos de Períodos

        /// <summary>
        /// Obtiene la fecha actual
        /// </summary>
        public static DateTime Today()
        {
            return DateTime.Now.Date;
        }

        /// <summary>
        /// Obtiene el primer día del mes actual
        /// </summary>
        public static DateTime FirstDayOfCurrentMonth()
        {
            var today = DateTime.Now;
            return new DateTime(today.Year, today.Month, 1);
        }

        /// <summary>
        /// Obtiene el último día del mes actual
        /// </summary>
        public static DateTime LastDayOfCurrentMonth()
        {
            var today = DateTime.Now;
            return new DateTime(today.Year, today.Month,
                DateTime.DaysInMonth(today.Year, today.Month));
        }

        /// <summary>
        /// Obtiene el primer día del mes para una fecha dada
        /// </summary>
        public static DateTime FirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Obtiene el último día del mes para una fecha dada
        /// </summary>
        public static DateTime LastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month,
                DateTime.DaysInMonth(date.Year, date.Month));
        }

        /// <summary>
        /// Obtiene el primer día del año actual
        /// </summary>
        public static DateTime FirstDayOfCurrentYear()
        {
            return new DateTime(DateTime.Now.Year, 1, 1);
        }

        /// <summary>
        /// Obtiene el último día del año actual
        /// </summary>
        public static DateTime LastDayOfCurrentYear()
        {
            return new DateTime(DateTime.Now.Year, 12, 31);
        }

        /// <summary>
        /// Obtiene el primer día del año para una fecha dada
        /// </summary>
        public static DateTime FirstDayOfYear(DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        /// <summary>
        /// Obtiene el último día del año para una fecha dada
        /// </summary>
        public static DateTime LastDayOfYear(DateTime date)
        {
            return new DateTime(date.Year, 12, 31);
        }

        /// <summary>
        /// Agrega días hábiles a una fecha (excluyendo fines de semana)
        /// </summary>
        public static DateTime AddBusinessDays(DateTime date, int days)
        {
            var result = date;
            var daysAdded = 0;

            while (daysAdded < days)
            {
                result = result.AddDays(1);
                if (result.DayOfWeek != DayOfWeek.Saturday &&
                    result.DayOfWeek != DayOfWeek.Sunday)
                {
                    daysAdded++;
                }
            }

            return result;
        }

        #endregion

        #region Validación y Comparación

        /// <summary>
        /// Verifica si una fecha está dentro de un rango
        /// </summary>
        public static bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate)
        {
            return date >= startDate && date <= endDate;
        }

        /// <summary>
        /// Verifica si una fecha es válida
        /// </summary>
        public static bool IsValidDate(int year, int month, int day)
        {
            if (year < 1 || month < 1 || month > 12 || day < 1)
                return false;

            return day <= DateTime.DaysInMonth(year, month);
        }

        /// <summary>
        /// Calcula la diferencia en días entre dos fechas
        /// </summary>
        public static int DaysBetween(DateTime startDate, DateTime endDate)
        {
            return (int)(endDate.Date - startDate.Date).TotalDays;
        }

        /// <summary>
        /// Calcula la diferencia en meses entre dos fechas
        /// </summary>
        public static int MonthsBetween(DateTime startDate, DateTime endDate)
        {
            return (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
        }

        /// <summary>
        /// Calcula la diferencia en años entre dos fechas
        /// </summary>
        public static int YearsBetween(DateTime startDate, DateTime endDate)
        {
            var years = endDate.Year - startDate.Year;

            if (endDate.Month < startDate.Month ||
                (endDate.Month == startDate.Month && endDate.Day < startDate.Day))
            {
                years--;
            }

            return years;
        }

        /// <summary>
        /// Obtiene la edad basada en una fecha de nacimiento
        /// </summary>
        public static int GetAge(DateTime birthDate)
        {
            return YearsBetween(birthDate, DateTime.Today);
        }

        #endregion

        #region Fechas para Informes

        /// <summary>
        /// Obtiene el rango de fechas para el mes actual
        /// </summary>
        public static (DateTime Start, DateTime End) GetCurrentMonthRange()
        {
            return (FirstDayOfCurrentMonth(), LastDayOfCurrentMonth());
        }

        /// <summary>
        /// Obtiene el rango de fechas para el mes anterior
        /// </summary>
        public static (DateTime Start, DateTime End) GetPreviousMonthRange()
        {
            var firstDayOfCurrentMonth = FirstDayOfCurrentMonth();
            var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);
            var firstDayOfPreviousMonth = new DateTime(lastDayOfPreviousMonth.Year,
                lastDayOfPreviousMonth.Month, 1);

            return (firstDayOfPreviousMonth, lastDayOfPreviousMonth);
        }

        /// <summary>
        /// Obtiene el rango de fechas para el año actual
        /// </summary>
        public static (DateTime Start, DateTime End) GetCurrentYearRange()
        {
            return (FirstDayOfCurrentYear(), LastDayOfCurrentYear());
        }

        /// <summary>
        /// Obtiene el rango de fechas para el año anterior
        /// </summary>
        public static (DateTime Start, DateTime End) GetPreviousYearRange()
        {
            var previousYear = DateTime.Now.Year - 1;
            return (new DateTime(previousYear, 1, 1), new DateTime(previousYear, 12, 31));
        }

        /// <summary>
        /// Obtiene el rango de fechas para el trimestre actual
        /// </summary>
        public static (DateTime Start, DateTime End) GetCurrentQuarterRange()
        {
            var now = DateTime.Now;
            var currentQuarter = (now.Month - 1) / 3 + 1;
            var firstMonthOfQuarter = (currentQuarter - 1) * 3 + 1;

            var firstDayOfQuarter = new DateTime(now.Year, firstMonthOfQuarter, 1);
            var lastDayOfQuarter = new DateTime(now.Year, firstMonthOfQuarter + 2,
                DateTime.DaysInMonth(now.Year, firstMonthOfQuarter + 2));

            return (firstDayOfQuarter, lastDayOfQuarter);
        }

        /// <summary>
        /// Obtiene el rango de fechas para los últimos N días
        /// </summary>
        public static (DateTime Start, DateTime End) GetLastNDaysRange(int days)
        {
            var now = DateTime.Now.Date;
            return (now.AddDays(-days + 1), now);
        }

        #endregion

        #region Utilidades para Controles de Formulario

        /// <summary>
        /// Formatea la fecha actual para mostrar en un control de solo lectura
        /// </summary>
        public static string GetCurrentDateForDisplay()
        {
            return FormatLongDate(DateTime.Now);
        }

        /// <summary>
        /// Formatea la fecha actual para un valor de campo de fecha
        /// </summary>
        public static string GetCurrentDateForDateField()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Convierte una fecha para mostrar en formato amigable (hoy, ayer, etc.)
        /// </summary>
        public static string GetFriendlyDate(DateTime date)
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            if (date.Date == today)
                return "Hoy";

            if (date.Date == yesterday)
                return "Ayer";

            if (date.Date > today.AddDays(-7))
                return $"{GetDayOfWeekName(date)}, {FormatShortDate(date)}";

            return FormatLongDate(date);
        }

        /// <summary>
        /// Obtiene el nombre del día de la semana
        /// </summary>
        public static string GetDayOfWeekName(DateTime date)
        {
            var culture = new CultureInfo("es-CO");
            return culture.DateTimeFormat.GetDayName(date.DayOfWeek);
        }

        /// <summary>
        /// Obtiene el nombre del mes
        /// </summary>
        public static string GetMonthName(int month)
        {
            if (month < 1 || month > 12)
                return string.Empty;

            var culture = new CultureInfo("es-CO");
            return culture.DateTimeFormat.GetMonthName(month);
        }

        #endregion
    }
}