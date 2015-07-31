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
using MyUni.Web.ViewModels;

namespace MyUni.Web.Controllers
{
    public class InstructorController : Controller
    {
        private MyUniDbContext db = new MyUniDbContext();

        // GET: Instructor
        public ActionResult Index(int? id, int? courseId)
        {
            //var instructors = db.Instructors.Include(i => i.OfficeAssignment);
            //return View(instructors.ToList());

            var viewModel = new InstructorListViewModel
            {
                Instructors = db.Instructors
                    .Include(x => x.OfficeAssignment)
                    .Include(x => x.Courses.Select(y => y.Department))
                    .Include(x => x.Courses.Select(y => y.Enrollments.Select(z => z.Student)))
                    .OrderBy(x => x.FirstName)
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
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructor/Create
        public ActionResult Create()
        {
            var viewModel = new InstructorCreateViewModel
            {
                HireDate = DateTime.Now,
                Courses = db.Courses.Select(x=>new CourseViewModel{Id = x.Id, Name = x.Title, IsSelected = false}).ToList()
            };
            
            return View(viewModel);
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstructorCreateViewModel instructorViewModel)
        {
            if (ModelState.IsValid)
            {
                var instructor = new Instructor
                {
                    FirstName = instructorViewModel.FirstName,
                    LastName = instructorViewModel.LastName,
                    HireDate = instructorViewModel.HireDate,
                    OfficeAssignment = string.IsNullOrEmpty(instructorViewModel.OfficeLocation) ? null : new OfficeAssignment { Location = instructorViewModel.OfficeLocation}
                };

                db.Instructors.Add(instructor);
                db.SaveChanges();
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
            
            var instructor = db.Instructors
                .Include(x => x.OfficeAssignment)
                .Include(x=>x.Courses)
                .FirstOrDefault(x => x.Id == id);

            if (instructor == null)
            {
                return HttpNotFound();
            }

            var instructorCourses = instructor.Courses.ToList();

            var coursesTaught = db.Courses.ToList().Select(x => new CourseViewModel
            {
                Id = x.Id,
                Name = x.Title,
                IsSelected = instructorCourses.Any(y => y.Title == x.Title)
            }).ToList();

            var viewModel = new InstructorViewModel
            {
                Id = instructor.Id,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                HireDate = instructor.HireDate,
                OfficeAssignmentId = instructor.OfficeAssignmentId,
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

            var instructorToUpdate = db.Instructors.Include(x => x.OfficeAssignment).FirstOrDefault(x => x.Id == id);
            if (instructorToUpdate == null)
            {
                return new HttpNotFoundResult();
            }

            if (TryUpdateModel(instructorToUpdate, new[] {"FirstName", "LastName", "HireDate", "OfficeAssignment"}))
            {
                try
                {
                    if (string.IsNullOrEmpty(instructorToUpdate.OfficeAssignment.Location))
                    {
                        var currentOfficeAssignment = db.OfficeAssignments.FirstOrDefault(x => x.InstructorId == instructorToUpdate.Id);
                        if (currentOfficeAssignment != null)
                        {
                            db.Entry(currentOfficeAssignment).State = EntityState.Deleted;
                        }
                    }

                    db.SaveChanges();
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("", "Unable to save changes.");
                }
            }

            return View(instructorToUpdate);
        }

        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
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
            Instructor instructor = db.Instructors.Find(id);
            db.Instructors.Remove(instructor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

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
