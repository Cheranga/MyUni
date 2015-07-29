using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyUni.Business;

namespace MyUni.DAL.Configurations
{
    public class EnrollmentConfiguration : EntityTypeConfiguration<Enrollment>
    {
        public EnrollmentConfiguration()
        {
            this.HasRequired(x => x.Student).WithMany(x=>x.Enrollments).HasForeignKey(x => x.StudentId);
            this.HasRequired(x => x.Course).WithMany(x => x.Enrollments).HasForeignKey(x => x.CourseId);

            this.Property(x => x.Grade).IsOptional();
        }
    }
}
