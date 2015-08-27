using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.UI;
using MyUni.Business;
using MyUni.DAL;
using MyUni.Web.Infrastructure;
using MyUni.Web.ViewModels;
using MyUni.Web.ViewModels.Student;
using StageDocs.DAL.Abstract;
using WebGrease.Css.Extensions;

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
        [OutputCache(Duration = 60, VaryByParam = "search", Location = OutputCacheLocation.Client)]
        public ActionResult Index(string search, int currentPage = 1, bool fromSearch = false)
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

            students = students.OrderBy(x => x.FirstName);

            var viewModel = new PagedViewModel<Student>(students, 1, currentPage)
            {
                Search = search
            };

            if (Request.IsAjaxRequest() == false)
            {
                viewModel.FromSearch = true;

                return View(viewModel);
            }

            viewModel.FromSearch = fromSearch;

            return PartialView("_studentList", viewModel);
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

        public ActionResult GetTestData(DataTableInfo dataTableInfo)
        {
            if (dataTableInfo == null)
            {
                return Json(new
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0],
                    error = "Incorrect request"
                }, JsonRequestBehavior.AllowGet);
            }


            var allStudents = this.UoW.Get<Student>();

            var temp = allStudents.ToList();
            temp.AddRange(Enumerable.Range(1, 10).SelectMany(x => allStudents));

            allStudents = temp.AsQueryable();

            if (allStudents == null)
            {
                return Json(new
                {
                    draw = dataTableInfo.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new object[0],
                    error = "There are no students"
                }, JsonRequestBehavior.AllowGet);
            }


            //var filteredStudents = allStudents;
            IOrderedQueryable<Student> orderedList = null;

            if (dataTableInfo.OrderedColumns != null)
            {
                //
                // TODO: Need to consider the list of columns
                //
                var colList = dataTableInfo.OrderedColumns.ToList();
                var orderApplied = false;


                //
                // Getting the property dynamically
                // http://stackoverflow.com/questions/2728340/how-can-i-do-an-orderby-with-a-dynamic-string-parameter
                //

                //var expressions = colList.GetExpressionList<Student>();

                orderedList = allStudents.GetOrderedCollection(dataTableInfo.OrderedColumns);

                //if (expressions != null)
                //{
                //    var applied = false;
                //    expressions.ForEach(x =>
                //    {
                //        if (applied)
                //        {
                //            orderedList = x.ColumnOrder == ColumnOrder.Asc ? orderedList.ThenBy(y => x.Func(y)) : orderedList.ThenByDescending(y => x.Func(y));
                //        }
                //        else
                //        {
                //            orderedList = x.ColumnOrder == ColumnOrder.Asc ? allStudents.OrderBy(y => x.Func(y)) : allStudents.OrderByDescending(y => x.Func(y));
                //        }
                //    });
                //}

                //colList.ForEach(column =>
                //{
                //    var propertyInfo = typeof(Student).GetProperty(column.Field);
                //    if (propertyInfo == null)
                //    {
                //        return;
                //    }

                //    if (orderApplied && orderedList != null)
                //    {
                //        orderedList = column.ColumnOrder == ColumnOrder.Asc ?
                //            orderedList.ThenBy(x => propertyInfo.GetValue(x, null)) :
                //            orderedList.ThenByDescending(x => propertyInfo.GetValue(x, null));
                //    }
                //    else
                //    {
                //        orderedList = column.ColumnOrder == ColumnOrder.Asc ?
                //            allStudents.OrderBy(x => propertyInfo.GetValue(x, null)) :
                //            allStudents.OrderByDescending(x => propertyInfo.GetValue(x, null));

                //        orderApplied = true;
                //    }
                //});


                //var test = filteredStudents.OrderByDescending(x=>propertyInfo.GetValue(x,null)).ToList();

                //if (orderByColumn.Field == "FirstName")
                //{
                //    filteredStudents = orderByColumn.ColumnOrder == ColumnOrder.Asc ? filteredStudents.OrderBy(x => x.FirstName) : filteredStudents.OrderByDescending(x => x.FirstName);
                //}
                //else if (orderByColumn.Field == "LastName")
                //{
                //    filteredStudents = orderByColumn.ColumnOrder == ColumnOrder.Asc ? filteredStudents.OrderBy(x => x.LastName) : filteredStudents.OrderByDescending(x => x.LastName);
                //}

            }

            var filter = dataTableInfo.Search;
            var filteredStudents = orderedList ?? new List<Student>().AsQueryable();

            if (!string.IsNullOrEmpty(filter) && orderedList != null)
            {
                filteredStudents = orderedList.Where(x => x.FirstName.Contains(filter) || x.LastName.Contains(filter));
            }

            //filteredStudents = filteredStudents
            //       .Skip(dataTableInfo.PageNumber*dataTableInfo.Length)
            //       .Take(dataTableInfo.Length);

            return Json(new
            {
                draw = dataTableInfo.Draw,
                recordsTotal = allStudents.Count(),
                recordsFiltered = filteredStudents.Count(),
                data = filteredStudents.Skip(dataTableInfo.PageNumber * dataTableInfo.Length).Take(dataTableInfo.Length)

            }, JsonRequestBehavior.AllowGet);
        }


        //
        // Cache the search results in the client side
        //
        //[OutputCache(Duration = 60, VaryByParam = "search", Location = OutputCacheLocation.Client)]
        //public ActionResult SearchStudents(string search, int page = 1)
        //{
        //    IQueryable<Student> students = null;

        //    if (string.IsNullOrEmpty(search))
        //    {
        //        students = this.UoW.Get<Student>();
        //    }
        //    else
        //    {
        //        students = this.UoW.Get<Student>(x => x.FirstName.Contains(search) ||
        //                                              x.LastName.Contains(search));
        //    }

        //    page = page <= 0 ? 1 : page;

        //    var totalStudents = students.Count();


        //    var viewModel = new StudentListViewModel
        //    {
        //        Search = search,
        //        Students = students.OrderBy(x => x.FirstName).Skip((page - 1) * this.PageSize).Take(this.PageSize).ToList(),
        //        TotalPages = (totalStudents / this.PageSize) == 0 ? 1 : totalStudents / this.PageSize
        //    };

        //    return PartialView("_studentList", viewModel.Students);
        //}
    }

    public static class ModelExtensions
    {
        private static  IEnumerable<OrderExpression<T>> GetExpressionList<T>(IEnumerable<OrderedColumn> orderedColumns) where T : class
        {
            if (orderedColumns == null)
            {
                return null;
            }

            var expressions = orderedColumns.ToList().Select(column =>
            {
                var propertyInfo = typeof(T).GetProperty(column.Field);
                if (propertyInfo == null)
                {
                    return null;
                }

                Func<T, object> func = arg => propertyInfo.GetValue(arg, null);

                return new OrderExpression<T>
                {
                    ColumnOrder = column.ColumnOrder,
                    Func = func
                };
            }).Where(x => x != null).ToList();

            return expressions;

        }

        public static IOrderedQueryable<T> GetOrderedCollection<T>(this IQueryable<T> collection, IEnumerable<OrderedColumn> orderedColumns) where T : class
        {
            if (orderedColumns == null)
            {
                return collection as IOrderedQueryable<T>;
            }

            var expressions = GetExpressionList<T>(orderedColumns);

            if (expressions == null)
            {
                return collection as IOrderedQueryable<T>;
            }

            var applied = false;

            IOrderedQueryable<T> orderedCollection = null;
            expressions.ForEach(x =>
            {
                if (applied && orderedCollection != null)
                {
                    collection = x.ColumnOrder == ColumnOrder.Asc ? orderedCollection.ThenBy(y => x.Func(y)) : orderedCollection.ThenByDescending(y => x.Func(y));
                }
                else
                {
                    orderedCollection = x.ColumnOrder == ColumnOrder.Asc ? collection.OrderBy(y => x.Func(y)) : collection.OrderByDescending(y => x.Func(y));

                    applied = false;
                }
            });

            return orderedCollection;

        }
    }

    public class OrderExpression<T> where T : class
    {
        public ColumnOrder ColumnOrder { get; set; }
        public Func<T, object> Func { get; set; }
    }
}
