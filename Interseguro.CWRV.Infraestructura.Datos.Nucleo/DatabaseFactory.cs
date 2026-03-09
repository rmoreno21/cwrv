using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Infraestructura.Datos.Nucleo
{
public class DatabaseFactory : Disposable, IDatabaseFactory
{
    private CWRVDataContext dataContext;
    public CWRVDataContext Get()
    {
        return dataContext ?? (dataContext = new CWRVDataContext());
    }
    protected override void DisposeCore()
    {
        if (dataContext != null)
            dataContext.Dispose();
    }
}
}
