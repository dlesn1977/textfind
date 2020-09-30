using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using service;

namespace textfind.Models
{
    
    public class CoreList
    {
        public string msg = "";
        public List<coreprocess> prs = new List<coreprocess>();
        public bool getallprocesses()
        {
            string s = "select * from fncGetAllCores() order by coreid";
            try
            {
                helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
                var allrows = db.getmylist(s);
                if (allrows.Count > 0)
                {
                    foreach (shlpMyDictionary b in allrows)
                    {
                        int i = b.getint("coreid");
                        string ds = b.getvalue("descript");
                        string l = b.getvalue("rootlnk");
                        coreprocess c = new coreprocess(i, ds, l);
                        prs.Add(c);
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
}