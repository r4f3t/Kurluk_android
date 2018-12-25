using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace PTEKNIK
{
    public class GLOBALS
    {

        public static SqlConnection CONN() {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString);

            return con;
        }
        public static SqlDataReader GetDataReader(string query) {
            SqlConnection con = CONN();
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = query;
            SqlDataReader read=cmd.ExecuteReader();
            return read;
        }

        public static string GFirma = WebConfigurationManager.AppSettings["GFirma"].ToString();
        public static IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["PTEKNIKMSSQL"].ConnectionString);
        public static HttpCookie GetCookie(string CookieName)
        {
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
                return HttpContext.Current.Request.Cookies[CookieName];
            else
                return null;
        }

        public static HttpCookie setCookie(string CookieName,string CookieLine, string val)
        {
            HttpCookie giris = new HttpCookie(CookieName);
            giris[CookieLine] = val;
            giris.Expires = DateTime.Now.AddDays(1);
            return giris;
        }

        public static bool CookieAuth()
        {
            HttpCookie giris = GetCookie("giris");
            if (giris != null && giris["AUTHID"] == "0")
            {

                return true;
            }
            return false;
        }
        public static bool CookieAuthClients()
        {
            HttpCookie giris = GetCookie("giris");
            if (giris != null)
            {

                return true;
            }
            return false;
        }
    }
}