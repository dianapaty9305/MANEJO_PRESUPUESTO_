namespace ManejoPresupuesto.Models
{
    public class IndiceCuentasViewModel
    {
        public string TipoCuenta { get; set; }  
        public IEnumerable<Cuenta> Cuentas { get; set;}

        //En esta propiedad va la sumanoria de los distintos balances de las cuentas pertenecientes al tipo cuenta 
        public decimal Balance => Cuentas.Sum(x => x.Balance);
    }
}
