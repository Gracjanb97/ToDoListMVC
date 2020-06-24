using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ToDoListMVC.Models;

namespace ToDoListMVC.Controllers
{
    public class ToDoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToDoesController()
        {
            _context = new ApplicationDbContext();
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuildToDoTable()
        {
            return PartialView("_toDoTable", GetUserToDos());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,IsDone")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                var currentUser = _context.Users.FirstOrDefault(x => x.Id == currentUserId);
                toDo.User = currentUser;

                _context.ToDos.Add(toDo);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(toDo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AJAXCreate([Bind(Include = "Id,Description")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                var currentUser = _context.Users.FirstOrDefault(x => x.Id == currentUserId);
                toDo.User = currentUser;
                toDo.IsDone = false;
                _context.ToDos.Add(toDo);
                _context.SaveChanges();
            }
            return PartialView("_toDoTable", GetUserToDos());
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ToDo toDo = _context.ToDos.Find(id);

            if (toDo == null)
                return HttpNotFound();

            string currentUserId = User.Identity.GetUserId();
            var currentUser = _context.Users.FirstOrDefault(x => x.Id == currentUserId);

            if (toDo.User != currentUser)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(toDo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,IsDone")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(toDo).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toDo);
        }

        [HttpPost]
        public ActionResult AJAXEdit(int? id, bool value)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var toDo = _context.ToDos.Find(id);

            if (toDo == null)
                return HttpNotFound();
            else
            {
                toDo.IsDone = value;
                _context.Entry(toDo).State = EntityState.Modified;
                _context.SaveChanges();
            }

            return PartialView("_toDoTable", GetUserToDos());
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ToDo toDo = _context.ToDos.Find(id);

            if (toDo == null)
                return HttpNotFound();

            return View(toDo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ToDo toDo = _context.ToDos.Find(id);
            _context.ToDos.Remove(toDo);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        private IEnumerable<ToDo> GetUserToDos()
        {
            string currentUserId = User.Identity.GetUserId();
            var currentUser = _context.Users.FirstOrDefault(x => x.Id == currentUserId);
            var toDosList = _context.ToDos.ToList().Where(x => x.User == currentUser);

            int completedTasksCount = 0;
            foreach (var toDo in toDosList)
                if (toDo.IsDone)
                    completedTasksCount++;

            ViewBag.CompletedTaskPercentage = Math.Round(100f * ((float)completedTasksCount / (float)toDosList.Count()));

            return toDosList;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
