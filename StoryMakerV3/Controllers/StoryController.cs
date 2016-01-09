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
    public class StoryController : Controller
    {
        private StoryMakerContext db = new StoryMakerContext();

        // GET: /Story/
        [Authorize]
        public ActionResult Index()
        {
            ICollection<Story> stories = db.Stories.ToList();
            List<Story> validStories = new List<Story>();
            foreach(Story stry in stories)
            {
                if (stry.isArchieved != true)
                    validStories.Add(stry);
            }
            return View(validStories);
        }

        [Authorize]
        // GET: /Story/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Story story = db.Stories.Find(id);           
            if (story == null)
            {
                return HttpNotFound();
            }
            if (story.isArchieved == true)
                return HttpNotFound();
            return View(story);
        }

        [Authorize(Roles = "Dev")]
        // GET: /Story/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Story/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Dev")]
        public ActionResult Create([Bind(Include="StoryId,StoryName,StoryDescription,CreationDate,StoryPath,isArchieved")] Story story)
        {
            story.CreationDate = DateTime.Now;
            story.isArchieved = false;
            string path = "~/Content/MyStories";
            string _storyPath = path+ "/" + story.StoryName.Replace(" ", "");
            string _absStoryPath = Server.MapPath(_storyPath);
            System.IO.Directory.CreateDirectory(_absStoryPath);
            story.StoryPath = _storyPath;
            if (ModelState.IsValid)
            {
                db.Stories.Add(story);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(story);
        }

        // Display list of archieved stories

        [Authorize(Roles = "Admin")]
        public ActionResult ArchievedStories()
        {
            ICollection<Story> listOfStories = db.Stories.ToList();
            List<Story> archStories = new List<Story>();
            foreach(Story stry in listOfStories)
            {
                if (stry.isArchieved == true)
                    archStories.Add(stry);
            }
            return View(archStories);
        }

        // Unarchieve stories
        [Authorize(Roles = "Admin")]
        public ActionResult UnarchievedStories(int? id)
        {
            if(id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Story stry = db.Stories.Find(id);
            if(stry == null)
                return HttpNotFound();
            return View(stry);                     
        }

        // UnArchieve Story
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("UnarchievedStories")]
        public ActionResult ConfirmUnarchive(int? id)
        {
            Story stry = db.Stories.Find(id);
            stry.isArchieved = false;
            foreach(Collage coll in stry.ListOfCollages)
            {
                coll.isArchieved = false;
                foreach(StoryBlock stryBlock in coll.ListOfStoryBlocks)
                {
                    stryBlock.isArchieved = false;
                    db.Entry(stryBlock).State = EntityState.Modified;
                }
                db.Entry(coll).State = EntityState.Modified;
            }
            db.Entry(stry).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ArchievedStories");
        }

        // Archieve Stories

        [Authorize(Roles = "Admin")]
        public ActionResult Archieve(int? id)
        {
            if(id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Story _selectedStory = db.Stories.Find(id);
            if (_selectedStory == null)
                return HttpNotFound();
            //_selectedStory.isArchieved = true;           
            return View(_selectedStory);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Archieve")]
        [ValidateAntiForgeryToken]

        public ActionResult ArchiveConfirm(int? id)
        {
            Story stry = db.Stories.Find(id);
            stry.isArchieved = true;
            foreach(Collage coll in stry.ListOfCollages)
            {
                coll.isArchieved = true;
                foreach(StoryBlock stryblock in coll.ListOfStoryBlocks)
                {
                    stryblock.isArchieved = true;
                    db.Entry(stryblock).State = EntityState.Modified;
                }
                db.Entry(coll).State = EntityState.Modified;
            }
            db.Entry(stry).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Play Stories
        [Authorize]
        public ActionResult PlayStories(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string image_paths = "";
            string image_captions = "";
            string image_desc = "";
            Story _selectedStory = db.Stories.Find(id);
            //ICollection<Collage> listOfCollages = _selectedStory.ListOfCollages;
            //ICollection<StoryBlock> listofblocks;            
            // var _Collages = from coll in listOfCollages orderby coll.SequenceNum select coll;    
            var _Collages = from coll in _selectedStory.ListOfCollages orderby coll.SequenceNum select coll;
            foreach (Collage _coll in _Collages)
            {
                //listofblocks = _coll.ListOfStoryBlocks;              
                var _StoryBlocks = from _block in _coll.ListOfStoryBlocks orderby _block.BlockSeq select _block;
                foreach (StoryBlock _stryBlock in _StoryBlocks)
                {
                    string imagePath = _stryBlock.Imagepath.Substring(1, _stryBlock.Imagepath.Length-1);
                    image_paths = image_paths + "'" + imagePath + "'" + ",";
                    image_captions = image_captions + "!^" + _stryBlock.Caption;
                    image_desc = image_desc + "!^" + _stryBlock.Description;
                }
            }
            if (_selectedStory == null)
                return HttpNotFound();
            image_paths = image_paths.Substring(0, image_paths.Length-1);                              // Significance of these 3 functions 
            image_captions = image_captions.Substring(2, image_captions.Length-2);
            image_desc = image_desc.Substring(2, image_desc.Length-2);
            ViewBag.image_paths = image_paths;
            ViewBag.image_captions = image_captions;
            ViewBag.image_desc = image_desc;
            return View(_selectedStory);
        }

        // GET: /Story/Edit/5
        [Authorize(Roles = "Dev")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }            
            Story story = db.Stories.Find(id);
            TempData["editStory"] = story;
           // Story stry = new Story();
            if (story.isArchieved == true)
                return HttpNotFound();
            if (story == null)
            {
                return HttpNotFound();
            }
            return View(story);
        }

        // POST: /Story/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="StoryId,StoryName,StoryDescription,CreationDate,StoryPath,isArchieved")] Story story)
        {
            Story editStory = (Story)TempData["editStory"];
            story.isArchieved = editStory.isArchieved;
            story.CreationDate = editStory.CreationDate;
            story.StoryPath = editStory.StoryPath;
            if (ModelState.IsValid)
            {
                db.Entry(story).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(story);
        }

        // GET: /Story/Delete/5

       [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Story story = db.Stories.Find(id);
            if (story == null)
            {
                return HttpNotFound();
            }
            return View(story);
        }

        // POST: /Story/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Story story = db.Stories.Find(id);
            db.Stories.Remove(story);
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
