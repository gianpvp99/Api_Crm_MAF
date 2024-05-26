using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Crm_MAF.Models
{
    public class mLstPortal
    {
        public string CodigoCaso { get; set; }
        public string TipoCaso { get; set; }
        public int DiasTranscurridos { get; set; }
        public string Estado { get; set; }
        public string FechaRecepcion { get; set; }
        public string Motivo { get; set; }
        public string SubMotivo { get; set; }
    }
}