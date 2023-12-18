using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapfreHSBC.Models.Cotizacion
{
    public class ChkBase
    {
        public int value { get; set; }
        public string text { get; set; }
        public bool isChecked { get; set; }
        public string id {get;set;}
    }

    public class ChkList
    {
        public List<ChkBase> listChk { get; set; }
    }
}