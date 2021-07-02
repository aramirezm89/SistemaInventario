using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    [Authorize(Roles = DS.roleAdmin + "," + DS.roleInventario)]
    public class InventarioController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public InventarioVM inventarioVM { get; set; }

        public InventarioController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NuevoInventario(int? inventarioId)
        {
            inventarioVM = new InventarioVM();
            inventarioVM.BodegaLista = _db.Bodegas.ToList().Select(b => new SelectListItem
            {
                Text = b.Nombre,
                Value = b.Id.ToString()

            });
            inventarioVM.ProductoLista = _db.Producto.ToList().Select(p => new SelectListItem
            {
                Text = p.Descripcion,
                Value = p.Id.ToString()
            });
            inventarioVM.InventarioDetalles = new List<InventarioDetalle>();
            if (inventarioId != null)
            {
                inventarioVM.Inventario = _db.Inventario.SingleOrDefault(i => i.Id == inventarioId);
                inventarioVM.InventarioDetalles = _db.InventarioDetalle.Include(p => p.Producto).Include(p => p.Producto.Marca)
                    .Where(d => d.InventarioId == inventarioId).ToList();
            }

            return View(inventarioVM);
        }

        [HttpPost]
        public IActionResult AgregarProductoPost(int producto, int cantidad, int inventarioId)
        {
            inventarioVM.Inventario.Id = inventarioId;
            if (inventarioVM.Inventario.Id == 0) //condicion que si se cumple significa que se grabara registro 
            {
                inventarioVM.Inventario.Estado = false;
                inventarioVM.Inventario.FechaInicial = DateTime.Now;
                //capturar ID usario conectado

                var claimIdentidad = (ClaimsIdentity)User.Identity;
                var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
                inventarioVM.Inventario.UsuarioAplicacionId = claim.Value;

                _db.Inventario.Add(inventarioVM.Inventario);
                _db.SaveChanges();
            }
            else
            {
                inventarioVM.Inventario = _db.Inventario.SingleOrDefault(i => i.Id == inventarioId);
            }

            var bodegaProducto = _db.BodegaProducto.Include(p => p.Producto)
                .FirstOrDefault(p => p.ProductoId == producto && p.BodegaId == inventarioVM.Inventario.BodegaId);

            var detalle = _db.InventarioDetalle.Include(p => p.Producto)
                .FirstOrDefault(p => p.ProductoId == producto && p.InventarioId == inventarioVM.Inventario.Id);

            if (detalle == null)
            {
                inventarioVM.InventarioDetalle = new InventarioDetalle();

                inventarioVM.InventarioDetalle.ProductoId = producto;
                inventarioVM.InventarioDetalle.InventarioId = inventarioVM.Inventario.Id;
                if (bodegaProducto != null)
                {
                    inventarioVM.InventarioDetalle.StockAnterior = bodegaProducto.Cantidad;
                }
                else
                {
                    inventarioVM.InventarioDetalle.StockAnterior = 0;
                }

                inventarioVM.InventarioDetalle.Cantidad = cantidad;

                _db.InventarioDetalle.Add(inventarioVM.InventarioDetalle);
                _db.SaveChanges();

            }
            else
            {
                detalle.Cantidad += cantidad;
                _db.SaveChanges();
            }

            return RedirectToAction("NuevoInventario", new { inventarioId = inventarioVM.Inventario.Id });// retorna a vista NuevoInventario y envia ID de inventario ;
        }

        public IActionResult IncrementaDisminuyeCantidad(int id, string op)
        {
            inventarioVM = new InventarioVM();

            var detalle = _db.InventarioDetalle.FirstOrDefault(d => d.Id == id);
            inventarioVM.Inventario = _db.Inventario.FirstOrDefault(i => i.Id == detalle.InventarioId);


            if (op.Equals("suma"))
            {
                detalle.Cantidad += 1;

                _db.SaveChanges();
                return RedirectToAction("NuevoInventario", new { inventarioId = inventarioVM.Inventario.Id });
            }
            else
            {
                if (op.Equals("resta"))
                {
                    if (detalle.Cantidad == 1)
                    {
                        _db.InventarioDetalle.Remove(detalle);
                        _db.SaveChanges();
                    }
                    else
                    {
                        detalle.Cantidad -= 1;
                        _db.SaveChanges();
                    }
                }

                return RedirectToAction("NuevoInventario", new { inventarioId = inventarioVM.Inventario.Id });
            }

        }

        public IActionResult GenerarStock(int id)
        {
            var inventario = _db.Inventario.FirstOrDefault(i => i.Id == id);
            var detalleLista = _db.InventarioDetalle.Where(d => d.InventarioId == id);

            foreach (var item in detalleLista)
            {
                var bodegaProduto = _db.BodegaProducto.Include(p => p.Producto).FirstOrDefault(b => b.ProductoId == item.ProductoId
                                     && b.BodegaId == inventario.BodegaId);

                if (bodegaProduto != null)
                {
                    bodegaProduto.Cantidad += item.Cantidad;

                }
                else
                {
                    bodegaProduto = new BodegaProducto();

                    bodegaProduto.BodegaId = inventario.BodegaId;
                    bodegaProduto.ProductoId = item.ProductoId;
                    bodegaProduto.Cantidad = item.Cantidad;
                    _db.BodegaProducto.Add(bodegaProduto);
                }
            }

            //Actualizar estado

            inventario.Estado = true;
            inventario.FechaFinal = DateTime.Now;
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));


           
        }

        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _db.BodegaProducto.Include(b => b.Bodega).Include(p => p.Producto).ToList(); // devuelve la relacion de tablas producto y bodega realziadas en el model BodegaProducto


            return Json(new { data = todos });


        }
        #endregion
    }
}
