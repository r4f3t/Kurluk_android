using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using PTEKNIK.Models;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
namespace PTEKNIK.Controllers.Garanti
{
    public class EskiGarantiController : Controller
    {
        IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString);
        HttpCookie giris = GLOBALS.GetCookie("giris");
        HttpCookie cookie = GLOBALS.GetCookie("fichenos");
        // GET: EskiGaranti
        [Route("EskiGaranti/Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("EskiGaranti/GarantiAdd")]
        public ActionResult GarantiAdd() {
            return View();
        }

        [HttpPost]
        [Route("EskiGaranti/GarantiAdd")]
        public ActionResult GarantiAdd(System.Web.HttpPostedFileBase PATH, FORMLINEGARANTI obj)
        {
            using (db)
            {
                int affectedRows = db.Execute("insert into ");
            }
            return View();
        }
    }
}