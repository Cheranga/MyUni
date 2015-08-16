using System.Data.Entity;
using System.Linq;
using MyUni.Business;

namespace MyUni.DAL.Concrete
{
    public class InstructorRepository : GenericRepository<Instructor>
    {
        public InstructorRepository(DbContext context) : base(context)
        {
        }

        //public override IQueryable<Instructor> GetAll()
        //{
        //    return this.dbSet
        //        .Include(x => x.OfficeAssignment)
        //        .Include(x => x.Courses.Select(y => y.Department))
        //        .Include(x => x.Courses.Select(y => y.Enrollments.Select(z => z.Student)));
        //}

        public override Instructor Add(Instructor instructor)
        {
            if (instructor == null)
            {
                return null;
            }

            //
            // Since this is a new entity (parent entity), the courses (the child entities) will also be considered as new entities. To avoid adding new courses, need to inform EF, that the course entities state is "Unchanged"
            //
            var trackedInstructorEntity = this.Context.Entry(instructor);
            trackedInstructorEntity.State = EntityState.Added;

            trackedInstructorEntity.Entity.Courses.ToList().ForEach(x => this.Context.Entry(x).State = EntityState.Unchanged);

            return trackedInstructorEntity.Entity;

        }

        public override void Update(Instructor instructorToUpdate)
        {
            if (instructorToUpdate == null)
            {
                return;
            }

            //
            // Get the entity in the database
            //
            var instructor = this.GetAll().Where(x => x.Id == instructorToUpdate.Id)
                .Include(x => x.Courses)
                .Include(x => x.Courses.Select(y => y.Department))
                .FirstOrDefault();

            if (instructor == null)
            {
                return;
            }

            var instructorDbEntity = this.Context.Entry(instructor);
            var instructorEntity = instructorDbEntity.Entity;
            //
            // Set first name, last name and hire date
            //
            instructorEntity.FirstName = instructorToUpdate.FirstName;
            instructorEntity.LastName = instructorToUpdate.LastName;
            instructorEntity.HireDate = instructorToUpdate.HireDate;
            //
            // Remove the current courses
            //
            instructorEntity.Courses.Clear();
            //
            // If there are courses to add, add them
            //
            if (instructorToUpdate.Courses != null)
            {
                //
                // http://jasonjtyler.blogspot.com.au/2013/01/using-contains-with-entity-dbset.html
                //
                //
                // Get all courses using uow
                //
                var currentCoursesInSystem = this.UoW.Get<Course>();
                if (currentCoursesInSystem != null)
                {
                    var instructorCourseIdList = instructorToUpdate.Courses.Select(x => x.Id).ToList();

                    var dbCourses = currentCoursesInSystem.Where(x => instructorCourseIdList.Contains(x.Id)).ToList();

                    dbCourses.ForEach(x => instructorEntity.Courses.Add(x));
                }
            }
            //
            // Setting of the office assignment
            //
            // 1. If there is no office assignment now, but if there was an office assignment in the database it needs to be deleted.
            // 2. If there is an office assignment now, it needs to be inserted or updated.
            //
            var currentOfficeAssignmentExists = instructorToUpdate.OfficeAssignment != null &&
                                                !string.IsNullOrEmpty(instructorToUpdate.OfficeAssignment.Location);

            if (currentOfficeAssignmentExists)
            {
                if (instructorEntity.OfficeAssignment == null)
                {
                    instructorEntity.OfficeAssignment = new OfficeAssignment
                    {
                        Location = instructorToUpdate.OfficeAssignment.Location
                    };
                }
                else
                {
                    instructorEntity.OfficeAssignment.Location = instructorToUpdate.OfficeAssignment.Location;
                }
            }
            else
            {
                var dbSetOfficeAssignment = this.Context.Set<OfficeAssignment>();
                if (dbSetOfficeAssignment == null)
                {
                    return;
                }

                var databaseOfficeAssignment = dbSetOfficeAssignment.FirstOrDefault(x => x.InstructorId == instructorToUpdate.Id);
                if (databaseOfficeAssignment != null)
                {
                    this.Context.Entry(databaseOfficeAssignment).State = EntityState.Deleted;
                }
                
            }

            instructorDbEntity.State = EntityState.Modified;

        }
    }
}