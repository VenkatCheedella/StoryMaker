using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Web.Mvc;

namespace StoryMakerV3.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            String path = "~//Models//Contact.xml";
            path = Server.MapPath(path);
            XDocument doc = XDocument.Load(path);
            var elems = from x in doc.Elements() select x;
            Dictionary<string, string> contactinfo = new Dictionary<string, string>();
            foreach (var elem in elems)
            {
                var elems2 = from x in elem.Elements() select x;
                foreach (var celem in elems2)
                {
                    Console.WriteLine("\n" + celem.Name + celem.Value);                  
                    contactinfo.Add(celem.Name.ToString(), celem.Value.ToString());
                }
            }
            ViewBag.ContactInfo = contactinfo;
            ViewBag.Message = "Contact Information";
            return View(contactinfo);
        }
    }
}