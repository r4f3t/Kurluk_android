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
namespace PTEKNIK.Controllers
{
    public class OnayController : Controller
    {
        IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString);

        HttpCookie cookie = GLOBALS.GetCookie("fichenos");

        // GET: Onay
        [Route("Onay/Index")]
        public ActionResult Index()
        {
            return View();
        }
        [Route("Onay/FormList")]
        public ActionResult FormList(string searchParams,string formtype,string sort,string durumSearch)
        {

            List<FORMLines> _formList = new List<FORMLines>();
            using (db)
            {
                string SqlAra = "";
                string sortSTR = (String.IsNullOrEmpty(sort)) ? "" : sort;
                if (!String.IsNullOrEmpty(formtype))
                {
                    SqlAra += " and FORMTYPE='"+formtype+"'";
                }
                if (!String.IsNullOrEmpty(searchParams))
                {
                    searchParams = " and replace(F.NAME+F.ADSOYAD,' ','') like '%"+searchParams.Replace(" ","")+"%'";
                }
                _formList = db.Query<FORMLines>("select (CASE WHEN DURUM=1 THEN 'bg-success' WHEN DURUM=-1 THEN 'bg-danger' ELSE '' END) AS TRSTYLE,F.FORMTYPE,F.FICHENO,F.ADSOYAD,F.ACIKLAMA,F.CACIKLAMA,C.DEFINITION_ AS NAME,C.CODE,"+
                       "(CASE WHEN DURUM=1 THEN 'ONAYLANDI' WHEN DURUM=-1 THEN 'REDDEDİLDİ' WHEN DURUM=2 THEN 'ÖDENDİ' ELSE '' END) AS ONAYDURUM" +
                       ",F.MONTAJDATE,F.ARIZADATE,F.ADDDATE"+
                    " from FORMLINE as F " +
                    " INNER JOIN LKSDb..LG_" + GLOBALS.GFirma + "_CLCARD AS C ON F.CLIENTREF=C.LOGICALREF WHERE 1=1 and GOSTER=1 " + searchParams +" "+SqlAra + " "+durumSearch+" "+sortSTR).ToList();
            }
            return View(_formList);
        }
        [Route("Onay/FormBas")]
        public ActionResult FormBas(string ficheno, string formtype)
        {
            FORMLines _DefaultModel = new FORMLines();
            using (db)
            {
                _DefaultModel = db.Query<FORMLines>("select * from FORMLINE where FICHENO='"+ficheno+"'").SingleOrDefault();
            }
            ViewData["MONTAJDATE"] = (formtype != "MONTAJ") ? "display:none;" : "";
            ViewData["ARIZADATE"] = (formtype != "ARIZA") ? "display:none;" : "";
            return View(_DefaultModel);
        }
        [Route("Onay/GetFormUrunList")]
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
                _list = db.Query<FormBarkod>("select   (CASE WHEN FATTURU='MD' THEN 'bg-success' ELSE '' END) AS TRSTYLE,"
                    + "(CASE WHEN FATTURU='MD' THEN 'MONTAJ DAHİL' ELSE 'HARİÇ' END) AS MONTAJ,LOGICALREF,ICBARKOD,DISBARKOD,BARKOD3,UrunAdi,FICHENO," +
                    "isnull((select isnull(datediff(day,getdate(),dateadd(year,2,ADDDATE)),0) from FORMURUNView as L where FORMTYPE='MONTAJ' and (L.ICBARKOD=F.ICBARKOD or L.DISBARKOD=F.DISBARKOD)),0) as GARANTI" +
                    ",FATNO,FATDATE,DEFINITION_ AS FATCARI"+
                    " from FORMURUNView as F where replace(FICHENO,' ','')='" + FICHENO.Replace(" ", "") + "'").ToList();
            }
            return Json(_list, JsonRequestBehavior.AllowGet);
        }
        [Route("Onay/FormOnay")]
        public ActionResult FormOnay(string ficheno, string onay,string aciklama) {
            using (db)
            {
                int rowsAffected = db.Execute("update FORMLINE set DURUM="+onay+",ADMACIKLAMA='"+aciklama+"' where FICHENO='"+ficheno+"'");
            }
            //mail atılacak
            return RedirectToAction("FormList");
        }
        [Route("Onay/GetFormServes")]
        [HttpGet]
        public JsonResult GetFormServes(string ficheno)
        {
            List<FORMServes> _logoSrv = new List<FORMServes>();
            using (db)
            {
                _logoSrv = db.Query<FORMServes>("select * from FORMServes where replace(FICHENO,' ','')='" + ficheno.Replace(" ", "") + "'").ToList();
            }
            return Json(_logoSrv, JsonRequestBehavior.AllowGet);
        }
    }
}