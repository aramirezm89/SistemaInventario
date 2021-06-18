using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    public class ProductosController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _hostEnviroment; //variable para carga de imagen

        public ProductosController(IUnidadTrabajo unidadTrabajo , IWebHostEnvironment hostEnviroment)
        {
            _unidadTrabajo = unidadTrabajo;
            _hostEnviroment = hostEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM() {

                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(categoria => new SelectListItem
                {
                    Text = categoria.Nombre,
                    Value = categoria.Id.ToString()

                }),
                MarcaLista = _unidadTrabajo.Marca.ObtenerTodos().Select(marca => new SelectListItem
                {
                    Text = marca.Nombre,
                    Value = marca.Id.ToString()
                }),
                PadreLista = _unidadTrabajo.Producto.ObtenerTodos().Select(producto => new SelectListItem
                {
                    Text = producto.Descripcion,
                    Value = producto.Id.ToString()
                }) 
            };

            if (id == null)
            {
                return View(productoVM);
            }

            productoVM.Producto = _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());
            if (productoVM.Producto == null)
            {
                return NotFound();
            }

            return View(productoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {

                //Cargar Imagenes

                string webRootPath = _hostEnviroment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if(files.Count > 0)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"imagenes\productos");
                    var extension = Path.GetExtension(files[0].FileName);
                    
                    if(productoVM.Producto.ImagenUrl != null)
                    {
                        //Esto es para editar, necesitamos borrar la imagen anterior.
                        var imagenPath = Path.Combine(webRootPath, productoVM.Producto.ImagenUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagenPath))
                        {
                            System.IO.File.Delete(imagenPath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }

                    productoVM.Producto.ImagenUrl = @"\imagenes\productos\" + filename + extension;
                }
                else
                {
                    // Update sin cambio de imagen por parte del usuario

                    if (productoVM.Producto.Id != 0)
                    {
                        Producto productoDb = _unidadTrabajo.Producto.Obtener(productoVM.Producto.Id);
                        productoVM.Producto.ImagenUrl = productoDb.ImagenUrl;
                    }
                }


                if (productoVM.Producto.Id == 0)
                {
                    _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }

                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productoVM.CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(categoria => new SelectListItem
                {
                    Text = categoria.Nombre,
                    Value = categoria.Id.ToString()

                });
                productoVM.MarcaLista = _unidadTrabajo.Marca.ObtenerTodos().Select(marca => new SelectListItem
                {
                    Text = marca.Nombre,
                    Value = marca.Id.ToString()
                });

                productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodos().Select(producto => new SelectListItem
                {
                    Text = producto.Descripcion,
                    Value = producto.Id.ToString()

                }); ;

                if(productoVM.Producto.Id != 0)
                {
                    productoVM.Producto = _unidadTrabajo.Producto.Obtener(productoVM.Producto.Id);
                }
            }

            return View(productoVM.Producto);
        }

        #region API
        [HttpGet]
        public IActionResult ObternerTodos()
        {
            var todos = _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades:"Categoria,Marca");
            return Json(new { data = todos });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productoDb = _unidadTrabajo.Producto.Obtener(id);

            if (productoDb == null)
            {
                return Json(new { success = false, message = "Error, no se encontro el Producto." });
            }

            //eliminar imagen
            string webRootPath = _hostEnviroment.WebRootPath;
            var imagenPath = Path.Combine(webRootPath, productoDb.ImagenUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagenPath))
            {
                System.IO.File.Delete(imagenPath);
            }

            _unidadTrabajo.Producto.Eliminar(productoDb.Id);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto borrado exitosamente" });
        }
        #endregion
    
    }
}
