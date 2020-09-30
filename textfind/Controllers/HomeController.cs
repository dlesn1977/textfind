using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webdll;
using service;

using textfind.Models;


namespace textfind.Controllers
{
    public class HomeController : Controller
    {
        // old
        public ActionResult getfile(HttpPostedFileBase filetoupload)
        {
            findtext b = new findtext();
            b.importfile(filetoupload);
            b.maketext();
            return View("result", b);

        }
        /*
        public void testlists()
        {
            Dictionary<int, markinginfo> lst = new Dictionary<int, markinginfo>();
            markinginfo a = new markinginfo()
            {
                flim = "1one",
                llim = "2one",
                txt = "1one extr<div>aone 1two extra<div>two 1three found<div>three 2three 2two 2one",
                //filler = "filler1out",
                vl = " extr<div>aone 1two extra<div>two 1three found<div>three 2three 2two ",
               //replaced = " 1onefiller1out2one "
            };
            lst.Add(1, a);
            a = new markinginfo()
            {
                flim = "1two",
                llim = "2two",
                txt = "  extr<div>aone 1two extra<div>two 1three found<div>three 2three 2two ",
                //filler = "filler2out",
                vl = " extra<div>two 1three found<div>three 2three ",
                //replaced = " 1twofiller2out2two "
            };
            lst.Add(2, a);
            a = new markinginfo()
            {
                flim = "1three",
                llim = "2three",
                txt = " extra<div>two 1three found<div>three 2three   ",
                //filler = "filler3out",
                vl = " found<div>three ",
                //replaced = "1threefiller3out2three"
            };
            lst.Add(3, a);

            string val = "";
            foreach (KeyValuePair<int, markinginfo> b in lst.OrderByDescending(t=>t.Key))
            {
                // if value is blank - last item then make val from the val
                if (String.IsNullOrEmpty(val))
                {
                    val = b.Value.vl.Replace("<div>", "<X>");
                }
                // replace value with a filler in text
                string cvtxt = b.Value.txt.Replace(b.Value.flim + b.Value.vl + b.Value.llim, "myfiller");
                // convert text with a filler
                cvtxt = cvtxt.Replace("<div>", "<X>");
                val = cvtxt.Replace("myfiller", "<div>" + b.Value.flim.Replace("<div>", "<X>") + "</div>" + val + "<div>" + b.Value.llim.Replace("<div>", "<X>") + "</div>");

            }
        }
        */
        // GET: index
        public ActionResult Index()
        {
            //testlists();
            //processdb();
            CoreList a = new CoreList();
            a.getallprocesses();
            return View("Index", a);
        }
        public JavaScriptResult testjavascript()
        {
            var script = "alert('Hello');";
            return new JavaScriptResult() { Script = script };
        }

        // downloading data
        public ActionResult opendownload()
        {
            return View("download");
        }
        public int startdownload()
        {
            oneprocess i = new oneprocess(1);
            i.getqry();
            return 1;
        }
        // browsing data and applying
        public ActionResult startbrowsejobs()
        {
            JobClass a = new JobClass();
            a.getalljobs();
            return View("browsejobs", a);
        }
        // 1 is applied, 2 - dismissed
        public ActionResult updatejob(int jobid, int kind)
        {
            helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
            string s = "exec spUpdateJob '" + jobid + "', '" + kind + "'";
            int i = db.getfirstint(s);
            return RedirectToAction("startbrowsejobs");

        }
        // setup core pages
        public string getlnkstring(string lnk, int coreid)
        {
            string a = getwebfile.getonepage(lnk);
            coreprocess b = new coreprocess(coreid);
            string c = b.processonetext(a, "", 0, 2);
            return c;
        }
        public ActionResult editonecore(int coreid)
        {
            coreprocess a = new coreprocess(coreid);
            return View("onecore", a);
        }

 
    }
}
