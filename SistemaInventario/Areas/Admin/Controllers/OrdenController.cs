using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
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
        public OrdenController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
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
