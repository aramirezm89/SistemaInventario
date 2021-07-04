using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class CompañiaRepositorio : Repositorio<Compañia>, ICompañiaRepositorio
    {
        private readonly ApplicationDbContext _db;
        public CompañiaRepositorio(ApplicationDbContext db) : base (db)
        {
            _db = db;
        }
        public void Actualizar(Compañia compañia)
        {
            var compañiaDb = _db.Compañia.FirstOrDefault(c => c.Id == compañia.Id);

            if(compañiaDb != null)
            {
                if(compañiaDb.logoUrl != null)
                {
                    compañiaDb.logoUrl = compañia.logoUrl;
                }
                compañiaDb.Nombre = compañia.Nombre;
                compañiaDb.Descripcion = compañia.Descripcion;
                compañiaDb.Direccion = compañia.Direccion;
                compañiaDb.BodegaVentaId = compañia.BodegaVentaId;
                compañiaDb.Pais = compañia.Pais;
                compañiaDb.Ciudad = compañia.Ciudad;
                compañiaDb.Telefono = compañia.Telefono;
            }
        }
    }
}
