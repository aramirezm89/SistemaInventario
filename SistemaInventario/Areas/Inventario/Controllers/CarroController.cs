using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using Stripe;
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
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public CarroComprasViewModel CarroComprasVM { get; set; }


        public CarroController(IUnidadTrabajo unidadTrabajo, ApplicationDbContext db, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unidadTrabajo = unidadTrabajo;
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);

            CarroComprasVM = new CarroComprasViewModel()
            {
                Orden = new Modelos.Orden(),
                CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId ==
                                                                claim.Value, incluirPropiedades: "Producto")
            };

            CarroComprasVM.Orden.TotalOrden = 0;
            CarroComprasVM.Orden.UsuaioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimerElemento(u => u.Id == claim.Value);

            foreach (var lista in CarroComprasVM.CarroComprasLista)
            {
                lista.Precio = lista.Producto.Precio;
                CarroComprasVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }

            return View(CarroComprasVM);
        }


        public IActionResult mas(int carroId,int proId)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimerElemento(c => c.Id == carroId, incluirPropiedades: "Producto");
            var stockProducto = _db.BodegaProducto.FirstOrDefault(p => p.ProductoId == proId);
            if(stockProducto.Cantidad > carroCompras.Cantidad)
            {
                carroCompras.Cantidad += 1;
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
          
        }

        public IActionResult menos(int carroId)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimerElemento(c => c.Id == carroId, incluirPropiedades: "Producto");
            if (carroCompras.Cantidad == 1)
            {
                var numeroProductos = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId ==
                                                                 carroCompras.UsuarioAplicacionId).ToList().Count();
                _unidadTrabajo.CarroCompras.Eliminar(carroCompras);
                _unidadTrabajo.Guardar();
                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos - 1);
            }
            else
            {
                carroCompras.Cantidad -= 1;
                _unidadTrabajo.Guardar();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult remover(int carroId)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimerElemento(c => c.Id == carroId, incluirPropiedades: "Producto");

            var numeroProductos = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId ==
                                                                 carroCompras.UsuarioAplicacionId).ToList().Count();
            _unidadTrabajo.CarroCompras.Eliminar(carroCompras);
            _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos - 1);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Proceder()
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);

            CarroComprasVM = new CarroComprasViewModel()
            {
                Orden = new Modelos.Orden(),
                CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId ==
                                                                claim.Value, incluirPropiedades: "Producto")
            };

            CarroComprasVM.Orden.TotalOrden = 0;
            CarroComprasVM.Orden.UsuaioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimerElemento(u => u.Id == claim.Value);

            foreach (var lista in CarroComprasVM.CarroComprasLista)
            {
                lista.Precio = lista.Producto.Precio;
                CarroComprasVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }

            CarroComprasVM.Orden.NombresCliente = CarroComprasVM.Orden.UsuaioAplicacion.Nombres + " " +
                                                  CarroComprasVM.Orden.UsuaioAplicacion.Apellidos;
            CarroComprasVM.Orden.Telefono = CarroComprasVM.Orden.UsuaioAplicacion.PhoneNumber;
            CarroComprasVM.Orden.Direccion = CarroComprasVM.Orden.UsuaioAplicacion.Direccion;
            CarroComprasVM.Orden.Pais = CarroComprasVM.Orden.UsuaioAplicacion.Pais;
            CarroComprasVM.Orden.Ciudad = CarroComprasVM.Orden.UsuaioAplicacion.Ciudad;

            return View(CarroComprasVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Proceder")]
        public IActionResult ProcederPost(string stripeToken)
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
            CarroComprasVM.Orden.UsuaioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimerElemento(c => c.Id == claim.Value);
            CarroComprasVM.CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(
                                                    c => c.UsuarioAplicacionId == claim.Value, incluirPropiedades: "Producto");
            CarroComprasVM.Orden.EstadoOrden = DS.EstadoPendiente;
            CarroComprasVM.Orden.EstadoPago = DS.PagoEstadoPendiente;
            CarroComprasVM.Orden.UsuarioAplicacionId = claim.Value;
            CarroComprasVM.Orden.FechaOrden = DateTime.Now;

            _unidadTrabajo.Orden.Agregar(CarroComprasVM.Orden);
            _unidadTrabajo.Guardar();

            foreach (var item in CarroComprasVM.CarroComprasLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                {
                    ProductoId = item.ProductoId,
                    OrdenId = CarroComprasVM.Orden.Id,
                    Precio = item.Producto.Precio,
                    Cantidad = item.Cantidad
                };
                CarroComprasVM.Orden.TotalOrden += ordenDetalle.Cantidad * ordenDetalle.Precio;
                _unidadTrabajo.OrdenDetalle.Agregar(ordenDetalle);

            }
            // Remover los productos del carro de Compras
            _unidadTrabajo.CarroCompras.EliminarRango(CarroComprasVM.CarroComprasLista);
            _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.ssCarroCompras, 0);

            if (stripeToken == null)
            {

            }
            else
            {
                //Procesar el Pago
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(CarroComprasVM.Orden.TotalOrden ),
                    Currency = "clp",
                    Description = "Numero de Orden: " + CarroComprasVM.Orden.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                {
                    CarroComprasVM.Orden.EstadoPago = DS.EstadoCancelado;
                }
                else
                {
                    CarroComprasVM.Orden.TransaccionId = charge.BalanceTransactionId;
                }
                if (charge.Status.ToLower() == "succeeded")
                {
                    foreach (var item in CarroComprasVM.CarroComprasLista)
                    {
                        var bodegaPro = _db.BodegaProducto.FirstOrDefault(p => item.ProductoId == p.Producto.Id);
                        var restaStock = bodegaPro.Cantidad - item.Cantidad;
                        bodegaPro.Cantidad = restaStock;
                        _unidadTrabajo.Guardar();

                    }
                    CarroComprasVM.Orden.EstadoPago = DS.PagoEstadoAprobado;
                    CarroComprasVM.Orden.EstadoOrden = DS.EstadoAprobado;
                    CarroComprasVM.Orden.FechaPago = DateTime.Now;

                }

            }
            _unidadTrabajo.Guardar();
            return RedirectToAction("OrdenConfirmacion", "Carro", new { id = CarroComprasVM.Orden.Id });

        }

        public IActionResult OrdenConfirmacion(int id)
        {
            return View(id);
        }

        public IActionResult ImprimirOrden(int id)
        {
            CarroComprasVM = new CarroComprasViewModel();
            CarroComprasVM.Compañia = _unidadTrabajo.Compañia.ObtenerPrimerElemento();
            CarroComprasVM.Orden = _unidadTrabajo.Orden.ObtenerPrimerElemento(o => o.Id == id, incluirPropiedades: "UsuaioAplicacion");
            CarroComprasVM.OrdenDetalleLista = _unidadTrabajo.OrdenDetalle.ObtenerTodos(o => o.OrdenId == id, incluirPropiedades: "Producto");

            return new ViewAsPdf("ImprimirOrden", CarroComprasVM)
            {
                FileName = $"Orden#{CarroComprasVM.Orden.Id}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"

            };

        }

    }
}
