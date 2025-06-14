namespace EmployeePortal.Controllers
{
    public class AddEmployeeViewModel
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public string Gender { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }
        public int BasicSalary { get; set; }
        public int Hra { get; set; }
        public int Convenience { get; set; }
        public int TotalSalary { get; set; }
        public string City { get; set; }
        public byte[] ImageData { get; set; }
        public string FileName { get; set; }
    }
}