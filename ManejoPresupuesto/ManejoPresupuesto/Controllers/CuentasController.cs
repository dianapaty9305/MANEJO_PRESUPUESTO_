using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;

        // CTRL + . en repositorioTiposCuenta para agregar como campo
        // CTRL + . en servicioUsuarios para agregar como campo  
        // CTRL + . en repositorioCuentas para agregar como campo
        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
        }

        //Acción Index
        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentasConTipoCuenta = await repositorioCuentas.Buscar(usuarioId);
            /*Construyendo el modelo, usando una tipica operaciónde GroupBy en la cual agrupamos por TipoCuenta y con el Select se hace una proyeccion
             * hacia IndiceCuentasViewModel donde se coloca el tipo cuenta. El key es el TipoCuenta osea representa el valor que utilizamos para
             reaaizar el GroupBy - luego cuentas se iguala a Enumerable, con esto se dice que asi como yo tengo una agrupación de cuentas por 
            tipo cuenta cuando se dice grupo.AsEnumerable() estoy obteniendo el IEnumerable de las cuentas pertenecienntes  al TipoCuenta que 
            se tiene */
            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();
            return View(modelo);    
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            //Pasando el modelo a la vista
            return View(modelo);          
            
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            //Necesito validar que el tipo cuenta que me envía el usuario es válido (existe y es accesible para ese usuarioId)
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if(!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }
            await repositorioCuentas.Crear(cuenta);
            return RedirectToAction("Index");
        }
        

        //Acción para realizar la Edición de la cuenta 
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }

            /*Armando el modelo el cual va a esperar la vista que es el CuentaCreacionViewModel - con esto hacemos el mapeo de cuenta 
              a CuentaCreacionViewModel */
            /* necesitamos el CuentaCreacionViewModel porque tendremos que mostrarle al usuario el listado de todos los tipos cuenta que tiene*/
            var modelo = new CuentaCreacionViewModel()
            {
                Id = cuenta.Id,
                Nombre = cuenta.Nombre,
                TipoCuentaId = cuenta.TipoCuentaId,
                Descripcion = cuenta.Descripcion,
                Balance = cuenta.Balance
            };
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }

        //Acción que va a recibir el posteo del formulario
        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaEditar.Id, usuarioId);
            if (cuenta is null) 
            { 
                return RedirectToAction("NoEncontrado", "Home");
            }
            //Validando el tipo cuenta
            var tipoCuenta = await repositorioCuentas.ObtenerPorId(cuentaEditar.TipoCuentaId, usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //Actualizar la cuenta - utiliza el método en repositorio cuentas - public async Task Actualizar(CuentaCreacionViewModel cuenta)
            await repositorioCuentas.Actualizar(cuentaEditar);
            return RedirectToAction("Index");
        }

        /*Sincronizando la operación de obtención de los tipos cuentas del usuario para poder volver a cargar la vista */
        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            //mapeando dé tiposCuentas a SelectListItem
            // CTRL + . en SelectListItem para importar el namespace correspondiente
            // SelectListItem(x.Nombre,x.Id.ToString()) el primer campo es el texto y el segundo el es valor
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
