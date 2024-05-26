using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Crm_MAF.Models
{
    public class mRegistroForm
    {
        public string RazonSocial { get; set; }
        public string TipoDoc { get; set; }
        public string NroDoc { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }     
        public string Motivo { get; set; }
        public string NumeroContrato { get; set; }
        public string SubMotivo { get; set; }
        public string DetalleCaso { get; set; }
        public string TipoCaso { get; set; }
        public string TipoProductoLib { get; set; }
        public string Telefono { get; set; }
        public string CorreoCorporativo { get; set; }
        public string Placa { get; set; }
        public string EnvioNotificacion { get; set; }
    }
}