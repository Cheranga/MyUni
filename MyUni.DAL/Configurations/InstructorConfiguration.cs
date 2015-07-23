using System.Data.Entity.ModelConfiguration;
using MyUni.Business;

namespace MyUni.DAL.Configurations
{
    public class InstructorConfiguration : EntityTypeConfiguration<Instructor>
    {
        public InstructorConfiguration()
        {
            this.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            this.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            this.Ignore(x => x.FullName);

            this.HasMany(x => x.Courses).WithMany(x => x.Instructors);
            

        }
    }
}