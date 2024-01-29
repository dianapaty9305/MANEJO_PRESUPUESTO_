using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace ManejoPresupuesto.Servicios
{
    /*vamos a mantenernos utilizando el principio de inyeccion de dependencias que dice que nuestras clases deben de depender de
     abstracciones y no de tipos concretos por tanto vamos a colocar una interfaz*/

    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }

    //Esta clase va a implementar la Interfaz IRepositorioTiposCuentas
    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {
        //Código para insertar un Tipo Cuenta en la BD
        //Colocamos el constructor de la clase, insertando un IConfiguration porque necesito acceder al connectionString

        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration) 
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //Método a través del cual vamos a poder crear un tipo cuenta en la BD
        //CTRL + . en TipoCuenta para importar Models (using ManejoPresupuesto.Models;)
        //CTRL + . sobre Crear Up IRepositorioTiposCuentas - para tener la asignatura en la Interfaz
        public async Task Crear(TipoCuenta tipoCuenta) 
        {
            using var connection = new SqlConnection(connectionString);
            /* CTRL + . QuerySingle para importar Dapper - QuerySingle permite hacer un queey que me va a devolver un solo 
               resultado, esto porque después que inserte el tipo cuenta quiero extraer el id de ese tipo cuenta */
            var id = await connection.QuerySingleAsync<int>($@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden)
                                                 values (@Nombre, @UsuarioId, 0);
                                                 SELECT SCOPE_IDENTITY();", tipoCuenta);
            // SELECT SCOPE_IDENTITY() es el que me trae el Id del registro recien creado

            tipoCuenta.Id = id;
        }

        //Para configurar el servicio vamos a ir a la clase Program.cs 

        /* Método que nos permite verificar si ya existe un tipo cuenta con el nombre indicado - retorna verdadero o falso*/
        /* CTRL + . en Existe para llevar la asignatura a la Interface IRepositorioTiposCuentas*/
        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            /*con QueryFirstOrDefaultAsync le estoy diciendo que me traiga el primer valor que encuentre o por defecto (cero) 
             sino encuentra ningun registo delk tipo de dato que se pone, en este caso int*/
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                                                        @"SELECT 1
                                                                        FROM TiposCuentas
                                                                        WHERE Nombre = @Nombre AND usuarioId = @UsuarioId;",
                                                                        new {nombre,usuarioId});
            return existe == 1;
        }
        //Ahora debemos agregar la validacion en la acción, vamos a TiposCuentasController 

        /* Método qu nos permite traer los registros de tipos cuentas de la BD para mostrarlos al Usuario que está utilizando
         la aplicación*/
        //CTRL + . para incluir el método en la interfaz
        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            /*QueryAsync me permite realizar un query de SELECT, trae un conjunto de resultados y me mapea ese conjunto de 
             resultados a un tipo especifico, es decir a la clase que se le indique en este caso la clase TipoCuenta*/
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                            FROM TiposCuentas
                                                            WHERE UsuarioId = @UsuarioId
                                                            ORDER BY Orden", new {usuarioId});
        }

        //Método que nos permite realizar actualización de registros, este método no retorna nada
        // CTRL + . para llevar Actualizar hacia la Interfaz
        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            //ExecuteAsync permite ejecutar un query que no va a retornar nada
            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                          SET Nombre = @Nombre
                                          WHERE Id = @Id", tipoCuenta);
        }

        /* Método qque me permite obtener elk tipo cuenta por Id, porque se va a permitir al usuario entrar a una página en la 
         cual va a poder visualizar todos los campos de tipo de cuenta*/
        // CTRL + . para llevar ObtenerPorId hacia la Interfaz
        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"
                                                                        SELECT Id, Nombre, Orden
                                                                        FROM TiposCuentas
                                                                        WHERE Id = @Id AND UsuarioId = @UsuarioId",
                                                                        new {id, usuarioId});
        }

        //Método de Borrado
        //CTRL + . Agregar la firma en el Interfaz
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE Id = @Id", new {id});
        }

        //Método que permite actualizarr  el campo ordenar de los tipos cuentas del usuario
        //CTRL + . Pull a la Interfaz
        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            /*Se está pasando un IEnumerable de tipos cuentas y el lo que está haciendo es que va a ejecutar el query por cada
             * tipo cuenta que se le pase*/
            var query = "UPDATE TiposCuentas SET Orden = @Orden Where Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
            //Luego vamos a ir hacia TiposCuentasController para implementar el método de Ordenar
        }
    }
}
