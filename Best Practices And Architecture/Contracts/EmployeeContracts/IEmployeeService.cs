using MyProject.Services.DTO_S.EmployeeDTO_S;
using System.Collections.Generic;

namespace MyProject.Services.EmployeeContracts.Contracts
{
    public interface IEmployeeService
    {
        EmployeeOutputDTO GetEmployeeWithHightSalary();

        EmployeeOutputDTO GetEmployeeById(int id);

        EmployeeDepartmentsOutputDTO GetEmployeeDepartment(int id);

        IEnumerable<EmployeeAddressOutputDTO> GetEmployeeAddress();

        IEnumerable<EmployeeOutputDTO> GetEmployees();

        void IncreaseEmployeesSalariesRange(int fromID, int toID);

        void IncreaseSingleEmployeeSalary(int id);
    }
}
