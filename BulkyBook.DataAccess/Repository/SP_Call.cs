using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulkyBook.DataAccess.Repository
{
    public class SP_Call : ISP_Call
    {
        private readonly ApplicationDbContext db;
        private static string ConnectionString = "";

        public SP_Call(ApplicationDbContext db)
        {
            this.db = db;
            ConnectionString = this.db.Database.GetDbConnection().ConnectionString;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public void Execute(string procedureName, DynamicParameters parameters = null)
        {
            using SqlConnection connection = new SqlConnection(ConnectionString);

            connection.Open();
            connection.Execute(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            connection.Close();
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters parameters = null)
        {
            using SqlConnection connection = new SqlConnection(ConnectionString);

            connection.Open();
            var result = connection.Query<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            connection.Close();

            return result;
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters parameters = null)
        {
            using SqlConnection connection = new SqlConnection(ConnectionString);

            connection.Open();
            var result = SqlMapper.QueryMultiple(connection, procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            connection.Close();

            var item1 = result.Read<T1>().ToList();
            var item2 = result.Read<T2>().ToList();

            if (item1 != null && item2 != null)
            {
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
        }

        public T OneRecord<T>(string procedureName, DynamicParameters parameters = null)
        {
            using SqlConnection connection = new SqlConnection(ConnectionString);

            connection.Open();
            var value = connection.Query<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            connection.Close();

            return (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T));
        }

        public T Single<T>(string procedureName, DynamicParameters parameters = null)
        {
            using SqlConnection connection = new SqlConnection(ConnectionString);
            return (T)Convert.ChangeType(connection.ExecuteScalar<T>(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure), typeof(T));
        }
    }
}
