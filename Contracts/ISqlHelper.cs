using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;


namespace Contracts
{
    public interface ISqlHelper
    {
        Task<List<TElement>> SQLQuery<TElement>(string spName, string connectionString, params SqlParameter[] parameters);
        Task<int> ExecuteNonQuery(string spName, string connectionString, params SqlParameter[] parameters);
        Task<DataSet> ExecuteDataSet(string spName, string connectionString, params SqlParameter[] Parameters);
    }
}
