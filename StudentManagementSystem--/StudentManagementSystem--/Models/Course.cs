using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementSystem__.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Hours { get; set; }

        // Navigation Property for Students
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
