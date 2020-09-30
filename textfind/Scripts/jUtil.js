/*---------------------
            String operations
*/
function findtextbetweenstrings(main, startstring, endstring, sequent) {
    if (sequent == undefined)
        sequent = 1;
    var finind = -1;

    var startind = main.indexOf(startstring);
    
    if (startind == -1) return "";;
    startind = Number(startind) + Number(startstring.length);

    var tmp = main.substring(startind);

    // if sesequent select endstring next to start string
    if (sequent)
    {
        finind = tmp.indexOf(endstring);
        if (finind != -1)
            return tmp.substring(0, finind);
    }
        // if not select end string from the back - bigger string selected
    else
    {
        finind = tmp.lastIndexOf(endstring);
        if (finind != -1)
            return tmp.substring(0, finind);
    }
    if ((finind == -1))
        return tmp;
    else
        return "";
}

function getstrinafterstr(text, fnd, fromend)
{
if (fromend==undefined)
    fromend = 0;
var i;
if (fromend)
    i = text.lastIndexOf(fnd);
else
    i = text.indexOf(fnd);
if (i < 0 | (i == (text.length - fnd.length))) 
    return "";
if (i == 0)
{
    return text.substring(fnd.length);
}
return text.substring(i + fnd.length);
}

function getstringbeforestr(text, fnd, fromend)
{
if (fromend==undefined)
    fromend = 0;
var i;
if (fromend)
    i = text.lastIndexOf(fnd);
else
    i = text.indexOf(fnd);
if (i < 1) return "";
return text.substring(0, i);
}

function removebetweenstring(instr, flim, llim, seq, toend)
{
var r = replacebetweenstrings(instr, flim, llim, seq, toend, "");
return r;
}


function replacebetweenstrings(instr, flim, llim, seq, toend, replstring)
{
    var o = instr;
    var rm = findtextbetweenstrings(o, flim, llim, seq, toend);
    while (rm != "") {
            rm = flim + rm + llim;
            o = o.replace(rm, replstring);
            var rm = findtextbetweenstrings(o, flim, llim, seq, toend);
    }


return o;
}
//-------------------------------------
//          Error operations
//-------------------------------------
function isdate(dt) {
    var timestamp = Date.parse(dt)

    /*
    var timestamp = new Date(dt)
    if (timestamp instanceof Date && isFinite(d)) {
        return true;
    }
    return false;
    */
    if (isNaN(timestamp) == false) {
        //var d = new Date(timestamp);
        return true;
    }
    return false;
}
function checkbq(item, qty) {
    item = item.trim();
    var s = "/Home/checkitembq?item=" + item + "&qty=" + qty;
    var txtres = getstringfromajax(s);
    return txtres;
}
function getitembq(item) {
    var s = "/Home/getitembq?item=" + item;
    var txtres = getstringfromajax(s);
    return txtres;
}
function isposnumber(txt, lowbound) {
    if (!(lowbound))
        lowbound = 0;
    if (isnumber(txt)) {
        var i = Number(txt);
        if (i > lowbound)
            return true;
    }
        return false;
}
function isnumber(txt) {
   return  $.isNumeric(txt)
}
function showerr(pref, error) {
    $("#val" + pref).css("border", "solid");
    $("#val" + pref).css("border-color", "red");
    $("#err" + pref).css("display", "block");
    $("#err" + pref).html(error);
    $("#lbl" + pref).css("color", "red");
}
function hideerr(pref) {
    $("#val" + pref).css("border", "none");
    $("#err" + pref).css("display", "none");
    $("#err" + pref).html("");
    $("#lbl" + pref).css("color", "black");
}

function getstringfromajax(ajaxtext) {
    var ajaxobj;
    var str = '';
    if (window.XMLHttpRequest) {
        ajaxobj = new XMLHttpRequest();
        ajaxobj.onreadystatechange = function (oEvent) {
            if (ajaxobj.readyState === 4) {
                if (ajaxobj.status === 200) {
                    str = ajaxobj.responseText;
                } else {
                    str = ajaxobj.statusText;
                }
            }
        };

    } else {
        ajaxobj = new ActiveXObject("Microsoft.XMLHTTP");
    }
    ajaxobj.open("Post", ajaxtext, false);
    ajaxobj.send();
    if (str)
        return str
    else
        return ajaxobj.responseText;
}

function isMobileSafari() {
    return navigator.userAgent.match(/(iPod|iPhone|iPad)/) && navigator.userAgent.match(/AppleWebKit/)
}
function getstringfrompost(controlertext, arguments, callback) {

    var m;
    $.ajax({
        type: 'POST',
        url: controlertext,
        data: arguments,
        async: false,
        success: function (msg) {
            m = msg;
            if ((callback))
                callback(msg);
        },
        error: function (xhr, ajaxOptions, thrownError) { showpopup(xhr.responseText); }
    });
    return m;
}

function getkey() {
    return $("#t").val();
}
function displaymsg(msg, toblink) {
    if (msg.length > 0) {
        $("#msg").text(msg);
        $("#msg").css("display", "inline");
        if (toblink) {
            var x = document.getElementById("msg");
            if ((x.className === "")) {
                x.className += "blink_class";
            }
        }
        else {
            var x = document.getElementById("msg");
            x.className = "";
        }
    }
    else {
        $("#msg").css("display", "none");
    }
}



// other then transfer only destlistid has list id for modification
//changetype int --  1 for adding, 2 for deleting, 3 for changing qty

function modifylistitem(sourcelistid, destlistid, qty, changetype, itemname, destlistname, price, description, callback) {
    var p = getmodifyprompt(sourcelistid, destlistid, itemname, destlistname, changetype);

    if (p.length > 0) {
        showpopup(p, 1,
        "modifylistitemparttwo('" + sourcelistid + "', '" + destlistid + "', '" +
        qty + "', '" + changetype + "', '" + itemname + "', '" + destlistname + "', '" +
        price + "', '" + description + "', " + callback + ")", "return 0");
    }
    else {
        //alert("inner test")
        modifylistitemparttwo(sourcelistid, destlistid, qty, changetype, itemname, destlistname, price, description, callback);
    }
}
function modifylistitemparttwo(sourcelistid, destlistid, qty, changetype, itemname,
        destlistname, price, description, callback) {

    var k = getkey();
    itemname = encodeURIComponent(itemname);
    if (!(price))
        price = "0";
    var s = "/Home/modifylistitem?sourcelistid=" + sourcelistid + "&destlistid=" + destlistid +
            "&item=" + itemname + "&qty=" + qty + "&t=" + k + "&changetype=" + changetype + "&price=" + price;
   // alert(s);
    var txtres = getstringfromajax(s);
    //var callfunct = callback.name;
    var callfunct = getfunctionname(callback);
   // alert(callfunct);
    if (callfunct == "additemtolistparttwo")
        callback(txtres, qty);
    if (callfunct == "removeitemparttwo")
        callback(itemname, destlistid, txtres);
    if (callfunct == "transferitemparttwo")
        callback(sourcelistid, destlistid, itemname, txtres);
    if (callfunct == "additemtocartparttwo")
        callback(txtres);

}
function ismodifylistmsgerror(msg) {
    // if this is error
    if (msg.toLowerCase().indexOf("error:") != -1) {
        return true;
    }
        // not error
    else {
        return false;
    }
}
/*
function modifylistitem(sourcelistid, destlistid, qty, changetype, itemname, destlistname, price, description) {
    var k = getkey();
    var p = getmodifyprompt(sourcelistid, destlistid, itemname, destlistname, changetype);
    if (p.length > 0) {
        if (!confirm(p))
            return 0;
    }
    if (!(price))
        price = "0";
    var s = "/Home/modifylistitem?sourcelistid=" + sourcelistid + "&destlistid=" + destlistid +
            "&item=" + itemname + "&qty=" + qty + "&t=" + k + "&changetype=" + changetype + "&price=" + price;
    var txtres = getstringfromajax(s);
    alert(txtres);
    return 1;
}
*/



function getmodifyprompt(sourcelistid, destlistid, itemname, destlistname, changetype) {

    if ((Number(sourcelistid) > 0))
        return "Are you sure you want to transfer " + itemname + " to " + destlistname + "?";
    else if (Number(changetype)== 2)
        return "Are you sure you want to remove " + itemname + "?";
    else
        return "";
}

// processtype 1 for add, 2 for delete
/*
function managemultilist(itemlist, listid, iscart, processtype) {
    var k = getkey();
    var a = "add";
    if (processtype == 2)
        a = "remove";
    var p = "Do you want to " + a + " these items";
    if (!confirm(p))
        return 0;
    var s = "/Home/managelistmultiselect?itemlist=" + itemlist + "&listid=" + listid +
            "&iscart=" + iscart + "&processtype=" + processtype + "&t=" + k;
    var txtres = getstringfromajax(s);
    alert(txtres);
    return 1;
}
*/
function managemultilist(itemlist, listid, iscart, processtype) {
    var k = getkey();
    var a = "add";
    if (processtype == 2)
        a = "remove";
    var p = "Do you want to " + a + " these items";
    showpopup(p, 1, "managemultilistparttwo('" + itemlist + "', '" + listid + "', '" +
            iscart + "', '" + processtype + "')", "return 0");

}
function managemultilistparttwo(itemlist, listid, iscart, processtype) {
    var s = "/Home/managelistmultiselect?itemlist=" + itemlist + "&listid=" + listid +
            "&iscart=" + iscart + "&processtype=" + processtype + "&t=" + k;
    var txtres = getstringfromajax(s);
    return 1;
}



// refreshing the cart
function refreshcartinfo() {
    var k = getkey();
    var s = "/Home/getcartinfo?t=" + k;
    var txtres = getstringfromajax(s);
    // if there are items then display data
    if ((txtres)) {
        if (txtres.indexOf(":") != -1) {
            var ar = txtres.split(":");
            var t = parseFloat(ar[1]);
            //alert(t.toFixed(2));
            $("#carttotal").html("Total:$" + t.toFixed(2));
            $("#cartqty").html(ar[0]);
            return txtres;
        }
    }
    // if items then diplay 0
    else {
        $("#carttotal").html("Total:$0.00");
        $("#cartqty").html("0");
    }
}

function getitemdescription(item, includecnt) {
    if (!(includecnt))
        includecnt = 0;
    var s = "/Home/getitemdesc?item=" + item + "&kind=" + includecnt;
    var txtres = getstringfromajax(s);
    return txtres;
}
function checkifinputnumber(qty) {
    if (!isposnumber(qty)) {
        return false;
    }
    return true;
}

function useridexpired(msg) {
    if (msg.indexOf("User expired") != -1) {
        var a = $("#expirelink").val();
        showpopup(msg);
        window.location.href = a;
        return true;
    }
    return false;
}

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

//-----------------File uploader object
// type 1 for promologo, 2, for promoicon
function fileuploader(test) {
    this.tmp = "kozel";
    this.tmp2 = test;

    this.storefile = function (type) {
        var a = getmethod(type);
        showpopup(a + "final");
    };
    function getmethod(type) {
        if (type == '1') {
            return "one method";
        }
        else {
            return "bad method";
        }
    }

}

function getimagefolder() {
    return "../images/";
}
/*
function fileuploader(type, filename, file) {
    alert("created");
    if (type == '1') {
        this.path = "/home/uploadpromofile";
    }
    
    this.storefile = function () {
        alert("file is stored");
    }
    
}*/
function testcallbackfunc(fnc) {
    //var callfunct = getfunctionname(fnc);
    alert(getfunctionname(fnc));
    fnc(callfunct)
}

function getfunctionname(fct) {
    if (!(fct.name)) {
        var f = findtextbetweenstrings(fct.toString().toLowerCase(), "function", "(", 1);
        return f.trim();
    }
    else {
        return fct.name;
    }
}
function getfunctionnamebad(fnct) {
    var nm = '';
    if (!(function fnct() { }).name) {
        Object.defineProperty(Function.prototype, 'name', {
            get: function () {
                var name = (this.toString().match(/^function\s*([^\s(]+)/) || [])[1];
                // For better performance only parse once, and then cache the
                // result through a new accessor for repeated access.
                Object.defineProperty(this, 'name', { value: name });
                nm = name;
            }
        });
    }
    else {
        nm = fnct.name;
    }
    return nm;
}
function makespacer(needlong, hasdiv, spacername) {

    var haslngtxt = $("#" + hasdiv).css("height");
    var haslng = haslngtxt.replace("px", "");
    if (haslng >= needlong) {
        $("#" + spacername).css("display", "none");
    }
    else {
        var mr = needlong - haslng;

        $("#" + spacername).css("display", "block");
        $("#" + spacername).css("min-height", mr + "px");
    }
}