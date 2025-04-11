using Azure;
using System.ComponentModel.Design;
using Contracts;
using Dapper;
using Domain.Appsettings;
using Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Entities.Extensions;

namespace Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ISqlHelper sqlHelper;
        private readonly IDapperHelper dapperHelper;
        private readonly IOptions<ConnectionString> connectionStrings;

        public EmployeeService(ISqlHelper sqlHelper, IDapperHelper dapperHelper, IOptions<ConnectionString> connectionStrings)
        {
            this.sqlHelper = sqlHelper;
            this.dapperHelper = dapperHelper;
            this.connectionStrings = connectionStrings;
        }
        public async Task<IEnumerable<Employee>> GetEmployeesAdo()
        {
            var employees = new List<Employee>()
            {
                new Employee()
                {
                    Id = Guid.NewGuid(),
                    Name = null,
                    Age = 0,
                    CompanyId = Guid.NewGuid()
                },
                new Employee()
                {
                    Id = Guid.NewGuid(),
                    Name = null,
                    Age = 0,
                    CompanyId = Guid.NewGuid()
                },
            };

            SqlParameter[] @param = [
                new SqlParameter("@test1","Marwan"),
                new SqlParameter("@test2",employees.MapToDataTable())
                {
                    SqlDbType = System.Data.SqlDbType.Structured,
                    TypeName = "dbo.Customer"
                }];



            return await sqlHelper.SQLQuery<Employee>("dbo.GetEmployees", connectionStrings.Value.LocalDb, @param);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesDapper()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@test1", "Marwan");
            return await dapperHelper.ExecuteQueryAsync<Employee>("dbo.GetEmployees", connectionStrings.Value.LocalDb, parameters);
        }
    }
}
