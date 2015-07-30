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

            var viewModel = new InstructorViewModel
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
            ViewBag.Id = new SelectList(db.OfficeAssignments, "InstructorId", "Location");
            return View();
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OfficeAssignmentId,FirstName,LastName,HireDate")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.OfficeAssignments, "InstructorId", "Location", instructor.Id);
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var instructor = db.Instructors.Include(x => x.OfficeAssignment).FirstOrDefault(x => x.Id == id);

            if (instructor == null)
            {
                return HttpNotFound();
            }
            
            return View(instructor);
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
    }
}
