using Facturacion.Web.Models.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Web.Models.Auth
{
    /// <summary>
    /// ViewModel para la vista <c>Registro.aspx</c>.
    /// Encapsula los datos que el usuario debe completar, sus
    /// validaciones y la conversión al <see cref="UsuarioRegistroDto"/>
    /// que consume la API.
    /// </summary>
    public class RegistroViewModel
    {
        #region Propiedades ligadas a la vista

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "El nombre de usuario debe tener entre 3 y 100 caracteres")]
        [Display(Name = "Nombre de Usuario")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(50, MinimumLength = 6,
            ErrorMessage = "La contraseña debe tener entre 6 y 50 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "Confirme la contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContraseña { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        [Range(1, int.MaxValue, ErrorMessage = "Rol no válido")]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        #endregion

        #region Constructores

        public RegistroViewModel()
        {
            NombreUsuario = string.Empty;
            Contraseña = string.Empty;
            ConfirmarContraseña = string.Empty;
            Nombre = string.Empty;
            Apellido = string.Empty;
            Email = string.Empty;
            RolId = 0;
        }

        public RegistroViewModel(UsuarioRegistroDto dto) : this()
        {
            FromDto(dto);
        }

        #endregion

        #region Conversión DTO ⇄ ViewModel

        /// <summary>
        /// Llena el ViewModel desde un <see cref="UsuarioRegistroDto"/>.
        /// Útil si la pantalla soporta edición de datos existentes.
        /// </summary>
        public void FromDto(UsuarioRegistroDto dto)
        {
            if (dto == null) return;

            NombreUsuario = dto.NombreUsuario;
            Contraseña = dto.Contraseña;
            ConfirmarContraseña = dto.Contraseña; // para que coincida al mostrar
            Nombre = dto.Nombre;
            Apellido = dto.Apellido;
            Email = dto.Email;
            RolId = dto.RolId;
        }

        /// <summary>
        /// Convierte el ViewModel al DTO que espera la API.
        /// </summary>
        public UsuarioRegistroDto ToDto()
            => new UsuarioRegistroDto
            {
                NombreUsuario = NombreUsuario,
                Contraseña = Contraseña,
                Nombre = Nombre,
                Apellido = Apellido,
                Email = Email,
                RolId = RolId
            };

        #endregion
    }
}