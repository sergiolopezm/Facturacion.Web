using System;

namespace Facturacion.Web.Models.DTOs.Auth
{
    /// <summary>
    /// DTO para mostrar información de perfil de usuario
    /// </summary>
    public class UsuarioPerfilDto
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public int RolId { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaUltimoAcceso { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        public UsuarioPerfilDto()
        {
            NombreUsuario = string.Empty;
            Nombre = string.Empty;
            Apellido = string.Empty;
            Email = string.Empty;
            Rol = string.Empty;
        }
    }
}