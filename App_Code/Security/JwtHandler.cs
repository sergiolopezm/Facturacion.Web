using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Facturacion.Web.Core.App_Code.Security
{
    /// <summary>
    /// Clase para manipulación y validación de tokens JWT
    /// </summary>
    public class JwtHandler
    {
        // Singleton para evitar múltiples instancias
        private static JwtHandler _instance;
        private static readonly object _lock = new object();

        private JwtSecurityTokenHandler _tokenHandler;
        private TokenValidationParameters _validationParameters;

        private JwtHandler()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
            // Inicializar parámetros básicos - No se valida firma en frontend
            _validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }

        /// <summary>
        /// Obtiene la instancia única de JwtHandler
        /// </summary>
        public static JwtHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new JwtHandler();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Obtiene los claims del token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Colección de claims o null si el token es inválido</returns>
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                // Validar el token sin verificar la firma (ya lo hizo el backend)
                SecurityToken securityToken;
                var principal = _tokenHandler.ValidateToken(token, _validationParameters, out securityToken);

                return principal;
            }
            catch (Exception ex)
            {
                // Registrar error
                System.Diagnostics.Debug.WriteLine($"Error validando token JWT: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Verifica si el token está expirado
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>True si el token expiró, False si es válido</returns>
        public bool IsTokenExpired(string token)
        {
            if (string.IsNullOrEmpty(token))
                return true;

            try
            {
                // Leer el token sin validarlo
                var jwtToken = _tokenHandler.ReadToken(token) as JwtSecurityToken;
                if (jwtToken == null)
                    return true;

                // Verificar expiración
                var expiration = jwtToken.ValidTo;
                return expiration < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Obtiene la fecha de expiración del token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Fecha de expiración o DateTime.MinValue si el token es inválido</returns>
        public DateTime GetTokenExpirationDate(string token)
        {
            if (string.IsNullOrEmpty(token))
                return DateTime.MinValue;

            try
            {
                var jwtToken = _tokenHandler.ReadToken(token) as JwtSecurityToken;
                if (jwtToken == null)
                    return DateTime.MinValue;

                return jwtToken.ValidTo;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Obtiene un claim específico del token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="claimType">Tipo de claim a obtener</param>
        /// <returns>Valor del claim o null si no existe</returns>
        public string GetClaimValue(string token, string claimType)
        {
            var principal = GetPrincipalFromToken(token);
            if (principal == null)
                return null;

            var claim = principal.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }

        /// <summary>
        /// Obtiene el ID del usuario desde el token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>ID del usuario o null si no se puede obtener</returns>
        public string GetUserId(string token)
        {
            return GetClaimValue(token, ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Obtiene el nombre de usuario desde el token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Nombre de usuario o null si no se puede obtener</returns>
        public string GetUsername(string token)
        {
            return GetClaimValue(token, ClaimTypes.Name);
        }

        /// <summary>
        /// Obtiene el rol del usuario desde el token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Rol del usuario o null si no se puede obtener</returns>
        public string GetUserRole(string token)
        {
            return GetClaimValue(token, ClaimTypes.Role);
        }

        /// <summary>
        /// Obtiene el email del usuario desde el token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Email del usuario o null si no se puede obtener</returns>
        public string GetUserEmail(string token)
        {
            return GetClaimValue(token, ClaimTypes.Email);
        }

        /// <summary>
        /// Verifica si el token tiene un rol específico
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="role">Rol a verificar</param>
        /// <returns>True si tiene el rol, False en caso contrario</returns>
        public bool HasRole(string token, string role)
        {
            var userRole = GetUserRole(token);
            return !string.IsNullOrEmpty(userRole) && userRole.Equals(role, StringComparison.OrdinalIgnoreCase);
        }
    }
}