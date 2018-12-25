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
namespace PTEKNIK.Controllers.Home
{
    public class HomeController : Controller
    {
        // GET: Home
        [Route("Home/Index")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Home/Index")]
        public ActionResult Index(Login PostedLogin) {
            Login _login = new Login();
            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString))
            {

                _login = db.Query<Login>("Select * From USERS where USERNAME='"+PostedLogin.USERNAME+"' AND  PASSWORD='"+PostedLogin.PASSWORD+"' AND FLAG=0").SingleOrDefault();
            }
            if (_login!=null)
            {
                HttpCookie giris = new HttpCookie("giris");
                giris["giris"] = "1";
                giris["AUTHID"] = _login.AUTHID.ToString();
                giris["AUTHORITY"] = _login.AUTHORITY;
                giris["BELGEID"] = _login.BELGEID.ToString();
                giris["USERID"] = _login.LOGICALREF.ToString();
                giris["CLIENTREF"] = _login.CLIENTREF.ToString();
                giris["NAME"] = Server.UrlEncode(_login.NAME);
                giris["CODE"] = _login.CODE;
                giris.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(giris);

                HttpCookie cookie = new HttpCookie("fichenos");
                cookie["BELGEID"] = _login.BELGEID.ToString();
                cookie["USERID"] = _login.LOGICALREF.ToString();
                cookie["CLIENTREF"] = _login.CLIENTREF.ToString();
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(cookie);
                if (_login.AUTHID==1)
                {
                    return RedirectToAction("Index", "Forms", "");
                }
                else if (_login.AUTHID == 0)
                {
                    return RedirectToAction("Index", "Admin", "");
                }
                else if (_login.AUTHID == 2)
                {
                    return RedirectToAction("HakOnayList", "Yonetim",  new { durum = 1 });
                }
                return View();
            }
            else
            {
                return View();
            }
          
        }
    }
}