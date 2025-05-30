namespace Facturacion.Web.Models.DTOs.Common
{
    /// <summary>
    /// DTO genérico para las respuestas de la aplicación
    /// </summary>
    /// <typeparam name="T">Tipo del resultado</typeparam>
    public class RespuestaDto<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public string Detalle { get; set; }
        public T Resultado { get; set; }

        public RespuestaDto()
        {
            Exito = false;
            Mensaje = string.Empty;
            Detalle = string.Empty;
        }

        public static RespuestaDto<T> CrearExitoso(string mensaje, T resultado = default(T), string detalle = "")
        {
            return new RespuestaDto<T>
            {
                Exito = true,
                Mensaje = mensaje,
                Detalle = detalle,
                Resultado = resultado
            };
        }

        public static RespuestaDto<T> CrearError(string mensaje, string detalle = "")
        {
            return new RespuestaDto<T>
            {
                Exito = false,
                Mensaje = mensaje,
                Detalle = detalle
            };
        }

    }

    /// <summary>
    /// DTO para respuestas sin tipo específico
    /// </summary>
    public class RespuestaDto : RespuestaDto<object>
    {
        public static RespuestaDto CrearExitoso(string mensaje, object resultado = null, string detalle = "")
        {
            return new RespuestaDto
            {
                Exito = true,
                Mensaje = mensaje,
                Detalle = detalle,
                Resultado = resultado
            };
        }

        public static new RespuestaDto CrearError(string mensaje, string detalle = "")
        {
            return new RespuestaDto
            {
                Exito = false,
                Mensaje = mensaje,
                Detalle = detalle
            };
        }
    }
}