using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class CarroController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public CarroComprasViewModel CarroComprasVM { get; set; }

        public CarroController(IUnidadTrabajo unidadTrabajo,IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unidadTrabajo = unidadTrabajo;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);//capturar identidad de usuario conectado

            CarroComprasVM = new CarroComprasViewModel()
            {
                Orden = new Modelos.Orden(),
                CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId == claim.Value, 
                                    incluirPropiedades: "Producto"),
            };
            CarroComprasVM.Orden.TotalOrden = 0;
            CarroComprasVM.Orden.UsuaioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimerElemento(u => u.Id == claim.Value);

            foreach (var item in CarroComprasVM.CarroComprasLista)
            {
                item.Precio = item.Producto.Precio;
                CarroComprasVM.Orden.TotalOrden += (item.Precio * item.Cantidad);

            }
            return View(CarroComprasVM);
        }

        public IActionResult Mas(int id)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimerElemento(c => c.Id == id, incluirPropiedades: "Producto");
            carroCompras.Cantidad += 1;
            _unidadTrabajo.Guardar();
            return RedirectToActionPermanent(nameof(Index));
        }

        public IActionResult Menos(int id)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimerElemento(c => c.Id == id, incluirPropiedades: "Producto");
            if(carroCompras.Cantidad  == 1)
            {
                var numeroProductos = _unidadTrabajo.CarroCompras.ObtenerTodos(c => c.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId)
                                      .ToList().Count();
                _unidadTrabajo.CarroCompras.Eliminar(carroCompras);
                _unidadTrabajo.Guardar();
                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos - 1);

            }
            else
            {
                carroCompras.Cantidad -= 1;
                _unidadTrabajo.Guardar();
              
            }

            return RedirectToActionPermanent(nameof(Index));
        }

        public IActionResult Remover(int id)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimerElemento(c => c.Id == id, incluirPropiedades: "Producto");
          
                var numeroProductos = _unidadTrabajo.CarroCompras.ObtenerTodos(c => c.UsuarioAplicacionId == carroCompras.UsuarioAplicacionId)
                                      .ToList().Count();
                _unidadTrabajo.CarroCompras.Eliminar(carroCompras);
                _unidadTrabajo.Guardar();
                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos - 1);
                return RedirectToActionPermanent(nameof(Index));

        }

        public IActionResult Proceder()
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);

            CarroComprasVM = new CarroComprasViewModel()
            {
                Orden = new Modelos.Orden(),
                CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(
                                    c => c.UsuarioAplicacionId == claim.Value, incluirPropiedades: "Producto")
            };

            CarroComprasVM.Orden.TotalOrden = 0;
            CarroComprasVM.Orden.UsuaioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimerElemento(u => u.Id == claim.Value);

            foreach (var item in CarroComprasVM.CarroComprasLista)
            {
                item.Precio = item.Producto.Precio;
                CarroComprasVM.Orden.TotalOrden += (item.Precio * item.Cantidad);
            }

            CarroComprasVM.Orden.NombresCliente = $"{CarroComprasVM.Orden.UsuaioAplicacion.Nombres} {CarroComprasVM.Orden.UsuaioAplicacion.Apellidos}";
            CarroComprasVM.Orden.Telefono = CarroComprasVM.Orden.UsuaioAplicacion.PhoneNumber;
            CarroComprasVM.Orden.Direccion = CarroComprasVM.Orden.UsuaioAplicacion.Direccion;
            CarroComprasVM.Orden.Pais = CarroComprasVM.Orden.UsuaioAplicacion.Pais;
            CarroComprasVM.Orden.Ciudad = CarroComprasVM.Orden.UsuaioAplicacion.Ciudad;

            return View(CarroComprasVM);
        }
    }
}
