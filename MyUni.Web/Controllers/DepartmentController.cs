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

namespace MyUni.Web.Controllers
{
    public class DepartmentController : Controller
    {
        private MyUniDbContext db = new MyUniDbContext();

        // GET: Department
        public async Task<ActionResult> Index()
        {
            var departments = db.Departments.Include(d => d.Administrator);
            return View(await departments.ToListAsync());
        }

        // GET: Department/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            ViewBag.AdministratorId = new SelectList(db.Instructors, "Id", "FullName");
            return View();
        }

        // POST: Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,AdministratorId,Name,Budget,StartDate")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AdministratorId = new SelectList(db.Instructors, "Id", "FullName", department.AdministratorId);
            return View(department);
        }

        // GET: Department/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdministratorId = new SelectList(db.Instructors, "Id", "FullName", department.AdministratorId);
            return View(department);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var fieldsToBind = new[] { "Name", "Budget", "StartDate", "AdministratorId", "RowVersion" };

            var departmentToUpdate = await db.Departments.FindAsync(id);
            var isDepartmentExists = departmentToUpdate != null;

            if (isDepartmentExists)
            {
                if (TryUpdateModel(departmentToUpdate, fieldsToBind))
                {
                    try
                    {
                        departmentToUpdate.RowVersion = Guid.NewGuid().ToByteArray();

                        db.Entry(departmentToUpdate).OriginalValues["RowVersion"] = rowVersion;

                        await db.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                    catch (DbUpdateConcurrencyException exception)
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
                                ModelState.AddModelError("InstructorID", "Current value: " + db.Instructors.Find(databaseValues.AdministratorId).FullName);
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

                ViewBag.AdministratorId = new SelectList(db.Instructors, "Id", "FullName", id);
                return View(departmentToUpdate);
            }
            else
            {
                var department = new Department();
                TryUpdateModel(department, fieldsToBind);
                ModelState.AddModelError("", "This department does not exist anymore.");
                ViewBag.AdministratorId = new SelectList(db.Instructors, "Id", "FullName", department.AdministratorId);

                return View(department);

            }

        }

        // GET: Department/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Department department = await db.Departments.FindAsync(id);
            db.Departments.Remove(department);
            await db.SaveChangesAsync();
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
