using System.Data.Entity.ModelConfiguration;
using MyUni.Business;

namespace MyUni.DAL.Configurations
{
    public class DepartmentConfiguration : EntityTypeConfiguration<Department>
    {
        public DepartmentConfiguration()
        {
            this.HasOptional(x => x.Administrator).WithMany(x => x.DepartmentsWhichAdministers).HasForeignKey(x => x.AdministratorId);

            this.Property(x => x.StartDate).HasColumnType("datetime2");

            //
            // Setting concurrency
            //
            this.Property(x => x.RowVersion).IsConcurrencyToken();

        }
    }
}