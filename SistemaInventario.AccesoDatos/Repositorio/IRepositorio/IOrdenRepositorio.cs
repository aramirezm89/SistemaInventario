using SistemaInventario.Modelos;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface IOrdenRepositorio : IRepositorio<Orden>
    {
        void Actualizar(Orden orden);
    }
}
