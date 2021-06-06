using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        T Obtener(int id);

        IEnumerable<T> ObtenerTodos(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string incluirPropiedades = null
            );
        T ObtenerPrimerElemento(
           Expression<Func<T, bool>> filter = null,
           string incluirPropiedades = null
           );
        void Agregar(T entidad);
        void Eliminar(int id);
        void Eliminar(T entidad);
        void EliminarRango(IEnumerable<T> entidad);
    }
}
