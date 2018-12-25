using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using PTEKNIK.Models;
using System.Configuration;
namespace PTEKNIK.Controllers.Yonetim
{
    public class YonetimController : Controller
    {
        IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString);
        // GET: Yonetim
        [Route("Yonetim/Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("Yonetim/HakOnayList")]
        public ActionResult HakOnayList(string durum) {
            List<FORMLines> _list = new List<FORMLines>();
            using (db)
            {
                //admin onaylamış parası verilmemiş
                _list = db.Query<FORMLines>("select * from FORMLINE where  DURUM="+durum+" ").ToList();
            }
            return View(_list);
        }
        [Route("Yonetim/FormBas")]
        public ActionResult FormBas(string ficheno, string formtype)
        {
            FORMLines _DefaultModel = new FORMLines();
            using (db)
            {
                _DefaultModel = db.Query<FORMLines>("select * from FORMLINE where FICHENO='" + ficheno + "'").SingleOrDefault();
            }
            ViewData["MONTAJDATE"] = (formtype != "MONTAJ") ? "display:none;" : "";
            ViewData["ARIZADATE"] = (formtype != "ARIZA") ? "display:none;" : "";
            return View(_DefaultModel);
        }
        [Route("Yonetim/GetFormUrunList")]
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
                    "(select dateadd(year,2,ADDDATE) from FORMURUNView as L where FORMTYPE='MONTAJ' and (L.ICBARKOD=F.ICBARKOD or L.DISBARKOD=F.DISBARKOD)) as GARANTI" +
                " from FORMURUNView as F where replace(FICHENO,' ','')='" + FICHENO.Replace(" ", "") + "'").ToList();
            }
            return Json(_list, JsonRequestBehavior.AllowGet);
        }
        [Route("Yonetim/FormOnay")]
        public ActionResult FormOnay(string ficheno, string onay, string aciklama)
        {
            using (db)
            {
                int rowsAffected = db.Execute("update FORMLINE set DURUM=" + onay + ",YONACIKLAMA='" + aciklama + "' where FICHENO='" + ficheno + "'");
            }
            //mail atılacak
            return RedirectToAction("HakOnayList","Yonetim",new { durum=1});
        }
        [Route("Yonetim/GetFormServes")]
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