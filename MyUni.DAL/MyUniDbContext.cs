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
        public IDbSet<Student> Students { get; set; }
        public IDbSet<Course> Courses { get; set; }
        public IDbSet<Enrollment> Enrollments { get; set; }

        public static string MyUniConnectionString
        {
            get
            {
                var conStringName = ConfigurationManager.AppSettings.Get("MyUniConnectionStringName");
                return conStringName;
            }
        }

        public MyUniDbContext():base(MyUniConnectionString)
        {
            //this.Configuration.LazyLoadingEnabled = false;

            this.Database.Log = (x) => Debug.WriteLine(x);
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
            
        }
    }
}
