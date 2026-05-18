using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementSystem__.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;

        // Foreign Key & Navigation Property for Department (One-to-Many)
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        // Navigation Property for Courses (Many-to-Many)
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
