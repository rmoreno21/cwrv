using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Interseguro.CWRV.Dominio
{
public interface IRepositorio<T> where T : class
{
    void Registrar(T entity);
    void Actualizar(T entity);
    void Eliminar(T entity);
    T ObtenerPorId(long Id);
    T ObtenerPorId(string Id);
    List<T> Listar();
}
}
