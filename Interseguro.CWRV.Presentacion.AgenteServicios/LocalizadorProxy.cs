using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloSeguridad;

namespace Interseguro.CWRV.Presentacion.AgenteServicios
{
    public class LocalizadorProxy
    {
        public static IServicioCWRV ObtenerServicio()
        {
            IServicioCWRV iServicioCWRV = new ServicioCWRVClient("EPCWRV");
            return iServicioCWRV;
        }

        public static IServicioAzman ObtenerServicioSeguridad()
        {
            IServicioAzman iServicioAzman = new ServicioAzmanClient("epAzman");
            return iServicioAzman;
        }

        
    }
}
