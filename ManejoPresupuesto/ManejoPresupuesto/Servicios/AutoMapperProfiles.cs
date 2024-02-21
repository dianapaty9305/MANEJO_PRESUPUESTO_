using AutoMapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public class AutoMapperProfiles : Profile
    {
        //Constructor de la Clase 
        public AutoMapperProfiles() 
        {
            //Diciendo de que tipo de dato a que tipo de dato vamos a Mappear
            //En nuestro caso habiamos mapeado de cuenta a CuentaCreacionViewModel
            //CTRL + . sobre Cuenta para importar el modelo
            CreateMap<Cuenta, CuentaCreacionViewModel>();
        }
    }
}
