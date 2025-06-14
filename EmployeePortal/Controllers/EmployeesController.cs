using EmployeePortal.Data;
using EmployeePortal.Models;
using EmployeePortal.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Drawing;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

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
        public async Task<IActionResult> Edit(AddEmployeeViewModel viewmodel)
        {
           var employee = await dbContext.Employees.FindAsync(viewmodel.Id);
            if (viewmodel.ImageData != null && viewmodel.ImageData.Length > 0) {
                using (var memoryStream = new MemoryStream())
                {
                    await viewmodel.ImageData.CopyToAsync(memoryStream);
                    if (employee is not null)
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
                        employee.Hra = viewmodel.Hra;
                        employee.FileName= viewmodel.FileName;
                        employee.ImageData= memoryStream.ToArray();
                        await dbContext.SaveChangesAsync();
                    }
                    return RedirectToAction("List", "Employees");
                };
            }
            if (employee is not null)
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
                employee.Hra = viewmodel.Hra;
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
        public IActionResult ExportToExcel()
        {
            // Get images from database
             var employees = dbContext.Employees.ToList();

            // Create a memory stream to hold the Excel file data
            using (var package = new ExcelPackage())
            {
                // Add a worksheet to the Excel file
                var worksheet = package.Workbook.Worksheets.Add("Employee");

                // Set the headers for the Excel sheet

                worksheet.Cells[1, 1].Value = "NAME";
                worksheet.Cells[1, 2].Value = "GENDER";
                worksheet.Cells[1, 3].Value = "PHONE";
                worksheet.Cells[1, 4].Value = "EMAIL";
                worksheet.Cells[1, 5].Value = "BASICSALARY";
                worksheet.Cells[1, 6].Value = "HRA";
                worksheet.Cells[1, 7].Value = "CONVENIENCE";
                worksheet.Cells[1, 8].Value = "TOTALSALARY";
                worksheet.Cells[1, 9].Value = "CITY";
                worksheet.Cells[1, 10].Value = "FILENAME";
                worksheet.Cells[1, 11].Value = "IMAGE";             
               

                // Insert images into the worksheet
                int row = 2;
                foreach (var employee in employees)
                {
                    worksheet.Cells[row, 1].Value = employee.Name;
                    worksheet.Cells[row, 2].Value = employee.Gender;
                    worksheet.Cells[row, 3].Value = employee.Phone;
                    worksheet.Cells[row, 4].Value = employee.Email;
                    worksheet.Cells[row, 5].Value = employee.BasicSalary;
                    worksheet.Cells[row, 6].Value = employee.Hra;
                    worksheet.Cells[row, 7].Value = employee.Convenience;
                    worksheet.Cells[row, 8].Value = employee.TotalSalary;
                    worksheet.Cells[row, 9].Value = employee.City;
                    worksheet.Cells[row, 10].Value = employee.FileName;
                    if (employee.ImageData != null && employee.ImageData.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream(employee.ImageData))
                        {
                            // Create an Image from the byte array
                            var img = System.Drawing.Image.FromStream(memoryStream);

                            // Convert the image to MemoryStream (this is compatible with EPPlus)
                            using (var imgMemoryStream = new MemoryStream())
                            {
                                // Save the image to the MemoryStream in PNG format
                                img.Save(imgMemoryStream, System.Drawing.Imaging.ImageFormat.Png);

                                // Make sure to set the position of the stream to the beginning
                                imgMemoryStream.Seek(0, SeekOrigin.Begin);

                                // Add the image to the worksheet (using the Image, not the MemoryStream)
                                var excelImage = worksheet.Drawings.AddPicture($"Image_{row}", img);

                                // Set image size (optional)
                                excelImage.SetSize(50, 50);
                                excelImage.SetPosition(row - 1, 0, 10, 0); // Adjust the position of the image in the sheet
                            }

                        }
                    }

                       

                    row++;
                }

                // Save the Excel file to a memory stream
                var fileContents = package.GetAsByteArray();

                // Return the file as a download
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
            }
        }
    }
}
