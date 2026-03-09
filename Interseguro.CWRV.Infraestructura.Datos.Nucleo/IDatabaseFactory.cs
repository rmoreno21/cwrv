using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Infraestructura.Datos.Nucleo
{
    public interface IDatabaseFactory : IDisposable
    {
        CWRVDataContext Get();
    }
}
