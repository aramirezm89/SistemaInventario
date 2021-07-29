using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    class OrdenRepositorio : Repositorio<Orden>, IOrdenRepositorio
    {
        public readonly ApplicationDbContext _db;
        public OrdenRepositorio(ApplicationDbContext db) : base (db)
        {
            _db = db;
        }
        public void Actualizar(Orden orden)
        {
            _db.Update(orden);
            
        }
    }
}
