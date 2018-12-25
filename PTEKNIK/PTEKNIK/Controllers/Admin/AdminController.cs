using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using PTEKNIK.Models;
using Dapper;
using System.Web.Configuration;
using System.IO;

namespace PTEKNIK.Controllers.Admin
{
    public class AdminController : Controller
    {
        IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString);
        HttpCookie giris = GLOBALS.GetCookie("giris");
        HttpCookie cookie = GLOBALS.GetCookie("fichenos");
        // GET: Admin
        [Route("Admin/Index")]
        public ActionResult Index()
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            return View();
        }
        [Route("Admin/UserList")]
        public ActionResult UserList()
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            List<USERS> USERLists = new List<USERS>();
            using (db)
            {
                USERLists = db.Query<USERS>("Select  U.LOGICALREF, U.USERNAME, U.PASSWORD, AUTHORITY, AUTHID, FLAG, ADDDATE,C.LOGICALREF as CLIENTREF, C.DEFINITION_ as NAME,U.CODE"
                    + " From USERS as U left outer join LKSDB..LG_" + GLOBALS.GFirma + "_CLCARD as C on U.CODE=C.CODE where 1=1 ").ToList();
            }
            return View(USERLists);
        }
        [Route("Admin/ClientList")]
        public ActionResult ClientList()
        {
            return View();
        }



        [HttpGet]
        [Route("Admin/GetClients")]
        public ActionResult GetClients(string searchSTR)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            List<CLIENTS> USERLists = new List<CLIENTS>();
            using (db)
            {
                USERLists = db.Query<CLIENTS>("Select DEFINITION_,CODE,SPECODE,LOGICALREF,TELNRS1 From LKSDB..LG_" + GLOBALS.GFirma + "_CLCARD where 1=1  and CODE like '120%' and replace(DEFINITION_+CODE,' ','') like '%" + searchSTR.Replace(" ", "") + "%' order by DEFINITION_").ToList();
            }
            return Json(USERLists, JsonRequestBehavior.AllowGet);
        }
        [Route("Admin/UserAdd")]
        public ActionResult UserAdd(string clientref, string def, string code, string authid,string telnrs1)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            //kullanıcı kontrolü
            SqlDataReader oku = GLOBALS.GetDataReader("select LOGICALREF from USERS where CLIENTREF="+clientref+"");
            if (oku.Read())
            {
                return RedirectToAction("UserEdit",new { id=oku[0].ToString(),fromAdd=1 });
            }
            USERS _users = new USERS();
            _users.CLIENTREF = Convert.ToInt32((clientref == null) ? "0" : clientref);
            _users.NAME = def;
            _users.CODE = code;
            _users.AUTHID = Convert.ToInt32(authid);
            _users.TEL2 = telnrs1;
            _users.AUTHORITY = (authid == "1") ? "Teknik Servis" : "Admin";
            return View(_users);
        }

        [HttpPost]
        [Route("Admin/UserAdd")]
        public ActionResult UserAdd(USERS PostedUsers)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            using (db)
            {
                
               

                string sqlQuery = "INSERT INTO [dbo].[USERS]" +
           "([USERNAME]" +
           ",[PASSWORD]" +
           ",[AUTHORITY]" +
           ",[AUTHID]" +
           ",[FLAG]" +
           ",[ADDDATE]" +
           ",[CLIENTREF]" +
           ",[TEL]" +
           ",[TEL2]" +
           ",[EMAIL]" +
           ",[NAME]" +
           ",[CODE])" +
     "VALUES" +
           "('" + PostedUsers.USERNAME + "'" +
           ",'" + PostedUsers.PASSWORD + "'" +
           ",'" + PostedUsers.AUTHORITY + "'" +
           ",'" + PostedUsers.AUTHID + "'" +
           ",0" +
           ",'" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'" +
           ",'" + PostedUsers.CLIENTREF + "'" +
           ",'" + PostedUsers.TEL + "'" +
           ",'" + PostedUsers.TEL2 + "'" +
           ",'" + PostedUsers.EMAIL + "'" +
           ",'" + PostedUsers.NAME + "'" +
           ",'" + PostedUsers.CODE + "')";

                int rowsAffected = db.Execute(sqlQuery);
            }
            return RedirectToAction("UserList");

        }
        [Route("Admin/UserEdit")]
        public ActionResult UserEdit(string id,string fromAdd)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            USERS _users = new USERS();
            if (fromAdd == "1") { TempData["msg"] = "<script>alert('Bu Firmanın Kullanıcısı Mevcut Güncelleme Yapabilirsiniz!!!');</script>"; }
            using (db)
            {
                _users = db.Query<USERS>("select * from USERS where LOGICALREF=" + id).SingleOrDefault();
            }
            return View(_users);
        }

        [Route("Admin/UserEdit")]
        [HttpPost]
        public ActionResult UserEdit(string id, USERS _users)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            using (db)
            {
                db.Execute("update USERS set USERNAME='"+_users.USERNAME+ "',PASSWORD='"+_users.PASSWORD+"',CODE='"+_users.CODE+"'"+
                    ",TEL='"+_users.TEL+ "',TEL2='" + _users.TEL2 + "',EMAIL='" + _users.EMAIL + "'" +
                    " where LOGICALREF="+id+"");
            }
            return RedirectToAction("UserList");
        }
        [Route("Admin/UserDetail")]
        public ActionResult UserDetail(string id)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            USERS _users = new USERS();
            using (db)
            {
                _users = db.Query<USERS>("select * from USERS where LOGICALREF=" + id).SingleOrDefault();
            }

            return View(_users);
        }
        [Route("Admin/UserDelete")]
        public ActionResult UserDelete(string id)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            USERS _users = new USERS();
            using (db)
            {
                _users = db.Query<USERS>("select * from USERS where LOGICALREF=" + id).SingleOrDefault();
            }

            return View(_users);
        }

        [Route("Admin/UserDelete")]
        [HttpPost]
        public ActionResult UserDelete(string id, FormCollection collection)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            using (db)
            {
                db.Execute("delete from USERS where LOGICALREF=" + id);
            }
            return RedirectToAction("UserList");
        }
        //userbitti
        [Route("Admin/TeknikTalep")]
        public ActionResult TeknikTalep(string clientid,string caridef,string carikod)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }

            FORMLines _fORMLines = new FORMLines();
            _fORMLines.BELGENO = Convert.ToInt32(cookie["BELGEID"]);
            _fORMLines.BELGESTR = cookie["USERID"];
            _fORMLines.CLIENTREF = Convert.ToInt32((clientid==null)?"0":clientid);
            _fORMLines.NAME = caridef;
            _fORMLines.CODE = carikod;
            return View(_fORMLines);
        }
        [HttpPost]
        [Route("Admin/TeknikTalep")]
        public ActionResult TeknikTalep(FORMLines _talep)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            using (db)
            {
                string sqlQuery = "INSERT INTO [dbo].[FORMLINE](ISTALEP,FICHENO,ACIKLAMA,CLIENTREF,NAME,CODE,FORMTYPE,ADMIN,DURUM,BELGESTR,BELGENO,ADSOYAD,FIRMA,ADRES,TEL,ADDDATE) VALUES" +
                    "(1,'" + _talep.BELGESTR + "-" + _talep.BELGENO + "','" + _talep.ACIKLAMA+"',"+_talep.CLIENTREF+",'"+_talep.NAME+"','"+_talep.CODE+"','" + _talep.FORMTYPE + "'," + giris["USERID"] + ",0,'" + _talep.BELGESTR + "'," + _talep.BELGENO + ",'" + _talep.ADSOYAD + "','" + _talep.FIRMA + "','" + _talep.ADRES + "','" + _talep.TEL + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "') ";

                int rowsAffected = GLOBALS.db.Execute(sqlQuery);
                GLOBALS.db.Execute("update USERS set BELGEID=BELGEID+1 where LOGICALREF=" + giris["USERID"] + "");
                int belgeid = Convert.ToInt32(cookie["BELGEID"]);
                belgeid++;
                cookie["BELGEID"] = belgeid.ToString();
                Response.Cookies.Add(cookie);
                //mail at
            }
            return RedirectToAction("TeknikTalepler", "Admin");
        }

        [Route("Admin/TeknikTalepler")]
        public ActionResult TeknikTalepler(string searchSTR)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            List<FORMLines> _formLines = new List<FORMLines>();
            using (db)
            {
                _formLines = db.Query<FORMLines>("select * from FORMLINE where DURUM=0 and ISTALEP=1 order by ADDDATE desc").ToList();
            }
            return View(_formLines);
        }
        [Route("Admin/UrunList")]
        public ActionResult UrunList(string searchSTR)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            List<BarcodeUrun> _barcodeUrun = new List<BarcodeUrun>();
            using (db)
            {
                _barcodeUrun = db.Query<BarcodeUrun>("Select top 200 * from LKSDB..AA_FormUrun").ToList();
            }
            return View(_barcodeUrun);
        }

        [Route("Admin/GetUrunList")]
        [HttpGet]
        public ActionResult GetUrunList(string searchSTR)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            string SqlAra = "";
            if (searchSTR != "")
            {
                SqlAra += " and replace(Barcode,' ','') like '%" + searchSTR.Replace(" ", "") + "%'";
            }
            List<BarcodeUrun> _barcodeUrun = new List<BarcodeUrun>();
            using (db)
            {
                _barcodeUrun = db.Query<BarcodeUrun>("Select top 200 * from LKSDB..AA_FormUrun where 1=1 " + SqlAra + " order by UrunAdi ").ToList();
            }
            return Json(_barcodeUrun, JsonRequestBehavior.AllowGet);
        }

        [Route("Admin/TeknikTalepDetail")]
        // GET: Forms/Details/5
        public ActionResult TeknikTalepDetail(int id, string formtype)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            FORMTYPECheck(formtype);
            FORMLines FormDetail = new FORMLines();
            using (db)
            {

                FormDetail = db.Query<FORMLines>("Select * From FORMLINE where LOGICALREF=" + id, new { id }).SingleOrDefault();
            }
            return View(FormDetail);

        }
        public void FORMTYPECheck(string formtype)
        {
            if (formtype == "Arıza")
            {
                ViewData["FormAddTitle"] = "Arıza Formu Ekranı";
                ViewData["montajdateStyle"] = "display:none";
            }
            else if (formtype == "montaj")
            {
                ViewData["FormAddTitle"] = "Montaj Formu Ekranı";
                ViewData["arizadateStyle"] = "display:none";
            }
        }

        [Route("Forms/TeknikTalepEdit")]
        // GET: Forms/Edit/5
        public ActionResult TeknikTalepEdit(int id, string formtype)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            FORMTYPECheck(formtype);
            FORMLines FormDetail = new FORMLines();
            using (db)
            {

                FormDetail = db.Query<FORMLines>("Select * From FORMLINE where LOGICALREF=" + id, new { id }).SingleOrDefault();
            }
            return View(FormDetail);
        }
        [Route("Forms/TeknikTalepEdit")]
        // POST: Forms/Edit/5
        [HttpPost]
        public ActionResult TeknikTalepEdit(int id, FORMLines FormLines)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            using (db)
            {
                string sqlQuery = "update [dbo].[FORMLINE] set " +
           "BELGENO=" + FormLines.BELGENO + "" +
           ",BELGESTR='" + FormLines.BELGESTR + "'" +
           ",ADSOYAD='" + FormLines.ADSOYAD + "'" +
           ",ADRES='" + FormLines.ADRES + "'" +
           ",TEL='" + FormLines.TEL + "'" +
            " WHERE LOGICALREF=" + id;
                int rowsAffected = db.Execute(sqlQuery);
            }

            return RedirectToAction("TeknikTalepler");
        }


        [Route("Forms/TeknikTalepDelete")]
        // GET: Forms/Delete/5
        public ActionResult TeknikTalepDelete(int id, string formtype)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            FORMTYPECheck(formtype);
            FORMLines FormDetail = new FORMLines();
            using (db)
            {
                FormDetail = db.Query<FORMLines>("select * from FORMLINE where LOGICALREF=" + id + "").SingleOrDefault();
            }

            return View(FormDetail);
        }
        [Route("Forms/TeknikTalepDelete")]
        // POST: Forms/Delete/5
        [HttpPost]
        public ActionResult TeknikTalepDelete(int id, FormCollection collection)
        {
            if (!GLOBALS.CookieAuth()) { return RedirectToAction("Index", "Home"); }
            using (db)
            {
                int rowsAffected = db.Execute("delete from FORMLINE where LOGICALREF=" + id);
            }

            return RedirectToAction("TeknikTalepler");

        }
        [Route("Admin/TeknikServisList")]
        public ActionResult TeknikServisList(string SearchSTR)
        {
            string SQLAra = "";
            if (SearchSTR != null)
            {
                SQLAra += " and replace(NAME+CODE,' ','') like '%" + SearchSTR + "%' ";
            }
            List<USERS> _userList = new List<USERS>();
            using (db)
            {
                _userList = db.Query<USERS>("select USERNAME,CLIENTREF,NAME,CODE,TEL,EMAIL from USERS where AUTHID=1 " + SQLAra).ToList();
            }
            return View(_userList);
        }

        [Route("Admin/Dokumanteler")]
        public ActionResult Dokumanteler() {
            
            return View();
        }
        [HttpPost]
        [Route("Admin/Dokumanteler")]
        public ActionResult Dokumanteler(System.Web.HttpPostedFileBase PATH, Documents _docs)
        {
            using (db)
            {
                if (!String.IsNullOrEmpty(_docs.PATH))
                {
                    string dosyaYolu = Path.GetFileName(PATH.FileName);
                    var yuklemeYeri = Path.Combine(Server.MapPath("~/Dokumantasyonlar"), dosyaYolu);
                    PATH.SaveAs(yuklemeYeri);
                    int affectedRows = db.Execute("insert into Documents (PATH,ACIKLAMA) values " +
                    "('" + PATH.FileName + "','" + _docs.ACIKLAMA + "')");
                }
            }
            return RedirectToAction("Dokumanteler");
        }

        [HttpGet]
        [Route("Admin/GetDokumans")]
        public JsonResult GetDokumans() {
            List<Documents> _list = new List<Documents>();
            using (db)
            {
                _list = db.Query<Documents>("select * from Documents order by LOGICALREF ").ToList();
            }
            return Json(_list,JsonRequestBehavior.AllowGet);
        }

    }
}