using Facturacion.Web.Models.DTOs.Reportes;
using Facturacion.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Facturacion.Web.Models.Reportes
{
    /// <summary>
    /// ViewModel para la página de Dashboard principal
    /// Consolida información de varios reportes para mostrar en una única vista
    /// </summary>
    public class DashboardViewModel
    {
        #region Propiedades principales

        /// <summary>
        /// Fecha en que se generó el dashboard
        /// </summary>
        public DateTime FechaGeneracion { get; set; }

        /// <summary>
        /// Resumen de ventas del día actual
        /// </summary>
        public ResumenDiarioInfo ResumenHoy { get; set; }

        /// <summary>
        /// Resumen de ventas del mes actual
        /// </summary>
        public ResumenMensualInfo ResumenMes { get; set; }

        /// <summary>
        /// Ventas agrupadas por categoría de artículo
        /// </summary>
        public List<CategoriaVentasInfo> VentasPorCategoria { get; set; }

        /// <summary>
        /// Lista de artículos más vendidos
        /// </summary>
        public List<ArticuloMasVendidoInfo> ArticulosMasVendidos { get; set; }

        /// <summary>
        /// Lista de clientes frecuentes
        /// </summary>
        public List<ClienteFrecuenteInfo> ClientesFrecuentes { get; set; }

        /// <summary>
        /// Cantidad de artículos con stock bajo
        /// </summary>
        public int ArticulosStockBajo { get; set; }

        /// <summary>
        /// Cantidad de facturas por estado
        /// </summary>
        public Dictionary<string, int> FacturasPorEstado { get; set; }

        /// <summary>
        /// Ventas por mes para el año actual
        /// </summary>
        public Dictionary<string, decimal> VentasPorMes { get; set; }

        #endregion

        #region Propiedades calculadas para UI

        /// <summary>
        /// Total de ventas del día formateado
        /// </summary>
        public string TotalVentasHoyFormateado => ResumenHoy?.TotalVentasFormateado ?? "$0";

        /// <summary>
        /// Total de facturas del día
        /// </summary>
        public int TotalFacturasHoy => ResumenHoy?.TotalFacturas ?? 0;

        /// <summary>
        /// Total de ventas del mes formateado
        /// </summary>
        public string TotalVentasMesFormateado => ResumenMes?.TotalVentasFormateado ?? "$0";

        /// <summary>
        /// Total de facturas del mes
        /// </summary>
        public int TotalFacturasMes => ResumenMes?.TotalFacturas ?? 0;

        /// <summary>
        /// Total de ventas del mes en valor decimal
        /// </summary>
        public decimal TotalVentasMes => ResumenMes?.TotalVentas ?? 0;

        /// <summary>
        /// Total de descuentos del mes formateado
        /// </summary>
        public string TotalDescuentosMesFormateado => ResumenMes?.TotalDescuentosFormateado ?? "$0";

        /// <summary>
        /// Total de IVA del mes formateado
        /// </summary>
        public string TotalIVAMesFormateado => ResumenMes?.TotalIVAFormateado ?? "$0";

        /// <summary>
        /// Total de categorías con ventas
        /// </summary>
        public int TotalCategorias => VentasPorCategoria?.Count ?? 0;

        /// <summary>
        /// Categoría con mayor venta
        /// </summary>
        public string CategoriaMasVendida => VentasPorCategoria?.OrderByDescending(c => c.Monto).FirstOrDefault()?.Nombre ?? "N/A";

        /// <summary>
        /// Indica si hay datos de ventas por mes
        /// </summary>
        public bool TieneDatosVentasPorMes => VentasPorMes != null && VentasPorMes.Count > 0;

        /// <summary>
        /// Indica si hay artículos con stock bajo
        /// </summary>
        public bool TieneArticulosStockBajo => ArticulosStockBajo > 0;

        /// <summary>
        /// Porcentaje de facturas activas
        /// </summary>
        public int PorcentajeFacturasActivas
        {
            get
            {
                if (FacturasPorEstado == null || !FacturasPorEstado.Any())
                    return 0;

                int totalFacturas = FacturasPorEstado.Values.Sum();
                if (totalFacturas == 0)
                    return 0;

                int facturasActivas = FacturasPorEstado.TryGetValue("Activa", out int valor) ? valor : 0;
                return (int)Math.Round((double)facturasActivas / totalFacturas * 100);
            }
        }

        #endregion

        #region Constructores

        public DashboardViewModel()
        {
            // Inicializar con valores por defecto
            FechaGeneracion = DateTime.Now;
            ResumenHoy = new ResumenDiarioInfo();
            ResumenMes = new ResumenMensualInfo();
            VentasPorCategoria = new List<CategoriaVentasInfo>();
            ArticulosMasVendidos = new List<ArticuloMasVendidoInfo>();
            ClientesFrecuentes = new List<ClienteFrecuenteInfo>();
            ArticulosStockBajo = 0;
            FacturasPorEstado = new Dictionary<string, int>();
            VentasPorMes = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// Constructor que crea el ViewModel a partir de un DTO
        /// </summary>
        public DashboardViewModel(DashboardDto dto)
        {
            if (dto != null)
            {
                FechaGeneracion = dto.FechaGeneracion;

                // Mapear resumen diario
                if (dto.ResumenHoy != null)
                {
                    ResumenHoy = new ResumenDiarioInfo
                    {
                        Fecha = dto.ResumenHoy.Fecha,
                        TotalVentas = dto.ResumenHoy.TotalVentas,
                        TotalVentasFormateado = dto.ResumenHoy.TotalVentasFormateado,
                        TotalFacturas = dto.ResumenHoy.TotalFacturas
                    };
                }
                else
                {
                    ResumenHoy = new ResumenDiarioInfo();
                }

                // Mapear resumen mensual
                if (dto.ResumenMes != null)
                {
                    ResumenMes = new ResumenMensualInfo
                    {
                        FechaInicio = dto.ResumenMes.FechaInicio,
                        FechaFin = dto.ResumenMes.FechaFin,
                        TotalVentas = dto.ResumenMes.TotalVentas,
                        TotalVentasFormateado = dto.ResumenMes.TotalVentasFormateado,
                        TotalFacturas = dto.ResumenMes.TotalFacturas,
                        TotalIVA = dto.ResumenMes.TotalIVA,
                        TotalIVAFormateado = dto.ResumenMes.TotalIVAFormateado,
                        TotalDescuentos = dto.ResumenMes.TotalDescuentos,
                        TotalDescuentosFormateado = dto.ResumenMes.TotalDescuentosFormateado
                    };
                }
                else
                {
                    ResumenMes = new ResumenMensualInfo();
                }

                // Mapear ventas por categoría
                VentasPorCategoria = new List<CategoriaVentasInfo>();
                if (dto.VentasPorCategoria != null)
                {
                    foreach (var item in dto.VentasPorCategoria)
                    {
                        VentasPorCategoria.Add(new CategoriaVentasInfo
                        {
                            Nombre = item.Key,
                            Monto = item.Value,
                            MontoFormateado = CurrencyHelper.FormatCurrency(item.Value)
                        });
                    }
                }

                // Mapear artículos más vendidos
                ArticulosMasVendidos = new List<ArticuloMasVendidoInfo>();
                if (dto.ArticulosMasVendidos != null)
                {
                    foreach (var item in dto.ArticulosMasVendidos)
                    {
                        ArticulosMasVendidos.Add(new ArticuloMasVendidoInfo
                        {
                            ArticuloId = item.ArticuloId,
                            Codigo = item.Codigo,
                            Nombre = item.Nombre,
                            CantidadVendida = item.CantidadVendida,
                            MontoVendido = item.MontoVendido,
                            MontoVendidoFormateado = item.MontoVendidoFormateado,
                            VecesVendido = item.VecesVendido
                        });
                    }
                }

                // Mapear clientes frecuentes
                ClientesFrecuentes = new List<ClienteFrecuenteInfo>();
                if (dto.ClientesFrecuentes != null)
                {
                    foreach (var item in dto.ClientesFrecuentes)
                    {
                        ClientesFrecuentes.Add(new ClienteFrecuenteInfo
                        {
                            ClienteId = item.ClienteId,
                            NombreCompleto = item.NombreCompleto,
                            NumeroDocumento = item.NumeroDocumento,
                            TotalFacturas = item.TotalFacturas,
                            MontoTotalCompras = item.MontoTotalCompras,
                            MontoTotalComprasFormateado = item.MontoTotalComprasFormateado,
                            UltimaCompra = item.UltimaCompra
                        });
                    }
                }

                ArticulosStockBajo = dto.ArticulosStockBajo;
                FacturasPorEstado = dto.FacturasPorEstado != null ? new Dictionary<string, int>(dto.FacturasPorEstado) : new Dictionary<string, int>();
                VentasPorMes = dto.VentasPorMes != null ? new Dictionary<string, decimal>(dto.VentasPorMes) : new Dictionary<string, decimal>();
            }
            else
            {
                // Inicializar con valores vacíos si el DTO es nulo
                FechaGeneracion = DateTime.Now;
                ResumenHoy = new ResumenDiarioInfo();
                ResumenMes = new ResumenMensualInfo();
                VentasPorCategoria = new List<CategoriaVentasInfo>();
                ArticulosMasVendidos = new List<ArticuloMasVendidoInfo>();
                ClientesFrecuentes = new List<ClienteFrecuenteInfo>();
                ArticulosStockBajo = 0;
                FacturasPorEstado = new Dictionary<string, int>();
                VentasPorMes = new Dictionary<string, decimal>();
            }
        }

        #endregion

        #region Métodos para UI y gráficos

        /// <summary>
        /// Obtiene los datos para el gráfico de ventas por categoría
        /// </summary>
        public string GetVentasPorCategoriaChartData()
        {
            if (VentasPorCategoria == null || !VentasPorCategoria.Any())
                return "[]";

            // Ordenar por monto descendente y tomar las top 5 categorías
            var topCategorias = VentasPorCategoria
                .OrderByDescending(c => c.Monto)
                .Take(5)
                .ToList();

            // Crear los datos para el gráfico
            var labels = topCategorias.Select(c => $"'{c.Nombre}'").ToList();
            var values = topCategorias.Select(c => c.Monto.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToList();

            return $"{{ labels: [{string.Join(",", labels)}], values: [{string.Join(",", values)}] }}";
        }

        /// <summary>
        /// Obtiene los datos para el gráfico de ventas por mes
        /// </summary>
        public string GetVentasPorMesChartData()
        {
            if (VentasPorMes == null || !VentasPorMes.Any())
                return "[]";

            // Ordenar los meses cronológicamente
            var meses = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            var datosOrdenados = new List<KeyValuePair<string, decimal>>();

            foreach (var mes in meses)
            {
                if (VentasPorMes.TryGetValue(mes, out decimal valor))
                {
                    datosOrdenados.Add(new KeyValuePair<string, decimal>(mes, valor));
                }
            }

            // Crear los datos para el gráfico
            var labels = datosOrdenados.Select(m => $"'{m.Key}'").ToList();
            var values = datosOrdenados.Select(m => m.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToList();

            return $"{{ labels: [{string.Join(",", labels)}], values: [{string.Join(",", values)}] }}";
        }

        /// <summary>
        /// Obtiene los datos para el gráfico de facturas por estado
        /// </summary>
        public string GetFacturasPorEstadoChartData()
        {
            if (FacturasPorEstado == null || !FacturasPorEstado.Any())
                return "[]";

            // Crear los datos para el gráfico
            var labels = FacturasPorEstado.Keys.Select(k => $"'{k}'").ToList();
            var values = FacturasPorEstado.Values.Select(v => v.ToString()).ToList();

            return $"{{ labels: [{string.Join(",", labels)}], values: [{string.Join(",", values)}] }}";
        }

        /// <summary>
        /// Obtiene el valor total de los artículos más vendidos
        /// </summary>
        public decimal GetTotalArticulosMasVendidos()
        {
            return ArticulosMasVendidos?.Sum(a => a.MontoVendido) ?? 0;
        }

        /// <summary>
        /// Obtiene el valor total de los artículos más vendidos formateado
        /// </summary>
        public string GetTotalArticulosMasVendidosFormateado()
        {
            return CurrencyHelper.FormatCurrency(GetTotalArticulosMasVendidos());
        }

        /// <summary>
        /// Obtiene el total de unidades vendidas de los artículos más vendidos
        /// </summary>
        public int GetTotalUnidadesVendidas()
        {
            return ArticulosMasVendidos?.Sum(a => a.CantidadVendida) ?? 0;
        }

        /// <summary>
        /// Obtiene el monto total de compras de los clientes frecuentes
        /// </summary>
        public decimal GetTotalComprasClientesFrecuentes()
        {
            return ClientesFrecuentes?.Sum(c => c.MontoTotalCompras) ?? 0;
        }

        /// <summary>
        /// Obtiene el monto total de compras de los clientes frecuentes formateado
        /// </summary>
        public string GetTotalComprasClientesFrecuentesFormateado()
        {
            return CurrencyHelper.FormatCurrency(GetTotalComprasClientesFrecuentes());
        }

        #endregion

        #region Clases internas para agrupación de datos

        /// <summary>
        /// Información de resumen diario
        /// </summary>
        public class ResumenDiarioInfo
        {
            public DateTime Fecha { get; set; }
            public decimal TotalVentas { get; set; }
            public string TotalVentasFormateado { get; set; }
            public int TotalFacturas { get; set; }

            public ResumenDiarioInfo()
            {
                Fecha = DateTime.Now.Date;
                TotalVentas = 0;
                TotalVentasFormateado = "$0";
                TotalFacturas = 0;
            }
        }

        /// <summary>
        /// Información de resumen mensual
        /// </summary>
        public class ResumenMensualInfo
        {
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public decimal TotalVentas { get; set; }
            public string TotalVentasFormateado { get; set; }
            public int TotalFacturas { get; set; }
            public decimal TotalIVA { get; set; }
            public string TotalIVAFormateado { get; set; }
            public decimal TotalDescuentos { get; set; }
            public string TotalDescuentosFormateado { get; set; }

            public ResumenMensualInfo()
            {
                DateTime hoy = DateTime.Now.Date;
                FechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
                FechaFin = FechaInicio.AddMonths(1).AddDays(-1);
                TotalVentas = 0;
                TotalVentasFormateado = "$0";
                TotalFacturas = 0;
                TotalIVA = 0;
                TotalIVAFormateado = "$0";
                TotalDescuentos = 0;
                TotalDescuentosFormateado = "$0";
            }
        }

        /// <summary>
        /// Información de ventas por categoría
        /// </summary>
        public class CategoriaVentasInfo
        {
            public string Nombre { get; set; }
            public decimal Monto { get; set; }
            public string MontoFormateado { get; set; }

            public CategoriaVentasInfo()
            {
                Nombre = string.Empty;
                Monto = 0;
                MontoFormateado = "$0";
            }
        }

        /// <summary>
        /// Información de artículo más vendido
        /// </summary>
        public class ArticuloMasVendidoInfo
        {
            public int ArticuloId { get; set; }
            public string Codigo { get; set; }
            public string Nombre { get; set; }
            public int CantidadVendida { get; set; }
            public decimal MontoVendido { get; set; }
            public string MontoVendidoFormateado { get; set; }
            public int VecesVendido { get; set; }

            public ArticuloMasVendidoInfo()
            {
                ArticuloId = 0;
                Codigo = string.Empty;
                Nombre = string.Empty;
                CantidadVendida = 0;
                MontoVendido = 0;
                MontoVendidoFormateado = "$0";
                VecesVendido = 0;
            }
        }

        /// <summary>
        /// Información de cliente frecuente
        /// </summary>
        public class ClienteFrecuenteInfo
        {
            public int ClienteId { get; set; }
            public string NombreCompleto { get; set; }
            public string NumeroDocumento { get; set; }
            public int TotalFacturas { get; set; }
            public decimal MontoTotalCompras { get; set; }
            public string MontoTotalComprasFormateado { get; set; }
            public DateTime? UltimaCompra { get; set; }
            public string UltimaCompraFormateada => UltimaCompra.HasValue ? UltimaCompra.Value.ToString("dd/MM/yyyy") : "N/A";

            public ClienteFrecuenteInfo()
            {
                ClienteId = 0;
                NombreCompleto = string.Empty;
                NumeroDocumento = string.Empty;
                TotalFacturas = 0;
                MontoTotalCompras = 0;
                MontoTotalComprasFormateado = "$0";
            }
        }

        #endregion
    }
}