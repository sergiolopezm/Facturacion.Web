using System;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Web.Models.DTOs.Articulos
{
    /// <summary>
    /// DTO para categoría de artículo
    /// </summary>
    public class CategoriaArticuloDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [StringLength(250, ErrorMessage = "La descripción no puede exceder los 250 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Propiedades de navegación para UI
        public string CreadoPor { get; set; }
        public string ModificadoPor { get; set; }
        public int TotalArticulos { get; set; }

        public CategoriaArticuloDto()
        {
            Nombre = string.Empty;
            Descripcion = string.Empty;
            CreadoPor = string.Empty;
            ModificadoPor = string.Empty;
            Activo = true;
        }
    }
}