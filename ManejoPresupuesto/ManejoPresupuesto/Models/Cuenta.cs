using ManejoPresupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Cuenta
    {
        //Clase con las mismas propiedades de la tabla Cuenta de nuestra BD
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [PrimelaLetraMayuscula]
        public string Nombre { get; set; }
        [Display(Name ="Tipo Cuenta")]
        public int TipoCuentaId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength:1000)]
        public string Descripcion { get; set; }
        public string TipoCuenta { get; set; }

    }
}
