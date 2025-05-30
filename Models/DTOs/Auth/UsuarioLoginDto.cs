using System.ComponentModel.DataAnnotations;

namespace Facturacion.Web.Models.DTOs.Auth
{
    /// <summary>
    /// DTO para el login de usuario
    /// </summary>
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        public string Ip { get; set; }

        public UsuarioLoginDto()
        {
            NombreUsuario = string.Empty;
            Contraseña = string.Empty;
            Ip = string.Empty;
        }
    }
}