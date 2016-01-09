using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StoryMakerV3.Controllers
{
    public class ErrorHandlingController : Controller
    {
        //
        // GET: /ErrorHandling/
        public ActionResult Index()
        {
            Response.StatusCode = 500;
            return View();
        }

        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
	}
}