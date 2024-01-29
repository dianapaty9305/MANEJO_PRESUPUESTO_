using ManejoPresupuesto.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    /* Lasd validaciones a nivel de modelo nos sirven para realizar validaciones dentro de la clase. 
     Para implementar la validación a nivel de modelo se tiene que implementar una interfaz llamada IValidatable<Object 
    la cual obliga a implementar el método Validate*/
    public class TipoCuenta    //: IValidatableObject
    {
        //debe tener las mismas propiedades creadas en la tabla TiposCuentas
        public int Id { get; set; }
        /*las anotaciones de datos reciben parámetros, y ahí se puede colocar el mensaje de error
        {0} - placeholder  se sustituye por el nombre del campo */
        [Required(ErrorMessage = "El campo {0} es requerido")]
        // [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "El valor del campo {0} debe estar entre {2} y {1}")]
        // [Display(Name = "Nombre del tipo cuenta")]
        [PrimelaLetraMayuscula]
        /*Enlazando la acción creada (VerificarExisteTipoCuenta) en TiposCuentasController - se indica el nombre de la accion 
         y el controlador en el que se encuentra*/
        [Remote(action: "VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }










        /* Ejemplo Validación a nivel de Modelo*/ 

        /*
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
         if(Nombre != null && Nombre.Length > 0)
            {
                var primeraLetra = Nombre[0].ToString();

                if(primeraLetra != primeraLetra.ToUpper())
                {
                    //acá se le pasa el campo al cual se le va a aplicar el error 
                    yield return new ValidationResult("La primera letra debe ser mayúscula",
                        new[] {nameof(Nombre)});
                }
            }
        }

        */

        /* Pruebas de otras validaciones por defecto*/
        /* 
             [Required(ErrorMessage = "El campo {0} es requerido")]
             [EmailAddress(ErrorMessage = "El campo debe ser un correo electrónico válido")]
             public string Email { get; set; }
             [Range(minimum:18,maximum:100,ErrorMessage ="El valor debe estar entre {1} y {2}")]
             public int Edad { get; set; }
             [Url(ErrorMessage ="El campo debe ser una URL válida")]
             public string URL { get; set; }
             [CreditCard(ErrorMessage ="La tarjeta de crédito no es válida")]
             [Display(Name ="Tarjeta de Crédito")]
             public string TarjetaDeCredito { get; set; }
        */

    }
}
