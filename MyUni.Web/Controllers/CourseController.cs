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
using StageDocs.DAL.Abstract;

namespace MyUni.Web.Controllers
{
    public class CourseController : MyUniBaseController
    {
        public CourseController(IUoW uow)
            : base(uow)
        {
        }

        public ActionResult Index()
        {
            var courses = this.UoW.Get<Course>();

            if (courses == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(courses.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var course = this.UoW.Get<Course>(x => x.Id == id);

            if (course == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            
            return View(course.FirstOrDefault());
        }

        public ActionResult Create()
        {
            PopulateDepartments(null);
            return View();
        }


        //
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DepartmentId,Title,Credits")] Course course)
        {
            var repository = this.GetRepository<Course>();

            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (ModelState.IsValid)
            {

                this.UoW.Commit(() =>
                {
                    repository.Add(course);
                });


                return RedirectToAction("Index");
            }

            PopulateDepartments(course.DepartmentId);
            return View(course);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var course = this.UoW.Get<Course>(x=>x.Id == id);

            if (course == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var requiredCourse = course.FirstOrDefault();

            PopulateDepartments(requiredCourse == null ? -1 : requiredCourse.DepartmentId);
            return View(course.FirstOrDefault());
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DepartmentId,Title,Credits")] Course course)
        {
            var repository = this.GetRepository<Course>();

            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (ModelState.IsValid)
            {
                this.UoW.Commit(() =>
                {
                    repository.Update(course);
                });
                return RedirectToAction("Index");
            }
            PopulateDepartments(course.DepartmentId);
            return View(course);
        }

        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var course = this.UoW.Get<Course>(x => x.Id == id);

            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course.FirstOrDefault());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var repository = this.GetRepository<Course>();

            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            this.UoW.Commit(() =>
            {
                var course = repository.GetById(id);

                repository.Delete(course);
            });

            return RedirectToAction("Index");
        }

        private void PopulateDepartments(int? id)
        {
            var repository = this.GetRepository<Department>();

            if (repository == null)
            {
                return;
            }
            
            var departments = repository.GetAll()
                .OrderBy(x=>x.Name)
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

            ViewBag.DepartmentId = new SelectList(departments, "Value", "Text", id);
        }
    }
}
