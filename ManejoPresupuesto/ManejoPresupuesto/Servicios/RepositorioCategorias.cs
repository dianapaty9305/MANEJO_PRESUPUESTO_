using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCategorias
    {
        Task Crear(Categoria categoria);
    }

    //Implementamos la Interface
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string connectionString;

        public RepositorioCategorias(IConfiguration configuration) 
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //Método para crear una categoría
        // CTRL + . en Categoria para importar el modelo
        // CTRL + . en Crear pull a la Interface
        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
                                        INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId)
                                        Values (@Nombre, @TipoOperacionId, @UsuarioId);

                                        SELECT SCOPE_IDENTITY();
                                        ", categoria);

            categoria.Id = id;
        }
    }
}
