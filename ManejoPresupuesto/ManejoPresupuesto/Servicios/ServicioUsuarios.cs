namespace ManejoPresupuesto.Servicios
{
    public interface IServicioUsuarios
    {
        int ObtenerUsuarioId();
    }

    // La clase debe implementar la Interfaz creada IServicioUsuarios
    public class ServicioUsuarios: IServicioUsuarios
    {
        // CTRL + . Subir ObtenerUsuarioId hacia la Interfaz
        public int ObtenerUsuarioId()
        {
            return 1;
        }
        //Ahora ir hacia la clase Program para poder configurar el servicio de usuarios
    }
}
