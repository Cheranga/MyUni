using System.Collections.ObjectModel;
using System.Diagnostics;
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
            try
            {
                SeedInstructors(context);
                SeedOfficeAssignments(context);
                SeedDepartments(context);
                SeedStudents(context);
                SeedCourses(context);
                SeedEnrollments(context);
            }
            catch (Exception exception)
            {
                
            }
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

            var microsoft = context.Departments.FirstOrDefault(x => x.Name == "Microsoft");
            var clientTech = context.Departments.FirstOrDefault(x => x.Name == "Client Technologies");
            var databases = context.Departments.FirstOrDefault(x => x.Name == "Databases");
            var career = context.Departments.FirstOrDefault(x => x.Name == "Career Development");

            var bill = context.Instructors.FirstOrDefault(x => x.FirstName == "Bill");
            var mrCareer = context.Instructors.FirstOrDefault(x => x.FirstName == "Mr. Career");

            context.Courses.AddOrUpdate(x => x.Title,
                new Course { Title = "C#", Credits = 5, DepartmentId = microsoft.Id, Instructors = new Collection<Instructor>(new []{bill})},
                new Course { Title = "ASP.NET Web Api", Credits = 4, DepartmentId = microsoft.Id, Instructors = new Collection<Instructor>(new[] { bill }) },
                new Course { Title = "Javascript", Credits = 3, DepartmentId = clientTech.Id },
                new Course { Title = "MS SQL Server", Credits = 5, DepartmentId = databases.Id, Instructors = new Collection<Instructor>(new[] { bill }) },
                new Course { Title = "Professional Development", Credits = 3, DepartmentId = career.Id, Instructors = new Collection<Instructor>(new[] { mrCareer }) }
                );

            context.SaveChanges();
        }

        private void SeedEnrollments(MyUniDbContext context)
        {
            //
            // Students
            //
            var cheranga = context.Students.FirstOrDefault(x => x.FirstName == "cheranga");
            var murali = context.Students.FirstOrDefault(x => x.FirstName == "murali");
            var van = context.Students.FirstOrDefault(x => x.FirstName == "van");
            //
            // Courses
            //
            var cSharp = context.Courses.FirstOrDefault(x => x.Title == "C#");
            var msSqlServer = context.Courses.FirstOrDefault(x => x.Title == "MS SQL Server");
            var webApi = context.Courses.FirstOrDefault(x => x.Title == "ASP.NET Web Api");

            context.Enrollments.AddOrUpdate(x=>new{x.StudentId, x.CourseId},
                new Enrollment { StudentId = cheranga.Id, CourseId = cSharp.Id, Grade = Grade.A},
                new Enrollment { StudentId = murali.Id, CourseId = msSqlServer.Id, Grade = Grade.A },
                new Enrollment { StudentId = van.Id, CourseId = webApi.Id, Grade = Grade.A }
                );

            //
            // Enrollments for Cheranga
            //
            //var exists = context.Enrollments.FirstOrDefault(x => x.StudentId == cheranga.Id && x.CourseId == cSharp.Id) != null;
            //if (!exists)
            //{
            //    context.Enrollments.Add(new Enrollment {StudentId = cheranga.Id, CourseId = cSharp.Id});
            //}
            ////
            //// Enrollments for Murali
            ////
            //exists = context.Enrollments.FirstOrDefault(x => x.StudentId == murali.Id && x.CourseId == msSqlServer.Id) != null;
            //if (!exists)
            //{
            //    context.Enrollments.Add(new Enrollment { StudentId = murali.Id, CourseId = msSqlServer.Id });
            //}
            ////
            //// Enrollments for Van
            ////
            //exists = context.Enrollments.FirstOrDefault(x => x.StudentId == van.Id && x.CourseId == webApi.Id) != null;
            //if (!exists)
            //{
            //    context.Enrollments.Add(new Enrollment { StudentId = van.Id, CourseId = webApi.Id });
            //}

            context.SaveChanges();
        }
    }
}
