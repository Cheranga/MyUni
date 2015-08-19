using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using MyUni.Business;
using MyUni.DAL;
using StageDocs.DAL.Abstract;

namespace MyUni.Web.Controllers
{
    public class DepartmentController : MyUniBaseController//Controller
    {

        // GET: Department
        public DepartmentController(IUoW uow) : base(uow)
        {
        }

        public ActionResult Index()
        {
            var departments = this.UoW.Get<Department>();
            if (departments == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            departments = departments.Include(d => d.Administrator);
            return View(departments);
        }

        // GET: Department/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var department = this.UoW.GetByFilter<Department>(x=>x.Id == id);
            if (department == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            
            return View(department);
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            var instructors = this.UoW.Get<Instructor>();
            ViewBag.AdministratorId = new SelectList(instructors, "Id", "FullName");
            return View();
        }

        // POST: Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdministratorId,Name,Budget,StartDate")] Department department)
        {
            if (ModelState.IsValid)
            {
                this.UoW.Commit(() =>
                {
                    var departmentRepository = this.UoW.GetRepository<Department>();
                    if (departmentRepository != null)
                    {
                        departmentRepository.Add(department);
                    }
                });
                
                return RedirectToAction("Index");
            }

            var instructors = this.UoW.Get<Instructor>();
            ViewBag.AdministratorId = new SelectList(instructors, "Id", "FullName", department.AdministratorId);
            return View(department);
        }

        // GET: Department/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Department department = this.UoW.GetByFilter<Department>(x => x.Id == id);
            if (department == null)
            {
                return HttpNotFound();
            }

            var instructors = this.UoW.Get<Instructor>();
            ViewBag.AdministratorId = new SelectList(instructors, "Id", "FullName", department.AdministratorId);
            return View(department);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var instructorList = this.UoW.Get<Instructor>();

            var fieldsToBind = new[] { "Name", "Budget", "StartDate", "AdministratorId", "RowVersion" };

            var departmentToUpdate = this.UoW.GetByFilter<Department>(x => x.Id == id);
            var isDepartmentExists = departmentToUpdate != null;

            if (isDepartmentExists)
            {
                if (TryUpdateModel(departmentToUpdate, fieldsToBind))
                {
                    departmentToUpdate.RowVersion = Guid.NewGuid().ToByteArray();

                    var status = this.UoW.Commit(() =>
                    {
                        var departmentRepository = this.UoW.GetRepository<Department>();
                        if (departmentRepository != null)
                        {
                            departmentRepository.Update(departmentToUpdate);
                        }
                    });

                    if (status.Status == false)
                    {
                        var exception = status.Exception as DbUpdateConcurrencyException;
                        
                        if (exception != null)
                        {
                            var entry = exception.Entries.Single();
                            var clientValues = (Department)entry.Entity;
                            var databaseEntry = entry.GetDatabaseValues();

                            if (databaseEntry == null)
                            {
                                ModelState.AddModelError(string.Empty, "Unable to save changes. The department was deleted by another user.");
                            }
                            else
                            {
                                var databaseValues = (Department)databaseEntry.ToObject();

                                if (databaseValues.Name != clientValues.Name)
                                {
                                    ModelState.AddModelError("Name", "Current value: " + databaseValues.Name);
                                }
                                if (databaseValues.Budget != clientValues.Budget)
                                {
                                    ModelState.AddModelError("Budget", "Current value: " + String.Format("{0:c}", databaseValues.Budget));
                                }
                                if (databaseValues.StartDate != clientValues.StartDate)
                                {
                                    ModelState.AddModelError("StartDate", "Current value: " + String.Format("{0:d}", databaseValues.StartDate));
                                }
                                if (databaseValues.AdministratorId != clientValues.AdministratorId)
                                {
                                    var instructors = this.UoW.Get<Instructor>();
                                    ModelState.AddModelError("InstructorID", "Current value: " + instructors.First(x => x.Id == databaseValues.AdministratorId).FullName);
                                }

                                ModelState.AddModelError(string.Empty,
                                    "The record you attempted to edit was modified by another user after you got the original value." +
                                    " If you still want to edit this record, click the Save button again. Otherwise click the Back to List hyperlink.");
                                //
                                // Now set the row version to be the latest value as per in the database
                                //
                                departmentToUpdate.RowVersion = databaseValues.RowVersion;
                            }
                        }
                    }
                }

                ViewBag.AdministratorId = new SelectList(instructorList, "Id", "FullName", id);
                return View(departmentToUpdate);
            }
            else
            {
                var department = new Department();
                TryUpdateModel(department, fieldsToBind);
                ModelState.AddModelError("", "This department does not exist anymore.");
                ViewBag.AdministratorId = new SelectList(instructorList, "Id", "FullName", department.AdministratorId);

                return View(department);

            }

        }

        // GET: Department/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = this.UoW.GetByFilter<Department>(x => x.Id == id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = this.UoW.GetByFilter<Department>(x => x.Id == id);
            if (department == null)
            {
                return HttpNotFound();
            }

            this.UoW.Commit(() =>
            {
                var repository = this.UoW.GetRepository<Department>();
                if (repository != null)
                {
                    repository.Delete(department);
                }
            });
            
            return RedirectToAction("Index");
        }
    }
}
