using Facturacion.Web.Models.DTOs.Auth;
using System;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Web.Models.Auth
{
    /// <summary>
    /// ViewModel que representa la información de perfil
    /// mostrada / editada en <c>Perfil.aspx</c>.
    /// Se encarga de las validaciones de interfaz y de mapearse
    /// hacia/desde <see cref="UsuarioPerfilDto"/> que devuelve la API.
    /// </summary>
    public class UsuarioPerfilViewModel
    {
        #region Propiedades ligadas a la vista

        [ScaffoldColumn(false)]
        public Guid Id { get; set; }

        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "Máx. 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "Máx. 100 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [ScaffoldColumn(false)]
        public int RolId { get; set; }

        [Display(Name = "Rol")]
        public string Rol { get; set; }

        [Display(Name = "Creado")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Último acceso")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? FechaUltimoAcceso { get; set; }

        /// <summary>
        /// Nombre completo calculado (solo lectura).
        /// </summary>
        public string NombreCompleto => $"{Nombre} {Apellido}";

        #endregion

        #region Constructores

        public UsuarioPerfilViewModel()
        {
            // Garantizar que las cadenas no sean null
            NombreUsuario = Nombre = Apellido = Email = Rol = string.Empty;
        }

        public UsuarioPerfilViewModel(UsuarioPerfilDto dto) : this()
        {
            FromDto(dto);
        }

        #endregion

        #region Conversión DTO ⇄ ViewModel

        /// <summary>
        /// Rellena las propiedades del ViewModel con los datos recibidos del backend.
        /// </summary>
        public void FromDto(UsuarioPerfilDto dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            NombreUsuario = dto.NombreUsuario;
            Nombre = dto.Nombre;
            Apellido = dto.Apellido;
            Email = dto.Email;
            Rol = dto.Rol;
            RolId = dto.RolId;
            FechaCreacion = dto.FechaCreacion;
            FechaUltimoAcceso = dto.FechaUltimoAcceso;
        }

        /// <summary>
        /// Convierte el ViewModel en <see cref="UsuarioPerfilDto"/>.
        /// Útil al actualizar información de perfil.
        /// </summary>
        public UsuarioPerfilDto ToDto()
            => new UsuarioPerfilDto
            {
                Id = Id,
                NombreUsuario = NombreUsuario,
                Nombre = Nombre,
                Apellido = Apellido,
                Email = Email,
                Rol = Rol,
                RolId = RolId,
                FechaCreacion = FechaCreacion,
                FechaUltimoAcceso = FechaUltimoAcceso
            };

        #endregion
    }
}