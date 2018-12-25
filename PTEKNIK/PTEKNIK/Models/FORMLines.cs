using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ExpressiveAnnotations.Attributes;
namespace PTEKNIK.Models
{
    public  class FORMLines
    {
        public int LOGICALREF { get; set; }
        public int BELGENO { get; set; }
        public string BELGESTR { get; set; }
        public string ADSOYAD { get; set; }
        public string ADRES { get; set; }
        [Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public string TEL { get; set; }

        public string TEL2 { get; set; }
        public string EMAIL { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> MONTAJDATE { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ARIZADATE { get; set; }
        public string FIRMA { get; set; }
        public string SATICIFIRMA { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FATURADATE { get; set; }
        public string FATURANO { get; set; }
        public string GARANTINO { get; set; }
        public string CIHAZ { get; set; }
        public string CIHAZSERINO { get; set; }
        public string FORMTYPE { get; set; }
        public Nullable<System.DateTime> ADDDATE { get; set; }
        public string MONTAJDURUM { get; set; }
        public string TIMEDIFF { get; set; }
        public Nullable<int> CLIENTREF { get; set; }
        public string NAME { get; set; }
        public string CODE { get; set; }
        public string ACIKLAMA { get; set; }
        public string CACIKLAMA { get; set; }
        public string FICHENO { get; set; }
        public string ONAYDURUM { get; set; }
        public string TRSTYLE { get; set; }
        public string YONACIKLAMA { get; set; }
        public string ADMACIKLAMA { get; set; }
        public string DURUM { get; set; }
    }
}