using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    class CarroComprasRepositorio : Repositorio<CarroCompras>, ICarroComprasRepositorio
    {
        public readonly ApplicationDbContext _db;
        public CarroComprasRepositorio(ApplicationDbContext db) : base (db)
        {
            _db = db;
        }
        public void Actualizar(CarroCompras carroCompras)
        {
            _db.Update(carroCompras);
            
        }
    }
}
