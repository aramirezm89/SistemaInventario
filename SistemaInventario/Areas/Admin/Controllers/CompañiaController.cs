using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System;
using System.IO;
using System.Linq;

namespace SistemaInventario.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = DS.roleAdmin)]
    public class CompañiaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _hostEnviroment; //variable para carga de imagen

        public CompañiaController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment hostEnviroment)
        {
            _unidadTrabajo = unidadTrabajo;
            _hostEnviroment = hostEnviroment;
        }
        public IActionResult Index()
        {
            var compañia = _unidadTrabajo.Compañia.ObtenerTodos();
            return View(compañia);
        }

        public IActionResult Upsert(int? id)
        {
            CompañiaVM compañiaVM = new CompañiaVM()
            {

                Compañia = new Compañia(),
                BodegaLista = _unidadTrabajo.Bodega.ObtenerTodos().Select(bodega => new SelectListItem
                {
                    Text = bodega.Nombre,
                    Value = bodega.Id.ToString()

                })
            };

            if (id == null)
            {
                return View(compañiaVM);
            }

            compañiaVM.Compañia = _unidadTrabajo.Compañia.Obtener(id.GetValueOrDefault());
            if (compañiaVM.Compañia == null)
            {
                return NotFound();
            }

            return View( compañiaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CompañiaVM compañiaVM)
        {
            if (ModelState.IsValid)
            {

                //Cargar Imagenes

                string webRootPath = _hostEnviroment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"imagenes\compañia");
                    var extension = Path.GetExtension(files[0].FileName);

                    if (compañiaVM.Compañia.logoUrl != null)
                    {
                        //Esto es para editar, necesitamos borrar la imagen anterior.
                        var imagenPath = Path.Combine(webRootPath, compañiaVM.Compañia.logoUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagenPath))
                        {
                            System.IO.File.Delete(imagenPath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }

                    compañiaVM.Compañia.logoUrl = @"\imagenes\compañia\" + filename + extension;
                }
                else
                {
                    // Update sin cambio de imagen por parte del usuario

                    if (compañiaVM.Compañia.Id != 0)
                    {
                         Compañia compañiaDB = _unidadTrabajo.Compañia.Obtener(compañiaVM.Compañia.Id);
                        compañiaVM.Compañia.logoUrl = compañiaDB.logoUrl;
                    }
                }


                if (compañiaVM.Compañia.Id == 0)
                {
                    _unidadTrabajo.Compañia.Agregar(compañiaVM.Compañia);
                }
                else
                {
                    _unidadTrabajo.Compañia.Actualizar(compañiaVM.Compañia);
                }

                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                compañiaVM.BodegaLista = _unidadTrabajo.Bodega.ObtenerTodos().Select(bodega => new SelectListItem
                {
                    Text = bodega.Nombre,
                    Value = bodega.Id.ToString()

                });
               

                if (compañiaVM.Compañia.Id != 0)
                {
                    compañiaVM.Compañia = _unidadTrabajo.Compañia.Obtener(compañiaVM.Compañia.Id);
                }
            }

            return View(compañiaVM.Compañia);
        }

        #region API
        [HttpGet]
        public IActionResult ObternerTodos()
        {
            var todos = _unidadTrabajo.Compañia.ObtenerTodos(incluirPropiedades: "Bodega");
            return Json(new { data = todos });
        }

        //[HttpDelete]
        //public IActionResult Delete(int id)
        //{
        //    var compañiaDb = _unidadTrabajo.Compañia.Obtener(id);

        //    if (compañiaDb == null)
        //    {
        //        return Json(new { success = false, message = "Error, no se encontro la Compañia." });
        //    }

        //    //eliminar imagen
        //    string webRootPath = _hostEnviroment.WebRootPath;
        //    var imagenPath = Path.Combine(webRootPath, compañiaDb.logoUrl.TrimStart('\\'));
        //    if (System.IO.File.Exists(imagenPath))
        //    {
        //        System.IO.File.Delete(imagenPath);
        //    }

        //    _unidadTrabajo.Compañia.Eliminar(compañiaDb.Id);
        //    _unidadTrabajo.Guardar();
        //    return Json(new { success = true, message = "Compañia borrada exitosamente" });
        //}
        #endregion

    }
}
