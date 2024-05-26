using Api_Crm_MAF.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Http;

namespace Api_Crm_MAF.Controllers
{
    [Authorize]
    [RoutePrefix("api/cliente")]
    public class ClienteController : ApiController
    {
        [HttpGet]
        [Route("ObtenerPorDocumento")]
        public IHttpActionResult ObtenerPorDocumento(string documento)
        {
            if (string.IsNullOrEmpty(documento))
            {
                return BadRequest("El documento no puede estar vacío.");
            }

            try
            {
                IOrganizationService servicio = CrmConexion.ConnectToCRM();
                string fetchXmlContact = $@"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                <entity name='contact'>
                    <attribute name='onetoone_numerodedocumento'/>
                    <attribute name='firstname'/>
                    <attribute name='lastname'/>
                    <attribute name='emailaddress1'/>
                    <attribute name='mobilephone'/>
                    <attribute name='onetoone_apellidopaterno'/>
                    <attribute name='onetoone_apellidomaterno'/>
                    <attribute name='onetoone_primernombre'/>
                    <attribute name='onetoone_segundonombre'/>
                    <filter type='and'>
                        <condition attribute='onetoone_numerodedocumento' operator='eq' value='{documento}'/>
                    </filter>
                </entity>
            </fetch>";

                string fetchXmlAccount = $@"
            <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                <entity name='account'>
                    <attribute name='name'/>
                    <attribute name='telephone1'/>
                    <attribute name='primarycontactid'/>
                    <attribute name='emailaddress1'/>
                    <filter type='and'>
                        <condition attribute='accountnumber' operator='eq' value='{documento}'/>
                    </filter>
                </entity>
            </fetch>";

                EntityCollection resultadoContact = servicio.RetrieveMultiple(new FetchExpression(fetchXmlContact));
                EntityCollection resultadoAccount = servicio.RetrieveMultiple(new FetchExpression(fetchXmlAccount));

                var clienteData = new object();

                if (resultadoContact.Entities.Any())
                {
                    var cliente = resultadoContact.Entities.First();
                    clienteData = new
                    {
                        //Nombre = cliente.GetAttributeValue<string>("firstname") + " " + cliente.GetAttributeValue<string>("lastname"),
                       
                        Primer_nombre = cliente.GetAttributeValue<string>("onetoone_primernombre"),
                        Segundo_nombre = cliente.GetAttributeValue<string>("onetoone_segundonombre"),
                        Apellido_paterno = cliente.GetAttributeValue<string>("onetoone_apellidopaterno"),
                        Apellido_materno = cliente.GetAttributeValue<string>("onetoone_apellidomaterno"),
                        Email = cliente.GetAttributeValue<string>("emailaddress1"),
                        Telefono = cliente.GetAttributeValue<string>("mobilephone"),
                        Telefono2 = cliente.GetAttributeValue<string>("telephone1"),
                        Nro_documento = cliente.GetAttributeValue<string>("onetoone_numerodedocumento"),
                        Tipo = "Persona"
                    };
                }
                else if (resultadoAccount.Entities.Any())
                {
                    var cliente = resultadoAccount.Entities.First();
                    clienteData = new
                    {
                        Nombre = cliente.GetAttributeValue<string>("name"),
                        Email = cliente.GetAttributeValue<string>("emailaddress1"),
                        Telefono = cliente.GetAttributeValue<string>("telephone1"),
                        Tipo = "Empresa"
                    };
                }
                else
                {
                    return NotFound();
                }

                string jsonResponse = JsonConvert.SerializeObject(clienteData);
                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}


