using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace textfind.Models
{
    
    public class findtext
    {
        public List<string> fl = new List<string>();
        public List<oneline> objlst = new List<oneline>();
        public Dictionary<int, oneline> onlyfound = new Dictionary<int, oneline>();
        public Dictionary<string, string> fnd = new Dictionary<string, string>();
        public List<KeyValuePair<int, int>> bookmarks = new List<KeyValuePair<int, int>>(); 
        public List<KeyValuePair<string, int>> f = new List<KeyValuePair<string,int>>();
        public int range = 3;
        public string txt;
        public findtext()
        {
            init();
        }
        public findtext(string txtin)
        {
            txt = txtin;
            init();
        }
        public void init(){

            fnd.Add("tblListDetail", "lightgreen");
            //fnd.Add("update", "lightcoral");
            fnd.Add("insert", "lightcoral");

            /* * 
            //
            fnd.Add("insert", "lightgreen");
            fnd.Add("itemid", "lightcoral");
            */
        }
        public void importfile(HttpPostedFileBase filetoupload)
        {
            fl = helper.hlpUtilities.gethttppostedtoarray(filetoupload);
            int ct = 0;
            int lastid = 0;
            // storing all lines and only found
            foreach (string a in fl)
            {
                ct++;
                if (ct == 14254)
                {
                    lastid = 0;
                }
                oneline b = new oneline(a, ct, fnd);
                if (b.hasfound()){
                    onlyfound.Add( b.lineno, b);
                }
                objlst.Add(b);
            }
            // get found 
            foreach (KeyValuePair<int, oneline> c in onlyfound){
                
                if ((c.Key == 14254) || (c.Key == 19932) || (c.Key == 20078))
                {
                    lastid = 0;
                }
                if (c.Value.hasallfnds())
                {
                    bookmarks.Add(new KeyValuePair<int, int>(c.Key, c.Key));
                }
                else if (c.Value.fnd.Count > 0)
                {
                    lastid = hasrangeline(c.Key);
                    if (lastid>0)
                    {
                        if (!(bookmarks.Exists(t => t.Value == lastid)))
                        {
                            bookmarks.Add(new KeyValuePair<int, int> (c.Key, lastid));
                        }
                        
                    }
                }

            }
        }
        
        public void maketext(){
            txt = "";
            string proname = "";
            foreach (oneline a in objlst)
            {
                if (a.txt.ToUpper().Contains("CREATE PROCEDURE"))
                {
                    proname = a.txt.ToUpper().Replace("CREATE PROCEDURE", "");
                }
                if ((bookmarks.Exists(t=>t.Key==a.lineno))){
                    txt += "<a name='" + a.lineno + "linenumber'>" + a.lineno + "</a>";
                    txt += "<div style='background-color:lightblue'>";
                }
                txt += a.lineno + ":" + proname + ":" + a.txt + "<br>";
                if ((bookmarks.Exists(t => t.Value == a.lineno)))
                {
                    txt += "</div>";
                }
            }
        }
        public int hasrangeline(int i)
        {
            List<string> tmpfnd = onlyfound[i].fnd;
            int lastid = i + range;
            int lastrangeid = 0;

            foreach (KeyValuePair<int, oneline> c in onlyfound.Where(c => ((c.Value.lineno > i) && (c.Value.lineno < lastid))).ToList())
            {
                if ((c.Value.hasdifferentfnd(tmpfnd)))
                {
                    lastrangeid = c.Value.lineno;
                }
                /*
                if (c.Key > lastid)
                {
                    break;
                }
                 * */
            }
            return lastrangeid;
        }


        public void swap()
        {
            foreach (KeyValuePair<string, string> a in fnd)
            {
                swapone(a.Key, a.Value);
            }
        }
        public void swapone(string f, string cl)
        {
            string rpl = "<span style = \"background-color:" + cl + "\">" + f + "</span>";
            txt = txt.Replace(f, rpl);
        }
/*
        public void findindexes()
        {
            foreach (KeyValuePair<string, string> a in fnd)
            {
                storeoneindex(a.Key);
            }
        }
        
        
        public void storeoneindex(string t)
        {
            int ind = 0;
            while(ind!=-1){
                ind = txt.IndexOf(t, ind);
                indx.Add(new KeyValuePair<string, int>( t, ind));
            }
        }
        public void findindex()
        {
            List<oneline> b = objlst.OrderBy(t => t.lineno).ToList();
            int startind;
            int finind;
            oneline fst = b[0];
            List<string> tmpfnd;
            
            foreach (oneline a in b)
            {
                if (a.hasfound())
                {

                }


                if (fst == null)
                {
                    fst = a;
                    if (fst.hasfound())
                        { 
                            tmpfnd = fst.fnd;
                            startind = fst.lineno;
                        }
                }
                else
                {
                    if (a.hasdifferentfnd(tmpfnd))
                    {

                    }
                }
                
                if (a.Value < (startind + range)){

                }
                 * 
            }
        }
*/
        public class oneline
        {
            public string txt { set; get; }
            public int lineno { set; get; }
            public bool hasallfnds(){
                if (fnd.Count == 2)
                {
                    return true;
                }
                return false;
            }
            public List<string> fnd;
            public oneline(string txtin, int ln, Dictionary<string, string> lst)
            {
                fnd = new List<string>();
                txt = txtin;
                lineno = ln;
                foreach (string a in lst.Keys)
                {
                    if (txt.ToLower().Contains(a.ToLower()))
                    {
                        fnd.Add(a);
                    }
                }
            }
            public bool hasfound()
            {
                if (fnd.Count > 0)
                    return true;
                return false;
            }
            // return true if only found one different, 
            // else not true
            public bool hasdifferentfnd(List<string> lst)
            {
                if (fnd.Count > 0)
                {
                    foreach (string a in lst)
                    {
                        if (!String.IsNullOrEmpty(a))
                        {
                            if (!fnd.Contains(a))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }
        public class finditem
        {
            public string fndtext { set; get; }
            public string color { set; get;  }
            
        }
    }
}