using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Validaciones
{
    //La clase hereda de ValidationAttribute
    public class PrimelaLetraMayusculaAttribute: ValidationAttribute
    {
        //En value se tiene el valor que tiene el campo en el cual se ha colocado el atributo, en este caso será el campo Nombre
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Primero verificamos que el campo no sea nulo
            if (value == null || string.IsNullOrEmpty(value.ToString())) 
            {
                return ValidationResult.Success;
            }

            //Obteniendo el primer caracter
            var primeraLetra = value.ToString()[0].ToString();

            if(primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }

            return ValidationResult.Success;
            //ahora vamos a TipoCuenta.css y colocamos el atributo creado sobre Nombre
        }
    }
}
