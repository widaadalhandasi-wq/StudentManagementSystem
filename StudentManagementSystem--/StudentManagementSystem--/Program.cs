using Microsoft.EntityFrameworkCore;
using StudentManagementSystem__.Data;
using StudentManagementSystem__.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentManagementSystem__

{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new AppDbContext())
            {
                // Ensure Db is created and dynamic seeding is applied if empty
                context.Database.EnsureCreated();
                SeedDataRuntime(context);

                bool running = true;
                while (running)
                {
                    Console.Clear();
                    Console.WriteLine("========================================");
                    Console.WriteLine("   STUDENT COURSE MANAGEMENT SYSTEM   ");
                    Console.WriteLine("========================================");
                    Console.WriteLine("1. Add Student");
                    Console.WriteLine("2. Update Student");
                    Console.WriteLine("3. Delete Student");
                    Console.WriteLine("4. Get All Students (Pagination & Include)");
                    Console.WriteLine("5. Search Student");
                    Console.WriteLine("6. Execute Advanced LINQ Requirements");
                    Console.WriteLine("7. View Department Performance Report");
                    Console.WriteLine("8. Exit");
                    Console.WriteLine("========================================");
                    Console.Write("Select an option: ");

                    string choice = Console.ReadLine() ?? "";
                    Console.Clear();

                    try
                    {
                        switch (choice)
                        {
                            case "1": AddStudent(context); break;
                            case "2": UpdateStudent(context); break;
                            case "3": DeleteStudent(context); break;
                            case "4": GetAllStudents(context); break;
                            case "5": SearchStudent(context); break;
                            case "6": RunLinqRequirements(context); break;
                            case "7": RunDepartmentReport(context); break;
                            case "8": running = false; break;
                            default: Console.WriteLine("Invalid option. Press any key to try again..."); Console.ReadKey(); break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[ERROR] An error occurred: {ex.Message}");
                        Console.ResetColor();
                        Console.WriteLine("Press any key to return to main menu...");
                        Console.ReadKey();
                    }
                }
            }
        }

        #region Seed Runtime Routine (Part 6)
        private static void SeedDataRuntime(AppDbContext context)
        {
            if (context.Students.Any()) return; // Database already contains seeded records

            var departments = context.Departments.ToList();
            var courses = context.Courses.ToList();

            var studentSeeds = new List<Student>
            {
                new Student { Name = "Alice Smith", Age = 21, Email = "alice@uni.edu", DepartmentId = departments[0].Id },
                new Student { Name = "Bob Jones", Age = 19, Email = "bob@uni.edu", DepartmentId = departments[0].Id },
                new Student { Name = "Charlie Brown", Age = 23, Email = "charlie@uni.edu", DepartmentId = departments[1].Id },
                new Student { Name = "Diana Prince", Age = 26, Email = "diana@uni.edu", DepartmentId = departments[2].Id },
                new Student { Name = "Evan Wright", Age = 22, Email = "evan@uni.edu", DepartmentId = departments[1].Id },
                new Student { Name = "Fiona Gallagher", Age = 20, Email = "fiona@uni.edu", DepartmentId = departments[0].Id },
                new Student { Name = "George Brooks", Age = 24, Email = "george@uni.edu", DepartmentId = departments[2].Id },
                new Student { Name = "Hannah Abbott", Age = 18, Email = "hannah@uni.edu", DepartmentId = departments[0].Id },
                new Student { Name = "Ian Malcolm", Age = 27, Email = "ian@uni.edu", DepartmentId = departments[1].Id },
                new Student { Name = "Julia Roberts", Age = 21, Email = "julia@uni.edu", DepartmentId = departments[2].Id }
            };

            // Associate students to random intersecting packages of courses
            var random = new Random();
            foreach (var student in studentSeeds)
            {
                // Assign 2 distinct random courses to each student
                var assignedCourses = courses.OrderBy(c => random.Next()).Take(2).ToList();
                foreach (var course in assignedCourses)
                {
                    student.Courses.Add(course);
                }
            }

            context.Students.AddRange(studentSeeds);
            context.SaveChanges();
        }
        #endregion

        #region CRUD Operations (Part 7 & Bonus validation)
        private static void AddStudent(AppDbContext context)
        {
            Console.WriteLine("--- Add New Student ---");
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Enter Age: ");
            if (!int.TryParse(Console.ReadLine(), out int age) || age <= 0) throw new Exception("Invalid Student Age Entered.");

            Console.Write("Enter Email: ");
            string email = Console.ReadLine() ?? "";

            // Bonus validation: Prevent duplicate emails
            if (context.Students.Any(s => s.Email.ToLower() == email.ToLower()))
            {
                throw new Exception("A student record with this exact email already exists inside the system.");
            }

            Console.WriteLine("\nAvailable Departments:");
            var depts = context.Departments.ToList();
            depts.ForEach(d => Console.WriteLine($"ID: {d.Id} | Name: {d.Name}"));
            Console.Write("Select Department ID: ");
            if (!int.TryParse(Console.ReadLine(), out int deptId) || !depts.Any(d => d.Id == deptId)) throw new Exception("Selected Department ID does not exist.");

            var student = new Student
            {
                Name = name,
                Age = age,
                Email = email,
                DepartmentId = deptId
            };

            context.Students.Add(student);
            context.SaveChanges();
            Console.WriteLine("\nStudent Record Successfully Added!");
            Console.ReadKey();
        }

        private static void UpdateStudent(AppDbContext context)
        {
            Console.WriteLine("--- Update Student ---");
            Console.Write("Enter Target Student ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) throw new Exception("Invalid ID layout.");

            var student = context.Students.Find(id);
            if (student == null) throw new Exception("Requested Student Record not located.");

            Console.Write($"Enter New Name (Current: {student.Name}): ");
            string name = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(name)) student.Name = name;

            Console.Write($"Enter New Age (Current: {student.Age}): ");
            string ageInput = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(ageInput) && int.TryParse(ageInput, out int age)) student.Age = age;

            Console.Write($"Enter New Email (Current: {student.Email}): ");
            string email = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(email))
            {
                if (context.Students.Any(s => s.Email.ToLower() == email.ToLower() && s.Id != student.Id))
                    throw new Exception("Email variation conflict: Email alternative belongs to another active user profile.");
                student.Email = email;
            }

            context.SaveChanges();
            Console.WriteLine("\nStudent Details Updated Successfully.");
            Console.ReadKey();
        }

        private static void DeleteStudent(AppDbContext context)
        {
            Console.WriteLine("--- Delete Student ---");
            Console.Write("Enter Target Student ID to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) throw new Exception("Invalid input sequence mapping.");

            var student = context.Students.Find(id);
            if (student == null) throw new Exception("Student record not found within data registry.");

            context.Students.Remove(student);
            context.SaveChanges();
            Console.WriteLine("\nStudent record permanently purged.");
            Console.ReadKey();
        }

        private static void GetAllStudents(AppDbContext context)
        {
            Console.WriteLine("--- All Registered Students ---");

            // Bonus implementation: Pagination elements
            int pageSize = 4;
            int totalRecords = context.Students.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            int currentPage = 1;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"--- Page {currentPage} of {totalPages} ---");

                var students = context.Students
                    .Include(s => s.Department)
                    .Include(s => s.Courses)
                    .OrderBy(s => s.Id)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var s in students)
                {
                    string courseList = s.Courses.Any() ? string.Join(", ", s.Courses.Select(c => c.Title)) : "None";
                    Console.WriteLine($"[ID: {s.Id}] Name: {s.Name} | Age: {s.Age} | Dept: {s.Department.Name} | Courses: [{courseList}]");
                }

                Console.WriteLine("\nOptions: [N] Next Page | [P] Previous Page | [B] Back to Main Menu");
                Console.Write("Select navigation rule: ");
                string nav = Console.ReadLine()?.ToUpper() ?? "";

                if (nav == "N" && currentPage < totalPages) currentPage++;
                else if (nav == "P" && currentPage > 1) currentPage--;
                else if (nav == "B") break;
            }
        }

        private static void SearchStudent(AppDbContext context)
        {
            Console.WriteLine("--- Search Filter Engine ---");
            Console.Write("Enter Name segment to query (or click return to pass): ");
            string queryName = Console.ReadLine() ?? "";

            Console.Write("Enter age context query parameter (or click return to pass): ");
            string queryAgeInput = Console.ReadLine() ?? "";

            var baseQuery = context.Students.Include(s => s.Department).Include(s => s.Courses).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryName))
            {
                baseQuery = baseQuery.Where(s => s.Name.Contains(queryName));
            }

            if (!string.IsNullOrWhiteSpace(queryAgeInput) && int.TryParse(queryAgeInput, out int searchAge))
            {
                baseQuery = baseQuery.Where(s => s.Age == searchAge);
            }

            var results = baseQuery.ToList();

            Console.WriteLine($"\nFound {results.Count} matches:");
            foreach (var s in results)
            {
                Console.WriteLine($"ID: {s.Id} | Name: {s.Name} | Age: {s.Age} | Dept: {s.Department.Name}");
            }
            Console.ReadKey();
        }
        #endregion

        #region LINQ Execution Rules (Part 8)
        private static void RunLinqRequirements(AppDbContext context)
        {
            Console.WriteLine("--- Executing Standard LINQ Checklist Operations --- \n");

            // 1. Where()
            var olderThan20 = context.Students.Where(s => s.Age > 20).ToList();
            Console.WriteLine($"* Students Older Than 20 (Count): {olderThan20.Count}");

            // 2. Select()
            var namesOnly = context.Students.Select(s => s.Name).ToList();
            Console.WriteLine($"* Isolated Profiles Matrix (Sample names via Select): {string.Join(", ", namesOnly.Take(3))}...");

            // 3. OrderBy()
            var sortedByAge = context.Students.OrderBy(s => s.Age).ToList();
            Console.WriteLine($"* Youngest Student: {sortedByAge.FirstOrDefault()?.Name} (Age: {sortedByAge.FirstOrDefault()?.Age})");

            // 4. Count()
            int count = context.Students.Count();
            Console.WriteLine($"* Master Global Student Registry Size (Count()): {count}");

            // 5. Any()
            bool existsAgeGt25 = context.Students.Any(s => s.Age > 25);
            Console.WriteLine($"* System evaluation flag checking if Any Student Age > 25: {existsAgeGt25}");

            // 6. Average()
            double runningAvg = context.Students.Average(s => s.Age);
            Console.WriteLine($"* System Mean Aggregate Age calculation: {runningAvg:F2}");

            // 7. GroupBy()
            var groupedRaw = context.Students.GroupBy(s => s.Department.Name).ToList();
            Console.WriteLine("* GroupBy Trace logs generated successfully. Iteration trace sample:");
            foreach (var grouping in groupedRaw)
            {
                Console.WriteLine($"  -> Dept: {grouping.Key} holds {grouping.Count()} target items.");
            }

            Console.WriteLine("\nTask completed cleanly. Press any key to return...");
            Console.ReadKey();
        }
        #endregion

        #region Advanced Requirement Engine (Part 9)
        private static void RunDepartmentReport(AppDbContext context)
        {
            Console.WriteLine("--- Department Analytical Performance Summary Report ---\n");

            var strategicReport = context.Students
                .GroupBy(s => s.Department.Name)
                .Select(group => new
                {
                    DepartmentName = group.Key,
                    NumberOfStudents = group.Count(),
                    AverageAge = group.Average(s => s.Age)
                })
                .ToList();

            Console.WriteLine(string.Format("{0,-25} | {1,-20} | {2,-15}", "Department Name", "Total Students", "Average Student Age"));
            Console.WriteLine(new string('-', 70));

            foreach (var analyticalMetric in strategicReport)
            {
                Console.WriteLine(string.Format("{0,-25} | {1,-20} | {2,-15:F1}",
                    analyticalMetric.DepartmentName,
                    analyticalMetric.NumberOfStudents,
                    analyticalMetric.AverageAge));
            }

            Console.WriteLine("\nPress any key to jump back into dashboard operations loop...");
            Console.ReadKey();
        }
        #endregion
    }
}