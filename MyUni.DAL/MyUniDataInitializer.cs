using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using MyUni.Business;

namespace MyUni.DAL
{
    public class MyUniDataInitializer : DropCreateDatabaseIfModelChanges<MyUniDbContext>
    {
        protected override void Seed(MyUniDbContext context)
        {
            SeedStudents(context);
            SeedCourses(context);
            SeedEnrollments(context);
        }

        private void SeedStudents(MyUniDbContext context)
        {
            context.Students.AddOrUpdate(x => x.FirstName,
                new Student {FirstName = "Cheranga", LastName = "Hatangala", EnrolledDate = DateTime.Now},
                new Student { FirstName = "Murali", LastName = "Mishra", EnrolledDate = DateTime.Now },
                new Student { FirstName = "Van", LastName = "Nguyen", EnrolledDate = DateTime.Now }
                );

            context.SaveChanges();
        }

        private void SeedCourses(MyUniDbContext context)
        {
            context.Courses.AddOrUpdate(x => x.Title,
                new Course {Title = "C#", Credits = 5},
                new Course {Title = "Web Services", Credits = 4},
                new Course {Title = "Javascript", Credits = 3},
                new Course {Title = "Databases", Credits = 5},
                new Course {Title = "Professional Development", Credits = 3}
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
                    Course = context.Courses.FirstOrDefault(x => x.Title == "Databases"),
                    Grade = Grade.A
                },
                new Enrollment
                {
                    Student = context.Students.FirstOrDefault(x => x.FirstName == "van"),
                    Course = context.Courses.FirstOrDefault(x => x.Title == "Web Services"),
                    Grade = Grade.A
                }
                );

            context.SaveChanges();
        }

    }
}