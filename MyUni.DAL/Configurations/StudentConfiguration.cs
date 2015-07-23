using System.Data.Entity.ModelConfiguration;
using MyUni.Business;

namespace MyUni.DAL.Configurations
{
    public class StudentConfiguration : EntityTypeConfiguration<Student>
    {
        public StudentConfiguration()
        {
            this.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("FirstName");

            this.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            this.Ignore(x => x.FullName);
        }
    }
}