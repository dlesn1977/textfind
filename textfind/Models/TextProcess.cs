using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webdll;
using service;
using System.Net;
using HtmlAgilityPack;
using helper;

namespace textfind.Models
{
    public static class getwebfile
    {
        public static string rooturl = "";
        public static string makelnk(string parturl)
        {
            if (!(parturl.StartsWith("http")) && !(parturl.StartsWith("www.")))
            {
                parturl = rooturl + parturl;
            }
            return parturl;
        }
        public static string getonepage(string url)
        {
            string s = "";
            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                //client.DownloadFile("http://yoursite.com/page.html", @"C:\localfile.html");

                // Or you can get the file content without saving it
                //string htmlCode = client.DownloadString("http://yoursite.com/page.html");
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    s = client.DownloadString(url);
                }
                catch(Exception e)
                {
                    s = e.Message;
                }
            }
            return s;
        }
    }
    
    
    
    public class oneprocess
    {
        Dictionary<int, string> qrys = new Dictionary<int, string>();
        public oneprocess() { }
        public oneprocess(int i) { }
        public void getallprocess(){

        }
        public  void getqry(){
             //try{
                string s = "select * from getallprocesses()";
                helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
                coreprocess txt = new coreprocess();
                var allrows = db.getmylist(s);
                if (allrows.Count > 0)
                {
                    foreach (shlpMyDictionary a in allrows)
                    {
                        if (a.getint("coreid") != txt.coreid)
                        {
                            txt.setcoreid(a.getint("coreid"));
                            getwebfile.rooturl = a.getvalue("rootlnk");
                        }
                        txt.queryid = a.getint("qryprocessid");
                        string strtbl = a.getvalue("storetable");
                        txt.processtxt(strtbl);
                    }
                // running post download
                s = "exec [dbo].[spCleantblStore]";
                int res = db.getfirstint(s); 
                

            }
            //}
            //catch (Exception e){}
        }
    }
    
  
    public class onefindcluster
    {
        public int markid = 0;
        public static string storelink = "";
        public int clusterid = 0;
        public string fldname = "";
        public int greatestcut = 0;
        public string vl = "";
        public int clustkind = 0;
        public int iskey = 0;
        public List<string> checkbad = new List<string>();
        public List<string> checkgood = new List<string>();
        public Dictionary<int, onefind> allfnds = new Dictionary<int, onefind>();

        // kind 1 for checkbad, 2 - for checkgood
        public bool getcheckvalues(int groupid, int kind)
        {
            string s = "select * from [dbo].[fncGetAllfndText]('" + groupid + "') order by seq asc";
            helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
            var allrows = db.getmylist(s);
            if (allrows.Count > 0)
            {
                foreach (shlpMyDictionary a in allrows) { 
                    if (kind == 1)
                    {
                        checkbad.Add(a.getvalue("txt"));
                    }
                    else if (kind == 2)
                    {
                        checkgood.Add(a.getvalue("txt")); 
                    }
                }
            }
            return true;               
        }
        public bool valueisbad()
        {
            foreach (string gd in checkgood)
            {
                if (valuecontainsword(gd))
                {
                    return false;
                }
            }

            // checking bad
            foreach (string bd in checkbad)
            {
                if (valuecontainsword(bd))
                {
                    return true;
                }
            }
            return false;
        }
        public bool valuecontainsword(string bad)
        {
            if ((vl.ToLower().Contains(" " + bad.ToLower() + " ")) ||
                (vl.ToLower().StartsWith(bad.ToLower() + " ")) || 
                (vl.ToLower().EndsWith(" " + bad.ToLower())) ||
                (vl.ToLower().Equals(bad.ToLower()))
                )
            {
                return true;
            }
            return false;
        }
        public onefindcluster(int clusteridin = 0, string fldnamein = "", 
                int clustkindin = 0, int iskeyin = 0, int checkbadid = 0, 
                    int checkgoodid = 0)
        {
            clusterid = clusteridin;
            fldname = fldnamein;
            clustkind = clustkindin;
            iskey = iskeyin;
            loadallfinds();
            if (checkbadid>0)
                { getcheckvalues(checkbadid, 1); }
            if (checkgoodid > 0)
                { getcheckvalues(checkgoodid, 2); }
        }
        // kind 1 - do nothing, kind 2 - store to static color array
        public string retrievedata(string txt, int offset)
        {
            vl = "";
            greatestcut = 0;
            int tmpoffset = offset;
            string tmp = txt;
            foreach(KeyValuePair<int,onefind> a in allfnds.OrderBy(t=>t.Key))
            {
                string bfr = a.Value.getvalue(tmp, clusterid, a.Key, tmpoffset);
                tmpoffset = markid =  a.Value.newoffset;
                // if anything is found, but not original input string, then store to tmp
                if ( (tmp == txt))
                {
                    if (!String.IsNullOrEmpty(bfr))
                    {
                        tmp = bfr;
                    }
                    else
                    {
                        return ""; 
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(bfr))
                    {
                        
                        tmp = bfr;
                    }
                    else
                    {
                        // if the inner value not found simply break
                        break;
                    }
                }
                 if (greatestcut < a.Value.lastindex) { greatestcut = a.Value.lastindex; }
                
            }
            if ((tmp != txt))
            {
                vl = WebUtility.HtmlDecode(tmp);
            }
            else
            {
                vl = "";
            }
            return vl;
        }


        public bool loadallfinds()
        {
            
            if (clusterid > 0)
            {
                helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
                string s = "select * from dbo.fncGetAllFndsFromClusterId('" + clusterid + "') order by seq asc";
                try
                {
                    var allfldrec = db.getmylist(s);
                    if (allfldrec.Count > 0)
                    {
                        foreach (shlpMyDictionary b in allfldrec)
                        {
                            onefind t = new onefind(b.getint("findid") );
                            allfnds.Add(b.getint("seq"), t);
                        }
                    }
                }
                catch(Exception e)
                {
                    return false;
                }
                
            }
            return true;
        }
    }
    public class onefind
    {
        public int id;
        public int newoffset = 0;
        public int lastindex;
        public string msg;
        public string color;
        public string innercolor;
        public string fstlimused = "";
        public string lstlimused = "";
        public Dictionary<string, int> frs = new Dictionary<string, int>();
        public Dictionary<string, int> lst = new Dictionary<string, int>();
        public Dictionary<string, int> toremove = new Dictionary<string, int>();
        public int removekind = 0;
        public string vl;
        public onefind(){}
        public onefind(int fndid )
        {
            setfindid(fndid);
            string s = "select * from dbo.fncGetLimitColors('" + fndid + "')";

            var recdt =  clsglobal.getdb().getmylist(s);
            if (recdt.Count > 0)
            {
                color = recdt[0].getvalue("col");
                innercolor = recdt[0].getvalue("innercolor");
            }
        }
        public bool setfindid(int findid){
            try{
                string s = "select * from fncGetFindTxtArray('" + (id = findid) + "') order by id asc, seq asc";
                helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
                var allrows = db.getmylist(s);
                if (allrows.Count > 0)
                    {
                    foreach (shlpMyDictionary a in allrows)
                        if (a.getint("id") == 1)
                        {
                            frs.Add(a.getvalue("txt"), a.getint("delid") );
                        }
                        else if (a.getint("id") == 2)
                        {
                            lst.Add(a.getvalue("txt"), a.getint("delid"));
                        }
                        else if (a.getint("id") == 3) 
                            {
                            string txt = a.getvalue("txt" );
                            toremove.Add(txt, a.getint("delid"));
                            /*
                            int i = 0;
                            string txt = a.getvalue("txt");
                            if (int.TryParse(txt, out i))
                                {
                                    if (i < 0) removekind = i;
                                }
                            else
                                {
                                    toremove.Add(txt);
                                }
                            */
                        }
                        else if (a.getint("id") == 4)
                        {
                            removekind = a.getint("seq");
                        }
                    }
                return true;
                }
            catch (Exception e)
            {
                msg = "Text Find error: " + e.Message;
                return false;
            }
        }
        
        public string getvalue(string s, int clusterid, int seqid, int offset)
        {
            vl = "";
            int fstind = findfirstindex(s);
            int lstind =0;
            if (fstind > -1)
            {
                lstind = findlastindex(s, fstind);
            }
            else
            {
                return "";
            }
             
            if ((fstind > -1) && (lstind > fstind))
            {
                vl = s.Substring(fstind, lstind - fstind);
                if (clsglobal.storemarking == 1)
                {
                    clsglobal.addcolor(fstind + offset, fstlimused, color, innercolor, 1);
                    newoffset = fstind + fstlimused.Length + offset;
                    clsglobal.addcolor(lstind + offset, lstlimused, color, "", 2);
                    return vl;
                }
            }
            if (clsglobal.storemarking != 1)
            {
                foreach (KeyValuePair<string, int> a in toremove.OrderByDescending(t => t.Key.Length))
                {
                    vl = vl.Replace(a.Key, "");
                }
                // if need to remove all html
                if ((removekind== 1) || (removekind == 3))
                {
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(vl);
                    vl = htmlDoc.DocumentNode.InnerText;
                }
                // if need to fix remove parts of html from end and begining
                if ((removekind == 2) || (removekind == 3))
                {
                    String a = vl;
                    if (a.Contains(">"))
                    {
                        a = hlpUtilities.getstrinafterstr(vl, ">");
                    }
                    if (a.Contains("<"))
                    {
                        a = hlpUtilities.getstringbeforestr(vl, "<");
                    }
                    vl = a;
                }
            }
            return (vl = vl.Trim());

        }
        public int findlastindex(string s, int fromind)
        {
            int i = 0;
            int tmp = 0;
            foreach (KeyValuePair<string, int> a in lst.OrderByDescending(t => t.Key.Length))
            {
                //string f = "jobsearch-SerpJobCard-footer";
                //int t = s.ToLower().IndexOf(f.ToLower());
                i = s.IndexOf(a.Key, fromind);
                if (i > -1)
                {
                    if (((tmp == 0)) || ((tmp > i)))
                    {
                        tmp = i;
                        lastindex = tmp + a.Key.Length;
                        lstlimused = a.Key;
                    }

                }

            }
            return tmp;
        }
        public int findfirstindex(string s)
        {
            int i = -1;
            int tmp = -1;
            foreach (KeyValuePair<string, int> a in frs.OrderByDescending(t=>t.Key.Length))
            {
                if (s.IndexOf(a.Key) > -1)
                {
                    i = s.IndexOf(a.Key) + a.Key.Length;
                    if (((tmp == 0)&&(i>-1)) || (tmp<i))
                    {
                        fstlimused = a.Key;
                         tmp = i ;
                    }
                }

            }
            return tmp;
        }

    }
    public class markinginfo
    {
        public int segindex = 0;
        public string limit = "";
        public string color;
        public string innercolor;
        public int parentid = 0;
        public string filler = "";
        public int limseq;

        public markinginfo()
        {

        }


    }
}