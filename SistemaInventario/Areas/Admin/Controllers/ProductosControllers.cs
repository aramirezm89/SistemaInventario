using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    public class ProductosController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _hostEnviroment;

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
                CategoriaLista = _unidadTrabajo.Categoria.ObtenerTodos().Select(elemento => new SelectListItem 
                {
                    Text = elemento.Nombre,
                    Value = elemento.Id.ToString()

                }),
                MarcaLista = _unidadTrabajo.Marca.ObtenerTodos().Select(Elemento => new SelectListItem
                {
                    Text = Elemento.Nombre,
                    Value = Elemento.Id.ToString()
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
        public IActionResult Upsert(Producto producto)
        {
            if (ModelState.IsValid)
            {
                if (producto.Id == 0)
                {
                    _unidadTrabajo.Producto.Agregar(producto);
                }
                else
                {
                    _unidadTrabajo.Producto.Actualizar(producto);
                }

                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
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
            _unidadTrabajo.Producto.Eliminar(productoDb.Id);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto borrado exitosamente" });
        }
        #endregion
    
    }
}
