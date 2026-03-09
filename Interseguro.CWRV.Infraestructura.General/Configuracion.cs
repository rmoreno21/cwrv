using System;
using System.Collections;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Collections.Generic;
using Interseguro.CWRV.Infraestructura.General.Configuracion;

namespace Interseguro.CWRV.Infraestructura.General
{
    #region Cotizaciones

    public class SeccionCotizaciones : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public ColeccionCotizaciones Cotizaciones
        {
            get { return (ColeccionCotizaciones)this[""]; }
            set { this[""] = value; }
        }
    }

    public class ColeccionCotizaciones : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ElementoCotizacion();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ElementoCotizacion)element).Nro;
        }
    }

    public class ElementoCotizacion : ConfigurationElement
    {
        [ConfigurationProperty("nro", IsKey = true, IsRequired = true)]
        public string Nro
        {
            get { return (string)base["nro"]; }
            set { base["nro"] = value; }
        }

        [ConfigurationProperty("moneda", IsRequired = true)]
        public string Moneda
        {
            get { return (string)base["moneda"]; }
            set { base["moneda"] = value; }
        }

        [ConfigurationProperty("producto", IsRequired = true)]
        public string Producto
        {
            get { return (string)base["producto"]; }
            set { base["producto"] = value; }
        }

        [ConfigurationProperty("modalidad", IsRequired = true)]
        public string Modalidad
        {
            get { return (string)base["modalidad"]; }
            set { base["modalidad"] = value; }
        }

        [ConfigurationProperty("periodoDiferido", IsRequired = true)]
        public string PeriodoDiferido
        {
            get { return (string)base["periodoDiferido"]; }
            set { base["periodoDiferido"] = value; }
        }

        [ConfigurationProperty("pjeEntreRentras", IsRequired = true)]
        public string PjeEntreRentras
        {
            get { return (string)base["pjeEntreRentras"]; }
            set { base["pjeEntreRentras"] = value; }
        }

        [ConfigurationProperty("periodoGarantizado", IsRequired = true)]
        public string PeriodoGarantizado
        {
            get { return (string)base["periodoGarantizado"]; }
            set { base["periodoGarantizado"] = value; }
        }

        [ConfigurationProperty("gratificacion", IsRequired = true)]
        public string Gratificacion
        {
            get { return (string)base["gratificacion"]; }
            set { base["gratificacion"] = value; }
        }

        [ConfigurationProperty("capital", IsRequired = true)]
        public string Capital
        {
            get { return (string)base["capital"]; }
            set { base["capital"] = value; }
        }
    }

    #endregion Cotizaciones

    #region Correo

    public class SeccionCorreo : ConfigurationSection
    {
        [ConfigurationProperty("asunto")]
        public ElementoAsunto Asunto
        {
            get { return (ElementoAsunto)this["asunto"]; }
            set { this["asunto"] = value; }
        }

        [ConfigurationProperty("mensaje")]
        public ElementoMensaje Mensaje
        {
            get { return (ElementoMensaje)this["mensaje"]; }
            set { this["mensaje"] = value; }
        }

        //<SRI.INI-20322>
        [ConfigurationProperty("mensajesimulacion")]
        public ElementoMensajeSimulacion MensajeSimulacion
        {
            get { return (ElementoMensajeSimulacion)this["mensajesimulacion"]; }
            set { this["mensajesimulacion"] = value; }
        }

        [ConfigurationProperty("asuntoParametrosEspeciales")]
        public ElementoAsuntoParametrosEspeciales AsuntoParametrosEspeciales
        {
            get { return (ElementoAsuntoParametrosEspeciales)this["asuntoParametrosEspeciales"]; }
            set { this["asuntoParametrosEspeciales"] = value; }
        }

        [ConfigurationProperty("mensajeParametrosEspeciales")]
        public ElementoMensajeParametrosEspeciales MensajeParametrosEspeciales
        {
            get { return (ElementoMensajeParametrosEspeciales)this["mensajeParametrosEspeciales"]; }
            set { this["mensajeParametrosEspeciales"] = value; }
        }

        //<SRI.FIN-20322>

        //<INIGTI_4081>
        [ConfigurationProperty("asuntoGestionVentas")]
        public ElementoAsuntoGestionVentas AsuntoGestionVentas
        {
            get { return (ElementoAsuntoGestionVentas)this["asuntoGestionVentas"]; }
            set { this["asuntoGestionVentas"] = value; }
        }

        [ConfigurationProperty("mensajeGestionVentas")]
        public ElementoMensajeGestionVentas MensajeGestionVentas
        {
            get { return (ElementoMensajeGestionVentas)this["mensajeGestionVentas"]; }
            set { this["mensajeGestionVentas"] = value; }
        }
        //<FINGTI_4081>
    }

    public class ElementoAsunto : ConfigurationElement
    {
        [ConfigurationProperty("texto", IsRequired = true)]
        public String Texto
        {
            get { return (String)this["texto"]; }
            set { this["texto"] = value; }
        }
    }

    public class ElementoMensaje : CDataConfigurationElement
    {
        [ConfigurationProperty("texto", IsRequired = true, IsKey = true)]
        [CDataConfigurationProperty]
        public string Texto
        {
            get { return (string)(base["texto"]); }
            set { base["texto"] = value; }
        }
    }

    //<SRI.INI-20322>
    public class ElementoMensajeSimulacion : CDataConfigurationElement
    {
        [ConfigurationProperty("texto", IsRequired = true, IsKey = true)]
        [CDataConfigurationProperty]
        public string Texto
        {
            get { return (string)(base["texto"]); }
            set { base["texto"] = value; }
        }
    }

    public class ElementoAsuntoParametrosEspeciales : ConfigurationElement
    {
        [ConfigurationProperty("texto", IsRequired = true)]
        public String Texto
        {
            get { return (String)this["texto"]; }
            set { this["texto"] = value; }
        }
    }

    public class ElementoMensajeParametrosEspeciales : CDataConfigurationElement
    {
        [ConfigurationProperty("texto", IsRequired = true, IsKey = true)]
        [CDataConfigurationProperty]
        public string Texto
        {
            get { return (string)(base["texto"]); }
            set { base["texto"] = value; }
        }
    }
    //<INIGTI_4081>

    public class ElementoAsuntoGestionVentas : ConfigurationElement
    {
        [ConfigurationProperty("texto", IsRequired = true)]
        public String Texto
        {
            get { return (String)this["texto"]; }
            set { this["texto"] = value; }
        }
    }

    public class ElementoMensajeGestionVentas : CDataConfigurationElement
    {
        [ConfigurationProperty("texto", IsRequired = true, IsKey = true)]
        [CDataConfigurationProperty]
        public string Texto
        {
            get { return (string)(base["texto"]); }
            set { base["texto"] = value; }
        }
    }
    //<FINGTI_4081>

    //<SRI.FIN-20322>

    #endregion







    public class PageAppearanceSection : ConfigurationSection
    {
        // Create a "remoteOnly" attribute.
        [ConfigurationProperty("remoteOnly", DefaultValue = "false", IsRequired = false)]
        public Boolean RemoteOnly
        {
            get { return (Boolean)this["remoteOnly"]; }
            set { this["remoteOnly"] = value; }
        }

        // Create a "font" element.
        [ConfigurationProperty("font")]
        public FontElement Font
        {
            get
            {
                return (FontElement)this["font"];
            }
            set
            { this["font"] = value; }
        }

        // Create a "color element."
        [ConfigurationProperty("color")]
        public ColorElement Color
        {
            get
            {
                return (ColorElement)this["color"];
            }
            set
            { this["color"] = value; }
        }
    }

    // Define the "font" element
    // with "name" and "size" attributes.
    public class FontElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "Arial", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Name
        {
            get
            {
                return (String)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("size", DefaultValue = "12", IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 24, MinValue = 6)]
        public int Size
        {
            get
            { return (int)this["size"]; }
            set
            { this["size"] = value; }
        }
    }

    // Define the "color" element 
    // with "background" and "foreground" attributes.
    public class ColorElement : ConfigurationElement
    {
        [ConfigurationProperty("background", DefaultValue = "FFFFFF", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\GHIJKLMNOPQRSTUVWXYZ", MinLength = 6, MaxLength = 6)]
        public String Background
        {
            get
            {
                return (String)this["background"];
            }
            set
            {
                this["background"] = value;
            }
        }

        [ConfigurationProperty("foreground", DefaultValue = "000000", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\GHIJKLMNOPQRSTUVWXYZ", MinLength = 6, MaxLength = 6)]
        public String Foreground
        {
            get
            {
                return (String)this["foreground"];
            }
            set
            {
                this["foreground"] = value;
            }
        }

    }



//<SOLINI26593>
    #region CotizacionesRP

    public class SeccionCotizacionesRP : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public ColeccionCotizacionesRP CotizacionesRP
        {
            get { return (ColeccionCotizacionesRP)this[""]; }
            set { this[""] = value; }
        }
    }

    public class ColeccionCotizacionesRP : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ElementoCotizacionRP();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ElementoCotizacionRP)element).Nro;
        }
    }

    public class ElementoCotizacionRP : ConfigurationElement
    {
        [ConfigurationProperty("nro", IsKey = true, IsRequired = true)]
        public string Nro
        {
            get { return (string)base["nro"]; }
            set { base["nro"] = value; }
        }

        [ConfigurationProperty("moneda", IsRequired = true)]
        public string Moneda
        {
            get { return (string)base["moneda"]; }
            set { base["moneda"] = value; }
        }

        //[ConfigurationProperty("producto", IsRequired = true)]
        //public string Producto
        //{
        //    get { return (string)base["producto"]; }
        //    set { base["producto"] = value; }
        //}

        //[ConfigurationProperty("modalidad", IsRequired = true)]
        //public string Modalidad
        //{
        //    get { return (string)base["modalidad"]; }
        //    set { base["modalidad"] = value; }
        //}

        //[ConfigurationProperty("periodoDiferido", IsRequired = true)]
        //public string PeriodoDiferido
        //{
        //    get { return (string)base["periodoDiferido"]; }
        //    set { base["periodoDiferido"] = value; }
        //}

        //[ConfigurationProperty("pjeEntreRentras", IsRequired = true)]
        //public string PjeEntreRentras
        //{
        //    get { return (string)base["pjeEntreRentras"]; }
        //    set { base["pjeEntreRentras"] = value; }
        //}

        [ConfigurationProperty("periodoGarantizado", IsRequired = true)]
        public string PeriodoGarantizado
        {
            get { return (string)base["periodoGarantizado"]; }
            set { base["periodoGarantizado"] = value; }
        }

        //[ConfigurationProperty("gratificacion", IsRequired = true)]
        //public string Gratificacion
        //{
        //    get { return (string)base["gratificacion"]; }
        //    set { base["gratificacion"] = value; }
        //}

        //[ConfigurationProperty("capital", IsRequired = true)]
        //public string Capital
        //{
        //    get { return (string)base["capital"]; }
        //    set { base["capital"] = value; }
        //}

        //[ConfigurationProperty("monedaFondo", IsRequired = true)]
        //public string MonedaFondo
        //{
        //    get { return (string)base["monedaFondo"]; }
        //    set { base["monedaFondo"] = value; }
        //}
    }

    #endregion CotizacionesRP
//<SOLFIN26593>


//<INIGTI_753>
    #region CotizacionesRPPlus

    public class SeccionCotizacionesRPPlus : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public ColeccionCotizacionesRPPlus CotizacionesRPPlus
        {
            get { return (ColeccionCotizacionesRPPlus)this[""]; }
            set { this[""] = value; }
        }
    }

    public class ColeccionCotizacionesRPPlus : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ElementoCotizacionRPPlus();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ElementoCotizacionRPPlus)element).Nro;
        }
    }

    public class ElementoCotizacionRPPlus : ConfigurationElement
    {
        [ConfigurationProperty("nro", IsKey = true, IsRequired = true)]
        public string Nro
        {
            get { return (string)base["nro"]; }
            set { base["nro"] = value; }
        }

        [ConfigurationProperty("moneda", IsRequired = true)]
        public string Moneda
        {
            get { return (string)base["moneda"]; }
            set { base["moneda"] = value; }
        }

        [ConfigurationProperty("periodoGarantizado", IsRequired = true)]
        public string PeriodoGarantizado
        {
            get { return (string)base["periodoGarantizado"]; }
            set { base["periodoGarantizado"] = value; }
        }
    }

    #endregion CotizacionesRP
//<FINGTI_753>
}
