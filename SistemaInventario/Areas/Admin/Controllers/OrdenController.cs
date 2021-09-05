using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrdenController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        [BindProperty]
        public OrdenDetalleViewModel OrdenDetalleVM { get; set; }

        public OrdenController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detalle(int id)
        {
            OrdenDetalleVM = new OrdenDetalleViewModel()
            {
                Orden = _unidadTrabajo.Orden.ObtenerPrimerElemento(o => o.Id == id, incluirPropiedades: "UsuaioAplicacion"),
                OrdenDetalleLista = _unidadTrabajo.OrdenDetalle.ObtenerTodos(o => o.OrdenId == id, incluirPropiedades: "Producto")
            };

            return View(OrdenDetalleVM);
        }

        [Authorize(Roles = DS.roleAdmin+","+DS.roleVentas)]
        public IActionResult Procesar(int id)
        {
            Orden orden = _unidadTrabajo.Orden.ObtenerPrimerElemento(o => o.Id == id);
            orden.EstadoOrden = DS.EstadoEnProceso;
            _unidadTrabajo.Guardar();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = DS.roleAdmin + "," + DS.roleVentas)]
        public IActionResult EnviarOrden()
        {
            Orden orden = _unidadTrabajo.Orden.ObtenerPrimerElemento(o => o.Id == OrdenDetalleVM.Orden.Id);
            orden.NumeroEnvio = OrdenDetalleVM.Orden.NumeroEnvio;
            orden.Courier = OrdenDetalleVM.Orden.Courier;
            orden.EstadoOrden = DS.EstadoEnviado;
            orden.FechaEnvio = DateTime.Now;
            _unidadTrabajo.Guardar();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = DS.roleAdmin + "," + DS.roleVentas)]
        public IActionResult CancelarOrden(int id)
        {
            Orden orden = _unidadTrabajo.Orden.ObtenerPrimerElemento(o => o.Id == id);

            if(orden.EstadoPago  == DS.PagoEstadoAprobado)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(orden.TotalOrden),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orden.TransaccionId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                orden.EstadoOrden = DS.EstadoDevuelto;
                orden.EstadoPago = DS.EstadoDevuelto;
            }
            else
            {
                orden.EstadoOrden = DS.EstadoCancelado;
                orden.EstadoPago = DS.EstadoCancelado;
            }

            _unidadTrabajo.Guardar();
            return RedirectToAction("Index");
        }
        #region
        [HttpGet]
        public IActionResult ObtenerOrdenLista(string estado)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Orden> ordenLista;
            if (User.IsInRole(DS.roleAdmin) || User.IsInRole(DS.roleVentas))
            {
                ordenLista = _unidadTrabajo.Orden.ObtenerTodos(incluirPropiedades: "UsuaioAplicacion");
            }
            else
            {
                ordenLista = _unidadTrabajo.Orden.ObtenerTodos(o => o.UsuarioAplicacionId == claim.Value, incluirPropiedades: "UsuaioAplicacion");
            }

            switch (estado)
            {
                case "pendiente":
                    ordenLista = ordenLista.Where(o => o.EstadoPago == DS.PagoEstadoPendiente || o.EstadoPago == DS.PagoEstadoRetrasado);
                    break;
                case "aprobado":
                    ordenLista = ordenLista.Where(o => o.EstadoPago == DS.PagoEstadoAprobado);
                    break;
                case "rechazado":
                    ordenLista = ordenLista.Where(o => o.EstadoOrden == DS.EstadoRechazado || o.EstadoOrden == DS.EstadoCancelado);
                    break;
                case "completado":
                    ordenLista = ordenLista.Where(o => o.EstadoOrden == DS.EstadoEnviado);
                    break;
                default:
                    break;
            }
            return Json(new { data = ordenLista });
        }
        #endregion
    }
}
