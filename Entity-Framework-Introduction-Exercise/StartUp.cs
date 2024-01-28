using EfIntroductionExerciseDbFirst.Data;
using EfIntroductionExerciseDbFirst.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EfIntroductionExerciseDbFirst
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();

            string employeesInfo = GetEmployeesFullInformation(context);
            Console.WriteLine(employeesInfo);

            string employeesOver50000Salary = GetEmployeesWithSalaryOver50000(context);
            Console.WriteLine(employeesOver50000Salary);

            string employeesFromReaserchAndDevDepart = GetEmployeesFromResearchAndDevelopment(context);
            Console.WriteLine(employeesFromReaserchAndDevDepart);

            string newAddressOfNakov = AddNewAddressToEmployee(context);
            Console.WriteLine(newAddressOfNakov);

            string employeesProjects = GetEmployeesInPeriod(context);
            Console.WriteLine(employeesProjects);

            string addressesByTowns = GetAddressesByTown(context);
            Console.WriteLine(addressesByTowns);

            string employee147 = GetEmployee147(context);
            Console.WriteLine(employee147);

            string deparmentsMoreThan5Employees = GetDepartmentsWithMoreThan5Employees(context);
            Console.WriteLine(deparmentsMoreThan5Employees);

            string latest10Projects = GetLatestProjects(context);
            Console.WriteLine(latest10Projects);

            string increaseSalaries = IncreaseSalaries(context);
            Console.WriteLine(increaseSalaries);

            string employeesStartWithSa = GetEmployeesByFirstNameStartingWithSa(context);
            Console.WriteLine(employeesStartWithSa);

            string deleteProjectById = DeleteProjectById(context);
            Console.WriteLine(deleteProjectById);

            string removeTown = RemoveTown(context);
            Console.WriteLine(removeTown);
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .OrderBy(x => x.EmployeeId)
                .Select(x => $"{x.FirstName} {x.LastName} {x.MiddleName} {x.JobTitle} {x.Salary:F2}");

            return string.Join(Environment.NewLine, employees);
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.Salary > 50000)
                .OrderBy(x => x.FirstName)
                .Select(x => $"{x.FirstName} - {x.Salary:F2}");

            return string.Join(Environment.NewLine, employees);
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employeesInfo = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => $"{e.FirstName} {e.LastName} from Research and Development - {e.Salary:F2}")
                .ToList();


            return string.Join(Environment.NewLine, employeesInfo);
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            context.Addresses.Add(new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            });

            context.SaveChanges();

            var employee = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");
            int addressId = context.Addresses
                .FirstOrDefault(a => a.AddressText == "Vitoshka 15").AddressId;
            employee.AddressId = addressId;

            context.SaveChanges();

            var resutl = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Select(x => x.Address.AddressText)
                .Take(10);

            return string.Join(Environment.NewLine, resutl);
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(p => p.Projects.Any(p => p.StartDate.Year >= 2001 && p.StartDate.Year <= 2003))
                .Take(10)
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    Lastname = x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Projects = x.Projects.Select(p =>
                        $"{p.Name} - {p.StartDate.ToString("M/d/yyyy h:mm:ss tt")} - {(p.EndDate.HasValue ? p.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished")}")
                });

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.Lastname} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach (var project in employee.Projects)
                {
                    sb.AppendLine($"--{project}");
                }
            }

            return sb.ToString();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .OrderByDescending(x => x.Employees.Count())
                .ThenBy(x => x.Town.Name)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .Select(x => $"{x.AddressText}, {x.Town.Name}, {x.Employees.Count()} employees");

            return string.Join(Environment.NewLine, addresses);
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                .Where(x => x.EmployeeId == 147)
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    Lastname = x.LastName,
                    JobTitle = x.JobTitle,
                    Projects = x.Projects
                        .OrderBy(p => p.Name)
                        .Select(p => p.Name)
                }).SingleOrDefault();

            var resutlString = $"{employee.FirstName} {employee.Lastname} - {employee.JobTitle}" +
                               $"{Environment.NewLine}{string.Join(Environment.NewLine, employee.Projects)}";

            return resutlString;
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(e => e.Employees.Count() > 5)
                .OrderBy(e => e.Employees.Count())
                .ThenBy(d => d.Name)
                .Select(x => new
                {
                    DepName = x.Name,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Employees = x.Employees
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle}")
                });

            var sb = new StringBuilder();

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.DepName} - {department.ManagerFirstName} {department.ManagerLastName}");
                sb.AppendLine(string.Join(Environment.NewLine, department.Employees));
            }

            return sb.ToString();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => $"{p.Name}{Environment.NewLine}{p.Description}{Environment.NewLine}{p.StartDate.ToString("M/dd/yyyy h:mm:ss tt")}");

            return string.Join(Environment.NewLine, projects);
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" ||
                            e.Department.Name == "Tool Design" ||
                            e.Department.Name == "Marketing" ||
                            e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName);

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                employee.Salary *= 1.12M;
                sb.AppendLine($"{employee.FirstName} {employee.LastName} ({employee.Salary:F2})");
            }

            context.SaveChanges();

            return sb.ToString();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            string startString = "Sa".ToLower();

            var result = context.Employees
                .Where(e => e.FirstName.ToLower().StartsWith(startString))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle} - ({e.Salary:F2})");

            return string.Join(Environment.NewLine, result);
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectsToDelete = context.Projects.Find(2);

            if (projectsToDelete != null)
            {
                var employeesWithProject2 = context.Employees
                .Include(e => e.Projects)
                .Where(e => e.Projects.Any(p => p.ProjectId == 2));

                foreach (var employee in employeesWithProject2)
                {
                    var projectToRemove = employee.Projects.FirstOrDefault(p => p.ProjectId == 2);

                    if (projectToRemove != null)
                    {
                        employee.Projects.Remove(projectToRemove);
                    }
                }

                context.Projects.Remove(projectsToDelete);
                context.SaveChanges();
            }

            var tenProjects = context.Projects
                .Take(10)
                .Select(p => p.Name);

            return string.Join(Environment.NewLine, tenProjects);
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var townToRemove = context.Towns.FirstOrDefault(t => t.Name == "Seattle");

            if (townToRemove != null)
            {
                var addressesInTown = context.Addresses
                    .Where(a => a.TownId == townToRemove.TownId)
                    .ToList();

                foreach (var address in addressesInTown)
                {
                    var employeesWithAddress = context.Employees
                        .Where(e => e.AddressId == address.AddressId);

                    foreach (var employee in employeesWithAddress)
                    {
                        employee.AddressId = null;
                    }
                }

                context.Addresses.RemoveRange(addressesInTown);
                context.SaveChanges();

                context.Towns.Remove(townToRemove);
                context.SaveChanges();

                return $"{addressesInTown.Count()} addresses in Seattle were deleted.";
            }
            else
            {
                return "Town not found.";
            }
        }
    }
}
