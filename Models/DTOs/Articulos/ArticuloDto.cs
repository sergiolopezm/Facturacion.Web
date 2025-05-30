using System;
using System.ComponentModel.DataAnnotations;

namespace Facturacion.Web.Models.DTOs.Articulos
{
    /// <summary>
    /// DTO para artículo
    /// </summary>
    public class ArticuloDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código del artículo es requerido")]
        [StringLength(20, ErrorMessage = "El código no puede exceder los 20 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre del artículo es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [StringLength(250, ErrorMessage = "La descripción no puede exceder los 250 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; }

        [Display(Name = "Categoría")]
        public int? CategoriaId { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Propiedades de navegación para UI
        public string Categoria { get; set; }
        public string CreadoPor { get; set; }
        public string ModificadoPor { get; set; }
        public string PrecioUnitarioFormateado { get; set; }
        public bool StockBajo { get; set; }
        public int VecesVendido { get; set; }

        public ArticuloDto()
        {
            Codigo = string.Empty;
            Nombre = string.Empty;
            Descripcion = string.Empty;
            Categoria = string.Empty;
            CreadoPor = string.Empty;
            ModificadoPor = string.Empty;
            PrecioUnitarioFormateado = string.Empty;
            Activo = true;
        }
    }
}