using EmployeePortal.Data;
using EmployeePortal.Models;
using EmployeePortal.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public EmployeesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel viewModel)
        {
            if (viewModel.ImageData != null && viewModel.ImageData.Length > 0)
            {
                using (var memoryStream = new MemoryStream()) {
                    await viewModel.ImageData.CopyToAsync(memoryStream);
                    var employeeData = new Models.Entities.Employee
                    {
                        Name = viewModel.Name,
                        Email = viewModel.Email,
                        Gender = viewModel.Gender,
                        Hra = viewModel.Hra,
                        Convenience = viewModel.Convenience,
                        BasicSalary = viewModel.BasicSalary,
                        City = viewModel.City,
                        Phone = viewModel.Phone,
                        TotalSalary = viewModel.Hra+viewModel.Convenience+viewModel.BasicSalary,
                        FileName = viewModel.ImageData.FileName,
                        ImageData =memoryStream.ToArray(),
                    };
                    await dbContext.Employees.AddAsync(employeeData);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("List", "Employees");
                }
                   
            }
            var employee = new Models.Entities.Employee
            {
                Name = viewModel.Name,
                Email = viewModel.Email,
                Gender = viewModel.Gender,
                Hra = viewModel.Hra,
                Convenience = viewModel.Convenience,
                BasicSalary = viewModel.BasicSalary,
                City = viewModel.City,
                Phone = viewModel.Phone,
                TotalSalary = viewModel.TotalSalary,                
            };
            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List", "Employees");

        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            var employees = await dbContext.Employees.ToListAsync();
                return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee =await dbContext.Employees.FindAsync(id);
            return View(employee);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Employee viewmodel)
        {
           var employee = await dbContext.Employees.FindAsync(viewmodel.Id);
            if(employee is not null)
            {
                employee.Name = viewmodel.Name;
                employee.Email = viewmodel.Email;
                employee.Phone = viewmodel.Phone;
                employee.TotalSalary = viewmodel.Hra + viewmodel.Convenience + viewmodel.BasicSalary;
                employee.FileName = viewmodel.FileName;
                employee.BasicSalary = viewmodel.BasicSalary;
                employee.Gender = viewmodel.Gender;
                employee.City = viewmodel.City;
                employee.Convenience = viewmodel.Convenience;
                employee.Hra=viewmodel.Hra;
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Employees");

        }

        [HttpPost]
        public async Task<IActionResult>Delete(Employee viewModel)
        {
            var employee = await dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == viewModel.Id);
            if(employee is not null)
            {
                dbContext.Employees.Remove(viewModel);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Employees");

        }
    }
}
