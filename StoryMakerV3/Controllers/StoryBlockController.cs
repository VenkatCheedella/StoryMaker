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
    public class StoryBlockController : Controller
    {
        private StoryMakerContext db = new StoryMakerContext();

        // GET: /StoryBlock/
        public ActionResult Index()
        {
            var storyblocks = db.StoryBlocks.Include(s => s.Collage);
            List<StoryBlock> unArchStryBlocks = new List<StoryBlock>();
            foreach(StoryBlock blck in storyblocks.ToList())
            {
                if (blck.isArchieved == false)
                    unArchStryBlocks.Add(blck);
            }
            return View(unArchStryBlocks);
        }

        // GET: /StoryBlock/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StoryBlock storyblock = db.StoryBlocks.Find(id);
            if (storyblock == null)
            {
                return HttpNotFound();
            }
            if (storyblock.isArchieved == true)
                return HttpNotFound();
            return View(storyblock);
        }

        // GET: /StoryBlock/Create
        [Authorize(Roles = "Dev")]
        public ActionResult Create()
        {
            ViewBag.CollegeId = new SelectList(db.Collages, "CollegeId", "CollegeName");
            return View();
        }

        // POST: /StoryBlock/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Dev")]
        public ActionResult Create([Bind(Include = "BlockId,BlockName,Caption,Description,BlockSeq,isArchieved,Imagepath,CollegeId,UploadTime")] StoryBlock storyblock, HttpPostedFileBase imgfile)
        {
            string _pholder = imgfile.FileName;            
            Collage _tempCol = db.Collages.Find(storyblock.CollegeId);
            string imagePath = System.IO.Path.Combine(Server.MapPath(_tempCol.CollagePath), _pholder);
            string imagePathForDB = _tempCol.CollagePath + "/" + _pholder;
            storyblock.Imagepath = imagePathForDB;
            imgfile.SaveAs(imagePath);
            storyblock.isArchieved = false;
            storyblock.UploadTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.StoryBlocks.Add(storyblock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CollegeId = new SelectList(db.Collages, "CollegeId", "CollegeName", storyblock.CollegeId);
            return View(storyblock);
        }

        // GET: /StoryBlock/Edit/5
        [Authorize(Roles = "Dev")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StoryBlock storyblock = db.StoryBlocks.Find(id);
            TempData["_editStoryBlock"] = storyblock;
            if (storyblock.isArchieved == true)
                return HttpNotFound();
            if (storyblock == null)
            {
                return HttpNotFound();
            }
            ViewBag.CollegeId = new SelectList(db.Collages, "CollegeId", "CollegeName", storyblock.CollegeId);
            return View(storyblock);
        }

        // POST: /StoryBlock/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Dev")]
        public ActionResult Edit([Bind(Include = "BlockId,BlockName,Caption,Description,BlockSeq,isArchieved,Imagepath,CollegeId,UploadTime")] StoryBlock storyblock, HttpPostedFileBase imgfile)
        {

            StoryBlock editStryBlock = (StoryBlock)TempData["_editStoryBlock"];
            storyblock.isArchieved = editStryBlock.isArchieved;
            storyblock.CollegeId = editStryBlock.CollegeId;
            storyblock.UploadTime = editStryBlock.UploadTime;

            string _pholder = imgfile.FileName;
            if (_pholder != null)
            {
                Collage _tempCol = db.Collages.Find(storyblock.CollegeId);
                string imagePath = System.IO.Path.Combine(Server.MapPath(_tempCol.CollagePath), _pholder);
                string imagePathForDB = _tempCol.CollagePath + "/" + _pholder;
                storyblock.Imagepath = imagePathForDB;
                imgfile.SaveAs(imagePath);
            }
            else
                storyblock.Imagepath = editStryBlock.Imagepath;
            if (ModelState.IsValid)
            {
                db.Entry(storyblock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CollegeId = new SelectList(db.Collages, "CollegeId", "CollegeName", storyblock.CollegeId);
            return View(storyblock);
        }

        // GET: /StoryBlock/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StoryBlock storyblock = db.StoryBlocks.Find(id);
            if (storyblock == null)
            {
                return HttpNotFound();
            }
            return View(storyblock);
        }

        // POST: /StoryBlock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            StoryBlock storyblock = db.StoryBlocks.Find(id);
            db.StoryBlocks.Remove(storyblock);
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
