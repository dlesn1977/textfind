using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace textfind.Models
{
    public static class clsglobal
    {
        //public const string cn = "server=W7050-ALVIN;Database=textmdb;uid=testuser;PWD=testuser;";
        public static int storemarking = 0;
        public const string cn = "Data Source = localhost\\SQLEXPRESS;Initial Catalog = textdb; User ID = textuser; Password = barbyc77";
        public static helper.hlpDbConnect1 getdb()
        {
            return new helper.hlpDbConnect1(clsglobal.cn);
        }
        
        public static List<markinginfo> colorlist = new List<markinginfo>();
        public static void clearcolorlist()
        {
            colorlist = new List<markinginfo>();
            string s = "exec spClearColor";
            helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
            int i = db.getfirstint(s);
        }
        public static int addcolor(int segidin, string lim,
            string col, string innercolorin, int lmseqin)
        {
            int id = colorlist.Count + 1;
            markinginfo a = new markinginfo()
            {
                segindex = segidin,
                limit = lim,
                color = col,
                innercolor = innercolorin,
                limseq = lmseqin
                //id = id
            };
            addtocolorlist(a);
            // storing to db
            string s = "exec spStoreColor '" + segidin +  "',  '" + lim + 
                "',  '" + col +  "',  '" + innercolorin + "',  '" + lmseqin + "' ";
            helper.hlpDbConnect1 db = new helper.hlpDbConnect1(clsglobal.cn);
            int i = db.getfirstint(s);

            return id;
        }

        public static void addtocolorlist( markinginfo a)
        {
            
            colorlist.Add(a);
        }
        public static int getlastcolorindex()
        {
            int i = colorlist.Count;
            return i;
        }
    }
}