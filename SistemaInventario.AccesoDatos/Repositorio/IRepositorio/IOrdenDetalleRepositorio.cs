using SistemaInventario.Modelos;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface IOrdenDetalleRepositorio : IRepositorio<OrdenDetalle>
    {
        void Actualizar(OrdenDetalle ordenDetalle);
    }
}
