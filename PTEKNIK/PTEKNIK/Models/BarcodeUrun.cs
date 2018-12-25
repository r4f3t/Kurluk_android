using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTEKNIK.Models
{
    public class BarcodeUrun
    {
        public int LOGICALREF { get; set; }
        public Nullable<int> ORFLINEREF { get; set; }
        public string Barcode { get; set; }
        public Nullable<int> UrunId { get; set; }
        public Nullable<int> LUrunId { get; set; }
        public Nullable<System.DateTime> ADDDATE { get; set; }
        public string UrunAdi { get; set; }
        public string UrunKodu { get; set; }
        public string LUrunAdi { get; set; }
        public string LUrunKodu { get; set; }
        public string ORFICHENO { get; set; }
        public Nullable<System.DateTime> ORFDATE { get; set; }
        public Nullable<int> AKTARIML { get; set; }
        public Nullable<int> ORDFICHEREF { get; set; }
        public string UrunTip { get; set; }
        public Nullable<int> OZELint { get; set; }
        public Nullable<int> GRUPint { get; set; }
        public string OZELSTR { get; set; }
        public Nullable<double> AMOUNT { get; set; }
        public Nullable<double> SHIPPEDAMOUNT { get; set; }
        public Nullable<System.DateTime> DATE_ { get; set; }
        public string FICHENO { get; set; }
        public string CODE { get; set; }
        public string DEFINITION_ { get; set; }
        public Nullable<int> CREF { get; set; }
        public Nullable<int> INVREF { get; set; }
        public Nullable<System.DateTime> ITARIH { get; set; }
        public string INVFISNO { get; set; }
        public string INVCYP { get; set; }
        public string INVSPE { get; set; }
        public string ETIKET { get; set; }


    }
}