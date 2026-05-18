using Microsoft.EntityFrameworkCore;
using StudentManagementSystem__.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementSystem__.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure local SQL Server connection string
            optionsBuilder.UseSqlServer("Server=. ; Database=StudentManagementSystem; Trusted_Connection=True ; TrustServerCertificate=True");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure seeding data directly during migration creation
            var dept1 = new Department { Id = 1, Name = "Computer Science" };
            var dept2 = new Department { Id = 2, Name = "Mathematics" };
            var dept3 = new Department { Id = 3, Name = "Physics" };

            var course1 = new Course { Id = 1, Title = "C# Programming", Hours = 60 };
            var course2 = new Course { Id = 2, Title = "Database Systems", Hours = 45 };
            var course3 = new Course { Id = 3, Title = "Calculus I", Hours = 50 };
            var course4 = new Course { Id = 4, Title = "Quantum Physics", Hours = 55 };
            var course5 = new Course { Id = 5, Title = "Web Development", Hours = 40 };

            modelBuilder.Entity<Department>().HasData(dept1, dept2, dept3);
            modelBuilder.Entity<Course>().HasData(course1, course2, course3, course4, course5);

            // Note: Seed core students without shadow-properties for basic setup. 
            // Manual population via Code First Seeding runtime engine shown in main instead 
            // to safely bridge the EF Core configuration limits for implicit skip-join tables.
        }
    }
}