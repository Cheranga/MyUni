using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUni.Business;
using MyUni.DAL.Configurations;

namespace MyUni.DAL
{
    public class MyUniDbContext : DbContext
    {
        //
        // NOTE: Changed from DbSet<T> to IDbSet<T> because, when you have entity configuration classes and, scaffolding code (which creates the related views), it throws an error
        //
        // http://stackoverflow.com/questions/24974218/scaffolding-controller-doesnt-work-with-visual-studio-2013-update-3-and-4
        //
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Instructor> Instructors { get; set; }
        public virtual DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public virtual DbSet<Department> Departments { get; set; }

        public static string MyUniConnectionString
        {
            get
            {
                var conStringName = ConfigurationManager.AppSettings.Get("MyUniConnectionStringName");
                return conStringName;
            }
        }

        public MyUniDbContext(string connectionString)
            : base(MyUniConnectionString)
        {
            Debug.WriteLine("MyUniDbContext created...");
            //
            // Disable lazy loading
            //
            this.Configuration.LazyLoadingEnabled = false;

            //
            // Redirect the log "Debug.WriteLine" when the project is in DEBUG mode
            //
//#if DEBUG
            this.Database.Log = (x) => Debug.WriteLine(x);
//#endif
        }

        public MyUniDbContext()
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //
            // Remove table name pluralization
            //
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //
            // Add entity configurations
            //
            modelBuilder.Configurations.Add(new EnrollmentConfiguration());
            modelBuilder.Configurations.Add(new StudentConfiguration());
            modelBuilder.Configurations.Add(new InstructorConfiguration());
            modelBuilder.Configurations.Add(new OfficeAssignmentConfiguration());
            modelBuilder.Configurations.Add(new CourseConfiguration());
            modelBuilder.Configurations.Add(new DepartmentConfiguration());

        }
    }
}
