using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    class RepositorioUsuarioContactado: IRepositorioUsuario
    {

        public void Registrar(Usuario entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Usuario entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Usuario entity)
        {
            throw new NotImplementedException();
        }

        public Usuario ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Usuario ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Usuario> Listar()
        {
            throw new NotImplementedException();
        }

        public List<Usuario> ListarPorNombres(string nombres)
        {
            throw new NotImplementedException();
        }

        public string obtenerNumAgente(string nombreUsuario)
        {
            throw new NotImplementedException();
        }

        public List<Usuario> Listar(string nombreUsuario, string idAgente)
        {
            throw new NotImplementedException();
        }
    }
}
