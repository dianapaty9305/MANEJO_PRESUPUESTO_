using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{
    /*Esta clase hereda de la clase cuenta porque va a tener todos los campos de la tabla Cuenta, solo que va 
     a agregar uno nuevo*/
    public class CuentaCreacionViewModel : Cuenta
    {
        /* SelectListItem es una clase especial que permite crear selects de una manera sencilla*/
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
