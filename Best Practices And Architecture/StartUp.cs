using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyPorject.Data;
using MyProject.Services.EmployeeContracts.Contracts;
using MyProject.Services.Services;
using System;

namespace MyProject.ConsoleApp
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            var service = new ServiceCollection()
            .AddDbContext<SoftUniContext>(opt => opt
            .UseSqlServer("Server=.;Database=SoftUni;Integrated Security=True"))
            .AddScoped<IEmployeeService, EmployeeService>()
            .BuildServiceProvider();


            var employeeService = service.GetService<IEmployeeService>();

            var result = employeeService.GetEmployeeById(1);
            Console.WriteLine(result.EmployeeFullName);
        }
    }
}
