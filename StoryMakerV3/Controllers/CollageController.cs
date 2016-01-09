using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoryMakerV3.Models;

namespace StoryMakerV3.Controllers
{
    [HandleError]
    [Authorize]
    public class CollageController : Controller
    {
        private StoryMakerContext db = new StoryMakerContext();

        // GET: /Collage/

        [Authorize]
        public ActionResult Index()
        {
            var collages = db.Collages.Include(c => c.StoryName);
            List<Collage> colls = new List<Collage>();
            foreach(Collage coll in collages.ToList())
            {
                if (coll.isArchieved == false)
                    colls.Add(coll);
            }
            return View(colls);
        }

        [Authorize]
        // GET: /Collage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collage collage = db.Collages.Find(id);
            if (collage == null)
            {
                return HttpNotFound();
            }
            if (collage.isArchieved == true)
                return HttpNotFound();
            return View(collage);
        }

        // GET: /Collage/Create
        [Authorize(Roles = "Dev")]
        public ActionResult Create()
        {
            ViewBag.StoryId = new SelectList(db.Stories, "StoryId", "StoryName");
            return View();
        }

        // POST: /Collage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Dev")]
        public ActionResult Create([Bind(Include="CollegeId,CollegeName,CollegeDescription,StoryId,CreationDate,SequenceNum,CollagePath,isArchieved")] Collage collage)
        {
            Story _stry = db.Stories.Find(collage.StoryId);
            collage.CollagePath = _stry.StoryPath;
            collage.CreationDate = DateTime.Now;
            collage.isArchieved = false;
            if (ModelState.IsValid)
            {
                db.Collages.Add(collage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StoryId = new SelectList(db.Stories, "StoryId", "StoryName", collage.StoryId);
            return View(collage);
        }

        // GET: /Collage/Edit/5
        [Authorize(Roles = "Dev")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collage collage = db.Collages.Find(id);
            TempData["_editCollage"] = collage;
            if (collage.isArchieved == true)
                return HttpNotFound();
            if (collage == null)
            {
                return HttpNotFound();
            }
            ViewBag.StoryId = new SelectList(db.Stories, "StoryId", "StoryName", collage.StoryId);
            return View(collage);
        }

        // POST: /Collage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Dev")]
        public ActionResult Edit([Bind(Include="CollegeId,CollegeName,CollegeDescription,StoryId,CreationDate,SequenceNum,CollagePath,isArchieved")] Collage collage)
        {
            Collage selCollage = (Collage)TempData["_editCollage"];
            collage.CreationDate = selCollage.CreationDate;
            collage.CollagePath = selCollage.CollagePath;
            collage.isArchieved = selCollage.isArchieved;
            collage.StoryId = selCollage.StoryId;
            if (ModelState.IsValid)
            {
                db.Entry(collage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StoryId = new SelectList(db.Stories, "StoryId", "StoryName", collage.StoryId);
            return View(collage);
        }

        // GET: /Collage/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collage collage = db.Collages.Find(id);
            if (collage == null)
            {
                return HttpNotFound();
            }
            return View(collage);
        }

        // POST: /Collage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Collage collage = db.Collages.Find(id);
            db.Collages.Remove(collage);
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
