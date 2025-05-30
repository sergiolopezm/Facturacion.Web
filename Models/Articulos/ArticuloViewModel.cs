using Facturacion.Web.Models.DTOs.Articulos;
using Facturacion.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.UI.WebControls;

namespace Facturacion.Web.Models.Articulos
{
    /// <summary>
    /// ViewModel para la gestión de artículos en la interfaz de usuario
    /// Contiene propiedades extendidas con validaciones y conversiones específicas para el frontend
    /// </summary>
    public class ArticuloViewModel
    {
        #region Propiedades básicas

        public int Id { get; set; }

        [Required(ErrorMessage = "El código del artículo es requerido")]
        [StringLength(20, ErrorMessage = "El código no puede exceder los 20 caracteres")]
        [Display(Name = "Código*")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre del artículo es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre*")]
        public string Nombre { get; set; }

        [StringLength(250, ErrorMessage = "La descripción no puede exceder los 250 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Display(Name = "Precio Unitario*")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        [Display(Name = "Stock*")]
        public int Stock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; }

        [Display(Name = "Categoría")]
        public int? CategoriaId { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        #endregion

        #region Propiedades calculadas y de UI

        [Display(Name = "Categoría")]
        public string Categoria { get; set; }

        [Display(Name = "Creado Por")]
        public string CreadoPor { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Modificado Por")]
        public string ModificadoPor { get; set; }

        [Display(Name = "Fecha Modificación")]
        public DateTime? FechaModificacion { get; set; }

        [Display(Name = "Precio Unitario")]
        public string PrecioUnitarioFormateado => CurrencyHelper.FormatCurrency(PrecioUnitario);

        [Display(Name = "Stock Bajo")]
        public bool StockBajo { get; set; }

        [Display(Name = "Veces Vendido")]
        public int VecesVendido { get; set; }

        #endregion

        #region Estados para UI

        public bool EsNuevo => Id == 0;
        public bool TieneStock => Stock > 0;
        public string EstadoStock => Stock <= StockMinimo ? "Stock Bajo" : "Stock Normal";
        public string EstadoStockClase => Stock <= StockMinimo ? "stock-bajo" : "stock-normal";
        public string EstadoActivo => Activo ? "Activo" : "Inactivo";
        public string EstadoActivoClase => Activo ? "estado-activo" : "estado-inactivo";
        public bool TieneCategoria => CategoriaId.HasValue && CategoriaId.Value > 0;

        #endregion

        #region Constructores

        public ArticuloViewModel()
        {
            // Inicializar con valores por defecto
            Id = 0;
            Codigo = string.Empty;
            Nombre = string.Empty;
            Descripcion = string.Empty;
            PrecioUnitario = 0;
            Stock = 0;
            StockMinimo = 0;
            Categoria = string.Empty;
            CreadoPor = string.Empty;
            ModificadoPor = string.Empty;
            FechaCreacion = DateTime.Now;
            Activo = true;
            StockBajo = false;
            VecesVendido = 0;
        }

        /// <summary>
        /// Constructor que crea el ViewModel a partir de un DTO
        /// </summary>
        public ArticuloViewModel(ArticuloDto dto)
        {
            if (dto != null)
            {
                Id = dto.Id;
                Codigo = dto.Codigo;
                Nombre = dto.Nombre;
                Descripcion = dto.Descripcion;
                PrecioUnitario = dto.PrecioUnitario;
                Stock = dto.Stock;
                StockMinimo = dto.StockMinimo;
                CategoriaId = dto.CategoriaId;
                Activo = dto.Activo;
                FechaCreacion = dto.FechaCreacion;
                FechaModificacion = dto.FechaModificacion;
                Categoria = dto.Categoria;
                CreadoPor = dto.CreadoPor;
                ModificadoPor = dto.ModificadoPor;
                StockBajo = dto.StockBajo;
                VecesVendido = dto.VecesVendido;
            }
        }

        #endregion

        #region Métodos de conversión

        /// <summary>
        /// Convierte el ViewModel a un DTO para enviar al backend
        /// </summary>
        public ArticuloDto ToDto()
        {
            return new ArticuloDto
            {
                Id = this.Id,
                Codigo = this.Codigo?.Trim().ToUpper(),
                Nombre = this.Nombre?.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(this.Descripcion) ? null : this.Descripcion.Trim(),
                PrecioUnitario = this.PrecioUnitario,
                Stock = this.Stock,
                StockMinimo = this.StockMinimo,
                CategoriaId = this.CategoriaId,
                Activo = this.Activo
            };
        }

        /// <summary>
        /// Crea una lista de ViewModels a partir de una lista de DTOs
        /// </summary>
        public static List<ArticuloViewModel> FromDtoList(List<ArticuloDto> dtos)
        {
            if (dtos == null) return new List<ArticuloViewModel>();

            var result = new List<ArticuloViewModel>();
            foreach (var dto in dtos)
            {
                result.Add(new ArticuloViewModel(dto));
            }
            return result;
        }

        #endregion

        #region Métodos de validación para WebForms

        /// <summary>
        /// Valida que el precio unitario sea válido
        /// </summary>
        public static bool ValidarPrecioUnitario(string precioUnitarioStr, out decimal precioUnitario, out string mensaje)
        {
            mensaje = string.Empty;
            precioUnitario = 0;

            if (string.IsNullOrWhiteSpace(precioUnitarioStr))
            {
                mensaje = "El precio unitario es requerido";
                return false;
            }

            // Intentar convertir el precio a decimal
            if (!decimal.TryParse(precioUnitarioStr.Replace(",", "."), out precioUnitario))
            {
                mensaje = "El precio unitario debe ser un número válido";
                return false;
            }

            // Validar que sea mayor a cero
            if (precioUnitario <= 0)
            {
                mensaje = "El precio unitario debe ser mayor a 0";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida que el stock sea válido
        /// </summary>
        public static bool ValidarStock(string stockStr, out int stock, out string mensaje)
        {
            mensaje = string.Empty;
            stock = 0;

            if (string.IsNullOrWhiteSpace(stockStr))
            {
                mensaje = "El stock es requerido";
                return false;
            }

            // Intentar convertir el stock a entero
            if (!int.TryParse(stockStr, out stock))
            {
                mensaje = "El stock debe ser un número entero válido";
                return false;
            }

            // Validar que no sea negativo
            if (stock < 0)
            {
                mensaje = "El stock no puede ser negativo";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida que el stock mínimo sea válido
        /// </summary>
        public static bool ValidarStockMinimo(string stockMinimoStr, out int stockMinimo, out string mensaje)
        {
            mensaje = string.Empty;
            stockMinimo = 0;

            // Si está vacío, usar 0 como valor por defecto
            if (string.IsNullOrWhiteSpace(stockMinimoStr))
            {
                return true;
            }

            // Intentar convertir el stock mínimo a entero
            if (!int.TryParse(stockMinimoStr, out stockMinimo))
            {
                mensaje = "El stock mínimo debe ser un número entero válido";
                return false;
            }

            // Validar que no sea negativo
            if (stockMinimo < 0)
            {
                mensaje = "El stock mínimo no puede ser negativo";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida el modelo completo para formularios WebForms
        /// </summary>
        public bool Validar(out List<string> errores)
        {
            errores = new List<string>();

            // Validaciones de campos requeridos
            if (string.IsNullOrWhiteSpace(Codigo))
                errores.Add("El código del artículo es requerido");

            if (string.IsNullOrWhiteSpace(Nombre))
                errores.Add("El nombre del artículo es requerido");

            // Validaciones de longitud
            if (Codigo?.Length > 20)
                errores.Add("El código no puede exceder los 20 caracteres");

            if (Nombre?.Length > 100)
                errores.Add("El nombre no puede exceder los 100 caracteres");

            if (!string.IsNullOrWhiteSpace(Descripcion) && Descripcion.Length > 250)
                errores.Add("La descripción no puede exceder los 250 caracteres");

            // Validaciones de tipo numérico
            if (PrecioUnitario <= 0)
                errores.Add("El precio unitario debe ser mayor a 0");

            if (Stock < 0)
                errores.Add("El stock no puede ser negativo");

            if (StockMinimo < 0)
                errores.Add("El stock mínimo no puede ser negativo");

            return errores.Count == 0;
        }

        #endregion

        #region Métodos para la UI WebForms

        /// <summary>
        /// Rellena un DropDownList con la lista de artículos
        /// </summary>
        public static void RellenarDropDownList(DropDownList dropDownList, List<ArticuloViewModel> articulos, bool incluirSeleccione = true)
        {
            dropDownList.Items.Clear();

            if (incluirSeleccione)
            {
                dropDownList.Items.Add(new ListItem("-- Seleccione un artículo --", "0"));
            }

            if (articulos != null && articulos.Count > 0)
            {
                foreach (var articulo in articulos.OrderBy(a => a.Nombre))
                {
                    string texto = $"{articulo.Nombre} ({articulo.Codigo})";
                    dropDownList.Items.Add(new ListItem(texto, articulo.Id.ToString()));
                }
            }
        }

        /// <summary>
        /// Busca un artículo por su código
        /// </summary>
        public static ArticuloViewModel BuscarPorCodigo(List<ArticuloViewModel> articulos, string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || articulos == null || articulos.Count == 0)
                return null;

            // Normalizar el código (mayúsculas y sin espacios)
            string codigoNormalizado = codigo.Trim().ToUpper();

            return articulos.FirstOrDefault(a => a.Codigo.Trim().ToUpper() == codigoNormalizado);
        }

        /// <summary>
        /// Verifica si hay stock suficiente para una cantidad
        /// </summary>
        public bool TieneStockSuficiente(int cantidad)
        {
            return Stock >= cantidad;
        }

        /// <summary>
        /// Calcula el valor de un artículo según la cantidad
        /// </summary>
        public decimal CalcularValor(int cantidad)
        {
            return PrecioUnitario * cantidad;
        }

        /// <summary>
        /// Formatea el precio del artículo
        /// </summary>
        public string FormatearPrecio()
        {
            return CurrencyHelper.FormatCurrency(PrecioUnitario);
        }

        #endregion
    }
}