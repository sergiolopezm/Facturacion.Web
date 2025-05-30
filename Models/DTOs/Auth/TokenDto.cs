using System;

namespace Facturacion.Web.Models.DTOs.Auth
{
    /// <summary>
    /// DTO para token de autenticación
    /// </summary>
    public class TokenDto
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
        public UsuarioPerfilDto Usuario { get; set; }

        public TokenDto()
        {
            Token = string.Empty;
            Usuario = new UsuarioPerfilDto();
        }
    }
}