using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.UI;
using MyUni.Business;
using MyUni.DAL;
using MyUni.Web.ViewModels.Student;
using StageDocs.DAL.Abstract;

namespace MyUni.Web.Controllers
{
  
    public class StudentsController : MyUniBaseController
    {
        public StudentsController(IUoW uow)
            : base(uow)
        {
        }


        /*
   * TODO:
   * 1. Include the view in side a  panel
   * 2. Use bootstrap tables with the correct admin lte styles with paging.
   * 3. Edit/Details/Delete should be shown as buttons with proper icons
   * 4. Search functionality should be there (AJAX), with paging support
   * */
        public ActionResult Index(int currentPage = 1)
        {
            var repository = this.GetRepository<Student>();
            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            currentPage = currentPage <= 0 ? 1 : currentPage;

            var allStudents = repository.GetAll();
            var totalStudentCount = allStudents.Count();
            
            var viewModel = new StudentListViewModel
            {
                Students = allStudents.OrderBy(x => x.FirstName).Skip((currentPage-1) * 10).Take(10).ToList(),
                TotalPages = (totalStudentCount/10) == 0? 1 : totalStudentCount/10
            };

            return View(viewModel);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var repository = this.GetRepository<Student>();
            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }


            var student = repository.GetAll().Where(x => x.Id == id).Include(x => x.Enrollments.Select(y => y.Course)).FirstOrDefault();
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        public ActionResult Create()
        {
            return View();
        }

        //
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FirstName,LastName,EnrolledDate")] Student student)
        {
            try
            {
                var repository = this.GetRepository<Student>();
                if (repository == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                UoW.Commit(() =>
                {
                    repository.Add(student);
                });

                return RedirectToAction("Index");

            }
            catch (Exception exception)
            {


            }

            return View(student);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var repository = this.GetRepository<Student>();
            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var student = repository.GetById(id.Value);

            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        //
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var repository = this.GetRepository<Student>();
            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            var studentToUpdate = repository.GetById(id.Value);

            if (TryUpdateModel(studentToUpdate, new[] { "FirstName", "LastName", "EnrolledDate" }))
            {
                try
                {
                    this.UoW.Commit();

                    return RedirectToAction("Index");
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("Update failed, try again", exception);

                }
            }

            return View(studentToUpdate);

        }

        public ActionResult Delete(int? id, bool showError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (showError)
            {
                ViewBag.ErrorMessage = "Delete failed, try again";
            }

            var repository = this.GetRepository<Student>();
            if (repository == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            Student student = repository.GetById(id.Value);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                //
                // This way of deleting the student will involve sending 2 queries to the database (one to fetch, and one to delete the entity)
                //
                //Student student = db.Students.Find(id);
                //db.Students.Remove(student);

                var repository = this.GetRepository<Student>();
                if (repository == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                UoW.Commit(() =>
                {
                    repository.Delete(id);
                });

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                return RedirectToAction("Delete", new { id, showError = true });
            }
        }


        //
        // Cache the search results in the client side
        //
        [OutputCache(Duration = 60,VaryByParam = "search", Location = OutputCacheLocation.Client)]
        public ActionResult SearchStudents(string search, int page=1)
        {
            IQueryable<Student> students = null;

            if (string.IsNullOrEmpty(search))
            {
                students = this.UoW.Get<Student>();
            }
            else
            {
                students = this.UoW.Get<Student>(x => x.FirstName.Contains(search) ||
                                                      x.LastName.Contains(search));
            }

            page = page <= 0 ? 1 : page;

            var totalStudents = students.Count();
            

            var viewModel = new StudentListViewModel
            {
                Search = search,
                Students = students.OrderBy(x => x.FirstName).Skip((page-1) * 10).Take(10).ToList(),
                TotalPages = (totalStudents/10) == 0 ? 1: totalStudents/10
            };

            return PartialView("_studentList", viewModel.Students);
        }
    }
}
