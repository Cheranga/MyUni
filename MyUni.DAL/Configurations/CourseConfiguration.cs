using System.Data.Entity.ModelConfiguration;
using MyUni.Business;

namespace MyUni.DAL.Configurations
{
    public class CourseConfiguration : EntityTypeConfiguration<Course>
    {
        public CourseConfiguration()
        {
            this.HasRequired(x => x.Department).WithMany(x => x.Courses).HasForeignKey(x => x.DepartmentId);
        }
    }
}