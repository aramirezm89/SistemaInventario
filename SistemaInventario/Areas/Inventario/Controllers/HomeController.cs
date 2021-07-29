using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public CarroComprasViewModel CarroComprasVM { get; set; }
        public HomeController(ILogger<HomeController> logger, IUnidadTrabajo unidadTrabajo, ApplicationDbContext db)
        {
            _logger = logger;
            _unidadTrabajo = unidadTrabajo;
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Producto> productoLista = _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria,Marca");
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var numeroProductos = _db.CarroCompras.Where(u => u.UsuarioAplicacionId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos);
            }
            return View(productoLista);
        }

        public IActionResult Detalle(int id)
        {
            CarroComprasVM = new CarroComprasViewModel();

            CarroComprasVM.Compañia = _db.Compañia.FirstOrDefault();
            CarroComprasVM.BodegaProducto = _db.BodegaProducto.Include(p => p.Producto)
                                            .Include(m => m.Producto.Marca).Include(c => c.Producto.Categoria)
                                            .FirstOrDefault(b => b.ProductoId == id && b.BodegaId == CarroComprasVM.Compañia.BodegaVentaId);
            if(CarroComprasVM.BodegaProducto == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                CarroComprasVM.CarroCompras = new CarroCompras()
                {
                    Producto = CarroComprasVM.BodegaProducto.Producto,
                    ProductoId = CarroComprasVM.BodegaProducto.ProductoId
                };
                return View(CarroComprasVM);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Detalle(CarroComprasViewModel carroComprasVM)
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity; //capturar usuario conectado
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
            carroComprasVM.CarroCompras.UsuarioAplicacionId = claim.Value;
            CarroCompras carroDb = _db.CarroCompras.Include(p => p.Producto).FirstOrDefault(u => u.UsuarioAplicacionId == carroComprasVM.CarroCompras.UsuarioAplicacionId
                                     && u.ProductoId == carroComprasVM.CarroCompras.ProductoId);
            if(carroDb == null)
            {
                //no hay registro para el usuario en el carro de compras del producto
                _db.CarroCompras.Add(carroComprasVM.CarroCompras);
            }
            else
            {
                carroDb.Cantidad += carroComprasVM.CarroCompras.Cantidad;
                _db.CarroCompras.Update(carroDb);
            }
            _db.SaveChanges();
            //Agregar valor a la sesion de usuario
            var numeroProductos = _db.CarroCompras.Where(u => u.UsuarioAplicacionId == carroComprasVM.CarroCompras.UsuarioAplicacionId).ToList().Count();
            HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region APIPruebas
         [HttpGet]
         public IActionResult ObtenerTodos()
        {
            var numeroProductos = _db.CarroCompras.Where(u => u.UsuarioAplicacionId == "3990a67f-8b85-4064-80bc-96bce5a38280").ToList().Count();
            return Json(new { data = numeroProductos });
         }
       
        #endregion

    }
}
