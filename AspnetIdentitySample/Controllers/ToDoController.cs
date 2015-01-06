using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.SqlClient;
using System.Web.Helpers;
using Newtonsoft.Json;
using System.Collections;

namespace AspnetIdentitySample.Controllers
{
    [Authorize]
    public class ToDoController : Controller
    {
        private MyDbContext db;
        private UserManager<ApplicationUser> manager;
        public ToDoController()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            //this.db = new MyDbContext();
        }

        //NOT CURRENTLY ACTIVE OR WORKING
        // GET: /ToDo/Charts
        // GET ToDo Charts for the logged in user
        public ActionResult Charts()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            return View(db.ToDoes.ToList().Where(todo => todo.User.Id == currentUser.Id));
        }
        
        // GET: /ToDo/Index
        // GET ToDo for the logged in user
        public ActionResult Index(string sortOrder)
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            //var indexmodel = db.ToDoes.ToList().Where(todo => todo.User.Id == currentUser.Id);
            //indexmodel = indexmodel.OrderByDescending(todo => todo.Date);
            //return View(indexmodel);
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            var dalist = from todo in db.ToDoes where todo.User.Id == currentUser.Id select todo;

            var dalistRD = dalist.ToArray();

            switch (sortOrder)
            {
                case "Date":
                    dalist = dalist.OrderBy(model => model.Date);
                    break;
                case "date_desc":
                    dalist = dalist.OrderByDescending(model => model.Date);
                    break;
                default:
                    dalist = dalist.OrderByDescending(model => model.Date);
                    break;
            }

            return View(dalist.ToList());
        }

        // GET: /ToDo/All
        [Authorize(Roles="Admin")]
        public async Task<ActionResult> All()
        {
            return View(await db.ToDoes.ToListAsync());
        }

        // GET: /ToDo/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo todo = await db.ToDoes.FindAsync(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            if (todo.User.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(todo);
        }

        // GET: /ToDo/AfterVisitDetails/5
        public async Task<ActionResult> AfterVisitDetails(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AfterVisit todo = await db.AfterVisit.FindAsync(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            if (todo.User.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(todo);
        }

        // GET: /ToDo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ToDo/Create ##### FOR APPT CREATE BUTTON
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,Company,Date,Description,CustomerContact,ApptType,Competition")] ToDo todo)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (ModelState.IsValid)
            {
                todo.User = currentUser;
                db.ToDoes.Add(todo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(todo);
        }

        // GET: /ToDo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo todo = await db.ToDoes.FindAsync(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            if (todo.User.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(todo);
        }

        // POST: /ToDo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Company,date,Description,CustomerContact,ApptType,Competition")] ToDo todo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(todo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(todo);
        }

        // GET: /ToDo/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo todo = await db.ToDoes.FindAsync(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            if (todo.User.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            } 
            return View(todo);
        }

        // POST: /ToDo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ToDo todo = await db.ToDoes.FindAsync(id);
            db.ToDoes.Remove(todo);
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
