using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SistemaInventario.Modelos.ViewModels
{
    public class InventarioVM
    {
        public Inventario Inventario { get; set; }
        public InventarioDetalle InventarioDetalle { get; set; }
        public List<InventarioDetalle> InventarioDetalles { get; set; }
        public IEnumerable<SelectListItem> BodegaLista { get; set; }
        public IEnumerable<SelectListItem> ProductoLista { get; set; }
    }
}
