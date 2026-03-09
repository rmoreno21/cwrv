using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interseguro.CWRV.Aplicacion.ModuloPrincipal
{
    public interface ISeguridadServicio
    {
        Acceso SolicitarAcceso(SolicitudAcceso solicitud);
        SolicitudAcceso ValidarToken(string token, string ip);
        void ActualizarSolicitudAcceso(SolicitudAcceso solicitud);

    }
}
