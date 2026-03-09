using System;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class Respuesta
    {
        public string estado { get; set; }
        public string titulo { get; set; }
        public string mensaje { get; set; }
        public DateTime fecha_hora { get; set; }
    }
}
