using Entities;

namespace Contracts
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetEmployeesAdo();
        Task<IEnumerable<Employee>> GetEmployeesDapper();

    }
}
