using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriasController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public CategoriasController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Categoria categoria = new Categoria();

            if (id == null)
            {
                return View(categoria);
            }

            categoria = _unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            if(categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if(categoria.Id == 0)
                {
                    _unidadTrabajo.Categoria.Agregar(categoria);
                }
                else
                {
                    _unidadTrabajo.Categoria.Actualizar(categoria);
                }

                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        #region API
        [HttpGet]
        public IActionResult ObternerTodos()
        {
            var todos = _unidadTrabajo.Categoria.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoriaDb = _unidadTrabajo.Categoria.Obtener(id);

            if (categoriaDb == null)
            {
                return Json(new { success = false, message = "Error, no se encontro la Categoria." });
            }
            _unidadTrabajo.Categoria.Eliminar(categoriaDb.Id);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria borrada exitosamente" });
        }
        #endregion
    }
}
