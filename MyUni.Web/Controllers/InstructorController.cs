using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyUni.Business;
using MyUni.DAL;
using MyUni.DAL.Concrete;
using MyUni.Web.ViewModels;
using StageDocs.DAL.Abstract;
using WebGrease.Css.Extensions;

namespace MyUni.Web.Controllers
{
    public class InstructorController : MyUniBaseController//Controller
    {
        public InstructorController(IUoW uow)
            : base(uow)
        {
        }

        public ActionResult Index(int? id, int? courseId)
        {
            var instructors = this.UoW.Get<Instructor>();
            if (instructors == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            instructors = instructors
                .Include(x => x.OfficeAssignment)
                .Include(x => x.Courses.Select(y => y.Department))
                .Include(x => x.Courses.Select(y => y.Enrollments.Select(z => z.Student)));

            var viewModel = new InstructorListViewModel
            {
                Instructors = instructors.OrderBy(x => x.FirstName)
            };

            if (id.HasValue)
            {
                var selectedInstructor = viewModel.Instructors.FirstOrDefault(x => x.Id == id);
                if (selectedInstructor != null)
                {
                    viewModel.Courses = selectedInstructor.Courses;

                    if (courseId.HasValue)
                    {
                        var selectedCourse = viewModel.Courses.FirstOrDefault(x => x.Id == courseId);
                        if (selectedCourse != null)
                        {
                            viewModel.Enrollments = selectedCourse.Enrollments;
                        }
                    }
                }
            }

            return View(viewModel);

        }

        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var repository = this.UoW.GetRepository<Instructor>();
            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var instructor = repository.GetById(id.Value);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructor/Create
        public ActionResult Create()
        {
            var repository = this.UoW.GetRepository<Course>();
            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var viewModel = new InstructorViewModel
            {
                HireDate = DateTime.Now,
                Courses = repository.GetAll().Select(x => new CourseViewModel { Id = x.Id, Name = x.Title, IsSelected = false }).ToList()
            };

            return View(viewModel);
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstructorViewModel instructorViewModel)
        {

            var repository = this.UoW.GetRepository<Instructor>();
            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (ModelState.IsValid)
            {
                //
                // If a location is given, need to create an office assignment record
                //
                var officeAssignment = string.IsNullOrEmpty(instructorViewModel.Location) ? null :
                    new OfficeAssignment { Location = instructorViewModel.Location };

                var instructor = new Instructor
                {
                    FirstName = instructorViewModel.FirstName,
                    LastName = instructorViewModel.LastName,
                    HireDate = instructorViewModel.HireDate,
                    OfficeAssignment = officeAssignment,
                    Courses = instructorViewModel.Courses.Where(x => x.IsSelected).Select(x => new Course { Id = x.Id }).ToList()
                };

                this.UoW.Commit(() =>
                {
                    repository.Add(instructor);
                    ////
                    //// Since this is a new entity (parent entity), the courses (the child entities) will also be considered as new entities. To avoid adding new courses, need to inform EF, that the course entities state is "Unchanged"
                    ////
                    //var trackedInstructorEntity = this.UoW.conte.Entry(instructor);
                    //trackedInstructorEntity.State = EntityState.Added;
                    //trackedInstructorEntity.Entity.Courses.ForEach(x => db.Entry(x).State = EntityState.Unchanged);
                });

                return RedirectToAction("Index");
            }

            return View(instructorViewModel);
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var instructors = this.UoW.Get<Instructor>();
            if (instructors == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var instructor = instructors
                .Include(x => x.OfficeAssignment)
                .Include(x => x.Courses)
                .FirstOrDefault(x => x.Id == id);

            //var instructor = db.Instructors
            //    .Include(x => x.OfficeAssignment)
            //    .Include(x => x.Courses)
            //    .FirstOrDefault(x => x.Id == id);

            if (instructor == null)
            {
                return HttpNotFound();
            }

            var instructorCourses = instructor.Courses.ToList();

            var coursesTaught = this.UoW.Get<Course>()
                .ToList()
                .Select(x => new CourseViewModel
                {
                    Id = x.Id,
                    Name = x.Title,
                    IsSelected = instructorCourses.Any(y => y.Title == x.Title)
                })
                .ToList();

            //var coursesTaught = db.Courses.ToList().Select(x => new CourseViewModel
            //{
            //    Id = x.Id,
            //    Name = x.Title,
            //    IsSelected = instructorCourses.Any(y => y.Title == x.Title)
            //}).ToList();

            var viewModel = new InstructorViewModel
            {
                Id = instructor.Id,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                HireDate = instructor.HireDate,
                Location = instructor.OfficeAssignment == null ? string.Empty : instructor.OfficeAssignment.Location,
                Courses = coursesTaught
            };

            return View(viewModel);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ActionName("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var instructors = this.UoW.Get<Instructor>(x => x.Id == id);
            if (instructors == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var instructorToUpdate = instructors
                .Include(x => x.OfficeAssignment)
                .Include(x => x.Courses)
                .FirstOrDefault();

            if (instructorToUpdate == null)
            {
                return new HttpNotFoundResult();
            }

            var instructorViewModel = new InstructorViewModel();

            try
            {
                if (TryUpdateModel(instructorViewModel, new[] { "Id", "FirstName", "LastName", "HireDate", "Location", "Courses" }))
                {
                    var updatedInstructor = new Instructor
                    {
                        Id = instructorViewModel.Id,
                        FirstName = instructorViewModel.FirstName,
                        LastName = instructorViewModel.LastName,
                        HireDate = instructorViewModel.HireDate,
                        Courses =
                            instructorViewModel.Courses == null
                                ? new List<Course>()
                                : instructorViewModel.Courses.Where(x => x.IsSelected)
                                    .Select(x => new Course { Id = x.Id })
                                    .ToList(),

                        OfficeAssignment =
                            new OfficeAssignment
                            {
                                InstructorId = instructorViewModel.Id,
                                Location = instructorViewModel.Location
                            }
                    };

                    this.UoW.Commit(() =>
                    {
                        var repository = this.UoW.GetRepository<Instructor>();
                        if (repository != null)
                        {
                            repository.Update(updatedInstructor);
                        }
                    });


                    //var instructorDbEntity = db.Entry(instructorToUpdate);

                    //var instructorEntity = instructorDbEntity.Entity;
                    //instructorEntity.FirstName = instructorViewModel.FirstName;
                    //instructorEntity.LastName = instructorViewModel.LastName;
                    //instructorEntity.HireDate = instructorViewModel.HireDate;
                    //instructorEntity.Courses.Clear();

                    //if (instructorViewModel.Courses != null)
                    //{
                    //    instructorViewModel.Courses.Where(x => x.IsSelected)
                    //        .ForEach(x => instructorEntity.Courses.Add(db.Courses.Find(x.Id)));
                    //}

                    //if (string.IsNullOrEmpty(instructorViewModel.Location))
                    //{
                    //    var currentOfficeAssignment = db.OfficeAssignments.FirstOrDefault(x => x.InstructorId == instructorToUpdate.Id);

                    //    if (currentOfficeAssignment != null)
                    //    {
                    //        db.Entry(currentOfficeAssignment).State = EntityState.Deleted;
                    //    }
                    //}
                    //else
                    //{
                    //    if (instructorEntity.OfficeAssignment == null)
                    //    {
                    //        instructorEntity.OfficeAssignment = new OfficeAssignment
                    //        {
                    //            Location = instructorViewModel.Location
                    //        };
                    //    }
                    //    else
                    //    {
                    //        instructorEntity.OfficeAssignment.Location = instructorViewModel.Location;
                    //    }

                    //}

                    //instructorDbEntity.State = EntityState.Modified;
                    //db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception exception)
            {

            }

            return View("Edit", id);



        }

        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var instructor = this.UoW.Get<Instructor>(x => x.Id == id).FirstOrDefault();

            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            this.UoW.Commit(() =>
            {
                var repository = this.UoW.GetRepository<Instructor>();
                if (repository != null)
                {
                    repository.Delete(id);
                }
            });

            //var instructor = this.UoW.Get<Instructor>(x => x.Id == id).FirstOrDefault();

            //if (instructor == null)
            //{
            //    return HttpNotFound();
            //}
            //db.Instructors.Remove(instructor);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public ActionResult CustomerList()
        {
            var customerList = new CustomerList
            {
                ListName = "Top Customers",
                Customers = new List<Customer>
                {
                    new Customer {Id = 1, Name = "Cheranga", IsSelected = true},
                    new Customer {Id = 2, Name = "Murali", IsSelected = false},
                    new Customer {Id = 3, Name = "Van", IsSelected = false}
                }
            };

            return View(customerList);
        }

        [HttpPost]
        public ActionResult CustomerList(CustomerList customerList)
        {
            return View(customerList);
        }
    }
}
