using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SistemaInventario.AccesoDatos.Data;
using Microsoft.EntityFrameworkCore;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {

        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repositorio(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public void Agregar(T entidad)
        {
            dbSet.Add(entidad); // insert into table
        }

        public void Eliminar(int id)
        {
            T entidad = dbSet.Find(id);
            Eliminar(entidad);
        }

        public void Eliminar(T entidad)
        {
            dbSet.Remove(entidad); // delete from 
        }

        public void EliminarRango(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }

        public T Obtener(int id)
        {
            return dbSet.Find(id); // select * from where ID = ID
        }

        public T ObtenerPrimerElemento(Expression<Func<T, bool>> filter = null, string incluirPropiedades = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter); // select * from where ....
            }

            if (incluirPropiedades != null)
            {
                foreach (var propiedad in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(propiedad);
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> ObtenerTodos(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null)
        {
            IQueryable<T> query = dbSet;
           
            if(filter != null)
            {
                query = query.Where(filter); // select * from where ....
            }

            if(incluirPropiedades != null)
            {
                foreach (var propiedad in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(propiedad);
                }
            }

            if(orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();
        }

       
    }
}
