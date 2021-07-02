using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class UsuarioAplicacionRepositorio : Repositorio<UsuaioAplicacion>, IUsuarioAplicacionRepositorio
    {

        private readonly ApplicationDbContext _db;
        public UsuarioAplicacionRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

    }
}
