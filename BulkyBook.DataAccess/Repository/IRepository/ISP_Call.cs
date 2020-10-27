using Dapper;
using System;
using System.Collections.Generic;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface ISP_Call : IDisposable
    {
        // one column of one row
        T Single<T>(string procedureName, DynamicParameters parameters = null);

        // one row
        T OneRecord<T>(string procedureName, DynamicParameters parameters = null);

        // multiple rows
        IEnumerable<T> List<T>(string procedureName, DynamicParameters parameters = null);

        // multiple tables
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters parameters = null);

        void Execute(string procedureName, DynamicParameters parameters = null);
    }
}
