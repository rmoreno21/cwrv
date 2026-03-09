using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Interseguro.CWRV.ServiciosAcceso
{
    [ServiceContract(Namespace = "JsonpAjaxService")]
    interface IServicioAccesoCWRV
    {
        //[OperationContract]
        //[WebInvoke(
        //    Method = "POST",
        //    BodyStyle = WebMessageBodyStyle.Wrapped,
        //    ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        Acceso SolicitarAcceso(string usuario, string cuspp);
    }
}
