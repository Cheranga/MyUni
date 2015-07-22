using System.Data.Entity.ModelConfiguration;
using MyUni.Business;

namespace MyUni.DAL.Configurations
{
    public class StudentConfiguration : EntityTypeConfiguration<Student>
    {
        public StudentConfiguration()
        {
            this.Property(x => x.FirstName).IsRequired();
            this.Property(x => x.LastName).IsRequired();
        }
    }
}