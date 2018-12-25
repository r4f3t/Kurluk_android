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

namespace PTEKNIK.Controllers.Forms
{
    public class FormsController : Controller
    {
        HttpCookie giris = GLOBALS.GetCookie("giris");
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
        private void LoginCheck()
        {
            if (!GLOBALS.CookieAuthClients()) { RedirectToAction("Index", "Home"); }
        }

        [Route("Forms/Index")]
        public ActionResult Index()
        {
            LoginCheck();
            FORMSIndex _formsIndex = new FORMSIndex();
            using (IDbConnection db=new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                _formsIndex = db.Query<FORMSIndex>("select (select count(LOGICALREF) FROM FORMLINE WHERE (DURUM IS NULL OR DURUM=0) AND CLIENTREF="+GLOBALS.GetCookie("giris")["CLIENTREF"]+") AS BEKLEYEN"+
                    ",(select count(LOGICALREF) FROM FORMLINE WHERE DURUM>=1 AND CLIENTREF=" + GLOBALS.GetCookie("giris")["CLIENTREF"] + ") AS ONAYLANAN  "+
                     ",(select count(LOGICALREF) FROM FORMLINE WHERE DURUM<=-1 AND CLIENTREF=" + GLOBALS.GetCookie("giris")["CLIENTREF"] + ") AS REDDEDILEN").SingleOrDefault();
            }
            return View(_formsIndex);
        }

        [Route("Forms/FormList")]
        // GET: Forms
        public ActionResult FormList(string formtype, string SearchSTR)
        {
            LoginCheck();
            if (formtype == "Arıza")
            {
                ViewData["FormListTitle"] = "Arıza Formları";
            }
            else if (formtype == "montaj")
            {
                ViewData["FormListTitle"] = "Montaj Formları";
            }
            else
            {
                return RedirectToAction("Index");
            }
            string SqlAra = "";
            if (!String.IsNullOrEmpty(SearchSTR))
            {
                SqlAra = " and replace(BELGESTR,' ','') like '%" + SearchSTR.Replace(" ", "") + "%'";
            }
            List<FORMLines> FormLists = new List<FORMLines>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {

                FormLists = db.Query<FORMLines>("Select * From FORMLINE where CLIENTREF=" + GLOBALS.GetCookie("giris")["CLIENTREF"] + " and formtype='" + formtype + "' " + SqlAra).ToList();
            }
            return View(FormLists);

        }
        [Route("Forms/Details")]
        // GET: Forms/Details/5
        public ActionResult Details(int id)
        {
            LoginCheck();
            FORMLines FormDetail = new FORMLines();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {

                FormDetail = db.Query<FORMLines>("Select * From FORMLINE where LOGICALREF=" + id, new { id }).SingleOrDefault();
            }
            return View(FormDetail);

        }
        [Route("Forms/FormAdd")]
        // GET: Forms/Create
        public ActionResult FormAdd(string formtype)
        {
            LoginCheck();
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
            else
            {
                return Redirect("Index");
            }
            FORMLines _DefaultModel = new FORMLines();

            _DefaultModel.BELGENO = Convert.ToInt32(GLOBALS.GetCookie("giris")["BELGEID"]);
            _DefaultModel.BELGESTR = GLOBALS.GetCookie("giris")["CLIENTREF"];
            _DefaultModel.FORMTYPE = formtype;
            _DefaultModel.MONTAJDATE = DateTime.Now;
            _DefaultModel.ARIZADATE = DateTime.Now;
            _DefaultModel.FATURADATE = DateTime.Now;
            return View(_DefaultModel);
        }
        [Route("Forms/FormAdd")]
        // POST: Forms/Create
        [HttpPost]
        public ActionResult FormAdd(FORMLines FormLines)
        {
            LoginCheck();
            //zorunlu alan kontrolü
            if (FormLines.TEL == "" && FormLines.TEL2 == "")
            {

                return View();
            }
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                string sqlQuery = "INSERT INTO [dbo].[FORMLINE]([BELGENO],[BELGESTR],[ADSOYAD],[ADRES],[TEL],[TEL2],[EMAIL],[MONTAJDATE],[ARIZADATE],[FIRMA],[SATICIFIRMA],[FATURADATE],[FATURANO],[GARANTINO],[CIHAZ],[CIHAZSERINO])VALUES" +
           "(" + FormLines.BELGENO + "" +
           ",'" + FormLines.BELGESTR + "'" +
           ",'" + FormLines.ADSOYAD + "'" +
           ",'" + FormLines.ADRES + "'" +
           ",'" + FormLines.TEL + "'" +
           ",'" + FormLines.TEL2 + "'" +
           ",'" + FormLines.EMAIL + "'" +
           ",cast('" + Convert.ToDateTime(FormLines.MONTAJDATE).ToString("MM/dd/yyyy") + "' as datetime)" +
           ",cast('" + Convert.ToDateTime(FormLines.ARIZADATE).ToString("MM/dd/yyyy") + "' as datetime)" +
           ",'" + FormLines.FIRMA + "'" +
           ",'" + FormLines.SATICIFIRMA + "'" +
           ",cast('" + Convert.ToDateTime(FormLines.FATURADATE).ToString("MM/dd/yyyy") + "' as datetime)" +
           ",'" + FormLines.FATURANO + "'" +
           ",'" + FormLines.GARANTINO + "'" +
           ",'" + FormLines.CIHAZ + "'" +
           ",'" + FormLines.CIHAZSERINO + "')";
                int rowsAffected = db.Execute(sqlQuery);
            }
            return RedirectToAction("FormList", "Forms", new { @formtype = FormLines.FORMTYPE });
        }
        [Route("Forms/FormEdit")]
        // GET: Forms/Edit/5
        public ActionResult FormEdit(string ficheno, string formtype)
        {
            LoginCheck();
            FORMTYPECheck(formtype);
            FORMLines FormDetail = new FORMLines();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {

                FormDetail = db.Query<FORMLines>("Select * From FORMLINE where FICHENO='" + ficheno + "'").SingleOrDefault();
            }
            return View(FormDetail);
        }
        [Route("Forms/FormEdit")]
        // POST: Forms/Edit/5
        [HttpPost]
        public ActionResult FormEdit(int id, string formtype, FORMLines FormLines)
        {
            LoginCheck();

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                string MontajDate = Convert.ToDateTime(((FormLines.MONTAJDATE == null) ? DateTime.Parse("1900-01-01") : FormLines.MONTAJDATE)).ToString("MM/dd/yyyy");
                string ARIZADATE = Convert.ToDateTime(((FormLines.ARIZADATE == null) ? DateTime.Parse("1900-01-01") : FormLines.ARIZADATE)).ToString("MM/dd/yyyy");
                string FatDate = Convert.ToDateTime(((FormLines.FATURADATE == null) ? DateTime.Parse("1900-01-01") : FormLines.FATURADATE)).ToString("MM/dd/yyyy");

                string sqlQuery = "update [dbo].[FORMLINE] set " +
           "BELGENO=" + FormLines.BELGENO + "" +
           ",BELGESTR='" + FormLines.BELGESTR + "'" +
           ",ADSOYAD='" + FormLines.ADSOYAD + "'" +
           ",ADRES='" + FormLines.ADRES + "'" +
           ",TEL='" + FormLines.TEL + "'" +
           ",TEL2='" + FormLines.TEL2 + "'" +
           ",EMAIL='" + FormLines.EMAIL + "'" +
           ",MONTAJDATE='" + MontajDate + "'" +
           ",ARIZADATE='" + ARIZADATE + "'" +
           ",FIRMA='" + FormLines.FIRMA + "'" +
           ",SATICIFIRMA='" + FormLines.SATICIFIRMA + "'" +
           ",FATURADATE='" + Convert.ToDateTime(FatDate).ToString("MM/dd/yyyy") + "'" +
           ",FATURANO='" + FormLines.FATURANO + "'" +
           ",GARANTINO='" + FormLines.GARANTINO + "'" +
           ",CIHAZ='" + FormLines.CIHAZ + "'" +
           ",CIHAZSERINO='" + FormLines.CIHAZSERINO + "',DURUM=1" +
           ",CACIKLAMA='" + FormLines.CACIKLAMA + "'" +
           " WHERE LOGICALREF=" + id;
                int rowsAffected = db.Execute(sqlQuery);
            }
            return RedirectToAction("Index");
        }

        [Route("Forms/Delete")]
        // GET: Forms/Delete/5
        public ActionResult Delete(int id)
        {
            LoginCheck();
            FORMLines FormDetail = new FORMLines();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                FormDetail = db.Query<FORMLines>("select * from FORMLINE where LOGICALREF=" + id + "").SingleOrDefault();
            }
            return View(FormDetail);
        }
        [Route("Forms/Delete")]
        // POST: Forms/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            LoginCheck();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                int rowsAffected = db.Execute("delete from FORMLINE where LOGICALREF=" + id);
            }
            return RedirectToAction("Index");
        }



        [Route("Forms/GetFormsJson")]
        [System.Web.Mvc.HttpGet]
        public JsonResult GetFormsJson(string SQLAra, string formtype)
        {

            string LSqlAra = "";
            if (SQLAra != "")
            {
                LSqlAra += "and replace(ADRES+ADSOYAD,' ','') like '%" + SQLAra.Replace(" ", "") + "%' ";
            }
            List<FORMLines> _formLists = new List<FORMLines>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                _formLists = db.Query<FORMLines>("select * from FORMLINE where  CLIENTREF=" + GLOBALS.GetCookie("giris")["CLIENTREF"] + " and formtype='" + formtype + "' " + LSqlAra).ToList();
            }
            return Json(_formLists, JsonRequestBehavior.AllowGet);
        }

        [Route("Forms/GetCihazFromBarcodeLOGO")]
        [System.Web.Mvc.HttpGet]
        public JsonResult GetCihazFromBarcodeLOGO(string GBarcode)
        {
            string GFirma = WebConfigurationManager.AppSettings["GFirma"].ToString();
            string GDonem = WebConfigurationManager.AppSettings["GDonem"].ToString();
            string LSqlAra = "";
            if (GBarcode != "")
            {
                //  LSqlAra += "and replace(ADRES+ADSOYAD,' ','') like '%" + SQLAra.Replace(" ", "") + "%' ";
            }
            List<BarcodeModel> _formLists = new List<BarcodeModel>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                _formLists = db.Query<BarcodeModel>("Select CONVERT(VARCHAR(10), ITARIH, 103) as ITARIH,Convert(VARCHAR(10),DATE_,103) as DATE_,* from C_SRG_FATURUN_" + GFirma + "_" + GDonem + " where Barcode='" + GBarcode + "'" + LSqlAra).ToList();
            }

            return Json(_formLists, JsonRequestBehavior.AllowGet);
        }


        [Route("Forms/GetConditioner")]
        [System.Web.Mvc.HttpGet]
        public JsonResult GetConditioner(string GBarcode)
        {
            string LSqlAra = "";
            List<BarcodeModel> _barcodes = new List<BarcodeModel>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                _barcodes = db.Query<BarcodeModel>("Select UrunAdi as NAME, BARCODE,INVCYP as FATTIPI,INVFISNO as FATTURU,ITARIH as FATTARIH,DEFINITION_ as FATMUSTERI from LKSDB..AA_FormUrun where BARCODE='" + GBarcode + "'" + LSqlAra).ToList();
            }

            return Json(_barcodes, JsonRequestBehavior.AllowGet);
        }

        [Route("Forms/GetWaitedForms")]
        [HttpGet]
        public JsonResult GetWaitedForms()
        {
            List<FORMLines> _formLines = new List<FORMLines>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                _formLines = db.Query<FORMLines>("select top 5 FORMTYPE,DATEDIFF(DAY,ADDDATE,GETDATE()) AS TIMEDIFF,FICHENO from FORMLINE where DURUM=0 and CLIENTREF=" + GLOBALS.GetCookie("giris")["CLIENTREF"] + " order by ADDDATE desc").ToList();
            }

            return Json(_formLines, JsonRequestBehavior.AllowGet);

        }

        [Route("Forms/FormUrunEkle")]
        [HttpPost]
        public ActionResult FormUrunEkle(FORMUrun obj)
        {
            bool durum = false;
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                SqlDataReader oku;
                if (obj.FORMTYPE == "montaj")
                {
                    oku = GLOBALS.GetDataReader("select LOGICALREF from FORMUrun where FORMTYPE = 'montaj' and(ICBARKOD in ('" + obj.ICBARKOD + "', '" + obj.DISBARKOD + "', '" + obj.BARKOD3 + "')" +
                    " or DISBARKOD in ('" + obj.ICBARKOD + "','" + obj.DISBARKOD + "','" + obj.BARKOD3 + "') or BARKOD3 in ('" + obj.ICBARKOD + "','" + obj.DISBARKOD + "','" + obj.BARKOD3 + "') ) ");
                    if (!oku.Read())
                    {
                        durum = true;
                    }
                }
                oku = GLOBALS.GetDataReader("select count(LOGICALREF) as Sayi from LKSDB..AA_FormUrun where Barcode='" + obj.ICBARKOD + "'  or Barcode='" + obj.DISBARKOD + "' or Barcode='" + obj.BARKOD3 + "' ");
                if (oku.Read())
                {
                    int sayi = Convert.ToInt32(oku[0].ToString());
                    if (sayi < 2)
                    {
                        durum = false;
                    }
                    else
                    {
                        durum = true;
                    }
                }
                else { durum = false; }
                if (durum == true)
                {
                    string query = "insert into FORMUrun (FORMTYPE,FICHENO,ADDDATE,ICBARKOD,DISBARKOD,BARKOD3,FORMREF) values" +
                        "('" + obj.FORMTYPE + "','" + obj.FICHENO + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','" + obj.ICBARKOD + "','" + obj.DISBARKOD + "','" + obj.BARKOD3 + "'," + obj.FORMREF + ")";
                    int affected = db.Execute(query);
                }
            }
            return Json(new
            {
                success = durum,

            }, JsonRequestBehavior.AllowGet);
        }

       
        [Route("Forms/FormKaydetJQ")]
        [HttpPost]
        public ActionResult FormKaydetJQ(FORMLines FormLines)
        {

            //zorunlu alan kontrolü
            if (FormLines.TEL == "" && FormLines.TEL2 == "")
            {
                return View();
            }
            string MontajDate = Convert.ToDateTime(((FormLines.MONTAJDATE == null) ? DateTime.Parse("1900-01-01") : FormLines.MONTAJDATE)).ToString("MM/dd/yyyy");
            string ARIZADATE = Convert.ToDateTime(((FormLines.ARIZADATE == null) ? DateTime.Parse("1900-01-01") : FormLines.ARIZADATE)).ToString("MM/dd/yyyy");
            string FatDate = Convert.ToDateTime(((FormLines.FATURADATE == null) ? DateTime.Parse("1900-01-01") : FormLines.FATURADATE)).ToString("MM/dd/yyyy");
            bool durum = true;
            if ((FormLines.FORMTYPE == "montaj" && FormLines.MONTAJDATE > DateTime.Now) || (FormLines.FORMTYPE == "Arıza" && FormLines.ARIZADATE > DateTime.Now))
            {
                durum = false;
            }
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                string sqlQuery = "INSERT INTO [dbo].[FORMLINE](FORMTYPE,FICHENO,[BELGENO],[BELGESTR],[ADSOYAD],[ADRES],[TEL],[TEL2],[EMAIL],[MONTAJDATE],[ARIZADATE],[FIRMA],[SATICIFIRMA],[FATURADATE],[FATURANO],[GARANTINO],[CIHAZ],[CIHAZSERINO],CLIENTREF)VALUES" +
           "('" + FormLines.FORMTYPE + "','" + FormLines.BELGESTR + "-" + FormLines.BELGENO + "'," + FormLines.BELGENO + "" +
           ",'" + FormLines.BELGESTR + "'" +
           ",'" + FormLines.ADSOYAD + "'" +
           ",'" + FormLines.ADRES + "'" +
           ",'" + FormLines.TEL + "'" +
           ",'" + FormLines.TEL2 + "'" +
           ",'" + FormLines.EMAIL + "'" +
           ",cast('" + MontajDate + "' as datetime)" +
           ",cast('" + ARIZADATE + "' as datetime)" +
           ",'" + FormLines.FIRMA + "'" +
           ",'" + FormLines.SATICIFIRMA + "'" +
           ",cast('" + FatDate + "' as datetime)" +
           ",'" + FormLines.FATURANO + "'" +
           ",'" + FormLines.GARANTINO + "'" +
           ",'" + FormLines.CIHAZ + "'" +
           ",'" + FormLines.CIHAZSERINO + "'" +
           ",'" + GLOBALS.GetCookie("giris")["CLIENTREF"] + "'" +
           ")";
                if (durum == true)
                {
                    int rowsAffected = db.Execute(sqlQuery);
                    int rowsUpdated = db.Execute("update USERS set BELGEID=BELGEID+1 where LOGICALREF=" + GLOBALS.GetCookie("giris")["USERID"] + "");
                    int belgeid = Convert.ToInt32(giris["BELGEID"]);
                    belgeid++;
                    giris["BELGEID"] = belgeid.ToString();
                    Response.Cookies.Add(giris);
                }
            }
            // Response.Redirect("FormEdit?ficheno="+ FormLines.BELGESTR + "-" + FormLines.BELGENO+ "&formtype="+Request.QueryString["formtype"]+"");
            return Json(new
            {
                success = durum
            }, JsonRequestBehavior.AllowGet);

            // return RedirectToAction("FormEdit",new {ficheno=FormLines.BELGESTR+"-"+FormLines.BELGENO,formtype=Request.QueryString["formtype"] });
        }

        [HttpPost]
        [Route("Forms/FormUrunSil")]
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
        [Route("Forms/GarantiList")]
        public ActionResult GarantiList()
        {
            List<UrunGaranti> _list = new List<UrunGaranti>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                _list = db.Query<UrunGaranti>("select UrunAdi as URUNADI,dateadd(year,2,OkutTarih) as GARANTISON,ICBARKOD,DISBARKOD,BARKOD3,OkutTarih as ADDDATE from [FORMURUNView] where FORMTYPE='MONTAJ' order by LOGICALREF desc").ToList();
            }
            return View(_list);
        }

        [Route("Forms/Dokumantasyon")]
        public ActionResult Dokumantasyon()
        {
            List<Documents> _list = new List<Documents>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                _list = db.Query<Documents>("select * from Documents order by LOGICALREF").ToList();
            }
            return View(_list);
        }

        [Route("Forms/HakedisList")]
        public ActionResult HakedisList()
        {
            List<FORMLines> _list = new List<FORMLines>();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {
                _list = db.Query<FORMLines>("select case when durum=2 then 'Ödendi' end as ONAYDURUM,"+
                    "* from FORMLINE WHERE CLIENTREF="+GLOBALS.GetCookie("giris")["CLIENTREF"]+" and DURUM=2").ToList();
            }
            return View(_list);
        }


    }
}
