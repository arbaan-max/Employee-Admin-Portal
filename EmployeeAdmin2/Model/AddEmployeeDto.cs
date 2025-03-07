namespace EmployeeAdmin2.Model
{
    public class AddEmployeeDto
    {
        public required string Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public decimal Salary { get; set; }
    }
}
