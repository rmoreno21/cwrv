using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Interseguro.CWRV.Infraestructura.Transversal
{
    public class HttpEventViewer
    {
        public void GenerarEventos(string aplicativo, string mensajeError)
        {
            string log = "Application";

            if (!EventLog.SourceExists(aplicativo))
                EventLog.CreateEventSource(aplicativo, log);

            EventLog.WriteEntry(aplicativo, mensajeError);
        }
    }
}
