using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Crm_MAF.Models
{
    public class Persona
    {
        // Asumiendo que estas son las propiedades que deseas de Dynamics CRM
        public string FullName { get; set; }
        public string Telephone1 { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        // ... cualquier otra propiedad que necesites ...

        // Por ejemplo, si tienes un campo en CRM para el tipo de documento y el número de documento,
        // puedes añadirlos también aquí
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
    }
}
