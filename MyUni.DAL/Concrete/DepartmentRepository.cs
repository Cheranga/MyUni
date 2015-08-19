using System;
using System.Data.Entity;
using MyUni.Business;

namespace MyUni.DAL.Concrete
{
    public class DepartmentRepository : GenericRepository<Department>
    {
        public DepartmentRepository(DbContext context) : base(context)
        {
        }

        public override void Update(Department departmentToUpdate)
        {
            if (departmentToUpdate == null)
            {
                return;
            }

            var dbEntity = this.Context.Entry(departmentToUpdate);
            dbEntity.OriginalValues["RowVersion"] = departmentToUpdate.RowVersion;

            departmentToUpdate.RowVersion = Guid.NewGuid().ToByteArray();

            dbEntity.State = EntityState.Modified;
        }
    }
}