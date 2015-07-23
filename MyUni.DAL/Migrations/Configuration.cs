using MyUni.Business;

namespace MyUni.DAL.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MyUniDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MyUniDbContext context)
        {
            //SeedInstructors(context);
            //SeedOfficeAssignments(context);
            //SeedDepartments(context);
            //SeedStudents(context);
            //SeedCourses(context);
            //SeedEnrollments(context);
        }

        private void SeedStudents(MyUniDbContext context)
        {
            context.Students.AddOrUpdate(x => x.FirstName,
                new Student { FirstName = "Cheranga", LastName = "Hatangala", EnrolledDate = DateTime.Now },
                new Student { FirstName = "Murali", LastName = "Mishra", EnrolledDate = DateTime.Now },
                new Student { FirstName = "Van", LastName = "Nguyen", EnrolledDate = DateTime.Now }
                );

            context.SaveChanges();
        }

        private void SeedInstructors(MyUniDbContext context)
        {
            context.Instructors.AddOrUpdate(x=>x.FirstName,
                new Instructor { FirstName = "Bill", LastName = "Gates", HireDate = DateTime.Now.AddYears(-50)},
                new Instructor { FirstName = "Mr. Career", LastName = "Agnostic", HireDate = DateTime.Now.AddYears(-50) }
                );

            context.SaveChanges();
        }

        private void SeedOfficeAssignments(MyUniDbContext context)
        {
            context.OfficeAssignments.AddOrUpdate(x=>x.Location,
                new OfficeAssignment { Location = "Silicon Valley", Instructor = context.Instructors.FirstOrDefault(x=>x.LastName == "Gates")},
                new OfficeAssignment { Location = "Google", Instructor = context.Instructors.FirstOrDefault(x => x.FirstName == "Mr. Career") }
                );

            context.SaveChanges();
        }

        private void SeedDepartments(MyUniDbContext context)
        {
            context.Departments.AddOrUpdate(x=>x.Name,
                new Department { Name = "Microsoft",Budget = 5000000, Administrator = context.Instructors.FirstOrDefault(x=>x.LastName == "Gates"),StartDate = DateTime.Now.AddYears(-40)},
                new Department { Name = "Career Development", Budget = 1000000, Administrator = context.Instructors.FirstOrDefault(x => x.FirstName == "Mr. Career"), StartDate = DateTime.Now.AddYears(-40) },
                new Department { Name = "Client Technologies", Budget = 1000000},
                new Department { Name = "Databases", Budget = 1000000},
                new Department { Name = "Web Services", Budget = 1000000}
                );

            context.SaveChanges();
        }

        private void SeedCourses(MyUniDbContext context)
        {
            context.Courses.AddOrUpdate(x => x.Title,
                new Course { Title = "C#", Credits = 5, Department = context.Departments.FirstOrDefault(x=>x.Name == "Microsoft")},
                new Course { Title = "ASP.NET Web Api", Credits = 4, Department = context.Departments.FirstOrDefault(x => x.Name == "Microsoft") },
                new Course { Title = "Javascript", Credits = 3, Department = context.Departments.FirstOrDefault(x => x.Name == "Client Technologies") },
                new Course { Title = "MS SQL Server", Credits = 5, Department = context.Departments.FirstOrDefault(x => x.Name == "Databases") },
                new Course { Title = "Professional Development", Credits = 3, Department = context.Departments.FirstOrDefault(x => x.Name == "Career Development") }
                );

            context.SaveChanges();
        }

        private void SeedEnrollments(MyUniDbContext context)
        {
            context.Enrollments.AddOrUpdate(
                new Enrollment
                {
                    Student = context.Students.FirstOrDefault(x => x.FirstName == "cheranga"),
                    Course = context.Courses.FirstOrDefault(x => x.Title == "C#"),
                    Grade = Grade.A
                },
                new Enrollment
                {
                    Student = context.Students.FirstOrDefault(x => x.FirstName == "murali"),
                    Course = context.Courses.FirstOrDefault(x => x.Title == "MS SQL Server"),
                    Grade = Grade.A
                },
                new Enrollment
                {
                    Student = context.Students.FirstOrDefault(x => x.FirstName == "van"),
                    Course = context.Courses.FirstOrDefault(x => x.Title == "ASP.NET Web Api"),
                    Grade = Grade.A
                }
                );

            context.SaveChanges();
        }
    }
}
