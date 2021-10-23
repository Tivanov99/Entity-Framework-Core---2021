using SoftUni.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftUni
{
    class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();
            Console.WriteLine(GetEmployeesFullInformation(context)) ;
        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.Select(x => x).OrderBy(x => x.EmployeeId).ToList();
            List<string> emp = new List<string>();
            foreach (var employee in employees)
            {
                emp.Add($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
                //Console.WriteLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }
            return string.Join("\n", emp);
        }
    }
    //Scaffold-DbContext -Connection "Server=DESKTOP-6RL5K65;Database=SoftUni;Integrated Security=True;TrustServerCertificate=True;" -Provider Microsoft.EntityFrameworkCore.SqlServer -OutputDir Data/Models
}
