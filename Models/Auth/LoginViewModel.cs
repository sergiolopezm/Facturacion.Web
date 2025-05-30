using Facturacion.Web.Models.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Web.Models.Auth
{
    /// <summary>
    /// ViewModel para la vista <c>Login.aspx</c>.
    /// Encapsula los datos requeridos por la interfaz, sus validaciones
    /// y los métodos de conversión a DTO para consumir la API.
    /// </summary>
    public class LoginViewModel
    {
        #region Propiedades vinculadas a la vista

        [Required(ErrorMessage = "El usuario es requerido")]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contraseña { get; set; }

        /// <summary>
        ///     Si el usuario decide mantener la sesión iniciada.
        ///     El backend no lo necesita, pero ayuda a la UX para
        ///     establecer la expiración de la cookie/token.
        /// </summary>
        public bool Recordarme { get; set; }

        #endregion

        #region Constructores

        public LoginViewModel()
        {
            NombreUsuario = string.Empty;
            Contraseña = string.Empty;
            Recordarme = false;
        }

        public LoginViewModel(string usuario, string contraseña, bool recordar = false)
        {
            NombreUsuario = usuario;
            Contraseña = contraseña;
            Recordarme = recordar;
        }

        #endregion

        #region Métodos de utilidad

        /// <summary>
        /// Convierte el ViewModel en el DTO que espera la API (<see cref="UsuarioLoginDto"/>).
        /// </summary>
        public UsuarioLoginDto ToDto(string ipCliente = "")
            => new UsuarioLoginDto
            {
                NombreUsuario = NombreUsuario,
                Contraseña = Contraseña,
                Ip = ipCliente
            };

        #endregion
    }
}