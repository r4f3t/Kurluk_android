using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTEKNIK.Models
{
    public class Login
    {
        public int LOGICALREF { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string AUTHORITY { get; set; }
        public int AUTHID { get; set; }
        public string NAME { get; set; }
        public string CODE { get; set; }
        public int BELGEID { get; set; }
        public int FLAG { get; set; }
        public Nullable<int> CLIENTREF { get; set; }
        public string TEL { get; set; }
        public string TEL2 { get; set; }
        // [Required(ErrorMessage ="Email Alanı Haberleşme için Zorunludur.")]
        public string EMAIL { get; set; }
    }
}