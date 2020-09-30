using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using service;

namespace textfind.Models
{
    
    public class JobClass
    {
        
        public string msg = "";
        public List<onejob> jbs = new List<onejob>();
        public bool getalljobs()
        {
            string s = "select * from dbo.fncJobsGetAll(1) order by rating  desc";
            try
            {
                helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
                var allrows = db.getmylist(s);
                if (allrows.Count > 0)
                {
                    foreach (shlpMyDictionary b in allrows)
                    {
                        int id = b.getint("id");
                        string title = b.getvalue("title");
                        string source = "indeed";
                        string lnk = b.getvalue("lnk");
                        string desc = b.getvalue("descript");
                        string dt = b.getvalue("postdate");
                        int apl = b.getint("applied");
                        string comp = b.getvalue("company");
                        string loc = b.getvalue("location");
                        int dist = b.getint("distance");
                        int grskillid = b.getint("skillid");
                        

                        onejob c = new onejob(id, title, dt, source, 
                                lnk, desc, apl, comp, loc, dist, grskillid);
                        jbs.Add(c);
                    }
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
                return false;
            }
            return true;
        }
    }
    public class onejob
    {
        public int id;
        public string title;
        public string dateposted;
        public string sourcedesc;
        public string lnk;
        public string ds;
        public int applied;
        public string company;
        public string location;
        public int distance;
        public Dictionary<string, int> skils = new Dictionary<string, int>();
        public onejob(int idin, string ttl, string dt, string src, 
            string ln, string dsin, int apl, string comp, 
            string loc, int dist, int skillgroupid)
        {
            id = idin;
            title = ttl;
            dateposted = dt;
            sourcedesc = src;
            lnk = ln;
            ds = dsin;
            applied = apl;
            company = comp;
            location = loc;
            distance = dist;
            if (skillgroupid > 0)
            {
                loadskills(skillgroupid);
                markdesc();
            }
        }
        public void markdesc()
        {
            string cl = "";
            foreach(KeyValuePair<string, int> a in skils)
            {
                if (a.Value == 1)
                    cl = "green";
                else
                    cl = "red";
                ds = ds.ToLower().Replace(a.Key.ToLower(), "<span style='color:" + cl + "; font-weight:800;'>" + a.Key.ToLower() + "</span>");
            }
        }
        public bool loadskills(int id)
        {
            string s = "select * from dbo.fncGetSkillsByGroupId('" + id + "') order by inresume desc";
            helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
            var allrows = db.getmylist(s);
            if (allrows.Count > 0)
            {
                foreach (shlpMyDictionary b in allrows)
                {
                    string sk = b.getvalue("skill");
                    int rs = b.getint("inresume");
                    skils.Add(sk, rs);
                }
            }
            return true;
        }
    }
}