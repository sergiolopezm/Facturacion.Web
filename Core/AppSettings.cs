using System.Configuration;

namespace Facturacion.Web.Core
{
    /// <summary>
    /// Configuraciones centralizadas de la aplicación
    /// Lee valores desde web.config y proporciona valores por defecto
    /// </summary>
    public static class AppSettings
    {
        #region Configuración de API

        /// <summary>
        /// URL base de la API de facturación
        /// </summary>
        public static string ApiBaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:7220/api/";
            }
        }

        /// <summary>
        /// Sitio para autenticación con la API
        /// </summary>
        public static string ApiSitio
        {
            get
            {
                return ConfigurationManager.AppSettings["ApiSitio"] ?? "FacturacionWeb";
            }
        }

        /// <summary>
        /// Clave para autenticación con la API
        /// </summary>
        public static string ApiClave
        {
            get
            {
                return ConfigurationManager.AppSettings["ApiClave"] ?? "WebApp2024*";
            }
        }

        /// <summary>
        /// Timeout en segundos para las peticiones HTTP
        /// </summary>
        public static int ApiTimeoutSeconds
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["ApiTimeoutSeconds"], out int timeout))
                {
                    return timeout;
                }
                return 30; // Valor por defecto
            }
        }

        #endregion

        #region Configuración de Sesión

        /// <summary>
        /// Nombre de la cookie/session para el token JWT
        /// </summary>
        public static string SessionTokenKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SessionTokenKey"] ?? "FacturacionToken";
            }
        }

        /// <summary>
        /// Nombre de la cookie/session para los datos del usuario
        /// </summary>
        public static string SessionUserKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SessionUserKey"] ?? "FacturacionUser";
            }
        }

        /// <summary>
        /// Tiempo de expiración de sesión en minutos
        /// </summary>
        public static int SessionTimeoutMinutes
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["SessionTimeoutMinutes"], out int timeout))
                {
                    return timeout;
                }
                return 60; // Valor por defecto
            }
        }

        #endregion

        #region Configuración de UI

        /// <summary>
        /// Número de elementos por página por defecto en las grillas
        /// </summary>
        public static int DefaultPageSize
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["DefaultPageSize"], out int pageSize))
                {
                    return pageSize;
                }
                return 10; // Valor por defecto
            }
        }

        /// <summary>
        /// Título de la aplicación
        /// </summary>
        public static string AppTitle
        {
            get
            {
                return ConfigurationManager.AppSettings["AppTitle"] ?? "Sistema de Facturación";
            }
        }

        /// <summary>
        /// Versión de la aplicación
        /// </summary>
        public static string AppVersion
        {
            get
            {
                return ConfigurationManager.AppSettings["AppVersion"] ?? "1.0.0";
            }
        }

        #endregion

        #region Configuración de Moneda

        /// <summary>
        /// Símbolo de moneda
        /// </summary>
        public static string CurrencySymbol
        {
            get
            {
                return ConfigurationManager.AppSettings["CurrencySymbol"] ?? "$";
            }
        }

        /// <summary>
        /// Separador decimal para moneda
        /// </summary>
        public static string CurrencyDecimalSeparator
        {
            get
            {
                return ConfigurationManager.AppSettings["CurrencyDecimalSeparator"] ?? ",";
            }
        }

        /// <summary>
        /// Separador de miles para moneda
        /// </summary>
        public static string CurrencyThousandSeparator
        {
            get
            {
                return ConfigurationManager.AppSettings["CurrencyThousandSeparator"] ?? ".";
            }
        }

        /// <summary>
        /// Número de decimales para moneda
        /// </summary>
        public static int CurrencyDecimalPlaces
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["CurrencyDecimalPlaces"], out int places))
                {
                    return places;
                }
                return 2; // Valor por defecto
            }
        }

        #endregion

        #region Configuración de Validación

        /// <summary>
        /// Longitud mínima para contraseñas
        /// </summary>
        public static int MinPasswordLength
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["MinPasswordLength"], out int length))
                {
                    return length;
                }
                return 6; // Valor por defecto
            }
        }

        /// <summary>
        /// Longitud máxima para campos de texto
        /// </summary>
        public static int MaxTextLength
        {
            get
            {
                if (int.TryParse(ConfigurationManager.AppSettings["MaxTextLength"], out int length))
                {
                    return length;
                }
                return 500; // Valor por defecto
            }
        }

        #endregion

        #region Configuración de Negocio

        /// <summary>
        /// Porcentaje de IVA por defecto
        /// </summary>
        public static decimal DefaultIvaPercentage
        {
            get
            {
                if (decimal.TryParse(ConfigurationManager.AppSettings["DefaultIvaPercentage"], out decimal iva))
                {
                    return iva;
                }
                return 19m; // Valor por defecto para Colombia
            }
        }

        /// <summary>
        /// Porcentaje de descuento por defecto
        /// </summary>
        public static decimal DefaultDiscountPercentage
        {
            get
            {
                if (decimal.TryParse(ConfigurationManager.AppSettings["DefaultDiscountPercentage"], out decimal discount))
                {
                    return discount;
                }
                return 5m; // Valor por defecto
            }
        }

        /// <summary>
        /// Monto mínimo para aplicar descuento
        /// </summary>
        public static decimal MinAmountForDiscount
        {
            get
            {
                if (decimal.TryParse(ConfigurationManager.AppSettings["MinAmountForDiscount"], out decimal amount))
                {
                    return amount;
                }
                return 500000m; // Valor por defecto
            }
        }

        #endregion

        #region Métodos de Utilidad

        /// <summary>
        /// Obtiene un valor de configuración personalizado
        /// </summary>
        /// <param name="key">Clave de configuración</param>
        /// <param name="defaultValue">Valor por defecto si no se encuentra la clave</param>
        /// <returns>Valor de configuración</returns>
        public static string GetCustomSetting(string key, string defaultValue = "")
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        /// <summary>
        /// Obtiene un valor de configuración personalizado como entero
        /// </summary>
        /// <param name="key">Clave de configuración</param>
        /// <param name="defaultValue">Valor por defecto si no se encuentra la clave</param>
        /// <returns>Valor de configuración</returns>
        public static int GetCustomSettingInt(string key, int defaultValue = 0)
        {
            if (int.TryParse(ConfigurationManager.AppSettings[key], out int value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Obtiene un valor de configuración personalizado como decimal
        /// </summary>
        /// <param name="key">Clave de configuración</param>
        /// <param name="defaultValue">Valor por defecto si no se encuentra la clave</param>
        /// <returns>Valor de configuración</returns>
        public static decimal GetCustomSettingDecimal(string key, decimal defaultValue = 0m)
        {
            if (decimal.TryParse(ConfigurationManager.AppSettings[key], out decimal value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Obtiene un valor de configuración personalizado como booleano
        /// </summary>
        /// <param name="key">Clave de configuración</param>
        /// <param name="defaultValue">Valor por defecto si no se encuentra la clave</param>
        /// <returns>Valor de configuración</returns>
        public static bool GetCustomSettingBool(string key, bool defaultValue = false)
        {
            if (bool.TryParse(ConfigurationManager.AppSettings[key], out bool value))
            {
                return value;
            }
            return defaultValue;
        }

        #endregion
    }
}