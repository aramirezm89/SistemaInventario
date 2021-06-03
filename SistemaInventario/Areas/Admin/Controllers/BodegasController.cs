using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BodegasController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public BodegasController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Bodega bodega = new Bodega();
            if (id == null)
            {
                //id null vista crear  nuevo usuario
                return View();
            }

            // Actualizar 
            bodega = _unidadTrabajo.Bodega.Obtener(id.GetValueOrDefault());
            if(bodega == null)
            {
                return NotFound();
            }
            return View(bodega);
        }

        #region API
        [HttpGet]
        public IActionResult ObternerTodos()
        {
            var todos = _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new { data = todos });
        }
        #endregion
    }
}
