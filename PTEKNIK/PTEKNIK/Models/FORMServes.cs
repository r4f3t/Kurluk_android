using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpressiveAnnotations.Attributes;
using System.ComponentModel.DataAnnotations;
namespace PTEKNIK.Models
{
    public class FORMServes
    {
        public int LOGICALREF { get; set; }
        public string FICHENO { get; set; }
        public string SERVNAME { get; set; }
        public string SERVCODE { get; set; }
        public string SERVTUTAR { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ADDDATE { get; set; }
        public string ACIKLAMA { get; set; }
    }
}