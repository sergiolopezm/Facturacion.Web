namespace Facturacion.Web.Models.DTOs.Common
{
    /// <summary>
    /// DTO para validaciones
    /// </summary>
    public class ValidoDto
    {
        public bool EsValido { get; set; }
        public string Detalle { get; set; }

        public ValidoDto()
        {
            EsValido = false;
            Detalle = string.Empty;
        }

        public static ValidoDto Valido(string detalle = "")
        {
            return new ValidoDto
            {
                EsValido = true,
                Detalle = detalle
            };
        }

        public static ValidoDto Invalido(string detalle)
        {
            return new ValidoDto
            {
                EsValido = false,
                Detalle = detalle
            };
        }
    }
}