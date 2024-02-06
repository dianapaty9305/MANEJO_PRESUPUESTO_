﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    //Creando la Interfaz
    public interface IRepositorioCuentas
    {
        Task Crear(Cuenta cuenta);
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
    }
}