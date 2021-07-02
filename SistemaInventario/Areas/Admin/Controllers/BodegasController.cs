using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.roleAdmin)]
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
                return View(bodega);
            }

            // Actualizar 
            bodega = _unidadTrabajo.Bodega.Obtener(id.GetValueOrDefault());
            if (bodega == null)
            {
                return NotFound();
            }
            return View(bodega);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Bodega bodega)
        {
            if (ModelState.IsValid)
            {
                if (bodega.Id == 0)
                {
                    _unidadTrabajo.Bodega.Agregar(bodega);
                }
                else
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
                }

                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
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

        [HttpDelete]

        public IActionResult Delete(int id)
        {
            var bodegaDb = _unidadTrabajo.Bodega.Obtener(id);

            if (bodegaDb == null)
            {
                return Json(new { success = false, message = "Error, no se encontro la Bodega." });
            }
            _unidadTrabajo.Bodega.Eliminar(bodegaDb.Id);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Bodega borrada exitosamente" });
        }
        #endregion
    }
}
