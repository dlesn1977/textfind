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

    
    public class coreprocess
    {
        public onefindcluster pgfind = new onefindcluster();
        public onefindcluster segfind = new onefindcluster();
        public onefindcluster checkfnd = new onefindcluster();
        public onefindcluster navfind = new onefindcluster();
        public List<onefindcluster> allflds = new List<onefindcluster>();
        

        public Dictionary<int, string> qrylst = new Dictionary<int, string>();

        public int coreid = 0;
        public int corestructid = 0;
        public int queryid = 0;
        public int fieldgroupid = 0;
        public int navtype = 0;
        public int itemsperpage = 0;
        public int curitemscnt = 0;
        public int pageno = 0;
        public int maxlimid = 0;
        public string desc = "";
        public string lnk = "";


        public Dictionary<int, KeyValuePair<string, int>> corestruct = new Dictionary<int,KeyValuePair<string,int>>();
        public string pagetxt = @"test chage at server start pg 
                                        onefirst test extra  
                                            twofirst 
                                                titlefirst
                                                    removeme2 find me 1
                                                titlelast 
                                                processfirst
                                                    find process removeproect
                                                processlast
                                            twolast 
                                        onelast 
                                        onefirst test extra 
                                            twofirst 
                                                titlefirst
                                                    removeme1 find title 
                                                titlelastex 
                                                processfirstex
                                                    find process removeproectex
                                                processlast 
                                            twolast 
                                        onelast  
                                        cknumbeg 
                                                replaceme 100 
                                        cknumend 
                                fin pg";

        public coreprocess(int i = 0)
        {
            setcoreid(i);
        }
        public coreprocess() { }
        public coreprocess(int idin, string ds, string lnkin)
        {
            coreid = idin;
            desc = ds;
            lnk = lnkin;

        }


    
    public bool setcoreid(int idin)
        {
            if (idin > 0)
            {
                
                try{
                    string s = "select * from fctGetOneCore('" + (coreid = idin) +"')";
                    helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
                    var allrows = db.getmylist(s);
                    if (allrows.Count > 0)
                        {
                            corestructid = allrows[0].getint("corestructid");
                            if (corestructid>0){
                                s = "select * from fncGetCoreStruct('" + corestructid + "')";
                                var allfldrec = db.getmylist(s);
                                if (allfldrec.Count > 0)
                                {
                                    foreach (shlpMyDictionary b in allfldrec)
                                    {
                                        string txt = b.getvalue("text");
                                        int seq = b.getint("seq");
                                        int tp = b.getint("type");
                                        KeyValuePair<string, int > ktmp = new KeyValuePair<string, int>(txt, tp);
                                        corestruct.Add(seq, ktmp);
                                    }
                                }
                            }

                            navtype = allrows[0].getint("navtype");
                            itemsperpage = allrows[0].getint("itemsperpage");
                            // getting all the finds
                            desc = allrows[0].getvalue("descript");
                            pgfind = new onefindcluster(allrows[0].getint("pagefindclustid"), "Page find");
                            segfind = new onefindcluster(allrows[0].getint("segfindclustid"), "Segment find");
                            checkfnd = new onefindcluster(allrows[0].getint("checkfindclustid"), "Check find");
                            navfind = new onefindcluster(allrows[0].getint("navfindclustid"), "Nav find");
                            allflds = new List<onefindcluster>();
                            // loading fields
                            int fldgroupid = allrows[0].getint("fieldgroupid");
                            if (fldgroupid > 0)
                            {
                                s = "select * from fncFieldGetAllFieldInGroup('" + fldgroupid + "') order by seq asc";
                                var allfldrec = db.getmylist(s);
                                if (allfldrec.Count > 0)
                                {
                                    foreach (shlpMyDictionary b in allfldrec)
                                    {
                                        onefindcluster t = new onefindcluster(b.getint("findclusterid"), 
                                            b.getvalue("fldname"), b.getint("fldkind"), b.getint("iskey"),
                                            b.getint("checkbadgroupid"), b.getint("goodgroupid")
                                            );
                                        allflds.Add(t);
                                    }
                                }
                            }
                            
                        }
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
            }
           return true;
        }

        public bool processtxt(string tbl)
        {
            // going through all query groups
            //string s = "select * from fncSelectAllSearchSkills()";

            string s = "select * from dbo.fcnGetQueryGyId('" + queryid + "')";
            helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
            var allrows = db.getmylist(s);
            if (allrows.Count > 0)
            {
                foreach (shlpMyDictionary b in allrows)
                {
                    string lnk = "";
                    //int groupid = b.getint("groupid");
                    //setquerygroup(groupid);


                    setqueryvalues(b.getvalue("one"), b.getvalue("two"));
                    string pagelink = makelnk();
                    pagetxt = getwebfile.getonepage(pagelink);
                    string tmpmxlim = checkfnd.retrievedata(pagetxt, 0);
                    int tstint = 0;
                    if (int.TryParse(tmpmxlim, out tstint))
                    {
                        maxlimid = tstint;
                    }
                    else
                    {
                        maxlimid = 0;
                    }
                    
                    int pageid = 0;
                    while (((curitemscnt == 0) && (maxlimid > 0)) || (curitemscnt < maxlimid))
                    {
                        processonetext(pagetxt, tbl, pageid, 1 );
                        curitemscnt += itemsperpage;
                        pageid++;
                        pagetxt = getwebfile.getonepage(makelnk());
                    }
                }
            }
            return true;
        }
        // kindid 1 to store to table, 2 to store to global array for marking
        public string processonetext(string txt, string tbl, int pageid, int kindid)
        {
            
            if (kindid == 2)
            {
                clsglobal.clearcolorlist();
                clsglobal.storemarking = 1;
            }
            int recid = 0;
            string tpage = pgfind.retrievedata(txt, 0);
            string seg = segfind.retrievedata(tpage, pgfind.markid);
            int cutfrom = segfind.greatestcut;
            while (!String.IsNullOrEmpty(seg))
            {

                if (kindid == 2) { clsglobal.storemarking = 1; }
                string storestr = "";
                //int mxprocessed = 0;
                foreach (onefindcluster a in allflds)
                {
                    // if this is a link then replace the seg
                    if (a.clustkind == 1)
                    {
                        clsglobal.storemarking = 0;
                        lnk = a.retrievedata(seg, segfind.markid);
                        lnk = getwebfile.makelnk(lnk);
                        seg = getwebfile.getonepage(lnk);
                    }
                    else
                    {
                        a.retrievedata(seg, segfind.markid);
                        if (a.valueisbad())
                        {
                            storestr = "";
                            break;
                        }
                        else
                        {
                            string vl = a.vl.Replace("'", "");
                            if (vl.Length > 7500)
                            {
                                vl = vl.Substring(0, 7500);
                            }
                            // making append string only if to add to tbl
                            if (kindid == 1)
                            {
                                storestr += a.fldname + "flddel" + vl + "keydel" + a.iskey + "rowdel";

                            }
                        }

                    }
                    // mxprocessed = a.lastindex;
                }
                if (!String.IsNullOrEmpty(storestr))
                {
                    if (kindid == 1)
                        storetodb(storestr, tbl, coreid, lnk, queryid, pageid, recid);
                }
                //int t = tpage.Length;
                tpage = tpage.Substring(cutfrom);
                seg = segfind.retrievedata(tpage, cutfrom);
                cutfrom = segfind.greatestcut;
                recid++;

                /*
                if (recid > 22)
                {
                    recid = recid;
                }
                */
            }
            if (kindid == 2)
            {

                string val = "";
                int fillid = 1;
                string myfill = "";
                string txttochange = txt;
                int offset = 0;
                
                foreach (markinginfo b in clsglobal.colorlist.OrderBy(t => t.segindex))
                {
                    myfill = "myfillerid" + fillid++ + "endfillerid";
                    string fst = "";
                    string lst = "";
                    fst = txttochange.Substring(0, b.segindex + offset);
                    lst = txttochange.Substring(b.segindex + offset + b.limit.Length);
                    txttochange = fst + myfill + lst;
                    b.filler = myfill;
                    offset = myfill.Length - b.limit.Length;


                    /*
                    // if value is blank - last item then make val from the val
                    if (String.IsNullOrEmpty(val))
                    {
                        val = dehtml(b.vl);
                    }
                    // replace value with a filler in text
                    string cvtxt = b.txt.Replace(b.flim + b.vl + b.llim, "myfiller");
                    // convert text with a filler
                    cvtxt = dehtml(cvtxt);
                    val = cvtxt.Replace("myfiller", "<div style='color=" + b.color + "'>" + dehtml(b.flim) +
                        "</div>" + val + "<div style='color=" + b.color + "'>" + dehtml(b.llim) + "</div>");
                    */
                }
                txttochange = dehtml(txttochange);
                foreach (markinginfo b in clsglobal.colorlist.OrderBy(t => t.segindex))
                {
                    string r = "<div style='background-color:" + b.color + "'>" + dehtml(b.limit) + "</div>";
                    txttochange = txttochange.Replace(b.filler, r);
                }

                return txttochange;
            }
            else
            {
                return "";
            }

        }
        public string dehtml(string s)
        {
            string  c = s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            return c;
        }

        /*
        public bool processtxt(string tbl)
        {
            // going through all query groups
            //string s = "select * from fncSelectAllSearchSkills()";

            string s = "select * from dbo.fcnGetQueryGyId('" + queryid + "')";
            helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
            var allrows = db.getmylist(s);
            if (allrows.Count > 0)
            {
                foreach (shlpMyDictionary b in allrows)
                {
                    string lnk = "";
                    //int groupid = b.getint("groupid");
                    //setquerygroup(groupid);


                    setqueryvalues(b.getvalue("one"), b.getvalue("two"));
                    string pagelink = makelnk();
                    pagetxt = getwebfile.getonepage(pagelink);
                    string tmpmxlim = checkfnd.retrievedata(pagetxt);
                    int tstint = 0;
                    if (int.TryParse(tmpmxlim, out tstint))
                    {
                        maxlimid = tstint;
                    }
                    else
                    {
                        maxlimid = 0;
                    }
                    int recid = 0;
                    int pageid = 0;
                    while (((curitemscnt == 0) && (maxlimid > 0)) || (curitemscnt < maxlimid))
                    {
                        string tpage = pgfind.retrievedata(pagetxt);
                        string seg = segfind.retrievedata(tpage);
                        int cutfrom = segfind.greatestcut;
                        while (!String.IsNullOrEmpty(seg))
                        {
                            string storestr = "";
                            //int mxprocessed = 0;
                            foreach (onefindcluster a in allflds)
                            {
                                // if this is a link then replace the seg
                                if (a.clustkind == 1)
                                {
                                    lnk = a.retrievedata(seg);
                                    lnk = getwebfile.makelnk(lnk);
                                    seg = getwebfile.getonepage(lnk);
                                }
                                else
                                {
                                    a.retrievedata(seg);
                                    if (a.valueisbad())
                                    {
                                        storestr = "";
                                        break;
                                    }
                                    else
                                    {
                                    string vl = a.vl.Replace("'", "");
                                        if (vl.Length > 7500)
                                        {
                                        vl = vl.Substring(0, 7500);
                                        }
                                        storestr += a.fldname + "flddel" + vl + "keydel" + a.iskey + "rowdel";
                                    }
     
                                }
                                // mxprocessed = a.lastindex;
                            }
                            if (!String.IsNullOrEmpty(storestr))
                            {
                                storetodb(storestr, tbl, coreid, lnk, queryid, pageid, recid);
                            }
                            int t = tpage.Length;
                            tpage = tpage.Substring(cutfrom);
                            seg = segfind.retrievedata(tpage);
                            cutfrom = segfind.greatestcut;
                            recid++;
                            if (recid > 22)
                            {
                                recid = recid;
                            }
                        }
                        curitemscnt += itemsperpage;
                        pageid++;
                        pagetxt = getwebfile.getonepage(makelnk());
                    }
                }
            }
            return true;
        }
        */
        public bool storetodb(string st, string tbl, int coreid, string lnk, int queryid, int pageid, int recid)
        {


            lnk = lnk.Replace("'", "");
            string s = "exec dbo.spstoreresult '" + tbl + "', '" + coreid + "', '" + lnk + "', '" + queryid + "', '" + pageid + "', '" + recid + "', '"  + st + "'";
            try
            {
                helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
                int i = db.getfirstint(s);
                if (i == 1)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public string makelnk()
        {
            string lnk = "";
            foreach(KeyValuePair<int, KeyValuePair<string, int>> a in corestruct.OrderBy(key=>key.Key)){
                string tmp = "";
                if (a.Value.Value == 1)
                {
                    if (a.Value.Key.Equals("pgcountvar"))
                    {
                        tmp = curitemscnt.ToString();
                    }
                    else
                    {
                        tmp = a.Value.Key;
                    }
                    
                }
                else if (a.Value.Value == 2){
                    tmp = qrylst[Convert.ToInt32(a.Value.Key)];
                }
                lnk += tmp;
            }
            return lnk;
        }
        public  void setqueryvalues(string first, string sec = "", string third = "")
        {
            qrylst.Clear();
            qrylst.Add(1, first);
            if (!String.IsNullOrEmpty(sec)) qrylst.Add(2, sec);
            if (!String.IsNullOrEmpty(third)) qrylst.Add(3, third);
        }

        public void setquerygroup(int qrygroupid)
        {
            // going through all query groups
            string s = "select * from fncQueryListByGroupId('" + qrygroupid + "')";
            helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
            var allrows = db.getmylist(s);
            if (allrows.Count > 0)
            {
                foreach (shlpMyDictionary b in allrows)
                {
                    int seq = b.getint("seqid");
                    string txt = b.getvalue("txt");
                    qrylst.Add(seq, txt);
                }
            }
        }

    }
  

}
