using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PTEKNIK.Models;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace PTEKNIK.Controllers.MONTAJ
{
    public class MontajBasController : Controller
    {
        IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString);
        // GET: MontajBas
        HttpCookie cookie = GLOBALS.GetCookie("fichenos");

        private void LoginCheck()
        {
            if (!GLOBALS.CookieAuthClients()) { RedirectToAction("Index", "Home"); }
        }
        //listeleme burda yapılacak
        [Route("MontajBas/Index")]
        public ActionResult Index()
        {
            return View();
        }
        [Route("MontajBas/GetFormsJson")]
        [System.Web.Mvc.HttpGet]
        public JsonResult GetFormsJson(string SQLAra, string formtype, string durum)
        {

            string LSqlAra = "";
            if (SQLAra != "")
            {
                LSqlAra += "and replace(ADRES+ADSOYAD,' ','') like '%" + SQLAra.Replace(" ", "") + "%' ";
            }
            if (!String.IsNullOrEmpty(durum))
            {
                LSqlAra += " and durum=" + durum;
            }
            List<FORMLines> _formLists = new List<FORMLines>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                _formLists = db.Query<FORMLines>("select (CASE WHEN DURUM=1 THEN 'bg-success' WHEN DURUM=-1 THEN 'bg-danger' ELSE '' END) AS TRSTYLE,"+
                    "(CASE WHEN DURUM=1 THEN 'ONAYLANDI' WHEN DURUM=-1 THEN 'REDDEDİLDİ' WHEN DURUM=2 THEN 'ÖDENDİ' ELSE 'Gönderildi Bekliyor' END) AS ONAYDURUM" +
                    ",* from FORMLINE where   CLIENTREF=" + GLOBALS.GetCookie("giris")["CLIENTREF"] + " and formtype='" + formtype + "' and ((GOSTER=1 and ISTALEP=0) or (ISTALEP=1) " + LSqlAra).ToList();
            }
            return Json(_formLists, JsonRequestBehavior.AllowGet);
        }
        //FormBaşlığı Eklenecek
        [Route("MontajBas/FormBas")]
        public ActionResult FormBas(string ficheno)
        {
            FORMLines _DefaultModel = new FORMLines();
            if (ficheno != "0")
            {
                using (db)
                {
                    _DefaultModel = db.Query<FORMLines>("select * from FORMLINE where FICHENO='" + ficheno + "'").SingleOrDefault();
                }
            }
            else
            {
                _DefaultModel.BELGENO = Convert.ToInt32(GLOBALS.GetCookie("fichenos")["BELGEID"]);
                _DefaultModel.BELGESTR = GLOBALS.GetCookie("fichenos")["CLIENTREF"];
                _DefaultModel.FORMTYPE = "MONTAJ";
                _DefaultModel.MONTAJDATE = DateTime.Now;
            }
            return View(_DefaultModel);
        }
        [Route("MontajBas/FormBas")]
        [HttpPost]
        public ActionResult FormBas(string ficheno, FORMLines FormLines)
        {

            bool durum = false;
            LoginCheck();
            //zorunlu alan kontrolü
            if (String.IsNullOrEmpty(FormLines.TEL) && String.IsNullOrEmpty(FormLines.TEL2))
            {
                TempData["msg"] = "<script>alert('En Az Bir Telefon Numarası Girmelisiniz!!!');</script>";
                return View();
            }
            if (String.IsNullOrEmpty(FormLines.ADRES) && String.IsNullOrEmpty(FormLines.ADSOYAD))
            {
                TempData["msg"] = "<script>alert('Müşteri Adı Ve Adres Girmeden İlerleyemezsiniz!!!');</script>";
                return View();
            }
            if ((FormLines.FORMTYPE == "MONTAJ" && FormLines.MONTAJDATE > DateTime.Now))
            {
                durum = false;
                TempData["msg"] = "<script>alert('İleri Bir Tarih Seçimi Yapamazsınız!!!');</script>";
                return View();
            }
            string MontajDate = Convert.ToDateTime(((FormLines.MONTAJDATE == null) ? DateTime.Parse("1900-01-01") : FormLines.MONTAJDATE)).ToString("MM/dd/yyyy");
            if (ficheno != "0")
            {
                string sqlQuery = "update [dbo].[FORMLINE] set " +
         "ADSOYAD='" + FormLines.ADSOYAD + "'" +
         ",ADRES='" + FormLines.ADRES + "'" +
         ",TEL='" + FormLines.TEL + "'" +
         ",TEL2='" + FormLines.TEL2 + "'" +
         ",EMAIL='" + FormLines.EMAIL + "'" +
         ",MONTAJDATE='" + MontajDate + "'" +
         ",FIRMA='" + FormLines.FIRMA + "'" +
         ",SATICIFIRMA='" + FormLines.SATICIFIRMA + "'" +
         ",FATURANO='" + FormLines.FATURANO + "'" +
         ",GARANTINO='" + FormLines.GARANTINO + "'" +
         ",CIHAZ='" + FormLines.CIHAZ + "'" +
         ",CIHAZSERINO='" + FormLines.CIHAZSERINO + "'" +
         ",CACIKLAMA='" + FormLines.CACIKLAMA + "'" +
         " WHERE FICHENO='" + ficheno + "'";
                int rowsAffected = db.Execute(sqlQuery);
                return RedirectToAction("FormLine", new { ficheno = ficheno,adsoyad=FormLines.ADSOYAD });
            }


            FormLines.BELGENO = Convert.ToInt32(GLOBALS.GetCookie("fichenos")["BELGEID"]);
            FormLines.BELGESTR = GLOBALS.GetCookie("fichenos")["CLIENTREF"];
            using (db)
            {
                string sqlQuery = "INSERT INTO [dbo].[FORMLINE](ADDDATE,NAME,FORMTYPE,FICHENO,[BELGENO],[BELGESTR],[ADSOYAD],[ADRES],[TEL],[TEL2],[EMAIL],[MONTAJDATE],[FIRMA],[SATICIFIRMA],ACIKLAMA,CLIENTREF)VALUES" +
           "(GETDATE(),(select DEFINITION_ FROM LKSDB..LG_"+GLOBALS.GFirma+"_CLCARD WHERE LOGICALREF="+ GLOBALS.GetCookie("giris")["CLIENTREF"] + "),"
           +"'" + FormLines.FORMTYPE + "','" + FormLines.BELGESTR + "-" + FormLines.BELGENO + "'," + FormLines.BELGENO + "" +
           ",'" + FormLines.BELGESTR + "'" +
           ",'" + FormLines.ADSOYAD + "'" +
           ",'" + FormLines.ADRES + "'" +
           ",'" + FormLines.TEL + "'" +
           ",'" + FormLines.TEL2 + "'" +
           ",'" + FormLines.EMAIL + "'" +
           ",cast('" + MontajDate + "' as datetime)" +
           ",'" + FormLines.FIRMA + "'" +
           ",'" + FormLines.SATICIFIRMA + "'" +
           ",'" + FormLines.ACIKLAMA + "'" +
           ",'" + GLOBALS.GetCookie("giris")["CLIENTREF"] + "'" +
           ")";
                int rowsAffected = db.Execute(sqlQuery);
                int rowsUpdated = db.Execute("update USERS set BELGEID=BELGEID+1 where LOGICALREF=" + GLOBALS.GetCookie("giris")["USERID"] + "");
                int belgeid = Convert.ToInt32(cookie["BELGEID"]);
                belgeid++;
                cookie["BELGEID"] = belgeid.ToString();
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("FormLine", new { ficheno = FormLines.BELGESTR + "-" + FormLines.BELGENO, adsoyad = FormLines.ADSOYAD });
        }
        [Route("MontajBas/FormLine")]
        public ActionResult FormLine(string ficheno)
        {
            FORMUrun _formUrun = new FORMUrun();
            _formUrun.FORMTYPE = "MONTAJ";
            _formUrun.FICHENO = ficheno;
            return View(_formUrun);
        }

        [HttpPost]
        [Route("MontajBas/FormLine")]
        public ActionResult FormLine(FORMUrun obj)
        {
            //barkodlar ile ilgili kontorller
            if (String.IsNullOrEmpty(obj.ICBARKOD) || String.IsNullOrEmpty(obj.DISBARKOD))
            {
                ViewData["msg"] = "<script>alert('İç Ve Dış Barkodlar Girilmeden İlerleyemezsiniz!!!');</script>";
                return View();
            }
            SqlDataReader oku;
            if (obj.FORMTYPE == "MONTAJ")
            {
                oku = GLOBALS.GetDataReader("select LOGICALREF from FORMUrun where FORMTYPE = 'MONTAJ' and GOSTER=1  and(ICBARKOD in ('" + obj.ICBARKOD + "', '" + obj.DISBARKOD + "')" +
                " or DISBARKOD in ('" + obj.ICBARKOD + "','" + obj.DISBARKOD + "') or BARKOD3 in ('" + obj.ICBARKOD + "','" + obj.DISBARKOD + "') ) ");
                if (oku.Read())
                {
                    ViewData["msg"] = "<script>alert('Önceden Montajı Yapılmış Bir Ürünü Monte Edemezsiniz!!!');</script>";
                    return View();
                }
                oku = GLOBALS.GetDataReader("select * from FORMUrun where FICHENO='"+obj.FICHENO+ "' and(ICBARKOD in ('" + obj.ICBARKOD + "', '" + obj.DISBARKOD + "')" +
                " or DISBARKOD in ('" + obj.ICBARKOD + "','" + obj.DISBARKOD + "') or BARKOD3 in ('" + obj.ICBARKOD + "','" + obj.DISBARKOD + "') ) ");
                if (oku.Read())
                {
                    ViewData["msg"] = "<script>alert('Aynı Barkoddan Giriş Yapamazsınız!!!');</script>";
                    return View();
                }
            }
            oku = GLOBALS.GetDataReader("select count(LOGICALREF) as Sayi from LKSDB..AA_FormUrun where Barcode='" + obj.ICBARKOD + "'  or Barcode='" + obj.DISBARKOD + "' or Barcode='" + obj.BARKOD3 + "' ");
            if (oku.Read())
            {
                int sayi = Convert.ToInt32(oku[0].ToString());
                if (sayi < 2)
                {
                    ViewData["msg"] = "<script>alert('Ticari sistemimizde Girdiğiniz Barkodlardan Eşleşen Bir Ürün Bulunmamaktadır!!!.');</script>";
                    return View();
                }
                else
                {
                    string query = "insert into FORMUrun (FORMTYPE,FICHENO,ADDDATE,ICBARKOD,DISBARKOD,BARKOD3,FORMREF) values" +
                        "('" + obj.FORMTYPE + "','" + obj.FICHENO + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','" + obj.ICBARKOD + "','" + obj.DISBARKOD + "','" + obj.BARKOD3 + "'," + obj.FORMREF + ")";
                    int affected = db.Execute(query);
                }
            }
            return RedirectToAction("FormLine", new { ficheno = obj.FICHENO });
        }
        [Route("MontajBas/GetFormUrunList")]
        [HttpGet]
        public JsonResult GetFormUrunList(string FICHENO)
        {
            List<FormBarkod> _list = new List<FormBarkod>();
            if (String.IsNullOrEmpty(FICHENO))
            {
                FICHENO = "0";
                return Json(_list, JsonRequestBehavior.AllowGet);
            }
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                _list = db.Query<FormBarkod>("select  (CASE WHEN FATTURU='MD' THEN 'MONTAJ DAHİL' ELSE 'HARİÇ' END) AS MONTAJ,LOGICALREF,ICBARKOD,DISBARKOD,BARKOD3,UrunAdi,FICHENO" +
                " from FORMURUNView as F where replace(FICHENO,' ','')='" + FICHENO.Replace(" ", "") + "'").ToList();
            }
            return Json(_list, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("MontajBas/FormUrunSil")]
        public ActionResult FormUrunSil(FormBarkod obj)
        {
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                int rowsAffected = db.Execute("delete from FORMUrun where LOGICALREF=" + obj.LOGICALREF + "");
            }
            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
        //yapılan hizmet ekranı methodları
        [Route("MontajBas/FormServ")]
        public ActionResult FormServ(string ficheno)
        {
            return View();
        }
        [HttpPost]
        [Route("MontajBas/FormServ")]
        public ActionResult FormServ(string ficheno, FORMServes _formServ)
        {
            using (db)
            {
                int affectedRows = db.Execute("insert into FORMServes (FICHENO,ACIKLAMA,SERVNAME,ADDDATE) values" +
                    "('" + ficheno + "','"+_formServ.ACIKLAMA+"','" + _formServ.SERVNAME + "','"+DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")+"')");
            }
            return RedirectToAction("FormServ",new {ficheno=ficheno });
        }
        [HttpGet]
        [Route("MontajBas/GetFormServesLogo")]
        public JsonResult GetFormServesLogo()
        {
            List<FORMServes> _logoSrv = new List<FORMServes>();
            using (db)
            {
                _logoSrv = db.Query<FORMServes>("select DEFINITION_ as SERVNAME from FORMServesLogo order by DEFINITION_").ToList();
            }
            return Json(_logoSrv, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("MontajBas/ServSatirSil")]
        public JsonResult ServSatirSil(string LOGICALREF)
        {
            using (db)
            {
                int affectedRows = db.Execute("delete from FORMServes where LOGICALREF="+ LOGICALREF + "");
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [Route("MontajBas/GetInsertedData")]
        public JsonResult GetInsertedData(string ficheno)
        {
            List<FORMServes> _logoSrv = new List<FORMServes>();
            using (db)
            {
                _logoSrv = db.Query<FORMServes>("select * from FORMServes where replace(FICHENO,' ','')='"+ ficheno.Replace(" ","") + "'").ToList();
            }
            return Json(_logoSrv, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("MontajBas/FormSilJson")]
        public JsonResult FormSilJson(string ficheno,string ADDDATE) {
            int affectedRow = 0;
            SqlDataReader oku = GLOBALS.GetDataReader("select DURUM from FORMLINE where FICHENO='"+ficheno+"'");
            if (oku.Read())
            {
                if (String.IsNullOrEmpty(oku[0].ToString())|| oku[0].ToString()=="0")
                {
                    using (db)
                    {
                        affectedRow += db.Execute("update FORMLINE set GOSTER=0 where  replace(FICHENO,' ','')='" + ficheno.Replace(" ", "") + "'");
                        affectedRow += db.Execute("delete from FORMServes  where  replace(FICHENO,' ','')='" + ficheno.Replace(" ", "") + "'");
                        affectedRow += db.Execute("delete from  FORMUrun  where  replace(FICHENO,' ','')='" + ficheno.Replace(" ", "") + "'");
                    }
                }
            }
           
            bool durum = (affectedRow > 0) ? true : false;
            return Json(new { success=durum},JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [Route("MontajBas/FormKayitBitir")]
        public JsonResult FormKayitBitir(string ficheno)
        {
            int affectedRow = 0;
            using (db)
            {
                affectedRow += db.Execute("update FORMLINE set GOSTER=1 where replace(FICHENO,' ','')='" + ficheno.Replace(" ","") + "'");
                affectedRow += db.Execute("UPDATE FORMServes set GOSTER=1 where replace(FICHENO,' ','')='" + ficheno.Replace(" ", "") + "'");
                affectedRow += db.Execute("update FORMUrun set GOSTER=1 where replace(FICHENO,' ','')='" + ficheno.Replace(" ", "") + "'");
            }
            bool durum = (affectedRow > 0) ? true : false;
            return Json(new { success = durum }, JsonRequestBehavior.AllowGet);
        }

        [Route("MontajBas/FormOnizle")]
        public ActionResult FormOnizle(string ficheno) {
            FORMLines _formLines = new FORMLines();
            using (db)
            {
                _formLines = db.Query<FORMLines>("select * from FORMLINE where FICHENO='"+ficheno+"'").SingleOrDefault();
            }
            return View(_formLines);
        }
    }
}