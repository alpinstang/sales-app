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
    public class AfterVisitController : Controller
    {
        private MyDbContext db;
        private UserManager<ApplicationUser> manager;
        public AfterVisitController()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        
        // GET: /AfterVisit/Index
        // GET ToDo for the logged in user
        public ActionResult Index()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            return View(db.AfterVisit.ToList().Where(todo => todo.User.Id == currentUser.Id));
        }

        // GET: /AfterVisit/All
        [Authorize(Roles="Admin")]
        public async Task<ActionResult> All()
        {
            return View(await db.AfterVisit.ToListAsync());
        }

        // GET: /AfterVisit/Details/5
        public async Task<ActionResult> Details(int? id)
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

        // GET: /AfterVisit/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: /AfterVisit/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Company,Date,VisitDate,Discussion,PurchaseMethod,PurchaseDate,FutureDate,NewVehicles,NewVehiclesCount,Competition,DiscussionReason")] AfterVisit todo)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (ModelState.IsValid)
            {
                todo.User = currentUser;
                db.AfterVisit.Add(todo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(todo);
        }

        // GET: /AfterVisit/Edit/5
        public async Task<ActionResult> Edit(int? id)
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

        // POST: /AfterVisit/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Company,Date,VisitDate,Discussion,PurchaseMethod,PurchaseDate,FutureDate,NewVehicles,NewVehiclesCount,Competition,DiscussionReason")] AfterVisit todo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(todo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(todo);
        }

        // GET: /AfterVisit/Delete/5
        public async Task<ActionResult> Delete(int? id)
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

        // POST: /AfterVisit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AfterVisit todo = await db.AfterVisit.FindAsync(id);
            db.AfterVisit.Remove(todo);
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
