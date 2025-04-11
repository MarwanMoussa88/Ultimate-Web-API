using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Repository
{
    public class DapperHelper : IDapperHelper
    {
        public async Task<SqlMapper.GridReader> ExecuteMultipleQueryAsync(string spName, string connectionString, Dictionary<string, object> parameters)
        {
            using IDbConnection connection = new SqlConnection(connectionString);
            return await connection.QueryMultipleAsync(spName, param: parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ExecuteNonQueryAsync(string spName, string connectionString, Dictionary<string, object> parameters)
        {
            using IDbConnection connection = new SqlConnection(connectionString);
            return await connection.ExecuteAsync(spName, param: parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string spName, string connectionString, DynamicParameters parameters)
        {
            using IDbConnection connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
        }

    }
}
