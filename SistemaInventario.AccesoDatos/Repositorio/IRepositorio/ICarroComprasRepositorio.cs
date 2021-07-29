using SistemaInventario.Modelos;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface ICarroComprasRepositorio : IRepositorio<CarroCompras>
    {
        void Actualizar(CarroCompras carroCompras);
    }
}
