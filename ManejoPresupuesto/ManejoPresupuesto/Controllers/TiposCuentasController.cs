using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Globalization;

namespace ManejoPresupuesto.Controllers
{
    // TiposCuentasController hereda de Controller - Con CTRL + .(Punto) - se utiliza para importar el namespace de AspNetCore.Mvc
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        /* Prueba de conexion a la BD
private readonly string connectionString;
public  TiposCuentasController(IConfiguration configuration) 
{
connectionString = configuration.GetConnectionString("DefaultConnection");
}

*/

        //Constructor
        //Utilizando el Servicio RepositorioTiposCuentas
        //CTRL + . en IRepositorioTiposCuentas para importar servicios
        /* CTRL + . repositorioTiposCuentas para crear e inicializar como un campo y así se tendrá acceso a ese repositorio
                en la  IActionResult Crear(TipoCuenta tipoCuenta) */

        // CTRL + . servicioUsuarios crear y asignar como un campo
        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, 
            IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        /*Acçión para el listado de Tipos Cuentas - por defecto o concención utilizamos Index para aquella vista que va a mostrar
         un listado de elementos */
        public async Task<IActionResult> Index()
        {
            //Provisional mientras se crean usuario reales 
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return View(tiposCuentas);
            //Ahora vamos a ir a crear una vista para mostrar el listado de Tipos Cuentas
        }

        //Creando una accion llamada crear 
        public IActionResult Crear()
        { 
            /* Prueba de Conexion a la BD
             
                    using (var connection = new SqlConnection(connectionString))
                    {
                        object query = connection.Query("SELECT 1").FirstOrDefault();
                    }

            */
                return View();
        }

        //accion que responde al HTTPOST del formulario de la vista
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            /*Es necesario para validar los datos que vienen del front, no podemos confiar ciegamente en las validadciones que ocurren en el 
             front - se deben realizar las validaciones en el back*/
            if(!ModelState.IsValid)
            {
                //permite volver a llenar el formulario con la información que el usuario ya tenía
                return View(tipoCuenta);
            }
            //Usuario Creado directamente el la BD para ejemplo
            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId(); 

            //Agregando la validación public async Task<bool> Existe(string nombre, int usuarioId) creada en RepositorioTiposCuentas
            var YaExisteTipoCuenta = 
                await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if(YaExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), 
                    $"El nombre {tipoCuenta.Nombre} ya existe.");

                return View(tipoCuenta);
            }
            await repositorioTiposCuentas.Crear(tipoCuenta);
            return RedirectToAction("Index");
        }

        /* Este se refiere a la página que me va a permitir cargar el registro por su Id*/
        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            //Tengo que pasar el id que me envía en usuario y el usuarioId para ver si tiene permiso de cargar el registro
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            //Si tipoCuenta es nulo quiere decir que el usuario noo tiene permisos para cargar el registro
            if(tipoCuenta is null)
            {
                //Home es una vista que se debe ser creada - y NoEncontrado es una acción que se crea en HomeController.cs
                return RedirectToAction("NoEncontrado","Home");
            }    
            return View(tipoCuenta);
            // Se debe crear la vista de Editar
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            /*Obteniendo el Id del usuario, porque no puedo permitir al usuario que me envíe su propio Id, ya que el usuario
             podría decir que es eladministrador cuando no lo es*/
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            // se necesita ver que el tipo cuenta existe 
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if(tipoCuentaExiste is null)
            {
                //La acción NoEncontrado  debe ser creada - Ir hacia el Home Controller y crearla
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Actualizar(tipoCuenta);

            return RedirectToAction("Index");
        }

        //Acción Borrar
        public async Task<IActionResult>Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if(tipoCuenta is null)
            {
               //NoEncontrado es una acción creada en HomeController.cs y la vista NoEncontrado creada en Home
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        //acá crearemos el HttpPost en el que realmente se va a realizar el borrado
        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                //NoEncontrado es una acción creada en HomeController.cs y la vista NoEncontrado creada en Home
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }


        /* Validación personalizada del servidor, acción especial que nos va a servir para realizar la validación y utilizamos 
         el atributo Remote para llamar dicha acción desde el navegador del usuario utilizando JavaScript - Remote es un atributo
        que permite realizar validaciones de manera remota desde JavaScript pero realizando una llamada hacia nuestra applicación
        de APS.NET CORE*/
        [HttpGet]
        public async Task<IActionResult>VerificarExisteTipoCuenta(String nombre)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var YaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);
            if (YaExisteTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }

        /* acción que se va a ejecutar cuando arrastremos las filas de la tabla TiposCuentas para reordenar*/
        [HttpPost]
        /*FromBody significa que vamos a recibir del cuerpo de la petición un arreglo de ids (que son los ids de los TiposCuentas 
          del usuario que van a venir en un orden específico y con ese orden vamos a actualizar el campo orden en la BD*/
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            //Obteniendo los tipos cuentas del usuarioId
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            //Utilizando linkQ para obtener los ids de los tipos cuentas
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);
            /*Verificano (comparando) que los ids de tiposCuentas que se están recibiendo sean del usuario, porque no podemos confiar en la data que nos trae
             el usuario - se debe validar - si es así el idsTiposCuentasNoPertenecenAlUsuario debe quedar vacio*/
            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
            {
                return Forbid();
            }

            /*Utilizando LinkQ se hace un mapeo  valor es cada valor que se encuentra en el arreglo de enteros y el 
             indice es simplemente el orden que tiene el id en el arreglo*/
            var tiposCuentasOrdenados = ids.Select((valor, indice) =>
               new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
            //Ahora vamos a ir hacia la vista Index de TiposCuentas a realizar el pos desde JavaScript
        }
    }
}