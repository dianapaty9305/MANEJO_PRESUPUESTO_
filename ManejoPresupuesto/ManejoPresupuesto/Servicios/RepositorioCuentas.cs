using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    //Creando la Interfaz
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }

    //Implementar la Interfaz
    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string connectionString;
        /*Inyectando el IConfiguration para poder extraer de ahí el connectionString */ 
        public RepositorioCuentas(IConfiguration configuration) 
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //Método Crear
        // CTRL + . en Cuenta para importar cuenta - (using ManejoPresupuesto)
        // CTRL + . en Crear para hacer un pull a la Interfaz
        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                    @"INSERT INTO Cuentas (Nombre, TipoCuentaId, Descripcion, Balance)
                    VALUES(@Nombre, @TipoCuentaId, @Descripcion, @Balance);
                    SELECT SCOPE_IDENTITY();", cuenta);
            cuenta.Id = id;
        }

        //Método para traer los datos de las cuentas, sus balanceas y el tipo de cuenta
        // CTRL + . sobre Buuscar y hacer pull para que aparezca ka firma en la Interfaz
        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            //Mapeando a cuenta
            return await connection.QueryAsync<Cuenta>(@"
                                                        SELECT Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre AS TipoCuenta
                                                        FROM Cuentas
                                                        INNER JOIN TiposCuentas tc
                                                        ON tc.Id = Cuentas.TipoCuentaId
                                                        WHERE tc.UsuarioId = @UsuarioId
                                                        ORDER BY tc.Orden", new {usuarioId});
        }
        //Método que nos va brindar la información de la cuenta que vamos a editar
        //CTRL + . en ObtenerPorId  para hacer un pull up a la Interface
        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            //obteniendo la información, mapeando a cuenta
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, tc.Id
                                                        FROM Cuentas
                                                        INNER JOIN TiposCuentas tc
                                                        ON tc.Id = Cuentas.TipoCuentaId
                                                        WHERE tc.UsuarioId = @UsuarioId AND Cuentas.Id = @Id", new {id, usuarioId});
        }

        //método que va a ser usado por la acción HttPost Editar que se encuentra en Cuentas controller
        //CTRL + . Actualizar para hacer un  pull up
        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            //Execute porque no queremos retornar nada
            await connection.ExecuteAsync(@"UPDATE Cuentas
                                           SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion, 
                                           TipoCuentaID = @TipoCuentaId
                                            WHERE  Id = @Id;", cuenta);
        }
    }
}
