namespace PedrosCantina.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsManager { get; set; }
    }
}
