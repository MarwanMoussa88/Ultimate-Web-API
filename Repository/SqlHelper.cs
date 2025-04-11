using System.Data;
using System.Xml.Linq;
using Contracts;
using Entities.Extensions;
using Microsoft.Data.SqlClient;

namespace Repository
{
    public class SqlHelper : ISqlHelper
    {
        private readonly int Timeout = 9999;

        public async Task<DataSet> ExecuteDataSet(string spName, string connectionString, params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand command = CreateSqlCommand(spName, connection, parameters);

            DataSet dataset = new DataSet();

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.NextResultAsync())
            {
                DataTable table = new DataTable();
                table.Load(reader);
                dataset.Tables.Add(table);
            }

            return dataset;

        }

        public async Task<int> ExecuteNonQuery(string spName, string connectionString, params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand command = CreateSqlCommand(spName, connection, parameters);

            return await command.ExecuteNonQueryAsync();

        }


        public async Task<List<TElement>> SQLQuery<TElement>(string spName, string connectionString, params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            SqlCommand command = CreateSqlCommand(spName, connection, parameters);

            using var reader = await command.ExecuteReaderAsync();

            var result = reader.MapToList<TElement>();
            return result;


        }

        private SqlCommand CreateSqlCommand(string spName, SqlConnection connection, params SqlParameter[] parameters)
        {
            using SqlCommand command = new SqlCommand();

            command.Connection = connection;
            command.CommandTimeout = Timeout;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }
    }
}

