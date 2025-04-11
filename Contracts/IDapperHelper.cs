using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.Appsettings;
using Microsoft.Data.SqlClient;

namespace Contracts
{
    public interface IDapperHelper
    {
        Task<int> ExecuteNonQueryAsync(string spName, string connectionString, Dictionary<string, object> parameters);
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(string spName, string connectionString, DynamicParameters parameters);
        Task<SqlMapper.GridReader> ExecuteMultipleQueryAsync(string spName, string connectionString, Dictionary<string, object> parameters);

    }
}
