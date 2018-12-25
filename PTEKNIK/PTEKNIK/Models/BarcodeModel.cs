using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace PTEKNIK.Models
{
    public class BarcodeModel
    {
        public string BARCODE { get; set; }
        public string FATTIPI { get; set; }
        public string FATTURU { get; set; }
        public string FATMUSTERI { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FATTARIH { get; set; }
        public string NAME { get; set; }
    }
}