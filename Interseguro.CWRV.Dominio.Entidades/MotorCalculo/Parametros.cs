using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class Parametros
    {
        public string tipo_calculo { get; set; }
        public string tipo_producto { get; set; }
        public DateTime fec_per_sistema { get; set; }
        public Cotizacion cotizacion { get; set; }
        public Poliza poliza { get; set; }
        public List<TablaMortalidad> tablas_mortalidad { get; set; }
        public List<FactorMejora> factores_mejoras { get; set; }
        public List<InformacionFactorMejora> informacion_factores_mejoras { get; set; }
        public List<TablaIcob> icob_nominales { get; set; }
        public List<TablaIcob> icob_ajustados { get; set; }
        public List<InversionTemporal> inversiones_temporales { get; set; }
        public List<InversionVitalicia> inversiones_vitalicias { get; set; }
        public List<FactorAntiSeleccion> factores_anti_seleccion { get; set; }
        public List<FactorPUMI> factores_pumi { get; set; }
        public List<RendimientosInversionTemporal> rendimientos_inversiones_temporales { get; set; }
        public List<TasaDuration> tasa_duration { get; set; }
        public List<SpreadRescate> spread_rescate { get; set; }
        public string usuario { get; set; }
        public List<TablaVTD> lista_VTD { get; set; }
        public List<TablaVOLA> lista_VOLA { get; set; }
        public bool isLogCotizacion { get; set; }
        public bool isLogReserva { get; set; }

        public T Clonar<T>()
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Position = 0;
                return (T)bf.Deserialize(ms);
            }
        }
    }
}