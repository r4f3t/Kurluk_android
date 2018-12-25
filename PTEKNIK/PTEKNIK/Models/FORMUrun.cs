using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PTEKNIK.Models
{
    public class FORMUrun
    {
        public string ICBARKOD { get; set; }
        public string DISBARKOD { get; set; }
        public string BARKOD3 { get; set; }
        public int FORMREF { get; set; }
        public string FICHENO { get; set; }
        public string CREF { get; set; }
        public string FORMTYPE { get; set; }
    }
}