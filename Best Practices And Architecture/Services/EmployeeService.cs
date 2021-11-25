namespace MyProject.Services.Services
{
    using Microsoft.EntityFrameworkCore;
    using MyPorject.Data;
    using MyProject.Services.DTO_S.EmployeeDTO_S;
    using MyProject.Services.EmployeeContracts.Contracts;
    using System.Collections.Generic;
    using System.Linq;
    public class EmployeeService : IEmployeeService
    {
        private readonly SoftUniContext _dbContext;
        public EmployeeService(SoftUniContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<EmployeeAddressOutputDTO> GetEmployeeAddress()
        {
            HashSet<EmployeeAddressOutputDTO> employees = _dbContext
                .Employees
                .AsNoTracking()
                .Select(x => new EmployeeAddressOutputDTO()
                {
                    EmployeeFullName = $"{x.FirstName} {x.MiddleName} {x.LastName}",
                    AddressText = x.Address.AddressText,
                    TownName = x.Address.Town.Name
                })
                .ToHashSet();

            return employees;
        }

        public EmployeeOutputDTO GetEmployeeById(int id)
        {
            EmployeeOutputDTO employeById = _dbContext
                .Employees
                .AsNoTracking()
                .Where(e => e.EmployeeId == id)
                .Select(x => new EmployeeOutputDTO()
                {
                    EmployeeFullName = $"{x.FirstName} {x.MiddleName} {x.LastName}",
                    JobTitle = x.JobTitle,
                    DepartmentName = x.Department.Name,
                    Salary = x.Salary
                })
                .FirstOrDefault();

            return employeById;
        }

        public EmployeeDepartmentsOutputDTO GetEmployeeDepartment(int id)
        {
            EmployeeDepartmentsOutputDTO employee = _dbContext
                .Employees
                .Where(x => x.EmployeeId == id)
                .AsNoTracking()
                .Select(x => new EmployeeDepartmentsOutputDTO()
                {
                    EmployeeFullName = $"{x.FirstName} {x.MiddleName} {x.LastName}",
                    DepartmentName = x.Department.Name
                })
                .FirstOrDefault();

            return employee;
        }

        public IEnumerable<EmployeeOutputDTO> GetEmployees()
        {
            HashSet<EmployeeOutputDTO> employees = _dbContext
                .Employees
                .AsNoTracking()
                .Select(x => new EmployeeOutputDTO()
                {
                    EmployeeFullName = $"{x.FirstName} {x.MiddleName} {x.LastName}",
                    JobTitle = x.JobTitle,
                    DepartmentName = x.Department.Name,
                    Salary = x.Salary
                })
                .ToHashSet();

            return employees;
        }

        public EmployeeOutputDTO GetEmployeeWithHightSalary()
        {
            EmployeeOutputDTO employee = _dbContext
                .Employees
                .AsNoTracking()
                .OrderByDescending(x => x.Salary)
                .Select(x => new EmployeeOutputDTO()
                {
                    EmployeeFullName = $"{x.FirstName} {x.MiddleName} {x.LastName}",
                    JobTitle = x.JobTitle,
                    DepartmentName = x.Department.Name,
                    Salary = x.Salary
                })
                .FirstOrDefault();

            return employee;
        }

        public void IncreaseEmployeesSalariesRange(int fromID, int toID)
        {
            var result = _dbContext
                 .Employees
                 .Where(x => x.EmployeeId >= fromID && x.EmployeeId <= toID)
                 .ToList();

            foreach (var employee in result)
            {
                employee.Salary *= 2;
            }
            _dbContext.SaveChanges();
        }

        public void IncreaseSingleEmployeeSalary(int id)
        {
            var employee = _dbContext
                .Employees
                .Where(x => x.EmployeeId == id)
                .FirstOrDefault();

            employee.Salary *= employee.Salary;
            _dbContext.SaveChanges();
        }
    }
}
